using Nest;

namespace RelevantSearch.DataIndexer
{
    public class Branch
    {
        public int Id { get; set; }
        public string UniqueCode { get; set; }
        public string LocationName { get; set; }
        public string LocationShortName { get; set; }
        public string LocationNumber { get; set; }
        public string LocationAdress1 { get; set; }
        public string LocationCounty { get; set; }
        public string LocationZipCode { get; set; }
        public string LocationContact { get; set; }
        public GeoLocation Location { get; set; }
        public bool IsAtm { get; set; }
    }
}