using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;
using Nest;
using ShellProgressBar;

namespace RelevantSearch.DataIndexer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Indexing data ..");

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "branches.zip");
            var branches = LoadBranches(filePath);

            var elasticClient = ElasticClientFactory.ElasticClient();

            DeleteIndex(elasticClient);
            CreateIndex(elasticClient);
            //CreateIndexSynonymsSupport(elasticClient);

            var size = 800;
            int totalTicks = branches.Count() / size;
            var options = new ProgressBarOptions
            {
                ProgressCharacter = '─',
                ProgressBarOnBottom = true
            };

            using (var pbar = new ProgressBar(totalTicks, "Initial message", options))
            {
                using (var bulkAllObservable = elasticClient.BulkAll(branches, d => d
                    .Size(size)
                    .MaxDegreeOfParallelism(8)
                    .RefreshOnCompleted()))
                {
                    var bulkObserver = bulkAllObservable.Wait(TimeSpan.FromMinutes(10),
                        response =>
                        {
                            pbar.Tick("of data indexed.");
                        });
                }
            }

            Console.WriteLine("Data indexed.");
            Console.ReadKey();
        }

        private static void DeleteIndex(ElasticClient elasticClient)
        {
            var deleteIndexResponse = elasticClient.DeleteIndex(elasticClient.ConnectionSettings.DefaultIndex);
            if (deleteIndexResponse.IsValid) Console.WriteLine($"Old index {Strings.IndexName} deleted");
        }

        private static void CreateIndex(ElasticClient elasticClient)
        {
            var createIndexResponse = elasticClient.CreateIndex(elasticClient.ConnectionSettings.DefaultIndex);
            if (createIndexResponse.IsValid) Console.WriteLine($"New index {Strings.IndexName} created");
        }
        private static void CreateIndexSynonymsSupport(ElasticClient elasticClient)
        {
            var createIndexResponse = elasticClient
                .CreateIndex(elasticClient.ConnectionSettings.DefaultIndex, i => i.Settings(s => s
                    .Analysis(a => a
                        .Analyzers(an => an.Custom("synonyms_analyzer", aa => aa
                            .Tokenizer("standard")
                            .Filters("lowercase", "synonyms")))
                        .TokenFilters(tf => tf
                            .Lowercase("lowercase", l => l)
                            .Synonym("synonyms", sy => sy
                                .Synonyms("K&S, K and S, Kozey & Sons => Kozey and Sons"))))));

            if (createIndexResponse.IsValid) Console.WriteLine($"New index {Strings.IndexName} created");
        }

        private static IEnumerable<Branch> LoadBranches(string filePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var zipFile = ZipFile.Read(filePath))
            {
                var zipEntry = zipFile["branches.txt"];

                bool skip = false;
                using (var s = new StreamReader(zipEntry.OpenReader()))
                {
                    while (!s.EndOfStream)
                    {
                        var readLine = s.ReadLine();
                        if (!skip)
                        {
                            skip = true;
                            continue;
                        }
                        var strings = readLine.Split(";");

                        var goe = strings[9].Split(",");

                        yield return new Branch
                        {
                            Id = Convert.ToInt32(strings[0]),
                            LocationName = strings[1],
                            LocationShortName = strings[2],
                            LocationAdress1 = strings[3],
                            LocationCounty = strings[4],
                            LocationNumber = strings[5],
                            LocationZipCode = strings[6],
                            LocationContact = strings[7],
                            IsAtm = Convert.ToBoolean(strings[8]),
                            Location = new GeoLocation(Convert.ToDouble(goe[0]), Convert.ToDouble(goe[1]))
                        };
                    }
                }
            }
        }
    }
}
