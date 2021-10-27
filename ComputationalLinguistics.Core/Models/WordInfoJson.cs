using System.Text.Json.Serialization;

namespace ComputationalLinguistics.Core.Models
{
    public class WordInfoJson
    {
        [JsonPropertyName("word")]
        public string Word { get; set; }
        [JsonPropertyName("prop")]
        public string Annotation { get; set; }
        [JsonPropertyName("offset")]
        public int OffSet { get; set; }
    }
}