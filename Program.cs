using System;
using System.Collections.Generic;


var results = XiaolinWuLine(0, 0, 100, 15);

foreach (var result in results)
{
    Console.WriteLine($"Pixel ({result.Item1}, {result.Item2}) has brightness {result.Item3:F2}.");
}

static List<Tuple<int, int, double>> XiaolinWuLine(int x0, int y0, int x1, int y1)
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

    int dx = x1 - x0;
    int dy = y1 - y0;
    double gradient = (dx == 0) ? 1.0 : (double)dy / dx;

    // Handle first endpoint
    double xEnd = Round(x0);
    double yEnd = y0 + gradient * (xEnd - x0);
    double xGap = Rfpart(x0 + 0.5);
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
    xGap = Fpart(x1 + 0.5);
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

static void Swap(ref int a, ref int b)
{
    int temp = a;
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