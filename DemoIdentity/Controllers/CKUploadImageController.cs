using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DemoIdentity.Controllers
{
    public static class ImageValidator
    {
        public static bool IsImage(this IFormFile file)
        {
            try
            {
                var img = Image.FromStream(file.OpenReadStream());
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class CKUploadImageController : Controller
    {
        [HttpPost]
        public ActionResult Upload(IFormFile upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            // upload file sai, ví dụ file ko lên, timeout
            if (upload.Length <= 0) 
                return null;

            if (!upload.IsImage())
            {
                var NotImageMessage = "Please choose a picture";
                // tạo dynamic object chứa thông báo lỗi
                var NotImage = new { uploaded = 0, error = new { message = NotImageMessage } };
                return Json(NotImage);
            }

            // tạo filename trên server ngẫu nhiên
            var filename = Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();

            Image img = Image.FromStream(upload.OpenReadStream());
            int width = img.Width;
            int height = img.Height;
            // chỉ nhận hình kích thước full hd
            if (width > 1920 || height > 1080)
            {
                var msg = "Wrong Size. Please upload Full HD Image or below";
                var WrongSize = new { uploaded = 0, error = new { message = msg } };
                return Json(WrongSize);
            }

            // file size max = 10 Mb
            if (upload.Length > 10 * 1024 * 1024)
            {
                var msg = "Please upload image size 10Mb or below";
                var WrongSize = new { uploaded = 0, error = new { message = msg } };
                return Json(WrongSize);
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", filename);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                upload.CopyTo(stream);
            }

            var url = $"/images/{filename}";
            var msgSuccess = "Image Uploaded Successfully.";
            var success = new { uploaded = 1, url, error = new { message = msgSuccess } };
            return Json(success);
        }
    }
}
