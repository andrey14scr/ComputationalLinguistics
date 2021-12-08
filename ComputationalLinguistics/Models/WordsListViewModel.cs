using System.Collections.Generic;

namespace ComputationalLinguistics.Models
{
    public class WordsListViewModel : SortBaseModel
    {
        public List<WordViewModel> Words { get; set; }
        public int WordsBlockSize { get; set; }
    }
}