using Nest;
using RelevantSearch.DataIndexer;

namespace RelevantSearch.IntegrationTests
{
    public class Playground
    {
        public void Foo()
        {
            var elasticClient = ElasticClientFactory.ElasticClient();

            elasticClient.Index(
                new Employee
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Age = 25,
                    About = "I love to go rock climbing",
                    Interests = new[] {"sports", "music"}
                }, d => d.Index("megacorp").Type("employee"));


            elasticClient.Get<Employee>(1);
        }

        class Employee
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
            public string About { get; set; }
            public string[] Interests { get; set; }
        }
    }
}