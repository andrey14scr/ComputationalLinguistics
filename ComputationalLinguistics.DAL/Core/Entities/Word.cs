using System;

namespace ComputationalLinguistics.DAL.Core.Entities
{
    public class Word : IBaseEntity
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string Frequency { get; set; }
    }
}