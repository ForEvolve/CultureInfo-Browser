using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CultureInfoBrowser
{
    public class CultureService
    {
        public IEnumerable<LightCultureInfo> GetNeutralCultures()
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Where(x => x.IsNeutralCulture)
                .Select(x => new LightCultureInfo { EnglishName = x.EnglishName, Name = x.Name })
                ;
        }

        public IEnumerable<LightCultureInfo> GetCultureSpecific()
        {

            return CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Where(x => !x.IsNeutralCulture && !x.Equals(CultureInfo.InvariantCulture))
                .Select(x => new LightCultureInfo { EnglishName = x.EnglishName, Name = x.Name })
                ;
        }
    }

    public class LightCultureInfo
    {
        public string Name { get; set; }
        public string EnglishName { get; set; }
    }
}
