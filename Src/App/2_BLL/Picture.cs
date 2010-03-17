using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using Wilson.ORMapper;

namespace Bll
{
	public class Picture 
	{
		private int id;

		private Product product; // Strongly Type as Product if not Lazy-Loading
		private int productId;

		public int Id
		{
			get { return id; }
		}


		public int ProductId
		{
			get { return productId; }
			set { productId = value; }
		}

		public Product Product
		{
			get { return product }
			set
			{
				product = value;
				
			}
		}

		public void SavePictures(HttpPostedFile file, string imagePath)
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