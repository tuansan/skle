﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Api.Models
{
    public class LoginModel
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string FirebaseId { get; set; }
    }
}