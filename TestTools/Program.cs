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

    public class WordTag
    {
        public string Word { get; set; }
        public string Tag { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var text = "asfdasd[RR] bbb[TT] cvxv[OP] dsfsdfdfs[IP] ";
            var arr = text.Split(' ');
            var pairs = new List<WordTag>();

            foreach (var item in arr)
            {
                if (!string.IsNullOrWhiteSpace(item)) 
                {
                    var ind = item.IndexOf('[');
                    pairs.Add(new WordTag
                    {
                        Word = item.Substring(0, ind),
                        Tag = item.Substring(ind + 1, item.Length - ind - 2),
                    });
                }
            }

            Console.WriteLine();
        }
    }
}
