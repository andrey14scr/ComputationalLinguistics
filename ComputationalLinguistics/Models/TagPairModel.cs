using ComputationalLinguistics.Core.Dto;

namespace ComputationalLinguistics.Models
{
    public class TagPairModel
    {
        public string FirstTag { get; set; }
        public string SecondTag { get; set; }
        public int Frequency { get; set; }
    }
}
