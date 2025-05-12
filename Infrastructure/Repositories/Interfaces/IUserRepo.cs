using Domain.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IUserRepo
    {
        Task<bool> RegisterUser(User user);
        Task<bool> UpdateUserPassword(User user);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserById(int id);
        Task<bool> UpdateUserProfile(User user);
        Task<bool> ConfirmEmail(string email);
    }
}
