using FluentValidation;
using Nop.Core.Domain.Skle;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Skle;
using Nop.Web.Framework.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Validators.Skle
{
    public partial class GroupValidator : BaseNopValidator<GroupModel>
    {
        public GroupValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.AvatarId).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Group.Fields.AvatarId.Required"));
            RuleFor(x => x.CoveId).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Group.Fields.CoveId.Required"));

            SetDatabaseValidationRules<Group>(dataProvider);
        }
    }
}
