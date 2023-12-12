using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Customer_OrderStatusModel : PageModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public int TotalCart = 0;
        public int TotalDP = 0;
        public int ShippingPrice = 0;
        public int cartcount = 0;
        public int TotalCost = 0;
        public int notifCount = 0;
        public int pnotifCount = 0;
        public int pubnotifCount = 0;
        public int cartCount = 0;
        public int cartCount3D = 0;
        public int totalcartCount = 0;
        public int totalnotifCount = 0;
        public string userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public CustomerInfo customerInfo = new CustomerInfo();
        public InvoiceInfo invoiceInfo = new InvoiceInfo();
        public OrderSimpleInfo orderInfo = new OrderSimpleInfo();
        public List<OrderSimpleInfo> listOrderSimple = new List<OrderSimpleInfo>();
        public String statusconfirm = "";
        public string imgconfirm = "";
        public string connectionProvider = "Server=tcp:eu-az-sql-serv6e425a9865434acc8b7d6d8badb306b5.database.windows.net,1433;Initial Catalog=d58anpzl3kll8nf;Persist Security Info=False;User ID=ufy99xlgudx1fh3;Password=%g2q&#dV&ECBX6Oyf0%QkXHe5;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


        public void OnGet()
        {

            userconfirm = HttpContext.Session.GetString("username");
            imgconfirm = HttpContext.Session.GetString("userimage");
            statusconfirm = HttpContext.Session.GetString("userstatus");
            if (userconfirm != null)
            {


            }
            else
            {
                Response.Redirect("/Index");
            }

            String user = Request.Query["user"];

            try
            {
                //CUSTOMER
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM LoginCustomer WHERE username= '" + userconfirm + "'";

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

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();

                    string selectsql = "SELECT * FROM Invoice WHERE username = @username AND OrderStatus != 'Complete Order'";
                    using (SqlCommand command = new SqlCommand(selectsql, connection))
                    {
                        command.Parameters.AddWithValue("@username", userconfirm);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                invoiceInfo.invoiceID = reader.GetInt32(0);
                                invoiceInfo.status = reader.GetFieldValue<string>(reader.GetOrdinal("status"));
                                invoiceInfo.invoiceExpectedD = reader.GetFieldValue<string>(reader.GetOrdinal("ExpectedDelivery"));
                                invoiceInfo.invoiceExpectedT = reader.GetFieldValue<string>(reader.GetOrdinal("ExpectedTime"));
                                invoiceInfo.orderStatus = reader.GetFieldValue<string>(reader.GetOrdinal("orderstatus"));

                                invoiceInfo.invoiceOccasion = reader.GetFieldValue<string>(reader.GetOrdinal("occasion"));
                                invoiceInfo.invoiceShapes = reader.GetFieldValue<string>(reader.GetOrdinal("shapes"));
                                invoiceInfo.invoiceTier = reader.GetFieldValue<string>(reader.GetOrdinal("tier"));
                                invoiceInfo.invoiceFlavors = reader.GetFieldValue<string>(reader.GetOrdinal("flavors"));
                                invoiceInfo.invoiceSizes = reader.GetFieldValue<string>(reader.GetOrdinal("sizes"));
                                invoiceInfo.invoiceDelivery = reader.GetFieldValue<string>(reader.GetOrdinal("delivery"));
                               
                                invoiceInfo.invoiceColor = reader.GetFieldValue<string>(reader.GetOrdinal("color"));
                                invoiceInfo.invoiceDedication = reader.GetFieldValue<string>(reader.GetOrdinal("dedication"));
                               
                            }
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionProvider)) //static
                {
                    connection.Open();
                    string sql = "select * from OrderSimple where username = @username"; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@username", userconfirm);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                orderInfo.osID = reader.GetFieldValue<int>(reader.GetOrdinal("OrderID"));
                                orderInfo.osUsername = reader.GetFieldValue<string>(reader.GetOrdinal("username"));
                                orderInfo.osOccasion = reader.GetFieldValue<string>(reader.GetOrdinal("occasion"));
                                orderInfo.osShapes = reader.GetFieldValue<string>(reader.GetOrdinal("shapes"));
                                orderInfo.osTier = reader.GetFieldValue<string>(reader.GetOrdinal("tier"));
                                orderInfo.osFlavors = reader.GetFieldValue<string>(reader.GetOrdinal("flavors"));
                                orderInfo.osSizes = reader.GetFieldValue<string>(reader.GetOrdinal("sizes"));
                                orderInfo.osInstruction = reader.GetFieldValue<string>(reader.GetOrdinal("instructions"));
                                orderInfo.osDelivery = reader.GetFieldValue<string>(reader.GetOrdinal("delivery"));
                                orderInfo.status = reader.GetFieldValue<string>(reader.GetOrdinal("status"));
                                orderInfo.osPrice = reader.GetFieldValue<int>(reader.GetOrdinal("OrderPrice"));
                                orderInfo.osShip = reader.GetFieldValue<int>(reader.GetOrdinal("ShippingPrice"));
                                orderInfo.osDP = reader.GetFieldValue<int>(reader.GetOrdinal("Downpayment"));
                                orderInfo.osColor = reader.GetFieldValue<string>(reader.GetOrdinal("color"));
                                orderInfo.osDedication = reader.GetFieldValue<string>(reader.GetOrdinal("dedication"));


                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.ToString());
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(NotificationID) from Notification where status = 'true'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                notifCount = reader.GetInt32(0);

                            }


                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(NotificationID) from PrivateNotification where status = 'true' AND isRead = 'false'  AND username = '" + userconfirm + "'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                pnotifCount = reader.GetInt32(0);
                            }
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(NotificationID) from ReadPublicNotif where username = '" + userconfirm + "'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                pubnotifCount = reader.GetInt32(0);
                            }
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(OrderID) from OrderSimple where status = 'true' AND username = '" + userconfirm + "'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cartCount = reader.GetInt32(0);
                            }
                        }
                    }
                }
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(OrderID) from Order3dForm where status = 'true' AND username = '" + userconfirm + "'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cartCount3D = reader.GetInt32(0);
                            }
                        }
                    }
                }
                totalcartCount = cartCount3D + cartCount;
                totalnotifCount = notifCount + pnotifCount - pubnotifCount;

            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }
}