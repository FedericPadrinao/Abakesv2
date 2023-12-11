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
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class View3DOrderModel : PageModel
    {
        public UserInfo userInfo = new UserInfo();
        public OrderSimpleInfo orderSimple = new OrderSimpleInfo();
        public CustomerInfo customerInfo = new CustomerInfo();
        public Order3DForm order3d = new Order3DForm();
        public List<Asset3DForm> listAsset3D = new List<Asset3DForm>();
        public string userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public string connectionProvider = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";
        public void OnGet()
        {
            String id = Request.Query["Id"];

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Order3DForm WHERE OrderID=@id ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
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


                            }
                        }
                    }


                }

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Asset3DForm WHERE OrderID=@id ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
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
            OnGet();
            string price = Request.Form["price"];
            string ship = Request.Form["ship"];
            string id = Request.Form["id"];
            string ExpectedDelivery = Request.Form["expectedD"];
            string ExpectedTime = Request.Form["expectedT"];
            string NotifTitle = "Review & Quotation for your 3D Order is done!";
            string NotifText = "Your order comes with  the price of " + price + "! " + "\n Shipping Fee of" + ship + ". \n" + "Please pay either 50% DP or pay in full" + "\n Please expect your delivery to come at " + ExpectedDelivery + " " + ExpectedTime + ".";
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            int ids = int.Parse(id);

            double productPrice = double.Parse(price);
            double downpayment = productPrice * 0.5;
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "UPDATE Order3DForm SET OrderPrice='" + price + "', status='true', ShippingPrice='" + ship + "', Downpayment='" + downpayment + "', ExpectedDelivery='" + ExpectedDelivery + "', ExpectedTime='" + ExpectedTime + "', orderstatus='Review & Quotation' WHERE OrderID='" + ids + "'";


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
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
                        command.Parameters.AddWithValue("@username", customerInfo.username);
                        command.Parameters.AddWithValue("@NotifImage", "");
                        command.Parameters.AddWithValue("@DateCreated", currentDate);

                        command.ExecuteNonQuery();

                        // Send email with image attachment
                        Console.WriteLine("CUSOTOMER" + customerInfo.username);
                        SendNotificationEmail(customerInfo.fname, customerInfo.email, NotifTitle, NotifText);
                        // Set TempData for success message
                        TempData["SuccessMessage"] = "Notification successfully sent!";
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
            Response.Redirect("/Admin_Manage3DOrders");
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
                <p>{notifText}</p>";

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
