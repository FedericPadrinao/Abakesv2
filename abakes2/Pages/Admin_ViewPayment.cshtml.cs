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
    public class Admin_ViewPaymentModel : PageModel
    {
        public string successMessage = "";
        public string errorMessage = "";
        public string connectionProvider = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";
        public NotificationInfo notifInfo = new NotificationInfo();
        public UserInfo userInfo = new UserInfo();
        public OrderSimpleInfo orderSimple = new OrderSimpleInfo();
        public CustomerInfo customerInfo = new CustomerInfo();
        public InvoiceInfo invoiceInfo = new InvoiceInfo();

        public string userconfirm = "";


        public void OnGet()
        {
            String user = Request.Query["user"];
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
                    String sql = "SELECT * FROM Invoice WHERE username=@user";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user", user);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                invoiceInfo.invoiceID = reader.GetInt32(0);                         
                                invoiceInfo.invoiceOccasion = reader.GetString(2);
                                invoiceInfo.invoiceShapes = reader.GetString(3);
                                invoiceInfo.invoiceTier = reader.GetString(4);
                                invoiceInfo.invoiceFlavors = reader.GetString(5);
                                invoiceInfo.invoiceSizes = reader.GetString(6);
                                invoiceInfo.invoiceInstruction = reader.GetString(7);
                                invoiceInfo.invoiceDelivery = reader.GetString(8);
                                invoiceInfo.status = reader.GetString(9);
                                invoiceInfo.invoicePrice = reader.GetInt32(10);
                                invoiceInfo.invoiceQuantity = reader.GetInt32(11);
                                invoiceInfo.invoiceShip = reader.GetInt32(12);
                                invoiceInfo.invoiceDP = reader.GetInt32(13);
                                invoiceInfo.invoicePreferredD = reader.GetString(14);
                                invoiceInfo.invoiceExpectedD = reader.GetString(15); 
                                invoiceInfo.invoiceExpectedT = reader.GetString(16); 
                                invoiceInfo.invoiceColor = reader.GetString(17);
                                invoiceInfo.invoiceDedication = reader.GetString(18);
                                invoiceInfo.invoiceDateCreated = reader.GetString(19);
                                invoiceInfo.orderStatus = reader.GetString(20);
                                invoiceInfo.receipt = reader.GetString(21);

                                invoiceInfo.paymentMethod = reader.GetString(22);
                                invoiceInfo.picture = reader.GetString(25);
                            }
                        }
                    }


                }

                //CUSTOMER
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM LoginCustomer WHERE username= '" + user + "'";

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
            string NotifTitle = "Order Status Update!";
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
                    String sql = "UPDATE Invoice SET orderstatus='" + orderstatus + "' WHERE username='" + user + "'";

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
                        String sql = "UPDATE LoginCustomer SET ordermax='false' WHERE username='" + user + "'";

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
            Response.Redirect("/Admin_ViewPayment?user=" +user);
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
}}

    