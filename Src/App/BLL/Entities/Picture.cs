using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using Bll;

namespace Core.Entities
{
	public class Picture
	{
		private int id;

		public virtual int Id
		{
			get { return id; }
			protected set { id = value; }
		}

		public virtual Product Product { get; set; }

		public virtual void SavePictures(Stream imageStream, string imagePath)
		{
			
			Image image = ImageHelper.FixedSize2(imageStream, 64, 48);
			image.Save(string.Format("{0}xs{1}.jpeg", imagePath, id), ImageFormat.Jpeg);
			ImageHelper.FixedSize(imageStream, 128, 96).Save(string.Format("{0}s{1}.jpeg", imagePath, id), ImageFormat.Jpeg);
			ImageHelper.FixedSize(imageStream, 400, 300).Save(string.Format("{0}m{1}.jpeg", imagePath, id), ImageFormat.Jpeg);
			ImageHelper.FixedSize(imageStream, 800, 600).Save(string.Format("{0}b{1}.jpeg", imagePath, id), ImageFormat.Jpeg);
		}
	}
}