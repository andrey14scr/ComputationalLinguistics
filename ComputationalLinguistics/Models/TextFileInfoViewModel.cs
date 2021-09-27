using System.Collections.Generic;

namespace ComputationalLinguistics.Models
{
    public class TextFileInfoViewModel
    {
        public string FileName { get; set; }
        public string Text { get; set; }
        public IEnumerable<int> Seeks { get; set; }
    }
}