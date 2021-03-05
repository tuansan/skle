using System;

namespace Nop.Core.Domain.Skle
{
    public partial class Member : BaseEntity
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
        public string FirebaseId { get; set; }
    }
}