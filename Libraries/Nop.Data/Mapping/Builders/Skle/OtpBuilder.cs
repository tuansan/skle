using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class OtpBuilder : NopEntityBuilder<Otp>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Otp.PhoneNumber)).AsString(20).Nullable()
                .WithColumn(nameof(Otp.OptCode)).AsString(10).Nullable()
                .WithColumn(nameof(Otp.Token)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(Otp.Response)).AsString(int.MaxValue).Nullable();
        }
    }
}
