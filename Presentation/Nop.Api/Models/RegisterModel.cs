using System.Collections.Generic;

namespace Nop.Api.Models
{
    public class RegisterModel
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string FirebaseId { get; set; }
    }
}