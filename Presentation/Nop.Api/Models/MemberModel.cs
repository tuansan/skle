using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Api.Models
{
    public class MemberModel : BaseNopEntityModel
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
        public int GenderId { get; set; }
        public int AvatarId { get; set; }
        public int CoveId { get; set; }
        public int ProvinceId { get; set; }
        public int CountPost { get; set; }
        public int CountFriend { get; set; }
        public int CountGroup { get; set; }
        public int StatusId { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Email { get; set; }
        public bool PhoneSet { get; set; }
        public string AvatarUrl { get; set; }
        public string CoveUrl { get; set; }
        public string Province { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Fields { get; set; }
        public int StatusFrienRequestId { get; set; }
        public bool isHiddenPost { get; set; }
    }
}
