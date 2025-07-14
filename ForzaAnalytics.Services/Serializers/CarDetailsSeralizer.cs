using ForzaAnalytics.Models.Core;
using ForzaAnalytics.UdpReader.Model;
using System.IO;
using System.Text.Json;
using Car = ForzaAnalytics.Models.Core.Car;

namespace ForzaAnalytics.Services.Serializers
{
    public static class CarDetailsSeralizer
    {
        public static List<Car> LoadCarDetails()
        {
            var path = $"{Environment.CurrentDirectory}\\Resources\\Files\\ManteoMaxs FM Spreadsheet.csv";
            try
            {
                var result = new List<Car>();
                var rawContents = File.ReadAllLines(path);
                foreach (var row in rawContents)
                {
                    var split = row.Split(',');
                    var car_detail = new Car();
                    car_detail.YearMakeModel = split[0];
                    car_detail.NickName = split[1]; 
                    car_detail.CarId = split[2]; 
                    car_detail.Year = split[3]; 

                    car_detail.Make = split[4];
                    car_detail.Model = split[5];
                    car_detail.Division = split[6];
                    car_detail.Spec = split[7];
                    car_detail.Pi = split[8];
                    car_detail.Class = split[9];
                    result.Add(car_detail);
                }
                return result;
            }
            catch (Exception e)
            {
                return new List<Car>();
            }
        }
    }
}