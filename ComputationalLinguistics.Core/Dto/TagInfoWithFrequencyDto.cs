using System;

namespace ComputationalLinguistics.Core.Dto
{
    public class TagInfoWithFrequencyDto
    {
        public Guid Id { get; set; }

        public string TagName { get; set; }

        public string Info { get; set; }

        public int Frequency { get; set; }

        public bool IsGeneric { get; set; }
    }
}
