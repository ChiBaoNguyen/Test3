using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Website.ViewModels.Mobile.Dispatch
{
    public class ImageUploadViewModel
    {
        public int ImageId { get; set; }

        public string DispatchImageID { get; set; }
        //public Bitmap Image { get; set; }

        public object[] DataImage { get; set; }
        public string CreatedDate { get; set; }
        public string ImageInfo { get; set; }
    }
}
