using System;
using System.Collections.Generic;

public class Stringer
{
    public int[] imageSize { get; set; };
    public int nailAmount { get; set; };
    public int rounds { get; set; };
    public double opacity { get; set; };
    public Drawer(int[] imageSize, int nailAmount, int rounds, double opacity)
    {
        this.imageSize = imageSize;
        this.nailAmount = nailAmount;
        this.rounds = rounds;
        this.opacity = opacity;
    }

    public void Draw()
    {
        
    }
}

var results = XiaolinWuLine(0, 0, 50, 80);

static List<Tuple<int, int, double>> XiaolinWuLine(double x0, double y0, double x1, double y1)
{
    List<Tuple<int, int, double>> pixels = new List<Tuple<int, int, double>>();

    bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
    if (steep)
    {
        Swap(ref x0, ref y0);
        Swap(ref x1, ref y1);
    }

    if (x0 > x1)
    {
        Swap(ref x0, ref x1);
        Swap(ref y0, ref y1);
    }

    double dx = x1 - x0;
    double dy = y1 - y0;
    double gradient = (dx == 0) ? 1.0 : dy / dx;

    // Handle first endpoint
    double xEnd = Round(x0);
    double yEnd = y0 + gradient * (xEnd - x0);
    double xGap = 1.0; // Set xGap to 1.0 to ensure full brightness
    int xPixel1 = (int)xEnd;
    int yPixel1 = (int)Math.Floor(yEnd);

    if (steep)
    {
        Plot(pixels, yPixel1, xPixel1, Rfpart(yEnd) * xGap);
        Plot(pixels, yPixel1 + 1, xPixel1, Fpart(yEnd) * xGap);
    }
    else
    {
        Plot(pixels, xPixel1, yPixel1, Rfpart(yEnd) * xGap);
        Plot(pixels, xPixel1, yPixel1 + 1, Fpart(yEnd) * xGap);
    }

    double intery = yEnd + gradient; // first y-intersection for the main loop

    // Handle second endpoint
    xEnd = Round(x1);
    yEnd = y1 + gradient * (xEnd - x1);
    xGap = 1.0; // Set xGap to 1.0 to ensure full brightness
    int xPixel2 = (int)xEnd;
    int yPixel2 = (int)Math.Floor(yEnd);

    if (steep)
    {
        Plot(pixels, yPixel2, xPixel2, Rfpart(yEnd) * xGap);
        Plot(pixels, yPixel2 + 1, xPixel2, Fpart(yEnd) * xGap);
    }
    else
    {
        Plot(pixels, xPixel2, yPixel2, Rfpart(yEnd) * xGap);
        Plot(pixels, xPixel2, yPixel2 + 1, Fpart(yEnd) * xGap);
    }

    // Main loop
    if (steep)
    {
        for (int x = xPixel1 + 1; x < xPixel2; x++)
        {
            Plot(pixels, (int)Math.Floor(intery), x, Rfpart(intery));
            Plot(pixels, (int)Math.Floor(intery) + 1, x, Fpart(intery));
            intery += gradient;
        }
    }
    else
    {
        for (int x = xPixel1 + 1; x < xPixel2; x++)
        {
            Plot(pixels, x, (int)Math.Floor(intery), Rfpart(intery));
            Plot(pixels, x, (int)Math.Floor(intery) + 1, Fpart(intery));
            intery += gradient;
        }
    }

    return pixels;
}

static void Plot(List<Tuple<int, int, double>> pixels, int x, int y, double brightness)
{
    if (brightness > 0) // Only add pixels with visible brightness
    {
        pixels.Add(new Tuple<int, int, double>(x, y, brightness));
    }
}

static void Swap(ref double a, ref double b)
{
    double temp = a;
    a = b;
    b = temp;
}

static double Round(double x)
{
    return Math.Floor(x + 0.5);
}

static double Fpart(double x)
{
    return x - Math.Floor(x);
}

static double Rfpart(double x)
{
    return 1 - Fpart(x);
}