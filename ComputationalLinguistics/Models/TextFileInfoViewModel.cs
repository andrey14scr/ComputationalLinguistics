using System;
using System.Collections.Generic;

namespace ComputationalLinguistics.Models
{
    public class TextFileInfoViewModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string Text { get; set; }
        public IEnumerable<int> OffSet { get; set; }
        public string Word { get; set; }
    }
}