using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace abakes2.Pages
{
    public class AccountModel : PageModel
    {
        public CustomerInfo customerInfo = new CustomerInfo();
        public string successMessage = "";
        public string errorMessage = "";
        private readonly ILogger<IndexModel> _logger;
        public string username { get; set; }
        public string useradmin { get; set; }
        public string password { get; set; }
        public string pass = "";
        public string userimage { get; set; }
        public string email { get; set; }
        public string userstatus { get; set; }
        public string userconfirm = "";
        public string imgconfirm = "";
        public string statusconfirm = "";
        public string connectionProvider = "Data Source=ROVIC\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";

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

        public IActionResult OnPostLogin()
        {
            username = Request.Form["username"];
            password = Request.Form["password"];
            int x = 0;

            try
            {
                // CUSTOMER
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM LoginCustomer WHERE username='" + username + "'";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@x", username);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                username = reader.GetString(1);
                                pass = reader.GetString(4);
                                userimage = reader.GetString(8);
                                userstatus = reader.GetString(11);
                            }
                        }
                    }
                }
                // ADMIN
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM LoginSample WHERE username='" + username + "'";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@x", username);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                x++;
                                useradmin = reader.GetString(1);
                                pass = reader.GetString(2);
                                userimage = reader.GetString(4);
                                userstatus = reader.GetString(5);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.ToString());
            }

            try
            {
                if (x > 0)
                {
                    if (password.Equals(pass))
                    {
                        HttpContext.Session.SetString("useradmin", useradmin);
                        HttpContext.Session.SetString("userimage", userimage);
                        HttpContext.Session.SetString("userstatus", userstatus);

                        return RedirectToPage("/AdminDashboard");
                    }
                    else if (BCrypt.Net.BCrypt.Verify(password, pass))
                    {
                        HttpContext.Session.SetString("useradmin", useradmin);
                        HttpContext.Session.SetString("userimage", userimage);
                        HttpContext.Session.SetString("userstatus", userstatus);

                        return RedirectToPage("/AdminDashboard");

                    }

                    else
                    {
                        TempData["FailMessage"] = "Invalid Credentials!";
                        errorMessage = "Invalid Credentials!";
                        return Page();
                    }
                }
                else
                {
                    bool isVerified = IsUserVerified(username);

                    if (!BCrypt.Net.BCrypt.Verify(password, pass))
                    {
                        TempData["FailMessage"] = "Invalid Credentials!";
                        errorMessage = "Invalid Credentials!";
                        return Page();
                    }
                    else if (isVerified)
                    {
                        HttpContext.Session.SetString("username", username);
                        HttpContext.Session.SetString("userimage", userimage);
                        HttpContext.Session.SetString("userstatus", userstatus);
                        // Check if it's the first-time login
                        if (IsFirstTimeLogin(username))
                        {

                            // Redirect to the specific page for first-time login
                            return RedirectToPage("/Customer_AccountInformation", new { user = username });
                        }
                        string returnUrl = HttpContext.Session.GetString("ReturnUrl");
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            HttpContext.Session.Remove("ReturnUrl"); // Remove the stored URL after using it
                            return Redirect(returnUrl);
                        }

                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        // User is not verified, redirect to verification page
                        TempData["FailMessage"] = "User is not verified. Please check your email for the verification code";
                        return RedirectToPage("/Account_Verify", new { email = customerInfo.email });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.ToString());

                // Redirect the user to the account page with an error message
                TempData["FailMessage"] = "Invalid Credentials";
                return RedirectToPage("/Account");
            }
        }
        private bool IsFirstTimeLogin(string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();
                string sql = "SELECT first_time FROM LoginCustomer WHERE username = @username";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToBoolean(result);
                    }
                }
            }
            return false;
        }
        bool IsUserVerified(string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();
                string sql = "SELECT is_verified FROM LoginCustomer WHERE username = @username";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToBoolean(result);
                    }
                }
            }
            return false;
        }


        public IActionResult OnPostRegister()
        {
            customerInfo.username = Request.Form["username"];
            customerInfo.password = Request.Form["password"];
            customerInfo.fname = Request.Form["fname"];
            customerInfo.lname = Request.Form["lname"];
            //customerInfo.phone = Request.Form["phone"];
            //customerInfo.address = Request.Form["address"];
            customerInfo.email = Request.Form["email"];

            try
            {
                if (IsEmailOrUsernameExists(customerInfo.email, customerInfo.username))
                {
                    TempData["FailMessage"] = "Email or username already exists!";
                    return Page();
                }

                string verificationCode = GenerateVerificationCode();
                DateTime verificationCodeExpiration = DateTime.Now.AddMinutes(3);

                var email = new MimeMessage();

                email.From.Add(new MailboxAddress("A-bakes", "abakes881@gmail.com"));
                email.To.Add(new MailboxAddress(customerInfo.fname, customerInfo.email));

                email.Subject = "A-Bakes Registration";
                email.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
                {
                    Text = $"Hello {customerInfo.fname} {customerInfo.lname}! Thank you for registering at A-bakes! Your verification code is: {verificationCode}. It will expire at {verificationCodeExpiration.ToString("yyyy-MM-dd HH:mm:ss")}"
                };

                using (var smtp = new SmtpClient())
                {
                    smtp.Connect("smtp.gmail.com", 465, true);
                    smtp.Authenticate("abakes881@gmail.com", "oclt owgw hcgf ttok");
                    smtp.Send(email);
                    smtp.Disconnect(true);
                }

                // Update database with verification code and set is_verified to false
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "INSERT INTO LoginCustomer (username, lname, fname, password, email, address, phone, picture, city, barangay, status, accstatus, ordermax, ordermax3D, verification_code, is_verified, passcode, first_time, verif_exp) " +
                        "VALUES (@username, @lname, @fname, @password, @email, '', '', '/img/Account/Default.jpg', '', '', 'true', 'true', 'false', 'false', @verificationCode, 'false', '', 'true', DATEADD(MINUTE, 3, GETDATE()))";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", customerInfo.email);
                        command.Parameters.AddWithValue("@username", customerInfo.username);
                        command.Parameters.AddWithValue("@lname", customerInfo.lname);
                        command.Parameters.AddWithValue("@fname", customerInfo.fname);
                        command.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(customerInfo.password));
                        command.Parameters.AddWithValue("@verificationCode", verificationCode);

                        command.ExecuteNonQuery();
                    }
                }

                // Create a default image file for the user
                string defaultImageFileName = "Default.jpg";
                string userImagePath = $"img/Account/{defaultImageFileName}"; // Adjust the path as needed

                // Copy the default image file to the user's image path
                System.IO.File.Copy("version 13\\version 13\\abakes2\\abakes2\\wwwroot\\img\\Account\\Default.jpg", userImagePath, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }


            return RedirectToPage("/Account_Reg_Succ");
        }

        // Method to check if email or username already exists
        private bool IsEmailOrUsernameExists(string email, string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM LoginCustomer WHERE email = @email OR username = @username";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@username", username);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    // If count is greater than 0, the email or username already exists
                    return count > 0;
                }
            }
        }

        // Method to generate a random verification code
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

