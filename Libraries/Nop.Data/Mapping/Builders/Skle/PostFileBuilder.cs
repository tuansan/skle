using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class PostFileBuilder : NopEntityBuilder<PostFile>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PostFile.MimeType)).AsString(255).Nullable()
                .WithColumn(nameof(PostFile.Extension)).AsString(40).Nullable()
                .WithColumn(nameof(PostFile.VideoUrl)).AsString(500).Nullable()
                .WithColumn(nameof(PostFile.MemberId)).AsInt32().ForeignKey<Member>();
        }
    }
}