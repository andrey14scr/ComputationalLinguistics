using Microsoft.EntityFrameworkCore;

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputationalLinguistics.DAL.Core.Entities
{
    [Index(nameof(TagName), Name = "ITagName")]
    public class TagInfo
    {
        public Guid Id { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string TagName { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string Info { get; set; }
    }
}
