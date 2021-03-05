using Nop.Web.Framework.Models;
using System;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public class GroupMemberModel : BaseNopEntityModel
    {
        public int MemberId { get; set; }
        public int GroupId { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedAt { get; set; }

        public string AvatarUrl { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}
