using System;

public class Board
{
    public PointD[] nails { get; set; }
    public int nailAmount { get; set; }
    private Stringer stringer { get; set; }
    public int[] imageSize { get; set; }

    public Board(int[] imageSize, int nailAmount)
    {
        this.nails = GetNailCoords(imageSize, nailAmount);
        this.imageSize = imageSize;
        this.nailAmount = nailAmount;
    }

    private PointD[] GetNailCoords(int[] imageSize, int nailAmount)
    {
        double radius = imageSize[0] < imageSize[1] ? imageSize[0] : imageSize[1];
        radius /= 2;
        radius -= 2;
        PointD[] coords = new PointD[nailAmount];

        for (int i = 0; i < nailAmount; i++)
        {
            double angle = 2 * Math.PI * i / nailAmount;

            double x = (radius * Math.Cos(angle)) + (imageSize[0] / 2);
            double y = (radius * Math.Sin(angle)) + (imageSize[1] / 2);

            coords[i] = new PointD(x, y);
        }
        return coords;
    }

    public void Draw(int rounds, double opacity, int skip, byte[] byteArr)
    {
        this.stringer = new Stringer(imageSize, nailAmount, rounds, opacity, byteArr, nails);
        stringer.Draw(skip);
    }
}