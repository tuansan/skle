namespace Nop.Core.Domain.Skle
{
    public partial class Field : BaseEntity
    {
        public string Name { get; set; }
        public int StatusId { get; set; }
        public bool Deleted { get; set; }
    }
}