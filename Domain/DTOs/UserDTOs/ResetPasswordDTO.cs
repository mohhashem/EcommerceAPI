using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.UserDTOs
{
    public class ResetPasswordDTO
    {
        public string NewPassword{ get; set; }
        public string ConfirmedNewPassword { get; set; }
    }

}
