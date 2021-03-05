using Nop.Core.Domain.Skle;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Skle
{
    public partial interface IOtpService
    {
        Otp GetOtpByPhoneNumber(string phone);
        Otp GetOtpByToken(string token);
        void Insret(Otp item);
        void Update(Otp item);
        string SendSMS(string phone, string message);
    }
}
