using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace GetChampIDs
{
    class Program
    {
        static void Main(string[] args)
        {
            string json = System.IO.File.ReadAllText("C:/Users/kelvi/Documents/Visual Studio 2019/Projects/League/GetChampIDs/champion.json");
            var top = JObject.Parse(json);
            var inner = top["data"].Value<JObject>();
            

            //var innerprops = inner.Properties();
            List<string> champNames = inner.Properties().Select(p => p.Name).ToList();

            foreach (string k in champNames)
            {
                var innerinner = inner[k];

                

                foreach (string j in championdata)
                {
                    Console.WriteLine(j);
                }
            }

        }
    }
}
