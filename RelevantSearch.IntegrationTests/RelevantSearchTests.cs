using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ionic.Zip;
using Nest;
using NUnit.Framework;
using RelevantSearch.DataIndexer;
using Shouldly;

namespace RelevantSearch.IntegrationTests
{
    public class RelevantSearchTests
    {
        [Test]
        public async Task CorrectVersionOfElasticsearchIsRunning()
        {
            var elasticClient = ElasticClient();

            var response = await elasticClient.RootNodeInfoAsync();
            var actual = response.IsValid;

            Assert.IsTrue(actual, $"Couldn't connect to elasticsearch {response.ServerError}");

            var version = response.Version.Number;

            Assert.AreEqual("6.2.4", version);
        }

        [Test]

        public async Task IndexContainsData()
        {
            var actual = (await ElasticClient()
                .CountAsync<Branch>()).Count;

            actual.ShouldBe(1000000);
        }

        [Test]
        public async Task FindBranchesByName()
        {
            var lookingFor = "morgan";

            var searchResponse = ElasticClient().Search<Branch>(s => s.Query(q => q
                .Match(m => m.Field(f => f.LocationName).Query(lookingFor))));

            searchResponse.IsValid.ShouldBe(true);

            var actual = searchResponse.Documents.ToList();

            actual[0].LocationName.ShouldBe("JP Morgan Retha, Ernser and Treutel");
        }

        [Test]
        public async Task LookingForExactValues()
        {
            var searchResponse = await ElasticClient().SearchAsync<Branch>(s => s
                .Query(q => q.Term(t => t
                    .Field(f => f.LocationZipCode.Suffix("keyword")).Value("48827-3158"))));

            searchResponse.IsValid.ShouldBe(true);

            var actual = searchResponse.Documents.ToList();

            actual[0].LocationName.ShouldBe("Bruen, Konopelski and Leffler");
        }

        [Test]
        public async Task MultiMatch()
        {
            var lookingFor = "jp morgan";

            var searchResponse = await ElasticClient().SearchAsync<Branch>(s => s
                .Query(q => q.MultiMatch(mm => mm
                    .Query(lookingFor)
                    .MinimumShouldMatch(MinimumShouldMatch.Percentage(100.0))
                    .Fields(f => f
                        .Fields(ff => ff.LocationName, ff => ff.LocationContact)))));

            searchResponse.IsValid.ShouldBe(true);

            var actual = searchResponse.Documents.ToList();

            actual[0].LocationName.ShouldBe("JP Morgan Retha, Ernser and Treutel");
        }

        [Test]
        public async Task DealingWithTypos()
        {
            var lookingFor = "mogran";

            var searchResponse = await ElasticClient().SearchAsync<Branch>(s => s
                .Query(q => q.Fuzzy(fuz => fuz.Value(lookingFor).Field(f => f.LocationContact))));

            searchResponse.IsValid.ShouldBe(true);

            var actual = searchResponse.Documents.ToList();

            actual[0].LocationContact.ShouldBe("Morgan Morar");
        }

        [Test]
        public async Task BoostingFields()
        {
            var lookingFor = "morgan";

            var searchResponse = await ElasticClient().SearchAsync<Branch>(s => s
                .Query(q => q.Bool(b => b
                    .Should(
                        sh => sh
                            .Match(m => m
                                .Query(lookingFor)
                                .Field(f => f.LocationName)
                                .Boost(2.0)),
                        sh => sh
                            .Match(m => m
                                .Query(lookingFor)
                                .Field(f => f.LocationContact)
                                .Boost(0.5))))));

            searchResponse.IsValid.ShouldBe(true);

            var actual = searchResponse.Documents.ToList();

            actual[0].LocationName.ShouldBe("JP Morgan Retha, Ernser and Treutel");
            actual[1].LocationName.ShouldBe("Kozey and Sons");
        }

        [Test]
        public async Task SynonymsSupport()
        {
            var lookingFor = "K and S";

            var searchResponse = await ElasticClient().SearchAsync<Branch>(s => s);

            searchResponse.IsValid.ShouldBe(true);

            var actual = searchResponse.Documents.ToList();

            actual[0].LocationName.ShouldBe("Kozey and Sons");
        }

        private static ElasticClient ElasticClient()
        {
            return ElasticClientFactory.ElasticClient();
        }
    }
}