using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Api.Models
{
    public partial class SendSMS
    {
        public string Phone { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string Otp { get; set; }
    }
}
