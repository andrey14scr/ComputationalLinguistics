using Microsoft.EntityFrameworkCore;

using System;

namespace ComputationalLinguistics.DAL.Core.Entities
{
    [Index(nameof(WordId), Name = "IWordId")]
    [Index(nameof(TextFileId), nameof(Seek), Name = "ITextFileId")]
    public class WordInText
    {
        public Guid TextFileId { get; set; }
        public virtual TextFile TextFile { get; set; }
        public int Seek { get; set; }
        public Guid WordId { get; set; }
        public virtual Word Word { get; set; }
    }
}