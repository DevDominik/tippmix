using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;

namespace Tippmixx
{
    public class Misc
    {
        public static PackIconKind GetPackIconKindFromString(string iconName)
        {
            // Try parsing the string to a PackIconKind enum
            if (Enum.TryParse(iconName, true, out PackIconKind iconKind))
            {
                return iconKind;
            }
            else
            {
                throw new ArgumentException($"'{iconName}' is not a valid PackIconKind.");
            }
        }
    }
}
