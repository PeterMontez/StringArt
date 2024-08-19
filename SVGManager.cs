using System;
using System.Drawing;
using System.Globalization;
using System.Windows;

public class SVGMaker
{
    public int[] imageSize { get; set; }
    public string svgContent { get; set; }

    public SVGMaker(int[] imageSize)
    {
        this.imageSize = imageSize;
    }

    public void Create()
    {
        svgContent = $@"
<svg width='{imageSize[0]}' height='{imageSize[1]}' xmlns='http://www.w3.org/2000/svg'>";
    }

    public void NewLine(PointD[] points)
    {
        svgContent += @"
    <polyline points='";
        foreach (var point in points)
        {
            svgContent += $"{point.X.ToString(CultureInfo.InvariantCulture)},{point.Y.ToString(CultureInfo.InvariantCulture)} ";
        }
        svgContent += $@"'
        style='fill:none;stroke:black;stroke-width:0.5' />";
    }

    public void Close()
    {
        svgContent += $@"</svg>";
    }

    public void Save(string filePath)
    {
        File.WriteAllText(filePath, svgContent);
        Console.WriteLine($"SVG file generated: {filePath}");
    }
}