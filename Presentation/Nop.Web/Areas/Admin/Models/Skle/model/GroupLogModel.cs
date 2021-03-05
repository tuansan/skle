using Nop.Web.Framework.Models;
using System;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public class GroupLogModel : BaseNopEntityModel
    {
        public int GroupId { get; set; }
        public string Content { get; set; }
        public string After { get; set; }
        public DateTime CreatedAt { get; set; }
        public int StatusId { get; set; }
    }
}