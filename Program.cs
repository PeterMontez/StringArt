using System;
using System.Drawing;

Bitmap BMP = ImgProcess.ImgToBmp("C:/Users/peter/OneDrive/Imagens/StringArtImg/img (3).jpg");

byte[] ByteArr = ImgProcess.BmpToArr(BMP);

Board board = new Board(ImgProcess.GetSize(BMP), 250, 295);

board.Draw(4000, 0.25, 5, 0.14, ByteArr);
board.GenerateTXT("C:/Users/peter/OneDrive/Imagens/StringArtImg/img1.txt", 50);
