using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class GroupMemberBuilder : NopEntityBuilder<GroupMember>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(GroupMember.MemberId)).AsInt32().ForeignKey<Member>()
                .WithColumn(nameof(GroupMember.GroupId)).AsInt32().ForeignKey<Group>();
        }
    }
}