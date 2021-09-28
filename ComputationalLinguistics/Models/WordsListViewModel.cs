using System.Collections.Generic;

namespace ComputationalLinguistics.Models
{
    public class WordsListViewModel
    {
        public string SortBy { get; set; }
        public List<WordViewModel> Words { get; set; }
        public string Pattern { get; set; }
        public int WordsBlockSize { get; set; }
    }
}