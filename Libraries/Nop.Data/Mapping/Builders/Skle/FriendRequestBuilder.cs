using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class FriendRequestBuilder : NopEntityBuilder<FriendRequest>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(FriendRequest.FromId)).AsInt32().ForeignKey<Member>()
                .WithColumn(nameof(FriendRequest.ToId)).AsInt32().ForeignKey<Member>();
        }
    }
}