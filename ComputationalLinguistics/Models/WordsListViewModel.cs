using System.Collections.Generic;

namespace ComputationalLinguistics.Models
{
    public class WordsListViewModel
    {
        public const string OnAlphabet = "_abb";
        public const string OnFrequency = "_freq";
        public const string OnPattern = "_pattern";

        public string SortBy { get; set; }
        public List<WordViewModel> Words { get; set; }
        public string Pattern { get; set; }
        public int WordsBlockSize { get; set; }
    }
}