using System.Collections.Generic;

namespace ComputationalLinguistics.Models
{
    public class TagPairsListViewModel : SortBaseModel
    {
        public IEnumerable<TagPairModel> TagPairs { get; set; }
    }
}
