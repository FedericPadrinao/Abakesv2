using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using MimeKit.Utils;
namespace abakes2.Pages
{
    public class ManageOrdersModel : PageModel
    {
        public CustomerInfo customerInfo = new CustomerInfo();
        public UserInfo userInfo = new UserInfo();  
        public List<OrderSimpleInfo> orderSimpleInfo = new List<OrderSimpleInfo>();
        public string status { get; set; }
        public String userconfirm = "";
        public String errorMessage = "";
        public String statusconfirm = "";
        public String successMessage = "";
        public string connectionString = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";

        public void GetOrders(string sortOrder)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "select * from OrderSimple where status ='false' "; //getting the data based from the odid variable
                    string search = Request.Query["search"];
                    if (!String.IsNullOrEmpty(search))
                    {
                        sql = "SELECT * FROM OrderSimple WHERE status ='false' AND username LIKE '%" + search + "%' ";
                    }

                    switch (sortOrder)
                    {
                        case "Sort Name":
                            sql += "ORDER BY username DESC";
                            break;
                        case "Sort Name2":
                            sql += "ORDER BY usernameame ASC";
                            break;
                        case "Sort Flavor":
                            sql += "ORDER BY flavors DESC";
                            break;
                        case "Sort Flavor2":
                            sql += "ORDER BY flavors ASC";
                            break;

                    }
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OrderSimpleInfo os = new OrderSimpleInfo();


                                os.osID = reader.GetFieldValue<int>(reader.GetOrdinal("OrderID"));
                                os.osUsername = reader.GetFieldValue<string>(reader.GetOrdinal("username"));
                                os.osOccasion = reader.GetFieldValue<string>(reader.GetOrdinal("occasion"));
                                os.osShapes = reader.GetFieldValue<string>(reader.GetOrdinal("shapes"));
                                os.osTier = reader.GetFieldValue<string>(reader.GetOrdinal("tier"));
                                os.osFlavors = reader.GetFieldValue<string>(reader.GetOrdinal("flavors"));
                                os.osSizes = reader.GetFieldValue<string>(reader.GetOrdinal("sizes"));
                                os.osInstruction = reader.GetFieldValue<string>(reader.GetOrdinal("instructions"));
                                os.osDelivery = reader.GetFieldValue<string>(reader.GetOrdinal("delivery"));
                                os.status = reader.GetFieldValue<string>(reader.GetOrdinal("status"));
                                os.osPreferredD = reader.GetFieldValue<string>(reader.GetOrdinal("PreferredDelivery"));
                                os.osColor = reader.GetFieldValue<string>(reader.GetOrdinal("color"));
                                os.osDedication = reader.GetFieldValue<string>(reader.GetOrdinal("dedication"));
                                orderSimpleInfo.Add(os);

                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading Products: " + e.ToString());

            }
        }
        public void OnGet(string sortOrder)
        {
            GetOrders(sortOrder);
        }
        
        public IActionResult OnGetReject(string sortOrder)
        {
            GetOrders(sortOrder);
            if (userconfirm == null)
            { //If not logged in, it will be redirected to login page
                return RedirectToPage("/Account");
            }
            else
            {

            }
            string id = Request.Query["id"]; //name from the front end "?id=
            String user = Request.Query["user"];
            string NotifTitle = "Order Rejected!";
            string NotifText = "Your order has been rejected! We apologize for this inconvenience. You may message our page for inquiries.";
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            try
            {
                

                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "select * from OrderSimple WHERE OrderID=@ID"; //getting the data based from the odid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {


                                customerInfo.username = reader.GetFieldValue<string>(reader.GetOrdinal("username"));


                            }
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM LoginCustomer WHERE username=@user";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user", customerInfo.username);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                
                                customerInfo.lname = reader.GetString(2);
                                customerInfo.fname = reader.GetString(3);
                                customerInfo.email = reader.GetString(5);
                                customerInfo.address = reader.GetString(6);
                                customerInfo.phone = reader.GetString(7);
                                customerInfo.city = reader.GetString(9);
                                customerInfo.barangay = reader.GetString(10);
                            }
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "delete from OrderSimple WHERE OrderID=@ID"; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);
                        command.ExecuteNonQuery();
                    }
                  
                    String sql2 = "UPDATE LoginCustomer SET ordermax='false' WHERE username='" + customerInfo.username + "'";
                    using (SqlCommand command2 = new SqlCommand(sql2, connection))
                    {
                        command2.Parameters.AddWithValue("@ID", id);
                        command2.ExecuteNonQuery();
                    }
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {


                    connection.Open();

                    string sql = "Insert into PrivateNotification (NotificationTitle,username,NotificationText,NotificationImage,status,DateCreated,isRead) values (@NotifTitle,@username,@NotifText,@NotifImage,'true',@DateCreated,'false')";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@NotifTitle", NotifTitle);
                        command.Parameters.AddWithValue("@NotifText", NotifText);
                        command.Parameters.AddWithValue("@username", customerInfo.username);
                        command.Parameters.AddWithValue("@NotifImage", "");
                        command.Parameters.AddWithValue("@DateCreated", currentDate);

                        command.ExecuteNonQuery();

                        // Send email with image attachment
                        SendNotificationEmail(customerInfo.fname, customerInfo.email, NotifTitle, NotifText);
                        // Set TempData for success message
                        TempData["SuccessMessage"] = "Notification successfully sent!";
                    }
                }

            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }

            return Redirect("/Admin_ManageOrders");
        }
        private void SendNotificationEmail(string customerName, string customerEmail, string notifTitle, string notifText)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress("A-bakes", "abakes881@gmail.com"));
            email.To.Add(new MailboxAddress(customerName, customerEmail));

            email.Subject = notifTitle;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = notifText;



            bodyBuilder.HtmlBody = $@"
                <p>{notifText}</p>
                <p>Please log in to website for a detailed review</p>";

            email.Body = bodyBuilder.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 465, true);
                smtp.Authenticate("abakes881@gmail.com", "gvok rqua fsbr ufuz");
                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }
        public IActionResult OnPostPauseOrder()
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "UPDATE LoginCustomer SET status='false'";


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                
            }
            TempData["Message"] = "Customers are now unable to take any orders";

            successMessage = "Customers are now unable to give any orders";
            return Page();
        }
        public IActionResult OnPostResumeOrder()
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "UPDATE LoginCustomer SET status='true'";


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
              
            }
            TempData["Message"] = "Customers are now able to take orders";

            successMessage = "Customers are now unable to give any orders";
            return Page();
            
        }
    }
    }
