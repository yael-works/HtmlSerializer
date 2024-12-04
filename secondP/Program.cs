using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace secondP
{
    public class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            try
            {
                var html = await Load("https://forum.netfree.link/");
                var cleanHtml = new Regex("\\s+").Replace(html, " ").Trim();
                var regex = new Regex("(<[^>]+>|[^<]+)");
                var htmlLines = regex.Matches(cleanHtml)
                    .Cast<Match>()
                    .Select(m => m.Value.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();
                HtmlElement root = new HtmlElement("html");
                HtmlElement currentElement = root;
                foreach (var line in htmlLines)
                {
                    if (line == "</html>") break;
                    if (!line.StartsWith("/") && !line.StartsWith("<"))
                    {
                        currentElement.InnerHtml += line;
                        continue;
                    }
               if (line.StartsWith("</"))
                    {
                        currentElement = currentElement.Parent ?? root;
                        continue;
                    }
                    var tagName = line.Split(' ')[0].Trim('<', '>');
                    if (HtmlHelper.Instance.IsValidTag(tagName))
                    {
                        HtmlElement newElement = new HtmlElement(tagName);
                        var attributeRegex = new Regex("([^\\s=]+)=\"(.*?)\"");
                        var attributes = attributeRegex.Matches(line);

                        foreach (Match match in attributes)
                        {
                            var attributeName = match.Groups[1].Value;
                            var attributeValue = match.Groups[2].Value;
                            if (attributeName == "class")
                            {
                                newElement.Classes.AddRange(attributeValue.Split(' '));
                            }
                            else if (attributeName == "id")
                            {
                                newElement.Id = attributeValue;
                            }
                            else
                            {
                                newElement.Attributes[attributeName] = attributeValue;
                            }
                        }

                        currentElement.AddChild(newElement);
                        currentElement = newElement; 
                    }
                }
             //   root.PrintTree();


                string query = "ul.nav.navbar-nav";
                var selector = Selector.Parse(query);
                var elements = root.FindElementsBySelector(selector);

                if (elements != null && elements.Any())
                {
                    Console.WriteLine($"נמצאו {elements.Count()} אלמנטים שתואמים את הסלקטור {query}:");
                    foreach (var element in elements)
                    {
                       Console.WriteLine($" <{element.Name}  Id: {element.Id}, Classes: {string.Join(", ", element.Classes)}><{element.Name}/>");
                    }
                }
                else
                {
                    Console.WriteLine("לא נמצאו אלמנטים שתואמים את הסלקטור.");
                }

            }
            catch (Exception ex)
            {
              Console.WriteLine($"שגיאה: {ex.Message}");
            }
        }
        public static async Task<string> Load(string url)
        {
            var response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
