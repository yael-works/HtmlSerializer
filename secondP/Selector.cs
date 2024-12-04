using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class Selector
{
    public string TagName { get; set; }
    public string Id { get; set; }
    public List<string> Classes { get; set; }
    public Selector Parent { get; set; }
    public Selector Child { get; set; }


    public Selector()
    {
        Classes = new List<string>();
    }
    public static Selector Parse(string query)
    {
        var parts = Regex.Split(query, @"\s*>\s*|\s*\+\s*|\s+");

        Selector root = null; 
        Selector current = null;



        foreach (var part in parts)
        {
            if (string.IsNullOrWhiteSpace(part)) continue;

            Selector newSelector = new Selector();
            var splitById = part.Split('#');
            var splitByClass = splitById[0].Split('.');
            if (splitByClass.Length > 0 && !string.IsNullOrWhiteSpace(splitByClass[0]))
            {
                newSelector.TagName = splitByClass[0];
                     }
            if (splitById.Length > 1)
            {
                newSelector.Id = splitById[1];
            }
            if (splitByClass.Length > 1)
            {
                foreach (var className in splitByClass.Skip(1))
                {
                    if (!string.IsNullOrWhiteSpace(className))
                    {
                        newSelector.Classes.Add(className);
                    }
                }
            }
            if (root == null)
            {
                root = newSelector;
            }
            else
            {
                current.Child = newSelector;
                newSelector.Parent = current;
            }
            current = newSelector;
        }

        return root; 
    }


    public void PrintSelector(int level = 0)
    {
        string indent = new string(' ', level * 2); 
        Console.WriteLine($"{indent}Tag: {TagName ?? "(ריק)"}, ID: {Id ?? "(ריק)"}, Classes: {string.Join(", ", Classes)}");
        Child?.PrintSelector(level + 1);
    }


}
