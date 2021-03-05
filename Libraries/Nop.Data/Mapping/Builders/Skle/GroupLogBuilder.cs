using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class GroupLogBuilder : NopEntityBuilder<GroupLog>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(GroupLog.Content)).AsString(1000).Nullable()
                .WithColumn(nameof(GroupLog.After)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(GroupLog.GroupId)).AsInt32().ForeignKey<Group>();
        }
    }
}