using System;

namespace Nop.Core.Domain.Skle
{
    public partial class MyNotification : BaseEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int TypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Times { get; set; }
        public string Targets { get; set; }
    }
}