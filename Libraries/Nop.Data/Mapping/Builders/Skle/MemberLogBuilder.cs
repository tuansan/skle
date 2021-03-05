using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class MemberLogBuilder : NopEntityBuilder<MemberLog>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(MemberLog.Content)).AsString(1000).Nullable()
                .WithColumn(nameof(MemberLog.After)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(MemberLog.MemberId)).AsInt32().ForeignKey<Member>();
        }
    }
}