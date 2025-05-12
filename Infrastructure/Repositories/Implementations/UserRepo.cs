using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementations
{
    public class UserRepo : IUserRepo
    {


        private readonly EcommerceDBContext _context;
        public UserRepo(EcommerceDBContext context)
        {
            _context = context;

        }
        public async Task<bool> RegisterUser(User user)
        {
            try
            {

                await _context.User.AddAsync(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while registering a new user.", ex);
            }
        }
        public async Task<User?> GetUserByEmail(string email)
        {
            try
            {
                return await _context.User.FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the user info.", ex);
            }
        }

        public async Task<User?> GetUserById(int userId)
        {
            try
            {
                return await _context.User.FirstOrDefaultAsync(u => u.UserID == userId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the user by id.", ex);
            }
        }


        public async Task<bool> UpdateUserPassword(User user)
        {
            try
            {

                _context.User.Update(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating user password.", ex);
            }
        }

        public async Task<bool> UpdateUserProfile(User user)
        {
            try
            {
                _context.User.Update(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating user profile.", ex);
            }
        }

        public async Task<bool> ConfirmEmail(string email)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false;

            if (user.IsEmailConfirmed) return true; 

            user.IsEmailConfirmed = true;
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
