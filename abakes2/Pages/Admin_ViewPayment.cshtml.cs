using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Admin_ViewPaymentModel : PageModel
    {
        public string successMessage = "";
        public string errorMessage = "";
        public string connectionProvider = "Server=tcp:eu-az-sql-serv4c4db8f1d3864bb6a62cb90162f811bf.database.windows.net,1433;Initial Catalog=dh60fwg8dvwho6h;Persist Security Info=False;User ID=uw1aiy40iqpmqas;Password=U6W5bT1pW3O37J#DEihhdV%p@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public NotificationInfo notifInfo = new NotificationInfo();

        public UserInfo userInfo = new UserInfo();
        public OrderSimpleInfo orderSimple = new OrderSimpleInfo();
        public CustomerInfo customerInfo = new CustomerInfo();

        public string userconfirm = "";


        public void OnGet()
        {
            String id = Request.Query["Id"];

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM OrderSimple WHERE OrderID=@id ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                orderSimple.osID = reader.GetInt32(0);
                                orderSimple.osUsername = reader.GetString(1);
                                orderSimple.osOccasion = reader.GetString(2);
                                orderSimple.osShapes = reader.GetString(3);
                                orderSimple.osTier = reader.GetString(4);
                                orderSimple.osFlavors = reader.GetString(5);
                                orderSimple.osSizes = reader.GetString(6);
                                orderSimple.osInstruction = reader.GetString(7);
                                orderSimple.osDelivery = reader.GetString(8);
                                orderSimple.status = reader.GetString(9);
                                orderSimple.osPrice = reader.GetInt32(10);
                                orderSimple.osQuantity = reader.GetInt32(11);
                                orderSimple.osShip = reader.GetInt32(12);
                                orderSimple.osPreferredD = reader.GetString(14);

                            }
                        }
                    }


                }

                //CUSTOMER
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM LoginCustomer WHERE username= '" + orderSimple.osUsername + "'";

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
    
        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            string NotifTitle = Request.Form["title"];
            string NotifText = Request.Form["textmessage"];


            try
            {
                //image upload
                if (file != null && file.Length > 0)
                {

                    using (SqlConnection connection = new SqlConnection(connectionProvider))
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "Notification", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // string sql = "insert into LoginCustomer (picture) values (@image)";
                        connection.Open();

                        string sql = "Insert into Notification (NotificationTitle,NotificationText,NotificationImage,status) values (@NotifTitle,@NotifText,@NotifImage,'true')";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@NotifTitle", NotifTitle);
                            command.Parameters.AddWithValue("@NotifText", NotifText);

                            command.Parameters.AddWithValue("@NotifImage", "/img/Notification/" + fileName);

                            command.ExecuteNonQuery();
                        }







                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Page();
            }

            return Redirect("/Admin_Notif");
        }
        public void OnPost()
        {
            string price = Request.Form["price"];
            string ship = Request.Form["ship"];
            string id = Request.Form["id"];
            string ExpectedDelivery = Request.Form["expectedD"];
            string ExpectedTime = Request.Form["expectedT"];
            int ids = int.Parse(id);

            double productPrice = double.Parse(price);
            double downpayment = productPrice * 0.5;
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "UPDATE OrderSimple SET OrderPrice='" + price + "', status='true', ShippingPrice='" + ship + "', Downpayment='" + downpayment + "', ExpectedDelivery='" + ExpectedDelivery + "', ExpectedTime='" + ExpectedTime + "' WHERE OrderID='" + ids + "'";


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
            Response.Redirect("/Admin_ManageSimpleOrders2");
        }
    }
}

    