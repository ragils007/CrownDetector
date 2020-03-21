using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Msdfa.Core.Tools
{
    public class ImageTools
    {
        public static Image CreateGrayscale(Image srcImage)
        {
            if (srcImage == null) return null;

            //create a blank bitmap the same size as original
            var newBitmap = new Bitmap(srcImage.Width, srcImage.Height);

            //get a graphics object from the new image
            var g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            var colorMatrix = new ColorMatrix(
                new float[][]
                {
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                });

            //create some image attributes
            var attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(srcImage, new Rectangle(0, 0, srcImage.Width, srcImage.Height),
                0, 0, srcImage.Width, srcImage.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }

        public static Image CreateAlphaImageLight(Image srcImage, int alpha = 128)
        {
            var r = new Rectangle(0, 0, srcImage.Width, srcImage.Height);
            alpha = 128;
            using (var g = Graphics.FromImage(srcImage))
            {
                using (var cloud_brush = new SolidBrush(Color.FromArgb(alpha, Color.White)))
                {
                    g.FillRectangle(cloud_brush, r);
                }
            }
            return srcImage;
        }

        public static Image CreateAlphaImageDark(Image srcImage, int alpha = 128)
        {
            var r = new Rectangle(0, 0, srcImage.Width, srcImage.Height);
            alpha = 128;
            using (var g = Graphics.FromImage(srcImage))
            {
                using (var cloud_brush = new SolidBrush(Color.FromArgb(alpha, Color.Black)))
                {
                    g.FillRectangle(cloud_brush, r);
                }
            }
            return srcImage;
        }

/*
        public static Image CreateDisabledImage(Image srcImage)
        {
            if (srcImage == null) return null;
            return ToolStripRenderer.CreateDisabledImage(srcImage);
        }
*/

        public static Image CreateBlackWhite(Image srcImage)
        {
            if (srcImage == null) return null;

            var newImage = (Image) srcImage.Clone();

            using (var gr = Graphics.FromImage(newImage)) // SourceImage is a Bitmap object
            {
                var gray_matrix = new[]
                {
                    new float[] {0.299f, 0.299f, 0.299f, 0, 0},
                    new float[] {0.587f, 0.587f, 0.587f, 0, 0},
                    new float[] {0.114f, 0.114f, 0.114f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                };

                var ia = new System.Drawing.Imaging.ImageAttributes();
                ia.SetColorMatrix(new System.Drawing.Imaging.ColorMatrix(gray_matrix));
                ia.SetThreshold(0.8f); // Change this threshold as needed
                var rc = new Rectangle(0, 0, newImage.Width, newImage.Height);
                gr.DrawImage(newImage, rc, 0, 0, newImage.Width, newImage.Height, GraphicsUnit.Pixel, ia);
            }

            return newImage;
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[]) converter.ConvertTo(img, typeof(byte[]));
        }

        public static Bitmap BytesToBitmap(byte[] imageData)
        {
            Bitmap bmp;
            using (var ms = new MemoryStream(imageData))
            {
                bmp = new Bitmap(ms);
            }

            return bmp;
        }
    }
}