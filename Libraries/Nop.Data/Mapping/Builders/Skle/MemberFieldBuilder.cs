using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Skle;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Skle
{
    public partial class MemberFieldBuilder : NopEntityBuilder<MemberField>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(MemberField.MemberId)).AsInt32().ForeignKey<Member>()
                .WithColumn(nameof(MemberField.FieldId)).AsInt32().ForeignKey<Field>();
        }
    }
}