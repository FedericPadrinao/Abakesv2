using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class ViewSimpleOrderModel : PageModel
    {

        public UserInfo userInfo = new UserInfo();
        public OrderSimpleInfo orderSimple = new OrderSimpleInfo();
        public CustomerInfo customerInfo = new CustomerInfo();

        public string userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public string connectionProvider = "Server=tcp:eu-az-sql-serv4c4db8f1d3864bb6a62cb90162f811bf.database.windows.net,1433;Initial Catalog=dh60fwg8dvwho6h;Persist Security Info=False;User ID=uw1aiy40iqpmqas;Password=U6W5bT1pW3O37J#DEihhdV%p@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
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