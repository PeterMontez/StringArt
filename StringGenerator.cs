using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Microsoft.VisualBasic;

public class Stringer
{
    public int[] ImageSize { get; set; }
    public int NailAmount { get; set; }
    public int Rounds { get; set; }
    public double Opacity { get; set; }
    public PointD[] Nails { get; set; }
    public byte[] ByteArr { get; set; }
    public int[] Path { get; set; }
    public double WireLength { get; set; }
    private ConcurrentDictionary<(int, int), List<Tuple<int, int, double>>> lineCache = new();

    public Stringer(
        int[] imageSize,
        int nailAmount,
        int rounds,
        double opacity,
        byte[] byteArr,
        PointD[] nails
    )
    {
        this.ImageSize = imageSize;
        this.NailAmount = nailAmount;
        this.Rounds = rounds;
        this.Opacity = opacity;
        this.Nails = nails;
        this.ByteArr = byteArr;
        this.Path = new int[Rounds + 2];
    }

    public void Draw(int skip, double thickness, int diameter)
    {
        SVGMaker maker = new SVGMaker(ImageSize, thickness, diameter);
        maker.Create();
        maker.Save("C:/Users/peter/OneDrive/Imagens/StringArtImg/draw.svg");

        PointD[] finalPoints = new PointD[Rounds + 2];

        int[] firstPath = FindFirstPath(skip);
        Path[0] = firstPath[0];
        Path[1] = firstPath[1];

        finalPoints[0] = Nails[Path[0]];
        finalPoints[1] = Nails[Path[1]];

        int previous = firstPath[0];
        int last = firstPath[1];

        double[,] distanceMatrix = new double[NailAmount, NailAmount];

        for (int i = 0; i < NailAmount; i++)
        for (int j = 0; j < NailAmount; j++)
            distanceMatrix[i, j] = GetDistance(Nails[i], Nails[j]);

        WireLength += distanceMatrix[previous, last];

        for (int i = 0; i < Rounds; i++)
        {
            int temp = last;
            last = FindPath(last, previous, skip);
            previous = temp;
            // System.Console.WriteLine($"Path {i}: {last}");
            finalPoints[i + 2] = Nails[last];
            Path[i + 2] = last;
            WireLength += distanceMatrix[previous, last];
        }

        maker.NewLine(finalPoints);
        maker.Close();
        // maker.Save("C:/Users/peter/OneDrive/Imagens/StringArtImg/draw.svg");
        ImgProcess.ArrToBmp(ByteArr, ImageSize);
    }

    private int[] FindFirstPath(int skip)
    {
        object lockObj = new object();
        double lowest = 255;
        int[] path = [0, 0];

        Parallel.For(
            0,
            NailAmount,
            i =>
            {
                for (int j = i + 1; j < NailAmount; j++)
                {
                    double sum = 0;
                    int count = 0;
                    foreach (var item in getLine(Nails[i], Nails[j], i, j))
                    {
                        sum += byteValueByCoord(item.Item1, item.Item2) * item.Item3;
                        count++;
                    }
                    sum /= count;

                    if (sum < lowest)
                    {
                        lock (lockObj)
                        {
                            if (sum < lowest)
                            {
                                lowest = sum;
                                path = [i, j];
                            }
                        }
                    }
                }
            }
        );

        clearImage(path[0], path[1]);
        return path;
    }

    private int FindPath(int start, int previous, int skip)
    {
        object lockObj = new object();
        double lowest = 255;
        int path = 0;

        Parallel.For(
            0,
            NailAmount,
            i =>
            {
                if (i == start || i == previous || Math.Abs(i - start) <= skip)
                    return;

                double sum = 0;
                int count = 0;
                foreach (var item in getLine(Nails[start], Nails[i], start, i))
                {
                    sum += byteValueByCoord(item.Item1, item.Item2) * item.Item3;
                    count++;
                }
                sum /= count;

                if (sum < lowest)
                {
                    lock (lockObj)
                    {
                        if (sum < lowest)
                        {
                            lowest = sum;
                            path = i;
                        }
                    }
                }
            }
        );

        clearImage(start, path);
        return path;
    }

    private double GetLineScore(int index1, int index2)
    {
        double sum = 0;
        int count = 0;

        foreach (var item in getLine(Nails[index1], Nails[index2], index1, index2))
        {
            sum += byteValueByCoord(item.Item1, item.Item2) * item.Item3;
            count++;
        }

        return (count == 0) ? double.MaxValue : sum / count;
    }

    private void clearImage(int start, int end)
    {
        foreach (var item in getLine(Nails[start], Nails[end], start, end))
        {
            byte crr = ByteArr[byteIndexByCoord(item.Item1, item.Item2)];
            crr = (byte)(
                (crr + (item.Item3 * (255 * Opacity)) > 255)
                    ? 255
                    : (crr + (item.Item3 * (255 * Opacity)))
            );
            ByteArr[byteIndexByCoord(item.Item1, item.Item2)] = crr;
        }
    }

    private double GetDistance(PointD p1, PointD p2)
    {
        double deltaX = p2.X - p1.X;
        double deltaY = p2.Y - p1.Y;
        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    public void Info(int diameter)
    {
        double imgDiameter = ImageSize[0] < ImageSize[1] ? ImageSize[0] : ImageSize[1];
        imgDiameter -= 4;
        System.Console.WriteLine(
            $"String length: {WireLength * diameter / imgDiameter / 1000:F2}m"
        );
    }

    private byte byteValueByCoord(int x, int y)
    {
        return ByteArr[((y - 1) * ImageSize[0]) + x];
    }

    private int byteIndexByCoord(int x, int y)
    {
        return (y * ImageSize[0]) + x;
    }

    private List<Tuple<int, int, double>> getLine(PointD P1, PointD P2, int index1, int index2)
    {
        // Ensure the key is always ordered (smallest first)
        var key = index1 < index2 ? (index1, index2) : (index2, index1);

        if (lineCache.TryGetValue(key, out var cachedLine))
        {
            return cachedLine;
        }

        // --- Your original line generation logic starts here ---
        PointD p1 = new PointD(P1.X, P1.Y);
        PointD p2 = new PointD(P2.X, P2.Y);
        List<Tuple<int, int, double>> pixels = new();

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

        double xEnd = Round(p1.X);
        double yEnd = p1.Y + gradient * (xEnd - p1.X);
        double xGap = 1.0;
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

        double intery = yEnd + gradient;

        xEnd = Round(p2.X);
        yEnd = p2.Y + gradient * (xEnd - p2.X);
        xGap = 1.0;
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
        // --- Line generation ends here ---

        lineCache[key] = pixels;
        return pixels;
    }

    private void Plot(ref List<Tuple<int, int, double>> pixels, int x, int y, double brightness)
    {
        if (brightness > 0) // Only add pixels with visible brightness
        {
            pixels.Add(new Tuple<int, int, double>(x, y, brightness));
        }
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
