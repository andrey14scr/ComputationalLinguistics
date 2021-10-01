using System;

namespace ComputationalLinguistics.Core.Dto
{
    public class WordInTextDto
    {
        public Guid TextFileId { get; set; }
        public int Seek { get; set; }
        public Guid WordId { get; set; }
    }
}