using System;
using System.IO;
using System.Text;

public static class TXTWriter
{
    public static void Writer(string filePath, int[] path, int separation)
    {
        using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            int counter = 0;
            foreach (var item in path)
            {
                writer.Write(item + " ");
                counter++;
                if (counter % separation == 0)
                {
                    writer.WriteLine();
                    writer.WriteLine();
                }
            }
        }

        Console.WriteLine($"TXT file generated: {filePath}");
    }
}
