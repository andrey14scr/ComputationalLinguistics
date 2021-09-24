using Microsoft.EntityFrameworkCore;

namespace ComputationalLinguistics.DAL.Core.Entities
{
    [Index(nameof(WordId), Name = "IWordId")]
    public class WordInText : IBaseEntity
    {
        public int Id { get; set; }
        public int TextId { get; set; }
        public virtual TextFile TextFile { get; set; }
        public int Seek { get; set; }
        public int WordId { get; set; }
        public virtual Word Word { get; set; }
    }
}