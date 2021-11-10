using System.Collections.Generic;

namespace ComputationalLinguistics.Core.Models
{
    public class WordForms
    {
        public string Initial { get; set; }
        public List<(string Word, string Tag)> Forms { get; set; }
    }
}