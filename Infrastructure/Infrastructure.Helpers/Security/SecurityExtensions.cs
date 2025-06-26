using System.Security.Cryptography;
using System.Text;

using Infrastructure.Extensions;

using Microsoft.AspNetCore.Http;

using MimeMapping;

namespace Infrastructure.Helpers.Security;

public static class SecurityExtensions
{
    private static readonly string password = "RjHIld5PuOs5G62Z";

    public static string Encrypt(this object obj)
    {
        if (obj == null)
        {
            return string.Empty;
        }

        return ConvertStringToBase64(Cryptography.Encrypt(Aes.Create(), obj.ToString(), password), Encoding.Unicode);
    }

    public static string Decrypt(this string text)
    {
        return Cryptography.Decrypt(Aes.Create(), ConvertBase64ToString(text, Encoding.Unicode), password);
    }


    public static T Decrypt<T>(this string text)
    {
        if (text.IsNotNullOrEmpty())
        {
            return Cryptography.Decrypt(Aes.Create(), ConvertBase64ToString(text, Encoding.Unicode), password).ToAnyType<T>();
        }
        return text.ToAnyType<T>();
    }


    public static bool Decrypt<T>(this string text, T ValueToCheck, out T value)
    {
        if (text.IsNotNullOrEmpty())
        {
            value = Cryptography.Decrypt(Aes.Create(), ConvertBase64ToString(text, Encoding.Unicode), password).ToAnyType<T>();
            return !value.Equals(ValueToCheck);
        }
        value = text.ToAnyType<T>();
        return false;
    }


    public static bool Decrypt<T>(this string text, out T value)
    {
        if (text.IsNotNullOrEmpty())
        {
            value = Cryptography.Decrypt(Aes.Create(), ConvertBase64ToString(text, Encoding.Unicode), password).ToAnyType<T>();
            return !value.Equals(default(T));
        }
        value = text.ToAnyType<T>();
        return false;
    }

    public static string ConvertStringToBase64(string input, Encoding encoding)
    {
        byte[] bytes = encoding.GetBytes(input);
        return Convert.ToBase64String(bytes);
    }

    public static string ConvertBase64ToString(string base64Input, Encoding encoding)
    {
        byte[] bytes = Convert.FromBase64String(base64Input);
        return encoding.GetString(bytes);
    }

    public static bool ValidateListFiles(this List<IFormFile> lstData)
    {
        try
        {
            var validImageMimeTypes = new List<string>
                {
                    "image/jpeg",      // JPEG/JPG
                    "image/pjpeg",     // Alternative for JPEG (commonly seen in old browsers)
                    "image/png",       // PNG
                    "image/gif",       // GIF
                    "image/bmp",       // BMP
                    "image/webp",      // WebP
                    "image/tiff",      // TIFF
                    "image/x-icon",    // ICO (Icon files)
                    "image/svg+xml",   // SVG
                    "image/avif",      // AVIF
                    "image/heic"       // HEIC (High Efficiency Image Format)
                };


            if (lstData.IsNotNullOrEmpty())
            {
                foreach (var item in lstData)
                {
                    string mimeType = MimeUtility.GetMimeMapping(item.FileName);
                    if (!validImageMimeTypes.Contains(mimeType))
                        return false;

                    string fileExtension = GetImageExtensionFromMIMEType(mimeType);
                    if (!fileExtension.IsNotNullOrEmpty())
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static string GetImageExtensionFromMIMEType(string mimeType)
    {
        switch (mimeType.ToLower())
        {
            case "image/png":
                return "png";

            case "image/jpeg":
            case "image/jpg":
            case "image/pjpeg":
                return "jpeg";

            case "image/gif":
                return "gif";

            case "image/bmp":
                return "bmp";

            case "image/webp":
                return "webp";

            case "image/tiff":
            case "image/tif": 
                return "tiff";

            case "image/x-icon":
            case "image/vnd.microsoft.icon": 
                return "ico";

            case "image/svg+xml":
                return "svg";

            case "image/avif":
                return "avif";

            case "image/heic":
                return "heic";

            default:
                return string.Empty;
        }

    }

    //private static async Task<byte[]> GetAttachmentContent(IFormFile attachment)
    //{
    //    try
    //    {
    //        using (var memoryStream = new MemoryStream())
    //        {
    //            memoryStream.Seek(0, SeekOrigin.Begin);
    //            await attachment.CopyToAsync(memoryStream);
    //            memoryStream.Seek(0, SeekOrigin.Begin);
    //            return memoryStream.ToArray();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        return Array.Empty<byte>();
    //    }
    //}
}
