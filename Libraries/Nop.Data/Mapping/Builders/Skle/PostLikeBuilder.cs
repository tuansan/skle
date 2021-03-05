using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class PostLikeBuilder : NopEntityBuilder<PostLike>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PostLike.PostId)).AsInt32().ForeignKey<Post>()
                .WithColumn(nameof(PostLike.MemberId)).AsInt32().ForeignKey<Member>();
        }
    }
}