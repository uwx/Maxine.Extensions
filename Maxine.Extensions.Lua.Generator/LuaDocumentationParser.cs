namespace Maxine.Extensions.Lua.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

public class LuaDocumentationParser
{
    public Dictionary<string, string> ParseManual(string htmlContent)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);
        
        var documentation = new Dictionary<string, string>();
        
        // Find all function/type headers (h3 elements with anchor names)
        var headers = doc.DocumentNode.SelectNodes("//h3/a[@name]");
        
        if (headers == null) return documentation;
        
        foreach (var anchor in headers)
        {
            var name = anchor.GetAttributeValue("name", "");
            
            // Skip non-C API items (like pdf- prefixed ones)
            if (string.IsNullOrEmpty(name) || name.StartsWith("pdf-"))
                continue;
            
            var h3 = anchor.ParentNode;
            
            // Get stack effect annotation
            var stackEffect = h3.SelectSingleNode(".//span[@class='apii']")?.InnerText.Trim();
            
            // Get function signature or typedef
            var signature = GetNextElement(h3, "pre")?.InnerText.Trim();
            
            // Get description paragraphs
            var description = GetDescription(h3);
            
            // Generate xmldoc
            var xmlDoc = GenerateXmlDoc(name, stackEffect, signature, description);
            
            documentation[name] = xmlDoc;
        }
        
        return documentation;
    }
    
    private HtmlNode GetNextElement(HtmlNode current, string tagName)
    {
        var sibling = current.NextSibling;
        while (sibling != null)
        {
            if (sibling.Name == tagName)
                return sibling;
            if (sibling.Name == "h3" || sibling.Name == "h2" || sibling.Name == "hr")
                break;
            sibling = sibling.NextSibling;
        }
        return null;
    }
    
    private string GetDescription(HtmlNode h3)
    {
        var sb = new StringBuilder();
        var sibling = h3.NextSibling;
        
        while (sibling != null)
        {
            // Stop at next section
            if (sibling.Name == "h3" || sibling.Name == "h2" || sibling.Name == "hr")
                break;
            
            if (sibling.Name == "p")
            {
                var text = CleanText(sibling.InnerText);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    sb.AppendLine(text);
                    sb.AppendLine();
                }
            }
            else if (sibling.Name == "ul")
            {
                foreach (var li in sibling.SelectNodes(".//li") ?? Enumerable.Empty<HtmlNode>())
                {
                    sb.AppendLine("- " + CleanText(li.InnerText));
                }
                sb.AppendLine();
            }
            
            sibling = sibling.NextSibling;
        }
        
        return sb.ToString().Replace("&", "&amp;").Replace("\r\n", "\n").Replace("\n", "<br/>\n").Trim();
    }
    
    private string CleanText(string text)
    {
        // Decode HTML entities
        text = System.Net.WebUtility.HtmlDecode(text);
        // Normalize whitespace
        text = Regex.Replace(text, @"\s+", " ");
        return text.Trim();
    }
    
    private string GenerateXmlDoc(string name, string stackEffect, string signature, string description)
    {
        var sb = new StringBuilder();
        sb.AppendLine("/// <summary>");
        
        if (!string.IsNullOrEmpty(description))
        {
            foreach (var line in description.Split('\n'))
            {
                if (!string.IsNullOrWhiteSpace(line))
                    sb.AppendLine($"/// {line.Trim()}");
            }
        }
        
        sb.AppendLine("/// </summary>");
        
        if (!string.IsNullOrEmpty(stackEffect))
        {
            sb.AppendLine("/// <remarks>");
            sb.AppendLine($"/// Stack effect: {stackEffect}");
            sb.AppendLine($"/// {ParseStackEffect(stackEffect)}");
            sb.AppendLine("/// </remarks>");
        }
        
        if (!string.IsNullOrEmpty(signature))
        {
            sb.AppendLine("/// <code>");
            foreach (var line in signature.Split('\n'))
            {
                if (!string.IsNullOrWhiteSpace(line))
                    sb.AppendLine($"/// {line.Trim()}");
            }
            sb.AppendLine("/// </code>");
        }
        
        return sb.ToString();
    }
    
    private string ParseStackEffect(string effect)
    {
        // Parse notation like [-1, +1, e]
        var match = Regex.Match(effect, @"\[([^,]+),\s*([^,]+),\s*([^\]]+)\]");
        if (!match.Success)
            return "Stack effect information available in manual.";
        
        var pops = match.Groups[1].Value.Trim();
        var pushes = match.Groups[2].Value.Trim();
        var errors = match.Groups[3].Value.Trim();
        
        var parts = new List<string>();
        
        // Pops
        if (pops != "-0")
            parts.Add($"Pops: {pops} elements from stack");
        
        // Pushes  
        if (pushes != "+0")
            parts.Add($"Pushes: {pushes} elements to stack");
        
        // Error handling
        switch (errors)
        {
            case "e":
                parts.Add("May throw errors");
                break;
            case "m":
                parts.Add("May throw memory allocation errors");
                break;
            case "v":
                parts.Add("May throw errors on purpose");
                break;
        }
        
        return string.Join(". ", parts) + ".";
    }
}
