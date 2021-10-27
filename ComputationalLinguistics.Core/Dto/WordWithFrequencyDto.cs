using System;

namespace ComputationalLinguistics.Core.Dto
{
    public class WordWithFrequencyDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string Annotation { get; set; }
        public int Frequency { get; set; }
    }
}