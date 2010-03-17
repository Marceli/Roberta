
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
        public static Image FixedSize(Image imgPhoto, int Width, int Height)
        {
            
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent ;
            float nPercentW ;
            float nPercentH ;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentW;
        //        switch (Anchor)
        //        {
        //            case AnchorPosition.Top:
        //                destY = 0;
        //                break;
        //            case AnchorPosition.Bottom:
        //                destY = (int)
        //                    (Height - (sourceHeight * nPercent));
        //                break;
        //            default:
                        destY = (int)
                            ((Height - (sourceHeight * nPercent)) / 2);
        //                break;
        //        }
            }
            else
            {
                nPercent = nPercentH;
        //        switch (Anchor)
        //        {
        //            case AnchorPosition.Left:
        //                destX = 0;
        //                break;
        //            case AnchorPosition.Right:
        //                destX = (int)
        //                  (Width - (sourceWidth * nPercent));
        //                break;
        //            default:
                        destX = (int)
                          ((Width - (sourceWidth * nPercent)) / 2);
        //                break;
        //        }
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width,
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
