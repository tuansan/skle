using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Api.Models
{
    public class PostModel : BaseNopEntityModel
    {
        public PostModel()
        {
            ListFile = new List<GetPostFileModel>();
        }
        public Guid PostGuid { get; set; }
        public string Content { get; set; }
        public int MemberId { get; set; }
        public int CountLike { get; set; }
        public int CountSpam { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Deleted { get; set; }

        public string AvatarUrl { get; set; }
        public string Name { get; set; }
        public bool isLike { get; set; }
        public bool isSpam { get; set; }
        public List<GetPostFileModel> ListFile { get; set; }
    }
}
