using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Nop.Api.Infrastructure.Api;
using Nop.Core.Domain.Customers;
using Nop.LibApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Api.Factories
{
    public partial interface ICommonFactory
    {
        ClientApiSetting isAuthentication(ClientApp model, string _dataConfig);

        string GenerateJWTToken(int MemberId, DateTime time);

        string GenerateToken(string key);

        Customer DecodeToken(string key);

        bool CheckCustommer(int MemberId);
    }
}