using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Api.Models
{
    public class UpdateProfileModel
    {
        public string Name { get; set; }
        public string Birthday { get; set; }
        public int GenderId { get; set; }
        public int ProvinceId { get; set; }
        public string Email { get; set; }
        public bool PhoneSet { get; set; }
    }
}
