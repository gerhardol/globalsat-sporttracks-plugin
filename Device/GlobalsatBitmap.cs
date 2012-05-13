/*
 *  Globalsat/Keymaze SportTracks Plugin
 *  Copyright 2009 John Philip 
 * 
 *  This software may be used and distributed according to the terms of the
 *  GNU Lesser General Public License version 2 or any later version.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;


namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public class GlobalsatBitmap
    {
        public static Bitmap GetBitmap(int bpp, System.Drawing.Size size, bool screenRowCol, RotateFlipType rotateFlip, byte[] binData)
        {
            Bitmap bmp;
            if (bpp == 1)
            {
                bmp = GetBitmapBpp0(size.Width, size.Height, screenRowCol, binData);
            }
            else{
                bmp = GetBitmapBpp2(size.Width, size.Height, screenRowCol, binData);
            }
            if (rotateFlip != RotateFlipType.RotateNoneFlipNone)
            {
                bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }
            return bmp;
        }

        protected static Bitmap GetBitmapBpp0(int width, int height, bool screenRowCol, byte[] binData)
        {
            int i = 0;
            bool[,] matrix = new bool[width, height];

            // each row is a byte
            int nrRows = (int)Math.Ceiling((double)height / 8.0);
            if (screenRowCol)
            {
                for (int row = 0; row < nrRows; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        SetBytePixels(matrix, row, col, binData[i]);
                        i++;
                    }
                }
            }
            else
            {
                for (int col = 0; col < width; col++)
                {
                    for (int row = 0; row < nrRows; row++)
                    {
                        SetBytePixels(matrix, row, col, binData[i]);
                        i++;
                    }
                }
            }
            Bitmap bmp = Matrix2Bitmap(matrix);

            return bmp;
        }

        protected static Bitmap GetBitmapBpp2(int width, int height, bool screenRowCol, byte[] binData)
        {
            int bpp = 2;

            int i = 0;
            float[,] matrix = new float[width, height];

            // each row is a byte
            int nrRows = (int)Math.Ceiling((double)height / 8.0);
            int nrCols = (int)Math.Ceiling((double)width / 8.0);
            nrCols *= bpp;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < nrCols; col++)
                {

                    SetBytePixelGray(matrix, row, col, binData[i], bpp);
                    i++;
                }
            }

            Bitmap bmp = Matrix2BitmapGray(matrix);

            return bmp;
        }

        private static Bitmap Matrix2BitmapGray(float[,] matrix)
        {
            int zoom = 1;

            int width = matrix.GetLength(0);
            int height = matrix.GetLength(1);
            Bitmap bitmap = new Bitmap(width * zoom, height * zoom);

            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int gray = 255 - (int)Math.Min(255, matrix[x, y] * 255);
                    Brush brush = new SolidBrush(Color.FromArgb(gray, gray, gray));
                    g.FillRectangle(brush, x * zoom, y * zoom, zoom, zoom);
                    
                }
            }

            return bitmap;
        }

        private static void SetBytePixelGray(float[,] matrix, int row, int column, byte value, int bpp)
        {

            int y = row;

            for (int i = 0; i < 8; i += bpp)
            {

                int pixelValue = 0;

                for (int j = 0; j < bpp; j++)
                {
                    int mask = 1 << (i + j);
                    int iValue = (value & mask) >> (i + j);

                    pixelValue |= iValue << j;
                }

                float grayLevel = (float)pixelValue / (float)(Math.Pow(2, bpp) - 1);

                int x = column * 8 / bpp;
                x += i / bpp;

                SetPixelGray(matrix, x, y, grayLevel);
            }
        }


        private static void SetPixelGray(float[,] matrix, int x, int y, float value)
        {
            if (x >= 0 && x < matrix.GetLength(0) && y >= 0 && y < matrix.GetLength(1) && value >= 0 && value <= 1)
            {
                matrix[x, y] = value;
            }
        }

        private static Bitmap Matrix2Bitmap(bool[,] matrix)
        {

            int zoom = 1;

            int width = matrix.GetLength(0);
            int height = matrix.GetLength(1);
            Bitmap bitmap = new Bitmap(width * zoom, height * zoom);

            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (matrix[x, y])
                    {
                        g.FillRectangle(Brushes.Black, x * zoom, y * zoom, zoom, zoom);
                    }
                }
            }

            return bitmap;
        }

        private static void SetBytePixels(bool[,] matrix, int row, int column, byte value)
        {
            int x = column;

            for (int i = 0; i < 8; i++)
            {
                int mask = 1 << i;
                int iValue = (value & mask) >> i;

                bool bValue = iValue > 0;

                int y = row * 8;
                y += i;

                SetPixel(matrix, x, y, bValue);
            }
        }


        private static void SetPixel(bool[,] matrix, int x, int y, bool value)
        {
            if (x >= 0 && x < matrix.GetLength(0) && y >= 0 && y < matrix.GetLength(1))
            {
                matrix[x, y] = value;
            }
        }
    }
}
