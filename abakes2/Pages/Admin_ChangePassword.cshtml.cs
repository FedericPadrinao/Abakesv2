// ... (existing using statements)

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Admin_ChangePasswordModel : PageModel
    {
        public string connectionProvider = "Data Source=eu-az-sql-serv5434154f0e9a4d00a109437d48355b69.database.windows.net;Initial Catalog=d5rw6jsfzbuks4y;Persist Security Info=True;User ID=uqqncmi3rkbksbc;Password=***********";

        public string username { get; set; }
        public string userconfirm = "";


        public IActionResult OnPost()
        {
            userconfirm = HttpContext.Session.GetString("username");
            string currentPassword = Request.Form["currentPassword"];
            string newPassword = Request.Form["newPassword"];
            string confirmNewPassword = Request.Form["confirmNewPassword"];

            // Assume admin credentials are already validated during login.
            // You may have a session or some other mechanism to ensure this.

            // Check if the new password and confirm password match
            if (newPassword != confirmNewPassword)
            {
                TempData["FailMessage"] = "New password and confirm password do not match!";
                return Page();
            }

            // Change the admin's password
            ChangePassword(currentPassword, newPassword);

            return RedirectToPage("/Admin_ChangePassSucc");
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

                        if (!string.IsNullOrEmpty(storedPassword) && BCrypt.Net.BCrypt.Verify(currentPassword, storedPassword))
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
