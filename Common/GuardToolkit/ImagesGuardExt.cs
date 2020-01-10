using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.IO;

namespace Common.GuardToolkit
{
    public static class ImagesGuardExt
    {
        public static bool IsImageFile(this byte[] photoFile)
        {
            if (photoFile == null || photoFile.Length == 0)
                return false;

            using var memoryStream = new MemoryStream(photoFile);
            using var img = Image.FromStream(memoryStream);

            return img.Width > 0;
        }

        public static bool IsValidImageFile(this IFormFile photoFile, int maxWidth = 150, int maxHeight = 150)
        {
            if (photoFile == null || photoFile.Length == 0) return false;
            using (var img = Image.FromStream(photoFile.OpenReadStream()))
            {
                if (img.Width > maxWidth) return false;
                if (img.Height > maxHeight) return false;
            }
            return true;
        }

        public static bool IsImageFile(this IFormFile photoFile)
        {
            if (photoFile == null || photoFile.Length == 0)
                return false;

            using var img = Image.FromStream(photoFile.OpenReadStream());

            return img.Width > 0;
        }
    }
}