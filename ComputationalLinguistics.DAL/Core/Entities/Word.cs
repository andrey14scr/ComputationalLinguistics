using Microsoft.EntityFrameworkCore;

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputationalLinguistics.DAL.Core.Entities
{
    [Index(nameof(Content), nameof(TagInfoId), Name = "IWordInfo")]
    public class Word
    {
        public Guid Id { get; set; }
        
        [Column(TypeName = "nvarchar(120)")]
        public string Content { get; set; }

        [Column(TypeName = "nvarchar(120)")]
        public string Initial { get; set; }

        public Guid TagInfoId { get; set; }
        public virtual TagInfo TagInfo { get; set; }
    }
}