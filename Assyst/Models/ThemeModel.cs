using System.Collections.Generic;

namespace Assyst.Models
{

    public static class ThemeModel
    {

    }

    public static class CustomTheme
    {
        public static List<string> Themes { get; } = new List<string>
        {
            "Cerulean",
            "Cosmo",
            "Cyborg",
            "Darkly",
            "Flatly",
            "Journal",
            "Litera",
            "Lumen",
            "Lux",
            "Materia",
            "Minty",
            "Pulse",
            "Sandstone",
            "Simplex",
            "Sketchy",
            "Slate",
            "Solar",
            "Spacelab",
            "Superhero",
            "United",
            "Yeti"
        };

        public static string CurrentTheme { get; set; } = "Flatly";
    }
}
