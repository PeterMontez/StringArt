using System;
using System.Drawing;

Bitmap BMP = ImgProcess.ImgToBmp("C:/Users/peter/OneDrive/Área de Trabalho/StringArt/img/test4.jpg");

byte[] ByteArr = ImgProcess.BmpToArr(BMP);

Board board = new Board(ImgProcess.GetSize(BMP), 350, 550);

board.Draw(20000, 0.03, 10, 0.06, ByteArr);
board.GenerateTXT("C:/Users/peter/OneDrive/Área de Trabalho/StringArt/img/test2.txt", 50);
