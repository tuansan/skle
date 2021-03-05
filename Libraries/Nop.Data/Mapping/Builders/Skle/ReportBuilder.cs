using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class ReportBuilder : NopEntityBuilder<Report>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Report.Title)).AsString(1000).Nullable()
                .WithColumn(nameof(Report.Content)).AsString(int.MaxValue).Nullable();
        }
    }
}
