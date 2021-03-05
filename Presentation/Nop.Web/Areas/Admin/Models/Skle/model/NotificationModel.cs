using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public class NotificationModel : BaseNopEntityModel
    {
        public NotificationModel()
        {
            AvailableTypes = new List<SelectListItem>();
            TargetMembers = new List<int>();
        }
        [NopResourceDisplayName("Admin.Fields.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.Fields.Content")]
        public string Content { get; set; }

        [NopResourceDisplayName("Admin.Fields.TypeId")]
        public int TypeId { get; set; }

        public DateTime CreatedAt { get; set; }
        public int Times { get; set; }

        public string TypeText { get; set; }

        [NopResourceDisplayName("Admin.Fields.TargetMembers")]
        public IList<int> TargetMembers { get; set; }

        [NopResourceDisplayName("Admin.Fields.TargetGroups")]
        public IList<int> TargetGroups { get; set; }

        public IList<SelectListItem> AvailableTypes { get; set; }

        public IList<SelectListItem> AvailableMembers { get; set; }
        public IList<SelectListItem> AvailableGroups { get; set; }
    }
}