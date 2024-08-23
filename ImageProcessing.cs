using System;
using System.Drawing;

public static class ImgProcess
{
    public static Bitmap ImgToBmp(string imagePath)
    {
        Bitmap image = new Bitmap(imagePath);
        image.Save("C:/Users/peter/OneDrive/Área de Trabalho/StringArt/img/test.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        return image;
    }

    public static int[] GetSize(Bitmap image)
    {
        return [image.Width, image.Height];
    }

    public static byte[] BmpToArr(Bitmap image)
    {
        int width = image.Width;
        int height = image.Height;
        byte[] pixelValues = new byte[width * height];

        int index = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixelColor = image.GetPixel(x, y);
                byte brightness = pixelColor.R;
                pixelValues[index++] = brightness;
            }
        }

        return pixelValues;
    }

    public static Bitmap ArrToBmp(byte[] pixelValues, int[] imageSize)
    {
        Bitmap image = new Bitmap(imageSize[0], imageSize[1]);

        int index = 0;
        for (int y = 0; y < imageSize[1]; y++)
        {
            for (int x = 0; x < imageSize[0]; x++)
            {
                byte brightness = pixelValues[index++];
                Color pixelColor = Color.FromArgb(brightness, brightness, brightness);
                image.SetPixel(x, y, pixelColor);
            }
        }
        image.Save("C:/Users/peter/OneDrive/Área de Trabalho/StringArt/img/test1.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        return image;
    }
}
