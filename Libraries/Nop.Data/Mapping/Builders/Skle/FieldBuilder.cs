using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class FieldBuilder : NopEntityBuilder<Field>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(Field.Name)).AsString(200).Nullable();
        }
    }
}