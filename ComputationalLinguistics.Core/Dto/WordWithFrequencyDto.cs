using System;

namespace ComputationalLinguistics.Core.Dto
{
    public class WordWithFrequencyDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string Tag { get; set; }
        public int Frequency { get; set; }
        public string Initial { get; set; }
    }
}