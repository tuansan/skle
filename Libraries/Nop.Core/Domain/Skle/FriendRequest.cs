namespace Nop.Core.Domain.Skle
{
    public partial class FriendRequest : BaseEntity
    {
        public int FromId { get; set; }
        public int ToId { get; set; }
        public bool Deleted { get; set; }
    }
}