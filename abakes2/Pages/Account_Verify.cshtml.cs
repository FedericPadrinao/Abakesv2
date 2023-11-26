using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Account_VerifyModel : PageModel
    {
        public string email { get; set; }
        public string verification_code { get; set; }
        public string connectionProvider = "Data Source=orange\\sqlexpress;Initial Catalog=Abakes;Integrated Security=True";

        public void OnGet()
        {
            email = HttpContext.Request.Query["email"];
        }


        public IActionResult OnPost()
        {
            string email = Request.Form["email"];
            string verificationCode = Request.Form["verificationCode"];

            // Validate the email and verification code against the database
            if (IsEmailAndVerificationCodeValid(email, verificationCode))
            {
                // Update the is_verified status to true in the database
                UpdateIsVerifiedStatus(email);
                return RedirectToPage("/Account_Verif_Succ"); // Change this to your success page
            }
            else
            {
                // Set an error message and stay on the same page
                TempData["FailMessage"] = "Error Message";
                return Page();
            }
        }

        // Method to validate email and verification code
        private bool IsEmailAndVerificationCodeValid(string email, string verificationCode)
        {
            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM LoginCustomer WHERE email = @email AND verification_code = @verificationCode";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@verificationCode", verificationCode);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    // If count is greater than 0, the email and verification code match
                    return count > 0;
                }
            }
        }

        // Method to update is_verified status
        private void UpdateIsVerifiedStatus(string email)
        {
            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();
                string sql = "UPDATE LoginCustomer SET is_verified = 1 WHERE email = @email";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.ExecuteNonQuery();
                }
            }
        }

        //Verification Code

        private string GenerateVerificationCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var verificationCode = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return verificationCode;
        }
    }
}
