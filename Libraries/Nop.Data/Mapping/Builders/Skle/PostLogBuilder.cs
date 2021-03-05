using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class PostLogBuilder : NopEntityBuilder<PostLog>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PostLog.Content)).AsString(1000).Nullable()
                .WithColumn(nameof(PostLog.After)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(PostLog.PostId)).AsInt32().ForeignKey<Post>();
        }
    }
}