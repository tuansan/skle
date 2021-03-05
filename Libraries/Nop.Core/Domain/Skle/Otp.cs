using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Skle
{
    public partial class Otp : BaseEntity
    {
        public string PhoneNumber { get; set; }
        public string OptCode { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Success { get; set; }
        public string Response { get; set; }
        public bool ResponseSuccess { get; set; }
    }
}
