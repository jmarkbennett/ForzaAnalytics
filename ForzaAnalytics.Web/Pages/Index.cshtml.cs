using ForzaAnalytics.Models.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Text.Json;

namespace ForzaAnalytics.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public IFormFile File { get; set; }
        public string FileContent { get; set; }

        public GroupedExtendedPositionalData Telemetry { get; set; }
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            Telemetry = new GroupedExtendedPositionalData();
            FileContent = string.Empty;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(File !=null && File.Length > 0)
            {
                using (var reader = new StreamReader(File.OpenReadStream())) {
                    FileContent = await reader.ReadToEndAsync();
                    List<GroupedExtendedPositionalData> telemetry = new List<GroupedExtendedPositionalData>();
                    Telemetry = JsonSerializer.Deserialize<GroupedExtendedPositionalData>(FileContent) ?? new GroupedExtendedPositionalData();
                }
            }
            return Page();
        }
    }
}
