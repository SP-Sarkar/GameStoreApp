using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Utility
{
    public static class SlugBuilder
    {
        public static string ToSlug(this string name)
        {
            name = name.Trim();
            if (name.Contains(" "))
                name = name.Replace(" ", "-");
            return name.ToLower(CultureInfo.InvariantCulture);
        }
    }
}
