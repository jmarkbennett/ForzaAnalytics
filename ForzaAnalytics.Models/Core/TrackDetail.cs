namespace ForzaAnalytics.Models.Core
{
    public class TrackDetail
    {
        public string TrackId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string CountryCode { get; set; }
        public string Layout { get; set; }
        public string Distance { get; set; }
        public TrackDetail()
        {
            TrackId = string.Empty;
            Name = string.Empty;
            Location = string.Empty;
            CountryCode = string.Empty;
            Layout = string.Empty;
            Distance = string.Empty;
        }
        public string FullTrackName { get { return $"{Name} - {Layout}"; } }
    }
}