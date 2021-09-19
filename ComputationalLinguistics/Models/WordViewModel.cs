using System;

namespace ComputationalLinguistics.Models
{
    public class WordViewModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public int Frequency { get; set; }
    }
}