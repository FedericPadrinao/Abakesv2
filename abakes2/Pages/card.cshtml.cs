using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace abakes2.Pages
{
    public class cardModel : PageModel
    {
        public string QRCodeImage { get; set; }

        public string code = "";
        public void OnGet()
        {
            string userconfirm = HttpContext.Session.GetString("username");

            if (userconfirm != null)
            {

            }
            else
            {
                Response.Redirect("/index");
            }
            code = Request.Query["maincode"];
            GenerateQRCode(code);

        }
        private void GenerateQRCode(string qrcode)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrcode, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                using (Bitmap bitMap = qrCode.GetGraphic(20))
                {
                    bitMap.Save(ms, ImageFormat.Png);
                    QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }
}