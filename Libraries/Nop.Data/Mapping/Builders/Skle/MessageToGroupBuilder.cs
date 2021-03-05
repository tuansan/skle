using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class MessageToGroupBuilder : NopEntityBuilder<MessageToGroup>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(MessageToGroup.Content)).AsString(int.MaxValue).Nullable();
        }
    }
}
