using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public class MemberModel : BaseNopEntityModel
    {
        public MemberModel()
        {
            AvailableGenders = new List<SelectListItem>();
            AvailableProvinces = new List<SelectListItem>();
            AvailableStatus = new List<SelectListItem>();
        }

        public int CustomerId { get; set; }

        [NopResourceDisplayName("Admin.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Fields.Birthday")]
        public DateTime? Birthday { get; set; }

        [NopResourceDisplayName("Admin.Fields.GenderId")]
        public int GenderId { get; set; }

        [NopResourceDisplayName("Admin.Fields.AvatarId")]

        [UIHint("Picture")]
        public int AvatarId { get; set; }

        [NopResourceDisplayName("Admin.Fields.CoveId")]
        [UIHint("Picture")]
        public int CoveId { get; set; }

        [NopResourceDisplayName("Admin.Fields.ProvinceId")]
        public int ProvinceId { get; set; }

        public int CountPost { get; set; }
        public int CountFriend { get; set; }
        public int CountGroup { get; set; }

        [NopResourceDisplayName("Admin.Fields.StatusId")]
        public int StatusId { get; set; }

        [NopResourceDisplayName("Admin.Fields.StatusId")]
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }

        [NopResourceDisplayName("Admin.Fields.Email")]
        public string Email { get; set; }

        [NopResourceDisplayName("Admin.Fields.PhoneSet")]
        public bool PhoneSet { get; set; }

        public string AvatarUrl { get; set; }
        public string PhoneNumber { get; set; }
        public bool Active { get; set; }

        [NopResourceDisplayName("Admin.Fields.Fields")]
        public IList<int> Fields { get; set; }

        public IList<SelectListItem> AvailableGenders { get; set; }
        public IList<SelectListItem> AvailableProvinces { get; set; }
        public IList<SelectListItem> AvailableStatus { get; set; }
        public IList<SelectListItem> AvailablFields { get; set; }
    }
}