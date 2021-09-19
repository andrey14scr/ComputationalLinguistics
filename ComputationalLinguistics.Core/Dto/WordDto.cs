using System;

namespace ComputationalLinguistics.Core.Dto
{
    public class WordDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string Frequency { get; set; }
    }
}