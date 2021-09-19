using System.Collections.Generic;

namespace ComputationalLinguistics.Models
{
    public class WordsList
    {
        public string SortBy { get; set; }
        public List<WordViewModel> Words { get; set; }
    }
}