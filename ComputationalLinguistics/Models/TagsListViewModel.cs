using System.Collections.Generic;

namespace ComputationalLinguistics.Models
{
    public class TagsListViewModel
    {
        public List<TagInfoViewModel> Tags { get; set; }

        public string SortBy { get; set; }

        public string Pattern { get; set; }
    }
}
