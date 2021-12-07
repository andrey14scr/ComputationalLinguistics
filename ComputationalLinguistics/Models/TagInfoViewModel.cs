using System;

namespace ComputationalLinguistics.Models
{
    public class TagInfoViewModel
    {
        public Guid Id { get; set; }

        public string TagName { get; set; }

        public string Info { get; set; }

        public int Frequency { get; set; }

        public bool IsGeneric { get; set; }
    }
}
