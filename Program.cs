using System;
using System.Drawing;
using System.Diagnostics;

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();

Bitmap BMP = ImgProcess.ImgToBmp("C:/Users/peter/OneDrive/Imagens/StringArtImg/Vivilow3.jpg");

// Good settings for 300mm board, 220 nails
// Board board = new Board(ImgProcess.GetSize(BMP), 220, 295);
// board.Draw(3000, 0.2, 11, 0.1, ByteArr);


byte[] ByteArr = ImgProcess.BmpToArr(BMP);

Board board = new Board(ImgProcess.GetSize(BMP), 150, 295);

board.Draw(1500, 0.13, 15, 0.17, ByteArr);
board.GenerateTXT("C:/Users/peter/OneDrive/Imagens/StringArtImg/img1.txt", 50);

stopwatch.Stop();
TimeSpan elapsed = stopwatch.Elapsed;
System.Console.WriteLine($"Elapsed time: {elapsed.TotalMilliseconds} ms");
