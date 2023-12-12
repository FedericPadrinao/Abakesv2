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
    public class Admin_ViewPayment_3DModel : PageModel
    {
        public UserInfo userInfo = new UserInfo();
        public OrderSimpleInfo orderSimple = new OrderSimpleInfo();
        public CustomerInfo customerInfo = new CustomerInfo();
        public Order3DForm order3d = new Order3DForm();
        public List<Asset3DForm> listAsset3D = new List<Asset3DForm>();
        public string userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public string connectionProvider = "Server=tcp:eu-az-sql-serv6e425a9865434acc8b7d6d8badb306b5.database.windows.net,1433;Initial Catalog=d58anpzl3kll8nf;Persist Security Info=False;User ID=ufy99xlgudx1fh3;Password=%g2q&#dV&ECBX6Oyf0%QkXHe5;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public void OnGet()
        {
            String id = Request.Query["Id"];
            string user = Request.Query["User"];
            userconfirm = HttpContext.Session.GetString("useradmin");
            if (userconfirm != null)
            {

            }
            else
            {
                Response.Redirect("/index");
            }
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Order3DForm WHERE username=@user ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                       
                        command.Parameters.AddWithValue("@user", user);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                order3d.ModelID = reader.GetInt32(0);
                                order3d.username = reader.GetString(1);
                                order3d.ModelName1 = reader.GetString(2);
                                order3d.Scale1 = reader.GetInt32(3);
                                order3d.Texture1 = reader.GetString(4);
                                order3d.Texture2 = reader.GetString(5);
                                order3d.Texture3 = reader.GetString(6);
                                order3d.Color = reader.GetString(7);
                                order3d.Color2 = reader.GetString(8);
                                order3d.Color3 = reader.GetString(9);
                                order3d.instructions = reader.GetString(10);
                                order3d.status = reader.GetString(11);
                                order3d.order3DPrice = reader.GetInt32(12);
                                order3d.order3DQuantity = reader.GetInt32(13);
                                order3d.order3DShip = reader.GetInt32(14);
                                order3d.order3DDP = reader.GetInt32(15);
                                order3d.order3DPreferredD = reader.GetString(16);
                                order3d.order3DExpectedD = reader.GetString(17);
                                order3d.order3DExpectedT = reader.GetString(18);
                                order3d.order3Dstatus = reader.GetString(21);
                               
                                order3d.receipt = reader.GetString(22);
                                order3d.paymentMethod = reader.GetString(23);
                                order3d.order3DDelivery = reader.GetString(24);
                                order3d.picture = reader.GetString(27);

                            }
                        }
                    }


                }

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Asset3DForm WHERE username=@user and OrderID=@orderID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user", order3d.username);
                        command.Parameters.AddWithValue("@orderID", order3d.ModelID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Asset3DForm asset3D = new Asset3DForm();
                                asset3D.AssetID = reader.GetInt32(0);
                                asset3D.OrderID = reader.GetInt32(1);
                                asset3D.username = reader.GetString(2);
                                asset3D.AssetName = reader.GetString(3);
                                asset3D.AssetPath = reader.GetString(4);
                                asset3D.AssetScale = reader.GetInt32(5);
                                asset3D.PositionX = reader.GetString(6);
                                asset3D.PositionY = reader.GetString(7);
                                asset3D.PositionZ = reader.GetString(8);

                                listAsset3D.Add(asset3D);
                                Console.WriteLine("Assets" + asset3D.AssetPath);
                                Console.WriteLine("Assets" + asset3D.AssetName);
                            }
                        }
                    }


                }

                //CUSTOMER
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM LoginCustomer WHERE username= '" + order3d.username + "'";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                customerInfo.username = reader.GetString(1);
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


            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
        public void OnPost()
        {
            string orderstatus = Request.Form["orderstatus"];
            String user = Request.Query["user"];
            string NotifTitle = "3D Order Status Update!";
            string NotifText = "Your order has been updated to " + orderstatus + "!";
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM LoginCustomer WHERE username=@user";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user", user);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                customerInfo.username = reader.GetString(1);
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
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "UPDATE Order3DForm SET orderstatus='" + orderstatus + "' WHERE username='" + user + "'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                }
                if (orderstatus == "Complete Order")
                {
                    using (SqlConnection connection = new SqlConnection(connectionProvider))
                    {
                        connection.Open();
                        String sql = "UPDATE LoginCustomer SET ordermax3D='false' WHERE username='" + user + "'";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {


                    connection.Open();

                    string sql = "Insert into PrivateNotification (NotificationTitle,username,NotificationText,NotificationImage,status,DateCreated,isRead) values (@NotifTitle,@username,@NotifText,@NotifImage,'true',@DateCreated,'false')";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@NotifTitle", NotifTitle);
                        command.Parameters.AddWithValue("@NotifText", NotifText);
                        command.Parameters.AddWithValue("@username", user);
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
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            Response.Redirect("/Admin_ViewPayment_3D?user=" + user);
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
    }
}
