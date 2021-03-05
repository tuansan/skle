using Nop.Api.Infrastructure.Api;
using Nop.Core.Domain.Media;
using Nop.Services.Localization;
using Nop.Services.Media;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nop.LibApi;
using Nop.Core;
using System.Linq;
using System.Security.Cryptography;
using System.IO;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Skle;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Messaging;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Nop.Api.Factories
{
    public partial class CommonFactory : ICommonFactory
    {
        #region Fields

        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly ApiSettings _settingContext;
        private readonly ICustomerService _customerService;
        private readonly IMemberService _memberService;
        private readonly IWebHelper _webHelper;
        private readonly IConfiguration _config;

        private static byte[] cryptkey = Encoding.ASCII.GetBytes("a1b2c3d4e512345abcdecft2020abcde");
        private static byte[] initVector = Encoding.ASCII.GetBytes("cft20202143abcde");


        #endregion Fields

        #region Ctor

        public CommonFactory(
            IPictureService pictureService,
            ILocalizationService localizationService,
            IConfiguration config,
            ApiSettings settingContext,
            IWebHelper webHelper,
            ICustomerService customerService,
            IMemberService memberService
            )
        {
            _pictureService = pictureService;
            _localizationService = localizationService;
            _settingContext = settingContext;
            _webHelper = webHelper;
            _config = config;
            _customerService = customerService;
            _memberService = memberService;
        }

        #endregion Ctor

        #region Utilities

        private static string DecryptAES(string cipherData)
        {
            try
            {
                using (var rijndaelManaged =
                       new RijndaelManaged { Key = cryptkey, IV = initVector, Mode = CipherMode.CBC })
                using (var memoryStream =
                       new MemoryStream(Convert.FromBase64String(cipherData)))
                using (var cryptoStream =
                       new CryptoStream(memoryStream,
                           rijndaelManaged.CreateDecryptor(cryptkey, initVector),
                           CryptoStreamMode.Read))
                {
                    return new StreamReader(cryptoStream).ReadToEnd();
                }
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }

        private static string CryptAES(string textToCrypt)
        {
            try
            {
                using (var rijndaelManaged =
                       new RijndaelManaged { Key = cryptkey, IV = initVector, Mode = CipherMode.CBC })
                using (var memoryStream = new MemoryStream())
                using (var cryptoStream =
                       new CryptoStream(memoryStream,
                           rijndaelManaged.CreateEncryptor(cryptkey, initVector),
                           CryptoStreamMode.Write))
                {
                    using (var ws = new StreamWriter(cryptoStream))
                    {
                        ws.Write(textToCrypt);
                    }
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }

        #endregion Utilities

        #region Methods

        public string GenerateToken(string key)
        {
            return CryptAES(key + "|||" + Guid.NewGuid().ToString());
        }

        public virtual Customer DecodeToken(string token)
        {
            try
            {
                var decryptAES = DecryptAES(token).Split("|||");
                var key = decryptAES.First();
                if (Guid.TryParse(key, out Guid g))
                {
                    return _customerService.GetCustomerByGuid(g);
                }
                else
                {
                    return _customerService.GetCustomerByPhoneNumber(key);
                }
            }
            catch
            {
                return null;
            }
        }

        public virtual ClientApiSetting isAuthentication(ClientApp model, string _dataConfig)
        {
            if (string.IsNullOrEmpty(model.ClientId))
                return new ClientApiSetting("ClientId không được rỗng");
            if (string.IsNullOrEmpty(model.Key))
                return new ClientApiSetting("Key không được rỗng");

            if (string.IsNullOrEmpty(_dataConfig))
                return new ClientApiSetting("API server is not config. Please contact to Adminstrator");

            var apiConfigs = _dataConfig.ToEntities<ClientApiSetting>();
            var clientInfo = apiConfigs.FirstOrDefault(c => c.ClientApp.Equals(model.ClientId));
            if (clientInfo == null)
            {
                return new ClientApiSetting(model.ClientId + " không có quyền truy xuất");
            }

            if (!model.Key.Equals(clientInfo.Key, StringComparison.Ordinal))
                return new ClientApiSetting("KeyPass không đúng");

            var ClientIP = _webHelper.GetCurrentIpAddress();

            if (!string.IsNullOrEmpty(clientInfo.ClientIP))
            {
                var listIp = clientInfo.ClientIP.Split(",");
                if (!listIp.Contains(ClientIP))
                    return new ClientApiSetting("IP(" + ClientIP + ") không được được phép truy cập");
            }

            clientInfo.ClientIP = ClientIP;
            return clientInfo;
        }

        public string GenerateJWTToken(int MemberId, DateTime time)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, MemberId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: time,
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public virtual bool CheckCustommer(int MemberId)
        {
            if (MemberId == 0)
                return true;
            var member = _memberService.GetMemberById(MemberId);
            if (member == null || member.Deleted || member.StatusId == (int)ENStatusMember.Blocked)
                return true;
            var customer = _customerService.GetCustomerById(member.CustomerId);
            if (customer == null || customer.Deleted || !customer.Active)
                return true;
            return false;
        }

        #endregion Methods
    }
}