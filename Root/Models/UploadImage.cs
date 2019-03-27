using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace Root.Models
{
    public class UploadImageMobile
    {
        public Guid ImageId { get; set; }

        public string OrderImageKey { get; set; }

        public string ImageLink { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
