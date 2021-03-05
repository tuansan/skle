using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class GroupBuilder : NopEntityBuilder<Group>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Group.Name)).AsString(255).Nullable();
        }
    }
}