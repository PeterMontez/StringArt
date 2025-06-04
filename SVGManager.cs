using System;
using System.Globalization;
using System.IO;
using System.Windows;

public class SVGMaker
{
    public int[] imageSize { get; set; }
    public double Width { get; set; }
    private StreamWriter writer;
    private string filePath;

    public SVGMaker(int[] imageSize, double thickness, int diameter)
    {
        this.imageSize = imageSize;
        this.Width = GetWidth(thickness, diameter);
    }

    public double GetWidth(double thickness, int diameter)
    {
        double imgDiameter = imageSize[0] < imageSize[1] ? imageSize[0] : imageSize[1];
        imgDiameter -= 4;
        return imgDiameter * thickness / diameter;
    }

    public void Create()
    {
        // No file path yet, will be defined in Save().
    }

    public void NewLine(PointD[] points)
    {
        if (writer == null)
            throw new InvalidOperationException(
                "SVG writer is not initialized. Call Save(filePath) before NewLine."
            );

        writer.Write("\n    <polyline points='");

        foreach (var point in points)
        {
            writer.Write(
                $"{point.X.ToString(CultureInfo.InvariantCulture)},{point.Y.ToString(CultureInfo.InvariantCulture)} "
            );
        }

        writer.Write(
            $"' style='fill:none;stroke:black;stroke-width:{Width.ToString("F2", CultureInfo.InvariantCulture)}' />"
        );
    }

    public void Close()
    {
        if (writer == null)
            throw new InvalidOperationException(
                "SVG writer is not initialized. Call Save(filePath) before Close."
            );

        writer.WriteLine("\n</svg>");
        writer.Flush();
        writer.Dispose();
        writer = null;
        Console.WriteLine($"SVG file generated: {filePath}");
    }

    public void Save(string filePath)
    {
        this.filePath = filePath;
        writer = new StreamWriter(filePath, false);
        writer.WriteLine(
            $"<svg width='{imageSize[0]}' height='{imageSize[1]}' xmlns='http://www.w3.org/2000/svg'>"
        );
    }
}
