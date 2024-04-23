using ForzaAnalytics.Models.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.IO;
using System;
using ForzaAnalytics.Services.Service;
using System.Reflection.PortableExecutable;
using ForzaAnalytics.UdpReader.Model;
namespace ForzaAnalytics.Web.Pages
{
    public class LapTimesModel : PageModel
    {
        [BindProperty]
        public IFormFile TelemetryFile { get; set; }
        public GroupedExtendedPositionalData Telemetry { get; set; }

        public List<LapTime> LapTimes { get; set; }

        public LapTimesModel()
        {
            Telemetry = new GroupedExtendedPositionalData();
            LapTimes = new List<LapTime>();
        }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync(string loadTelemetry)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "SampleTelemetry", "kyalami_example_tel.fztel");
            if(loadTelemetry == "loadSample")
            {
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    MemoryStream memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    TelemetryFile = new FormFile(memoryStream, 0, memoryStream.Length, null, Path.GetFileName(path));
                }
            }

            if (TelemetryFile != null && TelemetryFile.Length > 0)
            {
                using (var reader = new StreamReader(TelemetryFile.OpenReadStream()))
                {
                    LapTimes = new List<LapTime>();
                    Telemetry = JsonSerializer.Deserialize<GroupedExtendedPositionalData>(await reader.ReadToEndAsync()) ?? new GroupedExtendedPositionalData();

                    var laps = Telemetry.ExtendedPositions.Select(i => i.LapNumber).Distinct().ToList();
                    foreach (var lap in laps)
                    {
                        var row = new LapTime();
                        var lapInfo = Telemetry.ExtendedPositions.Select(i => i).Where(i => i.LapNumber == lap).ToList();

                        row.LapNumber = lap;
                        row.AverageSpeed = lapInfo.Average(i => i.Speed_Mph);
                        row.MaxSpeed = lapInfo.Max(i => i.Speed_Mph);
                        row.MinSpeed = lapInfo.Min(i => i.Speed_Mph);
                        row.TimeInSeconds = lapInfo.Max(i => i.LapTime);
                        row.PercentBrakeApplied = (double)lapInfo.Where(i => i.Brake > 0).Count() / (double)lapInfo.Count();
                        row.PercentFullThrottle = (double)lapInfo.Where(i => i.Acceleration == 100).Count() / (double)lapInfo.Count();
                        row.PercentCoasting = (double)lapInfo.Where(i=>i.Acceleration == 0 && i.Brake == 0 && i.Handbrake == 0).Count() / (double)lapInfo.Count();
                        row.FuelRemaining = (double)lapInfo.Min(i => i.FuelRemaining);
                        row.FuelUsed = (double)lapInfo.Max(i => i.FuelRemaining) - (double)lapInfo.Min(i => i.FuelRemaining);

                        LapTimes.Add(row);
                    }

                    if (laps.Count > 0)
                    {
                        var bestLap = LapTimes.Min(i => i.TimeInSeconds);
                        foreach (var lap in LapTimes)
                        {
                            if (lap.TimeInSeconds == bestLap)
                                lap.IsBestLap = true;
                        }
                    }
                }
            }

            return Page();
        }
    }
}
