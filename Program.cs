using System;
using System.Drawing;

Bitmap BMP = ImgProcess.ImgToBmp("C:/Users/peter/OneDrive/Imagens/StringArtImg/img (2darker).jpg");

byte[] ByteArr = ImgProcess.BmpToArr(BMP);

Board board = new Board(ImgProcess.GetSize(BMP), 200, 470);

board.Draw(2500, 0.23, 8, 0.26, ByteArr);
board.GenerateTXT("C:/Users/peter/OneDrive/Imagens/StringArtImg/img1.txt", 50);
