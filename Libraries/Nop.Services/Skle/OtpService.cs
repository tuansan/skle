using Newtonsoft.Json;
using Nop.Core.Domain.Skle;
using Nop.Data;
using RestSharp;
using System;
using System.Net;
using Nop.Core;
using System.Text.RegularExpressions;
using System.Linq;

namespace Nop.Services.Skle
{
    public partial class OtpService : IOtpService
    {
        private readonly IRepository<Otp> _repository;

        public OtpService(IRepository<Otp> repository)
        {
            _repository = repository;
        }

        private static string GetToken()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new RestClient("https://app.sms.fpt.net/oauth2/token");
            var request = new RestRequest(Method.POST);

            request.AddJsonBody(new
            {
                grant_type = "client_credentials",
                client_id = "546882f9d6b31f66003993fa6015617586dd1604",
                client_secret = "c45c6e95e2493423C86176bC10cf528a2248df969547fa9b465E2c1Bff76ff5f4895842a",
                scope = "send_brandname_otp",
                session_id = "C48b88e54f58ece5939f14c1995"
            });

            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        public virtual Otp GetOtpByPhoneNumber(string phone)
        {
            return _repository.Table.OrderByDescending(s => s.Id).FirstOrDefault(s => s.PhoneNumber == phone);
        }

        public virtual Otp GetOtpByToken(string token)
        {
            return _repository.Table.FirstOrDefault(s => s.Token == token);
        }

        public virtual void Insret(Otp item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(Otp));

            _repository.Insert(item);
        }

        public virtual void Update(Otp item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(Otp));

            _repository.Update(item);
        }

        public virtual string SendSMS(string phone, string message)
        {
            string token = GetToken();
            dynamic str = JsonConvert.DeserializeObject<object>(token);
            string accesstoken = str["access_token"];
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new RestClient("https://app.sms.fpt.net/api/push-brandname-otp");
            var request = new RestRequest(Method.POST);
            var messagebytes = System.Text.Encoding.UTF8.GetBytes(message.RemoveVietnameseTone());
            string base64 = Convert.ToBase64String(messagebytes);
            request.AddJsonBody(new
            {
                access_token = accesstoken,
                session_id = "C48b88e54f58ece5939f14c1995",
                BrandName = "SKVINA",
                Phone = phone,
                Message = base64
            });
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
    }
}