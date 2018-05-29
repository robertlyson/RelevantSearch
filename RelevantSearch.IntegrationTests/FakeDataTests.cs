using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Bogus;
using Nest;
using NUnit.Framework;
using RelevantSearch.DataIndexer;

namespace RelevantSearch.IntegrationTests
{
    public class FakeDataTests
    {
        [Test]
        //[NUnit.Framework.Ignore("Just for data generation")]
        public void GenerateFakeData()
        {
            var path = @"C:\tmp\branches.txt";

            using (StreamWriter file = new StreamWriter(path))
            {
                file.WriteLine(
                    "Id,LocationName,LocationShortName,LocationAdress1,LocationCounty,LocationNumber,LocationZipCode,LocationContact,IsAtm,Location");

                long i = 0;
                foreach (var item in FakeData.Branches.Take(1000000))
                {
                    file.WriteLine(
                        $"{item.Id};{item.LocationName};{item.LocationShortName};" +
                        $"{item.LocationAdress1};{item.LocationCounty};{item.LocationNumber};" +
                        $"{item.LocationZipCode};{item.LocationContact};{item.IsAtm};{item.Location}");
                    i++;

                    if (i % 1000 == 0) Debug.WriteLine(i);
                }
            }
        }
    }

    class FakeData
    {
        protected static int IdState = 0;

        public static Faker<Branch> Generator { get; } =
            new Faker<Branch>()
                .RuleFor(b => b.Id, b => IdState++)
                .RuleFor(b => b.LocationName, b => b.Company.CompanyName())
                .RuleFor(b => b.LocationShortName, b => b.Random.AlphaNumeric(3).ToUpper())
                .RuleFor(b => b.LocationAdress1, b => b.Address.StreetAddress())
                .RuleFor(b => b.LocationCounty, b => b.Address.County())
                .RuleFor(b => b.LocationNumber, b => b.Random.Number(1, 100000).ToString())
                .RuleFor(b => b.LocationZipCode, b => b.Address.ZipCode())
                .RuleFor(b => b.LocationContact, b => b.Name.FullName())
                .RuleFor(b => b.IsAtm, b => b.Random.Bool())
                .RuleFor(b => b.Location, b => new GeoLocation(b.Random.Number(-90, 90), b.Random.Number(-180, 180)));


        public static IEnumerable<Branch> Branches { get; } =
            FakeData.Generator.GenerateForever();
    }
}
