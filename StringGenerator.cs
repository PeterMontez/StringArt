using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

public class Stringer
{
    public int[] ImageSize { get; set; }
    public int NailAmount { get; set; }
    public int Rounds { get; set; }
    public double Opacity { get; set; }
    public PointD[] Nails { get; set; }
    public byte[] ByteArr { get; set; }
    public int[] Path { get; set; }

    public Stringer(int[] imageSize, int nailAmount, int rounds, double opacity, byte[] byteArr, PointD[] nails)
    {
        this.ImageSize = imageSize;
        this.NailAmount = nailAmount;
        this.Rounds = rounds;
        this.Opacity = opacity;
        this.Nails = nails;
        this.ByteArr = byteArr;
        this.Path = new int[Rounds+1];
    }

    public void Draw()
    {
        int[] firstPath = FindFirstPath();
        Path[0] = firstPath[0];
        Path[1] = firstPath[1];


        int last = firstPath[1];
        for (int i = 0; i < Rounds; i++)
        {
            FindPath(last);

        }
    }

    private int[] FindFirstPath()
    {
        double highest = 0;
        int[] path = [0, 0];

        for (int i = 0; i < NailAmount; i++)
        {
            for (int j = i + 1; j < NailAmount; j++)
            {
                double sum = 0;
                int count = 0;
                foreach (var item in getLine(Nails[i], Nails[j]))
                {
                    sum += byteValueByCoord(item.Item1, item.Item2) * item.Item3;
                }
                sum /= count;
                if (sum > highest)
                {
                    highest = sum;
                    path = [i, j];
                }
            }
        }
        clearImage(path[0], path[1]);

        return path;
    }


    private int FindPath(int start)
    {
        double highest = 0;
        int path = 0;

        for (int i = 0; i < NailAmount; i++)
        {
            if (i == start)
            {
                continue;
            }

            double sum = 0;
            int count = 0;
            foreach (var item in getLine(Nails[start], Nails[i]))
            {
                sum += byteValueByCoord(item.Item1, item.Item2) * item.Item3;
            }
            sum /= count;
            if (sum > highest)
            {
                highest = sum;
                path = i;
            }
        }
        clearImage(start, path);

        return path;
    }

    private void clearImage(int start, int end)
    {
        foreach (var item in getLine(Nails[start], Nails[end]))
        {
            byte crr = ByteArr[byteIndexByCoord(item.Item1, item.Item2)];
            crr = (byte)(crr - (255 * item.Item3) < 0 ? 0 : crr - (255 * item.Item3));
            ByteArr[byteIndexByCoord(item.Item1, item.Item2)] = crr;
        } 
    }

    private byte byteValueByCoord(int x, int y)
    {
        return ByteArr[(y * ImageSize[0]) + x];
    }

    private int byteIndexByCoord(int x, int y)
    {
        return (y * ImageSize[0]) + x;
    }

    private List<Tuple<int, int, double>> getLine(PointD p1, PointD p2)
    {
        List<Tuple<int, int, double>> pixels = new List<Tuple<int, int, double>>();

        bool steep = Math.Abs(p2.Y - p1.Y) > Math.Abs(p2.X - p1.X);
        if (steep)
        {
            (p1.Y, p1.X) = (p1.X, p1.Y);
            (p2.Y, p2.X) = (p2.X, p2.Y);
        }

        if (p1.X > p2.X)
        {
            (p1.X, p2.X) = (p2.X, p1.X);
            (p1.Y, p2.Y) = (p2.Y, p1.Y);
        }

        double dx = p2.X - p1.X;
        double dy = p2.Y - p1.Y;
        double gradient = (dx == 0) ? 1.0 : dy / dx;

        // Handle first endpoint
        double xEnd = Round(p1.X);
        double yEnd = p1.Y + gradient * (xEnd - p1.X);
        double xGap = 1.0; // Set xGap to 1.0 to ensure full brightness
        int xPixel1 = (int)xEnd;
        int yPixel1 = (int)Math.Floor(yEnd);

        if (steep)
        {
            Plot(ref pixels, yPixel1, xPixel1, Rfpart(yEnd) * xGap);
            Plot(ref pixels, yPixel1 + 1, xPixel1, Fpart(yEnd) * xGap);
        }
        else
        {
            Plot(ref pixels, xPixel1, yPixel1, Rfpart(yEnd) * xGap);
            Plot(ref pixels, xPixel1, yPixel1 + 1, Fpart(yEnd) * xGap);
        }

        double intery = yEnd + gradient; // first y-intersection for the main loop

        // Handle second endpoint
        xEnd = Round(p2.X);
        yEnd = p2.Y + gradient * (xEnd - p2.X);
        xGap = 1.0; // Set xGap to 1.0 to ensure full brightness
        int xPixel2 = (int)xEnd;
        int yPixel2 = (int)Math.Floor(yEnd);

        if (steep)
        {
            Plot(ref pixels, yPixel2, xPixel2, Rfpart(yEnd) * xGap);
            Plot(ref pixels, yPixel2 + 1, xPixel2, Fpart(yEnd) * xGap);
        }
        else
        {
            Plot(ref pixels, xPixel2, yPixel2, Rfpart(yEnd) * xGap);
            Plot(ref pixels, xPixel2, yPixel2 + 1, Fpart(yEnd) * xGap);
        }

        // Main loop
        if (steep)
        {
            for (int x = xPixel1 + 1; x < xPixel2; x++)
            {
                Plot(ref pixels, (int)Math.Floor(intery), x, Rfpart(intery));
                Plot(ref pixels, (int)Math.Floor(intery) + 1, x, Fpart(intery));
                intery += gradient;
            }
        }
        else
        {
            for (int x = xPixel1 + 1; x < xPixel2; x++)
            {
                Plot(ref pixels, x, (int)Math.Floor(intery), Rfpart(intery));
                Plot(ref pixels, x, (int)Math.Floor(intery) + 1, Fpart(intery));
                intery += gradient;
            }
        }

        return pixels;
    }

    private void Plot(ref List<Tuple<int, int, double>> pixels, int x, int y, double brightness)
    {
        if (brightness > 0) // Only add pixels with visible brightness
        {
            pixels.Add(new Tuple<int, int, double>(x, y, brightness));
        }
    }

    private void Swap(ref double a, ref double b)
    {
        double temp = a;
        a = b;
        b = temp;
    }

    private double Round(double x)
    {
        return Math.Floor(x + 0.5);
    }

    private double Fpart(double x)
    {
        return x - Math.Floor(x);
    }

    private double Rfpart(double x)
    {
        return 1 - Fpart(x);
    }
}