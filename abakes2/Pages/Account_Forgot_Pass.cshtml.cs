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
        public string Email { get; set; }
        public string ConnectionProvider = "Data Source=ROVIC\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";
        public IActionResult OnPost()
        {
            string email = Request.Form["email"];

            // Check if the email exists in the LoginCustomer table
            if (!IsEmailExists(email))
            {
                TempData["FailMessage"] = "Email does not exist!";
                return Page();
            }

            // Generate a new passcode
            string newPasscode = GeneratePasscode();

            // Update the passcode in the database
            UpdatePasscode(email, newPasscode);

            // Get the first name associated with the email
            string firstName = GetFirstName(email);

            // Send the new passcode to the user
            SendPasscodeByEmail(email, firstName, newPasscode);

            // Set a success message and stay on the same page
            TempData["AlertMessage"] = "Verification code sent! Please check your email for the new code to change your password.";
            return RedirectToPage("/Account_ChangePasscode");
        }

        // ... (existing code)

        // Method to update passcode
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
            message.From.Add(new MailboxAddress("A-bakes", "abakes881@gmail.com")); // Change to your information
            message.To.Add(new MailboxAddress(firstName, email));
            message.Subject = "New Passcode";

            var builder = new BodyBuilder();
            builder.TextBody = $"Hello {firstName},\n\nPassword Code is: {passcode}\n\nThank you,\nThe Abakes Team";

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("abakes881@gmail.com", "gvok rqua fsbr ufuz"); // Replace with your Mailtrap credentials

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