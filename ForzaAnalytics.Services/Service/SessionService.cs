using ForzaAnalytics.Models.Core;
using ForzaAnalytics.Services.Serializers;
using ForzaAnalytics.UdpReader.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForzaAnalytics.Services.Service
{
    public class SessionService
    {
        private List<Models.Core.CarDetail> carDetails;
        private List<Models.Core.TrackDetail> trackDetails;

        private ObservableCollection<Session> Sessions;
        public void Update(Telemetry payload)
        {
             
        }

        public SessionService()
        {
            carDetails = CarDetailsSeralizer.LoadCarDetails($"{Environment.CurrentDirectory}\\Resources\\Files\\ManteoMaxs FM Spreadsheet.csv");
            trackDetails = TrackDetailsSeralizer.LoadTrackDetails($"{Environment.CurrentDirectory}\\Resources\\Files\\Track Ordinals.csv");
            Sessions = [];
        }
    }
}
