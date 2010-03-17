
using System.Drawing;
using System.Drawing.Imaging;

using NUnit.Framework;
using Bll;

namespace TestBllProjekt
{
    
    [TestFixture]
    public class ImageHelperTest
    {
        [Test]
        public void ResizeTest()
        {
            
            var imgPhotoVert = Image.FromFile("olo.bmp");
            var converted=ImageHelper.FixedSize(imgPhotoVert, 128, 96);
            converted.Save(@"olo_small.bmp", ImageFormat.Bmp);
            
            
        }
    }
}
