﻿using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.IO;
using System.Drawing.Imaging;

namespace Ageless {
    class TextureControl {

		public static int terrain;

        public static Vector2[,] tex16x16Coords = new Vector2[16*16, 4];



        public static void loadTextures() {
			terrain = loadTexture(Game.dirTex+"terrain.png");

            int k = 0;

			float x1;
			float y1;
			float x2;
			float y2;

            for (int y = 0; y < 16; y++) {
                for (int x = 0; x < 16; x++) {
                    x1 = x * (1.0f / 16.0f);
                    y1 = y * (1.0f / 16.0f);
                    x2 = (x + 1) * (1.0f / 16.0f);
                    y2 = (y + 1) * (1.0f / 16.0f);
                    tex16x16Coords[k, 0] = new Vector2(x1, y1);
                    tex16x16Coords[k, 1] = new Vector2(x2, y1);
                    tex16x16Coords[k, 2] = new Vector2(x1, y2);
                    tex16x16Coords[k, 3] = new Vector2(x2, y2);
					//Console.WriteLine("Tex Coord: {0}, {1}, {2}, {3}", tex16x16Coords[k, 0]*16, tex16x16Coords[k, 1]*16, tex16x16Coords[k, 2]*16, tex16x16Coords[k, 3]*16);
					k++;
                }
            }

        }

        public static int loadTexture(string path) {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

			if(File.Exists(path)) {

				using(Bitmap bmp = new Bitmap(path)) {
					BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

					GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

					bmp.UnlockBits(bmpData);

					bmp.Dispose();
					Console.WriteLine("Texture loaded: {0}", path);
				}
			} else {
				Console.WriteLine("{0} does not exist.", path);
			}

            return id;
        }

    }
}