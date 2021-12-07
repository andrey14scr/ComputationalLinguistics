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
        static void Main(string[] args)
        {
            var emptyTag = Guid.Parse("00000000000000000000000000000001");
            Console.WriteLine(emptyTag);
        }
    }
}
