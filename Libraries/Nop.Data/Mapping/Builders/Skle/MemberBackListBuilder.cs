using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class MemberBackListBuilder : NopEntityBuilder<MemberBackList>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(MemberBackList.FromId)).AsInt32().ForeignKey<Member>()
                .WithColumn(nameof(MemberBackList.ToId)).AsInt32().ForeignKey<Member>();
        }
    }
}