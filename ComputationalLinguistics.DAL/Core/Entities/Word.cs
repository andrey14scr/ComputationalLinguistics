using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputationalLinguistics.DAL.Core.Entities
{
    public class Word : IBaseEntity
    {
        public Guid Id { get; set; }
        
        [Column(TypeName = "nvarchar(120)")]
        public string Content { get; set; }
        
        public int Frequency { get; set; }
    }
}