using System;
using System.Collections.Generic;
using System.Linq;

namespace secondP
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; private set; }
        public List<string> Classes { get; private set; }
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; private set; }

      
        public HtmlElement(string name)
        {
            Name = name;
            Attributes = new Dictionary<string, string>();
            Classes = new List<string>();
            Children = new List<HtmlElement>();
        }

      
        public void AddChild(HtmlElement child)
        {
            child.Parent = this;
            Children.Add(child);
        }

      
        public void AddAttribute(string key, string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            Attributes[key] = value;

           
            if (key.Equals("class", StringComparison.OrdinalIgnoreCase))
            {
                Classes = value.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            
            if (key.Equals("id", StringComparison.OrdinalIgnoreCase))
            {
                Id = value;
            }
        }

    
        public string ToString(bool includeInnerHtml = true)
        {
            string result = $"<{Name}";

            if (!string.IsNullOrEmpty(Id))
            {
                result += $" id=\"{Id}\"";
            }
            if (Classes.Any())
            {
                result += $" class=\"{string.Join(" ", Classes)}\"";
            }
            foreach (var attribute in Attributes)
            {
                result += $" {attribute.Key}=\"{attribute.Value}\"";
            }

            result += ">";

            if (includeInnerHtml && !string.IsNullOrEmpty(InnerHtml))
            {
                
                if (InnerHtml.Contains("<script") || InnerHtml.Contains("</script>"))
                {
                    result += "...";  
                }
                else
                {
                    result += InnerHtml;
                }
            }

            if (Children.Any())
            {
                foreach (var child in Children)
                {
                    result += child.ToString(); // Recursively print children
                }
            }

           
            result += $"</{Name}>";

            return result;
        }

        public void PrintTree(int indentLevel = 0)
        {
            string indent = new string(' ', indentLevel * 2);

          
            string result = $"{indent}<{Name}";

            if (!string.IsNullOrEmpty(Id))
            {
                result += $" id=\"{Id}\"";
            }
           
            if (Classes.Any())
            {
                result += $" class=\"{string.Join(" ", Classes)}\"";
            }
            foreach (var attribute in Attributes)
            {
                result += $" {attribute.Key}=\"{attribute.Value}\"";
            }

            result += ">";
            Console.WriteLine(result);
            if (!string.IsNullOrEmpty(InnerHtml))
            {
                Console.WriteLine($"{indent}  {InnerHtml}");
            }
            foreach (var child in Children)
            {
                child.PrintTree(indentLevel + 1);
            }
            Console.WriteLine($"{indent}</{Name}>");
        }




        public IEnumerable<HtmlElement> Descendants()
        {
            var queue = new Queue<HtmlElement>();
            queue.Enqueue(this); 

            while (queue.Count > 0)
            {
                var currentElement = queue.Dequeue(); 
                yield return currentElement; 
                foreach (var child in currentElement.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }

        public IEnumerable<HtmlElement> Ancestors()
        {
            var currentElement = this.Parent;
            while (currentElement != null)
            {
                yield return currentElement; 
                currentElement = currentElement.Parent; 
            }
        }


        public IEnumerable<HtmlElement> FindElementsBySelector(Selector selector)
        {
            var result = new List<HtmlElement>();

            foreach (var element in this.Descendants())
            {
                if (MatchesSelector(element, selector))
                {
                    result.Add(element);
                }
                if (selector.Child != null)
                {
                    var childResults = element.FindElementsBySelector(selector.Child);
                    result.AddRange(childResults);
                }
            }

            return result;
        }

        private bool MatchesSelector(HtmlElement element, Selector selector)
        {

            if (!string.IsNullOrEmpty(selector.TagName) &&
                !element.Name.Equals(selector.TagName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(selector.Id) &&
                !element.Id.Equals(selector.Id, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (selector.Classes.Any())
            {
                foreach (var cls in selector.Classes)
                {
                    if (!element.Classes.Contains(cls, StringComparer.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

}
