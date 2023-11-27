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

        public string connectionProvider = "Data Source=eu-az-sql-serv5434154f0e9a4d00a109437d48355b69.database.windows.net;Initial Catalog=d5rw6jsfzbuks4y;Persist Security Info=True;User ID=uqqncmi3rkbksbc;Password=***********";


        public UserInfo userInfo = new UserInfo();
        public OrderSimpleInfo orderInfo = new OrderSimpleInfo();
        public CustomerInfo customerInfo = new CustomerInfo();
        public InvoiceInfo invoiceInfo = new InvoiceInfo();
        public Order3DForm order3D = new Order3DForm();
        public List<Asset3DForm> listAsset3D = new List<Asset3DForm>();
        public String errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {
            userconfirm = HttpContext.Session.GetString("username");
            imgconfirm = HttpContext.Session.GetString("userimage");
            statusconfirm = HttpContext.Session.GetString("userstatus");

            String user = Request.Query["user"];

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Order3DForm WHERE username=@user ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("user", userconfirm);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                order3D.ModelID = reader.GetInt32(0);
                                order3D.username = reader.GetString(1);
                                order3D.ModelName1 = reader.GetString(2);
                                order3D.Scale1 = reader.GetInt32(3);
                                order3D.Texture1 = reader.GetString(4);
                                order3D.Texture2 = reader.GetString(5);
                                order3D.Texture3 = reader.GetString(6);
                                order3D.Color = reader.GetString(7);
                                order3D.Color2 = reader.GetString(8);
                                order3D.Color3 = reader.GetString(9);
                                order3D.status = reader.GetString(11);
                                order3D.order3DPrice = reader.GetInt32(12);
                                order3D.order3DQuantity = reader.GetInt32(13);
                                order3D.order3DShip = reader.GetInt32(14);
                                order3D.order3DDP = reader.GetInt32(15);
                                order3D.order3DPreferredD = reader.GetString(16);
                                order3D.order3DExpectedD = reader.GetString(17);
                                order3D.order3DExpectedT = reader.GetString(18);
                                order3D.order3Dstatus = reader.GetString(21);

                            }
                        }
                    }


                }

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Asset3DForm WHERE username=@user ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user", userconfirm);
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
                    String sql = "SELECT * FROM LoginCustomer WHERE username= '" + order3D.username + "'";

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
      
    }
}
