using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Admin_ChangePasswordModel : PageModel
    {
        public string connectionProvider = "Data Source=orange\\sqlexpress;Initial Catalog=Abakes;Integrated Security=True";

        public string username { get; set; }
        public string userconfirm = "";

        public IActionResult OnPost()
        {
            userconfirm = HttpContext.Session.GetString("username");
            string currentPassword = Request.Form["currentPassword"];
            string newPassword = Request.Form["newPassword"];
            string confirmNewPassword = Request.Form["confirmNewPassword"];

            // Ensure that the current password and stored password match
            if (!CheckCurrentPassword(userconfirm, currentPassword))
            {
                TempData["FailMessage"] = "Invalid current password.";
                return Page();
            }

            // Check if the new password and confirm password match
            if (newPassword != confirmNewPassword)
            {
                TempData["FailMessage"] = "New password and confirm password do not match!";
                return Page();
            }

            // If all conditions are met, change the password
            ChangePassword(currentPassword, newPassword);

            return RedirectToPage("/Admin_ChangePassSucc");
        }

        // Method to check if the current password matches the stored password
        private bool CheckCurrentPassword(string username, string currentPassword)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "SELECT password FROM LoginSample WHERE username = @username";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        string storedPassword = (string)command.ExecuteScalar();

                        return !string.IsNullOrEmpty(storedPassword) && ((currentPassword == storedPassword) || BCrypt.Net.BCrypt.Verify(currentPassword, storedPassword));
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["FailMessage"] = ex.Message;
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // Method to change the admin's password in the database
        private void ChangePassword(string currentPassword, string newPassword)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "SELECT password FROM LoginSample WHERE username = @userconfirm";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userconfirm", userconfirm);
                        string storedPassword = (string)command.ExecuteScalar();

                        if (!string.IsNullOrEmpty(storedPassword) && ((currentPassword == storedPassword) || BCrypt.Net.BCrypt.Verify(currentPassword, storedPassword)))
                        {
                            sql = "UPDATE LoginSample SET password = @newPassword WHERE username = @userconfirm";
                            command.Parameters.Clear();
                            command.CommandText = sql;
                            command.Parameters.AddWithValue("@newPassword", BCrypt.Net.BCrypt.HashPassword(newPassword));
                            command.Parameters.AddWithValue("@userconfirm", userconfirm);
                            command.ExecuteNonQuery();
                            TempData["SuccessMessage"] = "Password Changed!";
                        }
                        else
                        {
                            TempData["FailMessage"] = "Invalid current password or passwords do not match.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["FailMessage"] = ex.Message;
                Console.WriteLine(ex.Message);
            }
        }
    }
}
