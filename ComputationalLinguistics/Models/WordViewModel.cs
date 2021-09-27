using System;
using System.Collections.Generic;
using ComputationalLinguistics.Core.Models;

namespace ComputationalLinguistics.Models
{
    public class WordViewModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public int Frequency { get; set; }
        public IEnumerable<WordContextFile> WordContextFiles { get; set; }
    }
}