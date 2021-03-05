using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public class PostModel : BaseNopEntityModel
    {
        public PostModel()
        {
            PostFileSearchModel = new PostFileSearchModel();
            PostFileModel = new PostFileModel();
            PostFileSearchModel.SetGridPageSize();
        }
        public Guid PostGuid { get; set; }

        [NopResourceDisplayName("Admin.Fields.Content")]
        public string Content { get; set; }

        public int MemberId { get; set; }
        public int CountLike { get; set; }
        public int CountSpam { get; set; }

        [NopResourceDisplayName("Admin.Fields.CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [NopResourceDisplayName("Admin.Fields.Deleted")]
        public bool Deleted { get; set; }

        public bool Active => !Deleted; 

        public PostFileSearchModel PostFileSearchModel { get; set; }
        public PostFileModel PostFileModel { get; set; }
    }
}