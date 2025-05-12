using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using PasswordVerificationResult = Microsoft.AspNetCore.Identity.PasswordVerificationResult;
using Domain.DTOs.UserDTOs;

namespace Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<string> _passwordHasher;
        private readonly IEmailService _emailService;
        private readonly ICacheService _cacheService;

        public UserService(
            IUserRepo userRepository,
            IConfiguration configuration,
            IPasswordHasher<string> passwordHasher,
            IEmailService emailService,
            ICacheService cacheService)
        {
            _userRepo = userRepository;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
            _cacheService = cacheService;
        }

        private string GetUserByEmailCacheKey(string email) => $"user:email:{email.Trim().ToLower()}";
        private string GetUserByIdCacheKey(int userId) => $"user:id:{userId}";

        private async Task<User?> GetUserByEmailCached(string email)
        {
            var key = GetUserByEmailCacheKey(email);
            var cached = await _cacheService.GetAsync<User>(key);
            if (cached != null) return cached;

            var user = await _userRepo.GetUserByEmail(email);
            if (user != null)
                await SetUserCacheAsync(user);

            return user;
        }

        private async Task<User?> GetUserByIdCached(int userId)
        {
            var key = GetUserByIdCacheKey(userId);
            var cached = await _cacheService.GetAsync<User>(key);
            if (cached != null) return cached;

            var user = await _userRepo.GetUserById(userId);
            if (user != null)
                await SetUserCacheAsync(user);

            return user;
        }

        private async Task SetUserCacheAsync(User user)
        {
            await _cacheService.SetAsync(GetUserByIdCacheKey(user.UserID), user, TimeSpan.FromMinutes(30));
            await _cacheService.SetAsync(GetUserByEmailCacheKey(user.Email), user, TimeSpan.FromMinutes(30));
        }
        public async Task<string> Login(string email, string password)
        {
            var user = await GetUserByEmailCached(email);
            if (user == null) return null;

            if (!user.IsEmailConfirmed)
                throw new UnauthorizedAccessException("Please confirm your email before logging in.");

            var result = _passwordHasher.VerifyHashedPassword(null, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed) return null;

            await SetUserCacheAsync(user);

            var jwtSettings = _configuration.GetSection("Jwt");

            var jwtKey = jwtSettings["Key"];
            var jwtIssuer = jwtSettings["Issuer"];
            var jwtAudience = jwtSettings["Audience"];
            var jwtExpiry = jwtSettings["ExpiresInMinutes"];

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) ||
                string.IsNullOrEmpty(jwtAudience) || string.IsNullOrEmpty(jwtExpiry))
                throw new InvalidOperationException("Missing JWT configuration in appsettings.json.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
    };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtExpiry)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<bool> RegisterUser(NewUserDTO dto)
        {
            if (!IsValidEmailFormat(dto.Email))
                throw new ArgumentException("Invalid Gmail address format.");

            var existingUser = await _userRepo.GetUserByEmail(dto.Email);
            if (existingUser != null)
                throw new InvalidOperationException("A user with this email already exists.");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                ProfilePictureURL = dto.ProfilePictureURL,
                IsEmailConfirmed = false,
                PasswordHash = _passwordHasher.HashPassword(null, dto.Password)
            };

            var result = await _userRepo.RegisterUser(user);
            if (!result) return false;

            return true;
        }


        public async Task<bool> ChangePassword(int userId, ChangePasswordDTO dto)
        {
            var user = await GetUserByIdCached(userId);
            if (user == null) return false;

            var verify = _passwordHasher.VerifyHashedPassword(null, user.PasswordHash, dto.CurrentPassword);
            if (verify == PasswordVerificationResult.Failed) return false;

            user.PasswordHash = _passwordHasher.HashPassword(null, dto.NewPassword);
            var result = await _userRepo.UpdateUserPassword(user);

            if (result)
                await SetUserCacheAsync(user); 

            return result;
        }

        public async Task<bool> ResetPassword(int userId, ResetPasswordDTO dto)
        {
            var user = await GetUserByIdCached(userId);
            if (user == null || dto.NewPassword != dto.ConfirmedNewPassword) return false;

            user.PasswordHash = _passwordHasher.HashPassword(null, dto.NewPassword);
            var result = await _userRepo.UpdateUserPassword(user);

            if (result)
                await SetUserCacheAsync(user); 

            return result;
        }

        public async Task<bool> UpdateProfile(string email, string firstName, string lastName, string profilePictureUrl)
        {
            var user = await GetUserByEmailCached(email);
            if (user == null) return false;

            user.FirstName = firstName;
            user.LastName = lastName;
            user.ProfilePictureURL = profilePictureUrl;

            var result = await _userRepo.UpdateUserProfile(user);

            if (result)
                await SetUserCacheAsync(user); 

            return result;
        }
        private bool IsValidEmailFormat(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            return System.Text.RegularExpressions.Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }


        public async Task<string> SendConfirmationEmail(string email)
        {
            var user = await GetUserByEmailCached(email);
            if (user == null)
                throw new Exception("User not found");

            if (user.IsEmailConfirmed)
                return "Email is already confirmed.";

            var htmlBody = BuildEmailConfirmationBody(user.Email, user.FirstName);

            await _emailService.SendEmailAsync(email, "Email Confirmation", htmlBody);

            var result = await _userRepo.ConfirmEmail(email); 
            if (result)
            {
                user.IsEmailConfirmed = true;
                await SetUserCacheAsync(user);
            }

            return "Email confirmation sent and user marked as confirmed.";
        }


        public string BuildEmailConfirmationBody(string email, string firstName)
        {
            return $@"
    <html>
        <body style='font-family: Arial, sans-serif;'>
            <h2>Email Confirmation</h2>
            <p>Hello {firstName},</p>
            <p>Thank you for registering.</p>
            <p>Your email <strong>{email}</strong> has been successfully confirmed.</p>
            <p>You can now log in to your account.</p>
        </body>
    </html>";
        }


    }
}
