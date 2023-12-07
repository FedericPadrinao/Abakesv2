using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
namespace abakes2.Pages
{
    public class ChangeProfilePicModel : PageModel
    {
        public CustomerInfo customerInfo = new CustomerInfo();
        public string userconfirm = "";
        public string imgconfirm = "";
        public string username { get; set; }
        public String statusconfirm = "";
        public int notifCount = 0;
        public int pnotifCount = 0;
        public int pubnotifCount = 0;
        public int cartCount = 0;
        public int totalnotifCount = 0;
        public String errorMessage = "";
        public String successMessage = "";
        public string connectionProvider = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";




        public void OnGet()
        {
            userconfirm = HttpContext.Session.GetString("username");
            imgconfirm = HttpContext.Session.GetString("userimage");
            statusconfirm = HttpContext.Session.GetString("userstatus");
            notifCount = HttpContext.Session.GetInt32("NotificationCount") ?? 0;


            String user = Request.Query["user"];
            String id = Request.Query["id"];
            if (userconfirm != null)
            {


            }
            else
            {
                Response.Redirect("/Index");
            }

            try
            {
                //CUSTOMER
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM LoginCustomer WHERE username= '" + userconfirm + "'";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                customerInfo.id = reader.GetInt32(0);
                                customerInfo.username = reader.GetString(1);
                                customerInfo.img = reader.GetString(8);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.ToString());
            }
            //NAV COUNT
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(NotificationID) from Notification where status = 'true'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                notifCount = reader.GetInt32(0);

                            }


                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(NotificationID) from PrivateNotification where status = 'true' AND isRead = 'false'  AND username = '" + userconfirm + "'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                pnotifCount = reader.GetInt32(0);
                            }
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(NotificationID) from ReadPublicNotif where username = '" + userconfirm + "'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                pubnotifCount = reader.GetInt32(0);
                            }
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(OrderID) from OrderSimple where status = 'true' AND username = '" + userconfirm + "'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cartCount = reader.GetInt32(0);
                            }
                        }
                    }
                }
                totalnotifCount = notifCount + pnotifCount - pubnotifCount;

            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }



        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            userconfirm = HttpContext.Session.GetString("username");
            string successMessageProfile = "";
            string errorMessageProfile = "";


            try
            {
                // File format validation
                if (file != null && file.Length > 0)
                {
                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                    {
                        // Valid file format, proceed with upload
                        using (SqlConnection connection = new SqlConnection(connectionProvider))
                        {
                            string fileName = Path.GetFileName(file.FileName);
                            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "Account", fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            connection.Open();
                            string sql = "update LoginCustomer set picture = @image where username='" + userconfirm + "'";
                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                command.Parameters.AddWithValue("@image", "/img/Account/" + fileName + "?rnd=" + Guid.NewGuid().ToString());
                                command.ExecuteNonQuery();
                            }

                            successMessageProfile = "Profile picture changed successfully! Please log in again to take effect";


                        }
                    }
                    else
                    {
                        // Invalid file format, set error message
                        errorMessageProfile = "Invalid file format. Please upload a JPEG, JPG, or PNG file.";
                        TempData["errorMessageProfile"] = errorMessageProfile; // Store the success message in TempData

                        return Redirect("/Customer_AccountSettings");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                errorMessageProfile = "Profile picture upload failed. Please try again."; // Set error message

                TempData["errorMessageProfile"] = errorMessageProfile; // Store the success message in TempData

                return Redirect("/Customer_AccountSettings");
            }


            TempData["SuccessMessageProfile"] = successMessageProfile; // Store the success message in TempData

            return Redirect("/Customer_AccountSettings?user=" + userconfirm);
        }



        //CHANGE PASSWORD
        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            userconfirm = HttpContext.Session.GetString("username");
            String currentPassword = Request.Form["currentPassword"];
            String id = Request.Query["id"];
            String user = Request.Query["user"];
            string newPassword = Request.Form["newPassword"];
            string confirmPassword = Request.Form["confirmPassword"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT password FROM LoginCustomer WHERE username = @user";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user", userconfirm);
                        string storedPassword = (string)command.ExecuteScalar();


                        if (!string.IsNullOrEmpty(storedPassword) && BCrypt.Net.BCrypt.Verify(currentPassword, storedPassword) && newPassword == confirmPassword)
                        {
                            sql = "UPDATE LoginCustomer SET password = @newPassword WHERE username = @user";
                            command.Parameters.Clear();
                            command.CommandText = sql;
                            command.Parameters.AddWithValue("@newPassword", BCrypt.Net.BCrypt.HashPassword(newPassword));
                            command.Parameters.AddWithValue("@user", userconfirm);
                            command.ExecuteNonQuery();
                            TempData["SuccessMessage"] = "Password Changed!";

                            // Sign out the current user
                            await HttpContext.SignOutAsync();
                        }
                        else
                        {
                            TempData["FailMessage"] = "Please try again.";
                        }
                    }
                }
                return RedirectToPage("/Customer_AccountSettings", new { user = userconfirm });
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                Console.WriteLine(ex.Message);
                return RedirectToPage("/Customer_AccountSettings", new { user = userconfirm });
            }
        }
    }
}