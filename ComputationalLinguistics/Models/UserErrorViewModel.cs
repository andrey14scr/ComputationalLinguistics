using System.Collections.Generic;

namespace ComputationalLinguistics.Models
{
    public class UserErrorViewModel
    {
        public string Message { get; set; }
        public IEnumerable<string> InnerMessages { get; set; }
    }
}