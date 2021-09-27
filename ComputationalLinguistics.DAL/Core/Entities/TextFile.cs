using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ComputationalLinguistics.DAL.Core.Entities
{
    [Index(nameof(FilePath), Name = "IFilePath")]
    public class TextFile : IBaseEntity
    {
        public Guid Id { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string FilePath { get; set; }
    }
}