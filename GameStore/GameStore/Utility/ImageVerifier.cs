using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GameStore.Utility
{
    public static class ImageVerifier
    {
        public static bool IsImageIsValidContentType(this IFormFile image)
        {
            return string.Equals(image.ContentType.ToLower(), "image/jpg",
                       StringComparison.OrdinalIgnoreCase) || string.Equals(image.ContentType.ToLower(), "image/jpeg",
                       StringComparison.OrdinalIgnoreCase) || string.Equals(image.ContentType.ToLower(), "image/png",
                       StringComparison.OrdinalIgnoreCase) || string.Equals(image.ContentType.ToLower(), "image/gif",
                       StringComparison.OrdinalIgnoreCase) || string.Equals(image.ContentType.ToLower(), "image/bmp",
                       StringComparison.OrdinalIgnoreCase) || string.Equals(image.ContentType.ToLower(), "image/pjpeg",
                       StringComparison.OrdinalIgnoreCase) || string.Equals(image.ContentType.ToLower(), "image/x-png",
                       StringComparison.OrdinalIgnoreCase);
        }


        public static bool IsImageHasValidExtension(this IFormFile image)
        {
            return Path.GetExtension(image.FileName).ToLower() == ".jpg" || Path.GetExtension(image.FileName).ToLower() == ".png" || Path.GetExtension(image.FileName).ToLower() == ".gif" || Path.GetExtension(image.FileName).ToLower() == ".jpeg" || Path.GetExtension(image.FileName).ToLower() == ".bmp";
        }

        public static bool IsImageHasValidSize(this IFormFile image, int size)
        {
            return image.Length >= size;
        }

        public static bool IsImageByteReadable(this IFormFile image)
        {
            return image.OpenReadStream().CanRead;
        }

        public static bool IsImageCsrfFree(this IFormFile image, int size)
        {
            byte[] buffer = new byte[size];
            image.OpenReadStream().Read(buffer, 0, size);
            string content = System.Text.Encoding.UTF8.GetString(buffer);
            return !Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline);
        }
    }
}
