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
        public string connectionProvider = "Data Source=ROVIC\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";

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
            if (IsEmailAndVerificationCodeValid(email, verificationCode))
            {
                UpdateIsVerifiedStatus(email);
                return RedirectToPage("/Account_Verif_Succ");
            }
            else
            {
                TempData["FailMessage"] = "Invalid verification code or code has expired.";
                return Page();
            }
        }

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
                    return count > 0;
                }
            }
        }
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
                    return result != null && (bool)result;
                }
            }
        }
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
                    return count > 0;
                }

            }
        }
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
