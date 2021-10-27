using System;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

using static System.Net.Mime.MediaTypeNames;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection.Metadata;

namespace TestTools
{
    class WordInfoJson
    {
        [JsonPropertyName("word")]
        public string Word { get; set; }
        [JsonPropertyName("prop")]
        public string Prop { get; set; }
        [JsonPropertyName("offSet")]
        public int OffSet { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var values = new Dictionary<string, string>
            {
                { "text", "I accepted         your answer   as good because    it         is much more simpler and it is clearer and simpler." },
            };

            var content = new FormUrlEncodedContent(values);

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await httpClient.PostAsync("http://127.0.0.1:5000/texts?", content);

                var responseString = await response.Content.ReadAsStringAsync();

                using (var doc = JsonDocument.Parse(responseString))
                {
                    var root = doc.RootElement;
                    var answerElement = root.GetProperty("answer");

                    var answer = JsonSerializer.Deserialize<List<WordInfoJson>>(answerElement.GetString());

                    foreach (var item in answer)
                    {
                        Console.WriteLine(item.Word);
                    }
                }
            }
        }
    }
}
