using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class NotificationBuilder : NopEntityBuilder<MyNotification>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(MyNotification.Title)).AsString(1000).Nullable()
                .WithColumn(nameof(MyNotification.Content)).AsString(int.MaxValue).Nullable();
        }
    }
}