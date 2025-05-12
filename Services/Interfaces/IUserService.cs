using Domain.DTOs.UserDTOs;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterUser(NewUserDTO user);
        Task<string> Login(string email, string password);
        Task<bool> ChangePassword(int userId, ChangePasswordDTO changePasswordDTO);
        Task<bool> ResetPassword(int userId, ResetPasswordDTO resetPasswordDTO);
        Task<bool> UpdateProfile(string email, string firstName, string lastName, string profilePictureUrl);
        Task<string> SendConfirmationEmail(string email);
        string BuildEmailConfirmationBody(string email, string firstName);

    }
}
