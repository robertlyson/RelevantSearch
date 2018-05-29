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
        public async Task DealingWithTypos()
        {
            var lookingFor = "mogran";

            var searchResponse = await ElasticClient().SearchAsync<Branch>(s => s
                .Query(q => q.Fuzzy(fuz => fuz.Value(lookingFor).Field(f => f.LocationContact))));

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

            var actual = searchResponse.Documents.ToList();

            actual[0].LocationName.ShouldBe("JP Morgan Retha, Ernser and Treutel");
            actual[1].LocationName.ShouldBe("Kozey and Sons");
        }

        [Test]
        //remember to enable synonyms support during index creation
        public async Task SynonymsSupport()
        {
            var lookingFor = "K and S";

            var searchResponse = await ElasticClient().SearchAsync<Branch>(s => s
                .Query(q => q.Match(m => m
                    .Query(lookingFor).Field(f => f.LocationName).Analyzer("synonyms_analyzer"))));

            var actual = searchResponse.Documents.ToList();

            actual[0].LocationName.ShouldBe("Kozey and Sons");
        }

        [Test]
        public void JoiningItAll()
        {

        }

        private static ElasticClient ElasticClient()
        {
            return ElasticClientFactory.ElasticClient();
        }
    }
}