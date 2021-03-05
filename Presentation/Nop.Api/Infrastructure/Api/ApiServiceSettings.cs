using Nop.LibApi;
using Nop.Core;
using Nop.Core.Configuration;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace Nop.Api.Infrastructure.Api
{
    public class ClientApiSetting
    {
        public ClientApiSetting()
        {
        }

        public ClientApiSetting(string _errMsg)
        {
            this.ErrorMsg = _errMsg;
        }

        public string ClientApp { get; set; }
        public string Key { get; set; }
        public string ClientIP { get; set; }
        public string ErrorMsg { get; set; }
        public int MemberId { get; set; }
    }

    public class ApiSettings : ISettings
    {
        public string DataConfig { get; set; }
    }
}