using ForzaAnalytics.Models.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System;
namespace ForzaAnalytics.Web.Pages
{
    public class TelemetryModel : PageModel
    {
        [BindProperty]
        public IFormFile TelemetryFile { get; set; }
        public GroupedExtendedPositionalData Telemetry { get; set; }
        public GroupedPositionalData Map { get; set; }

        public TelemetryModel()
        {
            Telemetry = new GroupedExtendedPositionalData();
            Map = new GroupedPositionalData();
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(TelemetryFile != null && TelemetryFile.Length > 0)
            {
                using (var reader = new StreamReader(TelemetryFile.OpenReadStream()))
                {
                    List<GroupedExtendedPositionalData> telemetry = new List<GroupedExtendedPositionalData>();
                    Telemetry = JsonSerializer.Deserialize<GroupedExtendedPositionalData>(await reader.ReadToEndAsync()) ?? new GroupedExtendedPositionalData();
                
                }
            }
            if(Telemetry?.TrackId != null)
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Maps", $"{Telemetry.TrackId}.fzmap");
                if (System.IO.File.Exists(path))
                {
                    using (var reader = new StreamReader(path))
                    {
                        List<GroupedPositionalData> telemetry = new List<GroupedPositionalData>();
                        Map = JsonSerializer.Deserialize<GroupedPositionalData>(await reader.ReadToEndAsync()) ?? new GroupedPositionalData();
                    }
                }
            }

            return Page();
        }
    }
}
