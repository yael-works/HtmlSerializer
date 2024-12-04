using System;
using System.IO;
using System.Text.Json;

namespace secondP
{
    public class HtmlHelper
    {
        private static readonly HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;
        public string[] AllTags { get; private set; }
        public string[] SelfClosingTags { get; private set; }

        private HtmlHelper()
        {
        
            AllTags = LoadTagsFromJson("seed/all-tags.json");
            SelfClosingTags = LoadTagsFromJson("seed/self-closing-tags.json");
        }

        private string[] LoadTagsFromJson(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"שגיאה: הקובץ לא נמצא {filePath}");
                    return Array.Empty<string>();
                }

                var jsonContent = File.ReadAllText(filePath);
                var tags = JsonSerializer.Deserialize<string[]>(jsonContent);

                if (tags == null)
                {
                    Console.WriteLine($"שגיאה: הנתונים בקובץ {filePath} לא תואמים לפורמט הצפוי.");
                    return Array.Empty<string>();
                }

                return tags;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"שגיאה בקריאת קובץ JSON: {filePath}. שגיאה: {ex.Message}");
                return Array.Empty<string>();
            }
        }
        public bool IsSelfClosingTag(string tagName)
        {
            return Array.Exists(SelfClosingTags, tag => tag.Equals(tagName, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsValidTag(string tagName)
        {
            return Array.Exists(AllTags, tag => tag.Equals(tagName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
