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
    public class Admin_Manage3DOrdersModel : PageModel
    {
        public CustomerInfo customerInfo = new CustomerInfo();
        public UserInfo userInfo = new UserInfo();
        public List<OrderSimpleInfo> orderSimpleInfo = new List<OrderSimpleInfo>();
        public List<Order3DForm> order3DInfo = new List<Order3DForm>();
        public string status { get; set; }
        public String userconfirm = "";
        public String errorMessage = "";
        public String statusconfirm = "";
        public String successMessage = "";
        public string connectionString = "Server=tcp:eu-az-sql-serv6e425a9865434acc8b7d6d8badb306b5.database.windows.net,1433;Initial Catalog=d58anpzl3kll8nf;Persist Security Info=False;User ID=ufy99xlgudx1fh3;Password=%g2q&#dV&ECBX6Oyf0%QkXHe5;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public void GetOrders()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "select * from Order3DForm WHERE status ='false'"; //getting the data based from the odid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OrderSimpleInfo os = new OrderSimpleInfo();
                                Order3DForm order3DList = new Order3DForm();

                                order3DList.ModelID = reader.GetFieldValue<int>(reader.GetOrdinal("OrderId"));
                                order3DList.username = reader.GetFieldValue<string>(reader.GetOrdinal("username"));
                                order3DList.instructions = reader.GetFieldValue<string>(reader.GetOrdinal("instructions"));
                                order3DList.order3DPreferredD = reader.GetFieldValue<string>(reader.GetOrdinal("PreferredDelivery"));
                                order3DList.ModelType = reader.GetFieldValue<string>(reader.GetOrdinal("ModelType"));
                                order3DList.picture = reader.GetFieldValue<string>(reader.GetOrdinal("dedicationpic"));
                                orderSimpleInfo.Add(os);
                                order3DInfo.Add(order3DList);

                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading 3D Orders: " + e.ToString());

            }
        }
        public void OnGet()
        {
            GetOrders();
            userconfirm = HttpContext.Session.GetString("useradmin");
            if (userconfirm != null)
            {

            }
            else
            {
                Response.Redirect("/index");
            }
        }

        public IActionResult OnGetReject()
        {
            GetOrders();
            if (userconfirm == null)
            { //If not logged in, it will be redirected to login page
                return RedirectToPage("/Account");
            }
            else
            {

            }
            String id = Request.Query["id"];
            string NotifTitle = "3D Order Rejected!";
            string NotifText = "Your order has been rejected! We apologize for this inconvenience. You may message our page for inquiries.";
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "select * from Order3DForm WHERE OrderID=@ID"; //getting the data based from the odid variable
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
                    string sql = "delete from Order3DForm WHERE OrderID=@ID AND status = 'false'"; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);
                        command.ExecuteNonQuery();
                    }
                    string sql3 = "delete from Asset3DForm WHERE OrderID=@ID"; //getting the data based from the pdid variable
                    using (SqlCommand command3 = new SqlCommand(sql3, connection))
                    {
                        command3.Parameters.AddWithValue("@ID", id);
                        command3.ExecuteNonQuery();
                    }
                   
                    String sql2 = "UPDATE LoginCustomer SET ordermax3D='false' WHERE username='" + customerInfo.username + "'";
                    using (SqlCommand command2 = new SqlCommand(sql2, connection))
                    {
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
                Console.WriteLine("User" + customerInfo.username);

            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }

            return Redirect("/Admin_Manage3DOrders");
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
                smtp.Authenticate("abakes881@gmail.com", "oclt owgw hcgf ttok");
                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }

        public void OnPostPauseOrder()
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
                return;
            }

            Response.Redirect("/Admin_ManageOrders");
        }
        public void OnPostResumeOrder()
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
                return;
            }

            Response.Redirect("/Admin_ManageOrders");
        }
    }
}