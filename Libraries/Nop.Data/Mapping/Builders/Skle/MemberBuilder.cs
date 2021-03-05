using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Skle;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class MemberBuilder : NopEntityBuilder<Member>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Member.Name)).AsString(200).Nullable()
                .WithColumn(nameof(Member.Email)).AsString(200).Nullable()
                .WithColumn(nameof(Member.FirebaseId)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(Member.CustomerId)).AsInt32().ForeignKey<Customer>();
        }
    }
}