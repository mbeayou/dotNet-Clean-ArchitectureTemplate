using System.Globalization;

namespace Anis.Template.Domain.Helper
{
    public class CultureHelper
    {
        public static string Read(string arabic, string english)
            => CultureInfo.CurrentCulture.Name == "ar" ? arabic : english;
    }
}
