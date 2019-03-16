using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CultureInfoBrowser.Pages
{
    // TODO:
    // Add a "AND/OR" dropdown list in the filters
    // Make the filter work on GET so it is possible to link to a filtered page
    //

    public class IndexModel : PageModel
    {
        private readonly CultureService _cultureService;

        public IndexModel(CultureService cultureService)
        {
            _cultureService = cultureService ?? throw new ArgumentNullException(nameof(cultureService));
        }

        public IEnumerable<LightCultureInfo> NeutralCultures { get; private set; }
        public IEnumerable<LightCultureInfo> CultureSpecific { get; private set; }

        [BindProperty]
        public string NameCode { get; set; }
        [BindProperty]
        public string EnglishName { get; set; }

        public void OnGet()
        {
            GetInfo();
            FilterInfo();
        }

        public void OnPost()
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
            var hasNameCodeFilter = !string.IsNullOrWhiteSpace(NameCode);
            var hasEnglishNameFilter = !string.IsNullOrWhiteSpace(EnglishName);

            if (hasNameCodeFilter && hasEnglishNameFilter)
            {
                NeutralCultures = NeutralCultures
                    .Where(c => c.Name.Contains(NameCode, StringComparison.InvariantCultureIgnoreCase) || c.EnglishName.Contains(EnglishName, StringComparison.InvariantCultureIgnoreCase));
                CultureSpecific = CultureSpecific
                    .Where(c => c.Name.Contains(NameCode, StringComparison.InvariantCultureIgnoreCase) || c.EnglishName.Contains(EnglishName, StringComparison.InvariantCultureIgnoreCase));
            }
            else if (hasNameCodeFilter)
            {
                NeutralCultures = NeutralCultures.Where(c => c.Name.Contains(NameCode, StringComparison.InvariantCultureIgnoreCase));
                CultureSpecific = CultureSpecific.Where(c => c.Name.Contains(NameCode, StringComparison.InvariantCultureIgnoreCase));
            }
            else if (hasEnglishNameFilter)
            {
                NeutralCultures = NeutralCultures.Where(c => c.EnglishName.Contains(EnglishName, StringComparison.InvariantCultureIgnoreCase));
                CultureSpecific = CultureSpecific.Where(c => c.EnglishName.Contains(EnglishName, StringComparison.InvariantCultureIgnoreCase));
            }
        }
    }
}
