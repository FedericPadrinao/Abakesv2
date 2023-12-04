using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;
using System.Linq;
using MailKit.Net.Smtp;
using MimeKit;

namespace abakes2.Pages
{
    public class Account_Forgot_PassModel : PageModel
    {
        public String userconfirm = "";
        public string Email { get; set; }
        public string ConnectionProvider = "Server=tcp:eu-az-sql-serv8c295e6a1afc4f69be52fd159aeb63da.database.windows.net,1433;Initial Catalog=drt6diqvzxczvbi;Persist Security Info=False;User ID=uhsk2j20jhg6qgk;Password=3CZlMPeUY7D3yleRYezMeodZ2;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public void OnGet() {
            userconfirm = HttpContext.Session.GetString("username");

            if (userconfirm != null)
            {
                Response.Redirect("/Index");

            }
            else
            {

            }
        }
        public IActionResult OnPost()
        {
            string email = Request.Form["email"];

            if (!IsEmailExists(email))
            {
                TempData["FailMessage"] = "Email does not exist!";
                return Page();
            }

            string newPasscode = GeneratePasscode();

            UpdatePasscode(email, newPasscode);

            string firstName = GetFirstName(email);

            SendPasscodeByEmail(email, firstName, newPasscode);

            TempData["AlertMessage"] = "Verification code sent! Please check your email for the new code to change your password.";
            TempData["Email"] = email;
            return RedirectToPage("/Account_ChangePasscode");
        }


        private void UpdatePasscode(string email, string passcode)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionProvider))
            {
                connection.Open();
                string sql = "UPDATE LoginCustomer SET passcode = @passcode WHERE email = @email";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@passcode", passcode);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Method to generate a new passcode
        private string GeneratePasscode()
        {
            // Customize the passcode generation logic as needed
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var passcode = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return passcode;
        }

        // Method to send the passcode by email
        private void SendPasscodeByEmail(string email, string firstName, string passcode)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("A-bakes", "abakes881@gmail.com")); 
            message.To.Add(new MailboxAddress(firstName, email));
            message.Subject = "Password Change Verification Code";

            var builder = new BodyBuilder();
            builder.TextBody = $"Hello {firstName},\n\nYour verification code for changing your passord is: {passcode}\n\nThank you,\nThe Abakes Team";

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("abakes881@gmail.com", "gvok rqua fsbr ufuz");

                client.Send(message);
                client.Disconnect(true);
            }
        }

        private bool IsEmailExists(string email)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionProvider))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM LoginCustomer WHERE email = @email";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    // If count is greater than 0, the email exists
                    return count > 0;
                }
            }
        }
        private string GetFirstName(string email)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionProvider))
            {
                connection.Open();
                string sql = "SELECT fname FROM LoginCustomer WHERE email = @email";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);

                    return command.ExecuteScalar() as string;
                }
            }
        }
    }
}