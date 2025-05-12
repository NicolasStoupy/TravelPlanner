using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Presentation.MAUI.Models
{
    public class MapPlaceInfo
    {
        public string? Name { get; set; }
        public string? Rating { get; set; }
        public string? Address { get; set; }
        public string? OpeningHours { get; set; }
        public string? Description { get; set; }
        public string? Website { get; set; }
        public static string? ExtractDescriptionFromRawText(string rawText)
        {
            var lines = rawText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var clean = line.Trim();

                // Heuristique : ligne qui semble être une description
                if (clean.Length > 60 &&
                    char.IsUpper(clean[0]) &&
                    clean.EndsWith('.') &&
                    !clean.Contains("€") &&     // pas une ligne commerciale
                    !clean.Contains("www") &&
                    !clean.Contains(".com"))
                {
                    return clean;
                }
            }

            return null; // si rien trouvé
        }

    }

}
