using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace KtaCase
{
    public class ImageData
    {
        /// <summary>
        /// https://www.awaresystems.be/imaging/tiff/tifftags/compression.html
        /// </summary>
        public enum TiffCompression
        {
            NONE = 1,
            CCITTRLE = 2,
            CCITTFAX3 = 3,
            CCITTFAX4 = 4,
            LZW = 5,
            OJPEG = 6,
            JPEG = 7,
            ADOBE_DEFLATE = 8,
            NEXT = 32766,
            CCITTRLEW = 32771,
            PACKBITS = 32773,
            THUNDERSCAN = 32809,
            IT8CTPAD = 32895,
            IT8LW = 32896,
            IT8MP = 32897,
            IT8BL = 32898,
            PIXARFILM = 32908,
            PIXARLOG = 32909,
            DEFLATE = 32946,
            DCS = 32947,
            JBIG = 34661,
            SGILOG = 34676,
            SGILOG24 = 34677,
            JP2000 = 34712
        }

        /// <summary>
        /// https://stackoverflow.com/a/1561112/221018
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static int GetCompressionType(Image image)
        {
            int compressionTagIndex = Array.IndexOf(image.PropertyIdList, 0x103);
            PropertyItem compressionTag = image.PropertyItems[compressionTagIndex];
            return BitConverter.ToInt16(compressionTag.Value, 0);
        }

        public static string GetCompressionName(Image image)
        {
            int comptype = GetCompressionType(image);
            return Enum.GetName(typeof(TiffCompression), comptype);
        }
    }
}
