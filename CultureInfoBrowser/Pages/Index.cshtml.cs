using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CultureInfoBrowser.Pages
{
    public class IndexModel : PageModel
    {
        private readonly CultureService _cultureService;
        private readonly TelemetryClient _telemetryClient;
        private readonly Stopwatch _timer = new Stopwatch();

        public IndexModel(CultureService cultureService, TelemetryClient telemetryClient)
        {
            
            _cultureService = cultureService ?? throw new ArgumentNullException(nameof(cultureService));
            _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
        }

        public IEnumerable<LightCultureInfo> NeutralCultures { get; private set; }
        public IEnumerable<LightCultureInfo> CultureSpecific { get; private set; }

        [BindProperty(SupportsGet = true)]
        public string NameCode { get; set; }

        [BindProperty(SupportsGet = true)]
        public string EnglishName { get; set; }

        [BindProperty(SupportsGet = true)]
        public FilterType FilterType { get; set; }

        public void OnGet()
        {
            GetInfo();
            FilterInfo();
        }

        private void GetInfo()
        {
            NeutralCultures = _cultureService.GetNeutralCultures();
            CultureSpecific = _cultureService.GetCultureSpecific();
        }

        private void FilterInfo()
        {
            _timer.Start();
            var telemetryName = "Nothing";
            var hasNameCodeFilter = !string.IsNullOrWhiteSpace(NameCode);
            var hasEnglishNameFilter = !string.IsNullOrWhiteSpace(EnglishName);

            if (hasNameCodeFilter && hasEnglishNameFilter)
            {
                if(FilterType == FilterType.And)
                {
                    NeutralCultures = NeutralCultures
                        .Where(c => c.Name.Contains(NameCode, StringComparison.InvariantCultureIgnoreCase) && c.EnglishName.Contains(EnglishName, StringComparison.InvariantCultureIgnoreCase));
                    CultureSpecific = CultureSpecific
                        .Where(c => c.Name.Contains(NameCode, StringComparison.InvariantCultureIgnoreCase) && c.EnglishName.Contains(EnglishName, StringComparison.InvariantCultureIgnoreCase));
                    telemetryName = "NameAndEnglishName";
                }
                else
                {
                    NeutralCultures = NeutralCultures
                        .Where(c => c.Name.Contains(NameCode, StringComparison.InvariantCultureIgnoreCase) || c.EnglishName.Contains(EnglishName, StringComparison.InvariantCultureIgnoreCase));
                    CultureSpecific = CultureSpecific
                        .Where(c => c.Name.Contains(NameCode, StringComparison.InvariantCultureIgnoreCase) || c.EnglishName.Contains(EnglishName, StringComparison.InvariantCultureIgnoreCase));
                    telemetryName = "NameOrEnglishName";
                }
            }
            else if (hasNameCodeFilter)
            {
                NeutralCultures = NeutralCultures.Where(c => c.Name.Contains(NameCode, StringComparison.InvariantCultureIgnoreCase));
                CultureSpecific = CultureSpecific.Where(c => c.Name.Contains(NameCode, StringComparison.InvariantCultureIgnoreCase));
                telemetryName = "Name";
            }
            else if (hasEnglishNameFilter)
            {
                NeutralCultures = NeutralCultures.Where(c => c.EnglishName.Contains(EnglishName, StringComparison.InvariantCultureIgnoreCase));
                CultureSpecific = CultureSpecific.Where(c => c.EnglishName.Contains(EnglishName, StringComparison.InvariantCultureIgnoreCase));
                telemetryName = "EnglishName";
            }
            _timer.Stop();
            TrackEventTelemetry(telemetryName);
        }

        private void TrackEventTelemetry(string name)
        {
            var telemetry = new EventTelemetry($"FilterCultureBy{name}");
            telemetry.Properties.Add("NameCode", NameCode);
            telemetry.Properties.Add("EnglishName", EnglishName);
            telemetry.Properties.Add("FilterType", FilterType.ToString());
            telemetry.Metrics.Add("NeutralCultures.Count", NeutralCultures.Count());
            telemetry.Metrics.Add("CultureSpecific.Count", CultureSpecific.Count());
            telemetry.Metrics.Add("FilterTime (ms)", _timer.ElapsedMilliseconds);
            telemetry.Metrics.Add("FilterTime (ticks)", _timer.ElapsedTicks);
            _telemetryClient.TrackEvent(telemetry);
        }
    }

    public enum FilterType
    {
        Or,
        And
    }
}
