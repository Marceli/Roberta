
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Bll
{
    public class ImageHelper
    {

        public static Image FixedSize(Stream imgPhotoStram, int Width, int Height)
        {
            Image zdjecie = Image.FromStream(imgPhotoStram);
            return FixedSize(zdjecie, Width, Height);
            
        }
		public static Image FixedSize2(Stream imgPhotoStram, int Width, int Height)
		{
			var oryginalImage = Image.FromStream(imgPhotoStram);
			Image result = new Bitmap(Width, Height, oryginalImage.PixelFormat);
			var oGraphic =  Graphics.FromImage(result);
			oGraphic.CompositingQuality = CompositingQuality.HighQuality ;
			oGraphic.SmoothingMode = SmoothingMode.HighQuality ;
			oGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic ;
			var oRectangle = new Rectangle(0, 0, Width, Height);
			oGraphic.DrawImage(oryginalImage, oRectangle);
			return result;
		}
        public static Image FixedSize(Image imgPhoto, int Width, int Height)
        {
            
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            const int sourceX = 0;
            const int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent ;

        	var nPercentW = (Width / (float)sourceWidth);
            var nPercentH = (Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentW;
                        destY = (int)
                            ((Height - (sourceHeight * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentH;
                        destX = (int)
                          ((Width - (sourceWidth * nPercent)) / 2);
            }

            var destWidth = (int)(sourceWidth * nPercent);
            var destHeight = (int)(sourceHeight * nPercent);

            var bmPhoto = new  Bitmap(Width,
                    Height, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                    imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
    }
}
