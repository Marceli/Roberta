using System;
using System.Collections.Generic;
using System.Text;
using Wilson.ORMapper;

namespace Bll
{


    public class Picture:EntityBase
    {
        private int id;
        
        private int productId;
        private ObjectHolder<Product> product; // Strongly Type as Product if not Lazy-Loading

        public int Id
        {
            get { return this.id; }
        }

        

        public int ProductId
        {
            get { return this.productId; }
            set { this.productId = value; }
        }
        public Product Product
        {
            get { return product.InnerObject; }
            set { product.InnerObject = value;
                if(value!=null) productId= value.Id ;
            }
        }
        public void SavePictures(System.Web.HttpPostedFile file, string imagePath)
        {
            System.Drawing.Image image64x48 = ImageHelper.FixedSize(file.InputStream, 64, 48);
            System.Drawing.Image image128x96 = ImageHelper.FixedSize(file.InputStream, 128, 96);
            System.Drawing.Image image400x300 = ImageHelper.FixedSize(file.InputStream, 400, 300);
            System.Drawing.Image image800x600 = ImageHelper.FixedSize(file.InputStream, 800, 600);
            image64x48.Save(string.Format("{0}xs{1}.jpeg", imagePath, id), System.Drawing.Imaging.ImageFormat.Jpeg);            
            image128x96.Save(string.Format("{0}s{1}.jpeg", imagePath, id), System.Drawing.Imaging.ImageFormat.Jpeg);
            image400x300.Save(string.Format("{0}m{1}.jpeg", imagePath, id), System.Drawing.Imaging.ImageFormat.Jpeg);
            image800x600.Save(string.Format("{0}b{1}.jpeg", imagePath, id), System.Drawing.Imaging.ImageFormat.Jpeg);

        }
    }
}


