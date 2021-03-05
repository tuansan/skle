using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class ProvinceBuilder : NopEntityBuilder<Province>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Province.Name)).AsString(255).Nullable();
        }
    }
}