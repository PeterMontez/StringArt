using System;
using System.Drawing;

Bitmap BMP = ImgProcess.ImgToBmp("C:/Users/peter/OneDrive/Área de Trabalho/StringArt/img/test1.jpg");

byte[] ByteArr = ImgProcess.BmpToArr(BMP);

Board board = new Board(ImgProcess.GetSize(BMP), 200);

board.Draw(2000, 1, 10, ByteArr);

