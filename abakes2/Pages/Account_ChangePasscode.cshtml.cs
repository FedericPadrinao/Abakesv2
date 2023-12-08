using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Account_ChangePasscodeModel : PageModel
    {
        public String userconfirm = "";
        public String imgconfirm = "";
        public String statusconfirm = "";

        public int notifCount = 0;
        public int pnotifCount = 0;
        public int cartCount = 0;
        public int totalnotifCount = 0;
        public int NotificationCount { get; set; }
        public string connectionProvider = "Data Source=ROVIC\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";
        public IActionResult OnPost()
        {
            string email = Request.Form["email"];
            string passcode = Request.Form["passcode"];
            string newPassword = Request.Form["newPassword"];
            string confirmPassword = Request.Form["confirmPassword"];

            // Check if the email and passcode match the credentials in the database
            if (!IsCredentialsValid(email, passcode))
            {
                TempData["FailMessage"] = "Invalid email or passcode!";
                return Page();
            }

            // Check if the new password and confirm password match
            if (newPassword != confirmPassword)
            {
                TempData["FailMessage"] = "New password and confirm password do not match!";
                return Page();
            }

            // Update the password in the database
            UpdatePassword(email, newPassword);

            TempData["AlertMessage"] = "Password changed successfully!";
            return RedirectToPage("/Account");
        }

        // Method to check if the email and passcode are valid
        private bool IsCredentialsValid(string email, string passcode)
        {
            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM LoginCustomer WHERE email = @email AND passcode = @passcode";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@passcode", passcode);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    // If count is greater than 0, the email and passcode are valid
                    return count > 0;
                }
            }
        }

        // Method to update the password in the database
        private void UpdatePassword(string email, string newPassword)
        {
            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();
                string sql = "UPDATE LoginCustomer SET password = @newPassword WHERE email = @email";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@newPassword", BCrypt.Net.BCrypt.HashPassword(newPassword));
                    command.ExecuteNonQuery();
                }
            }
        }
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
    }
}