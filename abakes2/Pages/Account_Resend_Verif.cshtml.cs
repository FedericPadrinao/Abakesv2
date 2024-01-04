using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;
using System.Linq;
using MailKit.Net.Smtp;
using MimeKit;

namespace abakes2.Pages
{
    public class Account_Resend_VerifModel : PageModel
    {
        public string Email { get; set; }
        public String userconfirm = "";
        public string ConnectionProvider = "Data Source=ROVIC\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";

        public void OnGet()
        {
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

            if (IsEmailVerified(email))
            {
                TempData["FailMessage"] = "Email is already verified!";
                return Page();
            }
            string newVerificationCode = GenerateVerificationCode();
            UpdateVerificationCodeAndExpiration(email, newVerificationCode);
            string firstName = GetFirstName(email);

            DateTime verificationCodeExpiration = GetVerificationCodeExpiration(email);
            SendVerificationCodeByEmail(email, firstName, newVerificationCode, verificationCodeExpiration);

            TempData["ResendSucc"] = "Verification code sent. Please check your email for the new verificaiton code.";
            return RedirectToPage("/Account_Verify");
        }


        private bool IsEmailValid(string email)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionProvider))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM LoginCustomer WHERE email = @email";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);

                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        private void UpdateVerificationCodeAndExpiration(string email, string verificationCode)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionProvider))
            {
                connection.Open();
                string sql = "UPDATE LoginCustomer SET verification_code = @verificationCode, verif_exp = DATEADD(MINUTE, 3, GETDATE()) WHERE email = @email";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@verificationCode", verificationCode);
                    command.ExecuteNonQuery();
                }
            }
        }

        private DateTime GetVerificationCodeExpiration(string email)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionProvider))
            {
                connection.Open();
                string sql = "SELECT verif_exp FROM LoginCustomer WHERE email = @email";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToDateTime(result);
                    }
                }
            }
            return DateTime.Now;
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
        private string GenerateVerificationCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var verificationCode = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return verificationCode;
        }
        private void SendVerificationCodeByEmail(string email, string firstName, string verificationCode, DateTime verificationCodeExpiration)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("A-bakes", "abakes881@gmail.com")); 
            message.To.Add(new MailboxAddress(firstName, email));
            message.Subject = "New Verification Code";

            var builder = new BodyBuilder();
            builder.TextBody = $"Hello {firstName},\n\nYour new verification code is: {verificationCode}\nIt will expire at {verificationCodeExpiration.ToString("yyyy-MM-dd HH:mm:ss")}\n\nThank you,\nThe Abakes Team";

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("abakes881@gmail.com", "oclt owgw hcgf ttok"); 

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

        // Method to check if the email is already verified
        private bool IsEmailVerified(string email)
        {
            // First, check if the email exists
            if (!IsEmailExists(email))
            {
                return false;
            }

            using (SqlConnection connection = new SqlConnection(ConnectionProvider))
            {
                connection.Open();
                string sql = "SELECT is_verified FROM LoginCustomer WHERE email = @email";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);

                    object result = command.ExecuteScalar();

                    // If result is not null and is true, the email is verified
                    return result != null && (bool)result;
                }
            }
        }
    }
}
