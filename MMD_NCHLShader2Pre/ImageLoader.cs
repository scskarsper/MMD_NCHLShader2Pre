using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using DmitryBrant.ImageFormats;

namespace MMD_NCHLShader2Pre
{
    class ImageLoader
    {
        public static Image Load(string File)
        {
            Image Ret = null;
            try
            {
                Ret = Image.FromFile(File);
            }
            catch { Ret = null; }
            if (Ret == null)
            {
                try
                {
                    Bitmap bmp = TgaReader.Load(File);
                    Ret = bmp;
                }
                catch { ;}
            }
            Bitmap bmp2 = new Bitmap(Ret);
            Ret.Dispose();
            return bmp2;
        }
    }
}
