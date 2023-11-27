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
        public string ConnectionProvider = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";

        public void OnGet()
        {
            // Handle GET requests if needed
        }
        public IActionResult OnPost()
        {
            string email = Request.Form["email"];

            // Check if the email exists in the LoginCustomer table
            if (!IsEmailExists(email))
            {
                TempData["FailMessage"] = "Email does not exist!";
                return Page();
            }

            // Check if the email is already verified
            if (IsEmailVerified(email))
            {
                TempData["FailMessage"] = "Email is already verified!";
                return Page();
            }

            // Generate a new verification code
            string newVerificationCode = GenerateVerificationCode();

            // Update the verification code in the database
            UpdateVerificationCode(email, newVerificationCode);

            // Get the first name associated with the email
            string firstName = GetFirstName(email);

            // Send the new verification code to the user
            SendVerificationCodeByEmail(email, firstName, newVerificationCode);

            // Set a success message and stay on the same page
            TempData["ResendSucc"] = "Verification code sent. Please check your email for the new verificaiton code.";
            return RedirectToPage("/Account_Verify");
        }


        // Method to validate email
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

                    // If count is greater than 0, the email is valid
                    return count > 0;
                }
            }
        }

        // Method to update verification code
        private void UpdateVerificationCode(string email, string verificationCode)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionProvider))
            {
                connection.Open();
                string sql = "UPDATE LoginCustomer SET verification_code = @verificationCode WHERE email = @email";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@verificationCode", verificationCode);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Method to get the first name associated with the email
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

        // Method to generate a new verification code
        private string GenerateVerificationCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var verificationCode = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return verificationCode;
        }

        // Method to send the verification code by email
        private void SendVerificationCodeByEmail(string email, string firstName, string verificationCode)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("A-bakes", "abakes881@gmail.com")); // Change to your information
            message.To.Add(new MailboxAddress(firstName, email));
            message.Subject = "Verification Code Resent";

            var builder = new BodyBuilder();
            builder.TextBody = $"Hello {firstName},\n\nYour new verification code is: {verificationCode}\n\nThank you,\nThe Abakes Team";

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
