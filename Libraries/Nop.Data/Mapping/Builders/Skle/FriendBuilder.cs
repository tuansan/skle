using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class FriendBuilder : NopEntityBuilder<Friend>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Friend.FromId)).AsInt32().ForeignKey<Member>()
                .WithColumn(nameof(Friend.ToId)).AsInt32().ForeignKey<Member>();
        }
    }
}