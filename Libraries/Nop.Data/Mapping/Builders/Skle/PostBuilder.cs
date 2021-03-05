using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class PostBuilder : NopEntityBuilder<Post>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Post.Content)).AsString(1000).Nullable()
                .WithColumn(nameof(Post.MemberId)).AsInt32().ForeignKey<Member>();
        }
    }
}