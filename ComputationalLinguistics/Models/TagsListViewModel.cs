using System.Collections.Generic;

namespace ComputationalLinguistics.Models
{
    public class TagsListViewModel : SortBaseModel
    {
        public List<TagInfoViewModel> Tags { get; set; }
    }
}
