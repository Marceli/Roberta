using System.Drawing;
using System.Drawing.Imaging;
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

		public virtual void SavePictures(HttpPostedFile file, string imagePath)
		{
			Image image64x48 = ImageHelper.FixedSize(file.InputStream, 64, 48);
			Image image128x96 = ImageHelper.FixedSize(file.InputStream, 128, 96);
			Image image400x300 = ImageHelper.FixedSize(file.InputStream, 400, 300);
			Image image800x600 = ImageHelper.FixedSize(file.InputStream, 800, 600);
			image64x48.Save(string.Format("{0}xs{1}.jpeg", imagePath, id), ImageFormat.Jpeg);
			image128x96.Save(string.Format("{0}s{1}.jpeg", imagePath, id), ImageFormat.Jpeg);
			image400x300.Save(string.Format("{0}m{1}.jpeg", imagePath, id), ImageFormat.Jpeg);
			image800x600.Save(string.Format("{0}b{1}.jpeg", imagePath, id), ImageFormat.Jpeg);
		}
	}
}