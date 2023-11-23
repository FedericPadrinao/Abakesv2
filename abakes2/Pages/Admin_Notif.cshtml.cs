using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using MimeKit.Utils;

namespace abakes2.Pages
{
    public class AdminNotifModel : PageModel
    {
        public string successMessage = "";
        public string errorMessage = "";
        public string connectionProvider = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";
        public NotificationInfo notifInfo = new NotificationInfo();
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            string NotifTitle = Request.Form["title"];
            string NotifText = Request.Form["textmessage"];


            try
            {
                //image upload
                if (file != null && file.Length > 0)
                {

                    using (SqlConnection connection = new SqlConnection(connectionProvider))
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "Notification", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // string sql = "insert into LoginCustomer (picture) values (@image)";
                        connection.Open();

                        string sql = "Insert into Notification (NotificationTitle,NotificationText,NotificationImage,status) values (@NotifTitle,@NotifText,@NotifImage,'true')";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@NotifTitle", NotifTitle);
                            command.Parameters.AddWithValue("@NotifText", NotifText);
                            command.Parameters.AddWithValue("@NotifImage", "/img/Notification/" + fileName);

                            command.ExecuteNonQuery();

                            // Retrieve all users from the LoginCustomer table
                            string getUsersSql = "SELECT fname, email, is_verified FROM LoginCustomer WHERE is_verified = 1";
                            using (SqlCommand getUsersCommand = new SqlCommand(getUsersSql, connection))
                            {
                                using (SqlDataReader reader = getUsersCommand.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string customerName = reader["fname"].ToString();
                                        string customerEmail = reader["email"].ToString();

                                        // Send email with image attachment
                                        SendNotificationEmail(customerName, customerEmail, NotifTitle, NotifText, filePath);
                                    }
                                }
                            }
                        }
                    }
                }
            }


            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Page();
            }
            // Set TempData for success message
            TempData["SuccessMessage"] = "Notification successfully sent!";
            return Redirect("/Admin_Notif");
        }

        private void SendNotificationEmail(string customerName, string customerEmail, string notifTitle, string notifText, string imagePath)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress("A-bakes", "abakes881@gmail.com"));
            email.To.Add(new MailboxAddress(customerName, customerEmail));

            email.Subject = notifTitle;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = notifText;

            // Attach the image as an inline resource
            var image = bodyBuilder.LinkedResources.Add(imagePath);
            image.ContentId = MimeUtils.GenerateMessageId();

            // Set the HTML body with inline image
            bodyBuilder.HtmlBody = $@"
        <p>{notifText}</p>
        <p><center><img src=""cid:{image.ContentId}""></center></p>";

            email.Body = bodyBuilder.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 465, true);
                smtp.Authenticate("abakes881@gmail.com", "gvok rqua fsbr ufuz");
                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }
    }
}