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

            string newPasscode = GeneratePasscode();
            UpdatePasscode(email, newPasscode);
            UpdatePasscodeExpiration(email);

            string UserName = GetUserName(email);
            SendPasscodeByEmail(email, UserName, newPasscode, GetPasscodeExpiration(email));

            TempData["AlertMessage"] = "Verification code sent! Please check your email for the new code to change your password.";
            TempData["Email"] = email;
            return RedirectToPage("/Account_ChangePasscode");
        }

        private void UpdatePasscode(string email, string passcode)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionProvider))
            {
                connection.Open();
                string sql = "UPDATE LoginCustomer SET passcode = @passcode, passcode_exp = DATEADD(MINUTE, 3, GETDATE()) WHERE email = @email UPDATE LoginSample SET passcode = @passcode, passcode_exp = DATEADD(MINUTE, 3, GETDATE()) WHERE email = @email";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@passcode", passcode);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void UpdatePasscodeExpiration(string email)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionProvider))
            {
                connection.Open();
                string sql = "UPDATE LoginCustomer SET passcode_exp = DATEADD(MINUTE, 3, GETDATE()) WHERE email = @email UPDATE LoginSample SET passcode_exp = DATEADD(MINUTE, 3, GETDATE()) WHERE email = @email";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.ExecuteNonQuery();
                }
            }
        }

        private string GeneratePasscode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var passcode = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return passcode;
        }

        private void SendPasscodeByEmail(string email, string UserName, string passcode, DateTime passcodeExpiration)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("A-bakes", "abakes881@gmail.com"));
            message.To.Add(new MailboxAddress(UserName, email));
            message.Subject = "Password Change Verification Code";

            var builder = new BodyBuilder();
            builder.TextBody = $"Hello {UserName},\n\nYour verification code for changing your password is: {passcode}\nIt will expire at {passcodeExpiration.ToString("yyyy-MM-dd HH:mm:ss")}\n\nThank you,\nThe Abakes Team";

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
            if (IsEmailExistsInTable(email, "LoginCustomer"))
            {
                return true;
            }
            else if (IsEmailExistsInTable(email, "LoginSample"))
            {
                return true;
            }

            return false;
        }

        private bool IsEmailExistsInTable(string email, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionProvider))
            {
                connection.Open();
                string sql = $"SELECT COUNT(*) FROM {tableName} WHERE email = @email";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count > 0;
                }
            }
        }

        private string GetUserName(string email)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionProvider))
            {
                connection.Open();
                string sql = "SELECT username FROM LoginCustomer WHERE email = @email UNION SELECT username FROM LoginSample WHERE email = @email";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);

                    return command.ExecuteScalar() as string;
                }
            }
        }

        private DateTime GetPasscodeExpiration(string email)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionProvider))
            {
                connection.Open();
                string sql = "SELECT passcode_exp FROM LoginCustomer WHERE email = @email UNION SELECT passcode_exp FROM LoginSample WHERE email = @email";
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
    }
}
