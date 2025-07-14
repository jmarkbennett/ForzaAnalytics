using ForzaAnalytics.Models.Core;
using ForzaAnalytics.UdpReader.Model;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;
using Track = ForzaAnalytics.Models.Core.Track;

namespace ForzaAnalytics.Services.Serializers
{
    public static class TrackDetailsSeralizer
    {
        public static List<Track> LoadTrackDetails()
        {
            var path = $"{Environment.CurrentDirectory}\\Resources\\Files\\Track Ordinals.csv";
            try
            {
                var results = new List<Track>();
                var rawContents = File.ReadAllLines(path);
                foreach (var row in rawContents)
                {
                    var split = row.Split(',');
                    var result = new Track();
                    result.TrackId = split[0];
                    result.Name = split[1]; 
                    result.Location = split[2]; 
                    result.CountryCode = split[3]; 
                    result.Layout = split[4];
                    result.Distance = split[5];
                    results.Add(result);
                }
                return results;
            }
            catch (Exception e)
            {
                return new List<Track>();
            }
        }
    }
}