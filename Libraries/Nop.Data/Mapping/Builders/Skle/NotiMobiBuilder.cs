using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class NotiMobiBuilder : NopEntityBuilder<NotiMobi>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                   .WithColumn(nameof(NotiMobi.PictureUrl)).AsString(int.MaxValue).Nullable()
                   .WithColumn(nameof(NotiMobi.Name)).AsString(500).Nullable()
                   .WithColumn(nameof(NotiMobi.Title)).AsString(1000).Nullable()
                   .WithColumn(nameof(NotiMobi.Content)).AsString(int.MaxValue).Nullable();
        }
    }
}
