using System;
using System.Drawing;
using System.Diagnostics;

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();

Bitmap BMP = ImgProcess.ImgToBmp("C:/Users/peter/OneDrive/Imagens/StringArtImg/img (9).jpg");

byte[] ByteArr = ImgProcess.BmpToArr(BMP);

Board board = new Board(ImgProcess.GetSize(BMP), 400, 500);

board.Draw(5000, 0.2, 5, 0.1, ByteArr);
board.GenerateTXT("C:/Users/peter/OneDrive/Imagens/StringArtImg/img1.txt", 50);

stopwatch.Stop();
TimeSpan elapsed = stopwatch.Elapsed;
System.Console.WriteLine($"Elapsed time: {elapsed.TotalMilliseconds} ms");
