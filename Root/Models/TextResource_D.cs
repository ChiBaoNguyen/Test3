using System;
using System.Collections.Generic;

namespace Root.Models
{
    public partial class TextResource_D
    {
        public int TextID { get; set; }
        public string TextKey { get; set; }
        public string TextValue { get; set; }
        public Nullable<int> LanguageID { get; set; }
        public Nullable<int> ScreenID { get; set; }
        public string Description { get; set; }
    }
}
