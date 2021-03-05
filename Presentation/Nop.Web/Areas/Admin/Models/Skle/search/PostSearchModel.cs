using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public class PostSearchModel : BaseSearchModel
    {
        public PostSearchModel()
        {
            ListStatus = new List<SelectListItem>();
            SortBys = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Admin.Fields.Content")]
        public string KeySearch { get; set; }

        [NopResourceDisplayName("Admin.Fields.CountSpam")]
        public int CountSpam { get; set; }

        [NopResourceDisplayName("Admin.Fields.MemberId")]
        public int MemberId { get; set; }

        [NopResourceDisplayName("Admin.Fields.GroupId")]
        public int GroupId { get; set; }

        [NopResourceDisplayName("Admin.Fields.StatusId")]
        public int StatusId { get; set; }

        [NopResourceDisplayName("Admin.Fields.SortById")]
        public int SortById { get; set; }

        public IList<SelectListItem> ListStatus { get; set; }
        public IList<SelectListItem> Members { get; set; }
        public IList<SelectListItem> Groups { get; set; }
        public IList<SelectListItem> SortBys { get; set; }


    }
}