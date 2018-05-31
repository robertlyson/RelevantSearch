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

            var searchResponse = ElasticClient().Search<Branch>(s => s);

            searchResponse.IsValid.ShouldBe(true);

            var actual = searchResponse.Documents.ToList();

            actual[0].LocationName.ShouldContain("morgan");
        }

        private static ElasticClient ElasticClient()
        {
            return ElasticClientFactory.ElasticClient();
        }
    }
}