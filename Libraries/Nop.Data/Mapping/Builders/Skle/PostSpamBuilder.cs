using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class PostSpamBuilder : NopEntityBuilder<PostSpam>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PostSpam.PostId)).AsInt32().ForeignKey<Post>()
                .WithColumn(nameof(PostSpam.MemberId)).AsInt32().ForeignKey<Member>();
        }
    }
}