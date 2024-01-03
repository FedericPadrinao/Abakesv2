using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using MimeKit;
using MailKit.Net.Smtp;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Crypto;

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
        public bool ShowOTPForm { get; set; } = false;
        public string userconfirm = "";
        public string imgconfirm = "";
        public string statusconfirm = "";
        public DateTime? UnlockTime { get; set; }  // Added UnlockTime property
        public string connectionProvider = "Data Source=ROVIC\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";

        public void OnGet()
        {
            userconfirm = HttpContext.Session.GetString("username");

            if (userconfirm != null)
            {
                Response.Redirect("/Index");
            }
        }

        public IActionResult OnPostLogin()
        {
            username = Request.Form["username"];
            password = Request.Form["password"];
            HttpContext.Session.SetString("tempUsername", username);
            int x = 0;

            try
            {
                

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
                            }
                        }
                    }
                }

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
                        IncrementInvalidAttemptCount(HttpContext.Connection.RemoteIpAddress.ToString());
                        TempData["FailMessage"] = "Invalid Credentials!";
                        errorMessage = "Invalid Credentials!";
                        return Page();
                    }
                }
                else
                {

                    if (!BCrypt.Net.BCrypt.Verify(password, pass))
                    {
                        IncrementInvalidAttemptCount(HttpContext.Connection.RemoteIpAddress.ToString());
                        TempData["FailMessage"] = "Invalid Credentials!";
                        errorMessage = "Invalid Credentials!";
                        return Page();
                    }

                    else
                    {
                        // Set ShowOTPForm to true when credentials are correct
                        string generatedOTP = GenerateOTP();
                        SendOTPEmail(username, generatedOTP);

                        TempData["AlertMessage"] = "Login Successful. Please enter the OTP sent to you.";
                        ShowOTPForm = true;

                        return Page();

                    }
                }
            }
            catch (Exception e)
            {
                IncrementInvalidAttemptCount(HttpContext.Connection.RemoteIpAddress.ToString());
                Console.WriteLine("Error : " + e.ToString());
                TempData["FailMessage"] = "Invalid Credentials";
                return RedirectToPage("/Account");
            }
        }

        // Add a new method to get the unlock time
        private DateTime? GetUnlockTime(string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();
                string sql = "SELECT TOP 1 attemptime FROM InvalidAttempt WHERE IPAddress = @username ORDER BY attemptime DESC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToDateTime(result).AddMinutes(5); // Assuming the timeout period is 5 minutes
                    }
                }
            }

            return null;
        }

        private void IncrementInvalidAttemptCount(string ipAddress)
        {
            DateTime? lastattemptime = GetLastInvalidattemptime(ipAddress);

            if (lastattemptime.HasValue && DateTime.Now.Subtract(lastattemptime.Value).TotalMinutes > 3)
            {
                // Time gap is greater than 3 minutes, reset the attempt count
                ResetInvalidAttemptCount(ipAddress);
            }

            int attemptCount;
            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();
                string sql = "SELECT ISNULL(attempt, 0) FROM InvalidAttempt WHERE IPAddress = @ipAddress";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ipAddress", ipAddress);
                    attemptCount = Convert.ToInt32(command.ExecuteScalar());
                }
            }

            if (attemptCount >= 5)
            {

                // Set ShowOTPForm to true when credentials are correct
                UnlockTime = GetUnlockTime(username);  // Set the UnlockTime property
                if (IsIPBlocked(ipAddress, out DateTime? unlockTime))
                {

                    // Display timeout message or take appropriate action
                    TempData["FailMessage"] = $"Too many invalid attempts. Please try again after {unlockTime?.ToString("yyyy-MM-dd HH:mm:ss")}";
                    TempData["EstimatedUnlockTime"] = unlockTime; // Set TempData["EstimatedUnlockTime"]
                    return;
                }
            }

            // Increment the attempt count
            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();
                string sql = "IF NOT EXISTS (SELECT 1 FROM InvalidAttempt WHERE IPAddress = @ipAddress) " +
                             "INSERT INTO InvalidAttempt (IPAddress, attempt, attemptime) VALUES (@ipAddress, 1, GETDATE()) " +
                             "ELSE " +
                             "UPDATE InvalidAttempt SET attempt = attempt + 1, attemptime = GETDATE() WHERE IPAddress = @ipAddress";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ipAddress", ipAddress);
                    command.ExecuteNonQuery();
                }
            }
        }


        private void ResetInvalidAttemptCount(string ipAddress)
        {
            // Reset the attempt count, e.g., delete the row from the InvalidAttempt table
            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();
                string sql = "DELETE FROM InvalidAttempt WHERE IPAddress = @ipAddress";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ipAddress", ipAddress);
                    command.ExecuteNonQuery();
                }
            }
        }
        private DateTime? GetLastInvalidattemptime(string ipAddress)
        {
            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();
                string sql = "SELECT TOP 1 attemptime FROM InvalidAttempt WHERE IPAddress = @ipAddress ORDER BY attemptime DESC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ipAddress", ipAddress);
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToDateTime(result);
                    }
                }
            }

            return null;
        }

        public bool IsIPBlocked(string ipAddress, out DateTime? unlockTime)
        {
            unlockTime = null;

            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();
                string sql = "SELECT attempt FROM InvalidAttempt WHERE IPAddress = @ipAddress";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ipAddress", ipAddress);
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        int attemptCount = Convert.ToInt32(result);

                        // Check if the attempt count exceeds the limit (e.g., 5)
                        if (attemptCount >= 5)
                        {
                            // Check if the last attempt time is within the timeout period (e.g., 5 minutes)
                            sql = "SELECT TOP 1 attemptime FROM InvalidAttempt WHERE IPAddress = @ipAddress ORDER BY attemptime DESC";

                            using (SqlCommand timeoutCommand = new SqlCommand(sql, connection))
                            {
                                timeoutCommand.Parameters.AddWithValue("@ipAddress", ipAddress);
                                object lastattemptime = timeoutCommand.ExecuteScalar();

                                if (lastattemptime != null && lastattemptime != DBNull.Value)
                                {
                                    DateTime lastAttemptDateTime = Convert.ToDateTime(lastattemptime);
                                    unlockTime = lastAttemptDateTime.AddMinutes(5);

                                    if (DateTime.Now < unlockTime)
                                    {
                                        // IP is still in timeout period
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
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

        private bool IsUserVerified(string username)
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
                    String sql = "INSERT INTO LoginCustomer (username, lname, fname, password, email, address, phone, picture, city, barangay, status, accstatus, ordermax, ordermax3D, verification_code, is_verified, passcode, first_time, verif_exp,OTP) " +
                        "VALUES (@username, @lname, @fname, @password, @email, '', '', '/img/Account/Default.jpg', '', '', 'true', 'true', 'false', 'false', @verificationCode, 'false', '', 'true', DATEADD(MINUTE, 3, GETDATE()),'')";

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
        private string GenerateOTP()
        {
            const string chars = "0123456789";
            var random = new Random();
            var otp = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return otp;
        }
        public IActionResult OnPostVerifyOTP()
        {
            string username = Request.Form["username"];

            string enteredOTP = Request.Form["otp"];

            if (IsOTPValid(enteredOTP, username))
            {
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
                                pass = reader.GetString(4);
                                userimage = reader.GetString(8);
                                userstatus = reader.GetString(11);
                            }
                        }
                    }
                }
                bool isVerified = IsUserVerified(username);
                if (isVerified)
                {
                    HttpContext.Session.SetString("username", username);
                    HttpContext.Session.SetString("userimage", userimage);
                    HttpContext.Session.SetString("userstatus", userstatus);

                    if (IsFirstTimeLogin(username))
                    {
                        return RedirectToPage("/Customer_AccountInformation", new { user = username });
                    }

                    string returnUrl = HttpContext.Session.GetString("ReturnUrl");
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        // OTP is valid, perform actions
                        TempData["AlertMessage"] = "OTP Verified Successfully!";
                        HttpContext.Session.Remove("ReturnUrl");
                        return Redirect(returnUrl);
                    }

                }
                // OTP is valid, perform actions

                TempData["FailMessage"] = "User is not verified. Please check your email for the verification code";
                return RedirectToPage("/Account_Verify", new { email = customerInfo.email });
            }
            else
            {
                // OTP is invalid, display an error message and keep the OTP form visible
                TempData["FailMessage"] = "Invalid OTP. Please try again.";
                ShowOTPForm = true; // Keep the OTP form visible
                return Page();
            }
        }

        // Add a method to check if the entered OTP is valid
        private bool IsOTPValid(string enteredOTP, string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();
                string sql = "SELECT OTP FROM LoginCustomer WHERE username = @username";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@username", username);

                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        string storedOTP = result.ToString();

                        // Compare the entered OTP with the stored OTP
                        return enteredOTP == storedOTP;
                    }
                }
            }

            return false;
        }
        private void SendOTPEmail(string username, string otp)
        {
            string userEmail = GetUserEmail(username);
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress("A-bakes", "abakes881@gmail.com"));
            email.To.Add(new MailboxAddress(username, userEmail));

            email.Subject = "A-Bakes OTP";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                Text = $"Hello {username}! Your OTP for A-Bakes login is: {otp}."
            };

            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 465, true);
                smtp.Authenticate("abakes881@gmail.com", "oclt owgw hcgf ttok");
                smtp.Send(email);
                smtp.Disconnect(true);
            }

            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();
                String sql = "UPDATE LoginCustomer SET OTP = @otp WHERE username = @username;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@username", username);  // Use the correct parameter name
                    command.Parameters.AddWithValue("@otp", otp);            // Use the correct parameter name
                    command.ExecuteNonQuery();
                }
            }
        }

        // Method to retrieve the user's email from the LoginCustomer table
        private string GetUserEmail(string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();
                string sql = "SELECT email FROM LoginCustomer WHERE username = @username";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return result.ToString();
                    }
                }
            }
            return null;
        }

    }
}


