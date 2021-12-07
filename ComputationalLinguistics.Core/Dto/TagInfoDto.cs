using System;

namespace ComputationalLinguistics.Core.Dto
{
    public class TagInfoDto
    {
        public Guid Id { get; set; }

        public string TagName { get; set; }

        public string Info { get; set; }

        public bool IsGeneric { get; set; }
    }
}
