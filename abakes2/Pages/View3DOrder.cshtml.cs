using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class View3DOrderModel : PageModel
    {
        public UserInfo userInfo = new UserInfo();
        public OrderSimpleInfo orderSimple = new OrderSimpleInfo();
        public CustomerInfo customerInfo = new CustomerInfo();
        public Order3DForm order3d = new Order3DForm();
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
