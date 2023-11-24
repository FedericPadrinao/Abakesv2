using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Customer_OrderStatus3DModel : PageModel
    {
        public String userconfirm = "";
        public String imgconfirm = "";
        public String statusconfirm = "";

        public string connectionProvider = "Data Source=ROVIC\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";


        public UserInfo userInfo = new UserInfo();
        public OrderSimpleInfo orderSimple = new OrderSimpleInfo();
        public CustomerInfo customerInfo = new CustomerInfo();
        public Order3DForm order3d = new Order3DForm();
        public List<Asset3DForm> listAsset3D = new List<Asset3DForm>();
        public String errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {
            userconfirm = HttpContext.Session.GetString("username");
            imgconfirm = HttpContext.Session.GetString("userimage");
            statusconfirm = HttpContext.Session.GetString("userstatus");

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