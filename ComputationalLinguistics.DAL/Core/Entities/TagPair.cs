using System;

namespace ComputationalLinguistics.DAL.Core.Entities
{
    public class TagPair
    {
        public Guid Id { get; set; }

        public Guid CurrentId { get; set; }
        public virtual TagInfo Current { get; set; }

        public Guid NextId { get; set; }
        public virtual TagInfo Next { get; set; }
    }
}
