using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
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
        public string connectionProvider = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";

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

            // Check if the passcode is expired
            if (IsPasscodeExpired(email))
            {
                TempData["FailMessage"] = "Passcode has expired. Please request a new one.";
                return Page();
            }

            // Check if the new password and confirm password match
            if (newPassword != confirmPassword)
            {
                TempData["FailMessage"] = "New password and confirm password do not match!";
                return Page();
            }

            // Update the password and passcode expiration in the database
            try
            {
                if (IsEmailExistsInTable(email, "LoginCustomer"))
                {
                    UpdatePasswordAndPasscodeExpiration(email, newPassword, "LoginCustomer");
                }
                else if (IsEmailExistsInTable(email, "LoginSample"))
                {
                    UpdatePasswordAndPasscodeExpiration(email, newPassword, "LoginSample");
                }

                TempData["AlertMessage"] = "Password changed successfully!";
                return RedirectToPage("/Account");
            }
            catch (Exception ex)
            {
                TempData["FailMessage"] = $"An error occurred while updating the password: {ex.Message}";
                return Page();
            }
        }
        private bool IsEmailExistsInTable(string email, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionProvider))
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

        // Method to check if the email and passcode are valid
        private bool IsCredentialsValid(string email, string passcode)
        {
            if (IsEmailExistsInTable(email, "LoginCustomer"))
            {
                return CheckPasscode(email, passcode, "LoginCustomer");
            }
            else if (IsEmailExistsInTable(email, "LoginSample"))
            {
                return CheckPasscode(email, passcode, "LoginSample");
            }

            return false;
        }

        // Method to check if the passcode is valid
        private bool CheckPasscode(string email, string passcode, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();
                string sql = $"SELECT COUNT(*) FROM {tableName} WHERE email = @email AND passcode = @passcode";
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

        // Method to check if the passcode is expired
        private bool IsPasscodeExpired(string email)
        {
            if (IsEmailExistsInTable(email, "LoginCustomer"))
            {
                return CheckPasscodeExpiration(email, "LoginCustomer");
            }
            else if (IsEmailExistsInTable(email, "LoginSample"))
            {
                // Assume passcodes in LoginSample do not expire
                return false;
            }

            // Default to false if the email is not found in either table
            return false;
        }

        // Method to check if the passcode is expired for LoginCustomer
        private bool CheckPasscodeExpiration(string email, string tableName)
        {
            if (tableName == "LoginCustomer")
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = $"SELECT passcode_exp FROM {tableName} WHERE email = @email";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);
                        object result = command.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            DateTime passcodeExpiration = Convert.ToDateTime(result);
                            return passcodeExpiration < DateTime.Now;
                        }
                    }
                }

                // Default to false if no passcode expiration is found
                return false;
            }

            // For tables other than LoginCustomer, assume passcodes do not expire
            return false;
        }

        // Method to update the password and passcode expiration in the database
        private void UpdatePasswordAndPasscodeExpiration(string email, string newPassword, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();
                string sql = $"UPDATE {tableName} SET password = @newPassword, passcode_exp = NULL WHERE email = @email";
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
                // Additional logic if needed
            }
        }
    }
}
