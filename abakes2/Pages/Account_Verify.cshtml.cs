using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Account_VerifyModel : PageModel
    {
        public string email { get; set; }
        public String userconfirm = "";
        public string verification_code { get; set; }
        public DateTime verif_exp { get; set; }
        public string connectionProvider = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";

        public void OnGet()
        {
            email = HttpContext.Request.Query["email"];
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
            string verificationCode = Request.Form["verificationCode"];

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
                TempData["FailMessage"] = "Invalid verification code or code has expired.";
                return Page();
            }
        }

        // Method to validate email and verification code
        private bool IsEmailAndVerificationCodeValid(string email, string verificationCode)
        {
            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM LoginCustomer WHERE email = @email AND verification_code = @verificationCode AND verif_exp > GETDATE() AND is_verified = 0";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@verificationCode", verificationCode);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    // If count is greater than 0, the email, verification code, and expiration time are valid
                    return count > 0;
                }
            }
        }

        // Method to check if the email is already verified
        private bool IsEmailVerified(string email)
        {
            using (SqlConnection connection = new SqlConnection(connectionProvider))
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

        // Method to check if the email exists in the LoginCustomer table
        private bool IsEmailExists(string email)
        {
            using (SqlConnection connection = new SqlConnection(connectionProvider))
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
