using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Api.Models
{
    public class PasswordRecoveryModel
    {
        public string PhoneNumber { get; set; }
        public string Otp { get; set; }
        public string Token { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}