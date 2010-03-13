
using System.Drawing;
using System.Drawing.Imaging;

using NUnit.Framework;
using Roberta.Bll;

namespace TestBllProjekt
{
    
    [TestFixture]
    public class ImageHelperTest
    {
        [Test]
        public void ResizeTest()
        {
            string WorkingDirectory = @"C:\";
            Image imgPhotoVert = Image.FromFile(WorkingDirectory +
                                            @"\bolo.bmp");
            Image converted=ImageHelper.FixedSize(imgPhotoVert, 128, 96);
            converted.Save(WorkingDirectory +
    @"\bolo_smal.bmp", ImageFormat.Bmp);
            
            
        }
    }
}
