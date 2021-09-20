using Microsoft.EntityFrameworkCore;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace ComputationalLinguistics.DAL.Core.Entities
{
    [Index(nameof(Content), Name = "IContent")]
    public class Word : IBaseEntity
    {
        public Guid Id { get; set; }
        
        [Column(TypeName = "nvarchar(120)")]
        public string Content { get; set; }
        
        public int Frequency { get; set; }
    }
}