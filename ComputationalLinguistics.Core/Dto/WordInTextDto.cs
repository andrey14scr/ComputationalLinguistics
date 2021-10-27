using System;

namespace ComputationalLinguistics.Core.Dto
{
    public class WordInTextDto
    {
        public Guid TextFileId { get; set; }
        public int OffSet { get; set; }
        public Guid WordId { get; set; }
    }
}