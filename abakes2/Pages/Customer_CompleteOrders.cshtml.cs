using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Customer_CompleteOrdersModel : PageModel
    {
        public List<CustomerInfo> listCustomer = new List<CustomerInfo>();
        public List<OrderSimpleInfo> listOrderSimple = new List<OrderSimpleInfo>();
        public List<InvoiceInfo> listInvoice = new List<InvoiceInfo>();
        public List<Order3DForm> listOrder3D = new List<Order3DForm>();
        public String userconfirm = "";
        public string connectionProvider = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";
        public String errorMessage = "";
        public String successMessage = "";

        public String imgconfirm = "";
        public String statusconfirm = "";

        public int notifCount = 0;
        public int pnotifCount = 0;
        public int pubnotifCount = 0;
        public int cartCount = 0;
        public int totalnotifCount = 0;
        public int NotificationCount { get; set; }
        public void GetOrders()
        {   //will load getProducts everytime you launch the website
            string pdID = Request.Query["id"];
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider)) //get the data from the cart
                {
                    connection.Open();
                    string sql = "select * from Invoice where username='" + userconfirm + "' AND orderstatus ='Complete Order'"; //get all the data from the shopping cart based on the user.
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OrderSimpleInfo os = new OrderSimpleInfo();
                                InvoiceInfo invoice = new InvoiceInfo();

                                invoice.invoiceID = reader.GetFieldValue<int>(reader.GetOrdinal("InvoiceID"));
                                invoice.invoiceOccasion = reader.GetFieldValue<string>(reader.GetOrdinal("occasion"));
                                invoice.invoicePrice = reader.GetFieldValue<int>(reader.GetOrdinal("OrderPrice"));
                                invoice.invoiceExpectedD = reader.GetFieldValue<string>(reader.GetOrdinal("ExpectedDelivery"));
                                invoice.invoiceExpectedT = reader.GetFieldValue<string>(reader.GetOrdinal("ExpectedTime"));
                                invoice.invoiceDateCreated = reader.GetFieldValue<string>(reader.GetOrdinal("DateCreated"));
                                invoice.receipt = reader.GetFieldValue<string>(reader.GetOrdinal("receipt"));
                                
                                listInvoice.Add(invoice);


                            }
                        }
                    }
                }
                using (SqlConnection connection = new SqlConnection(connectionProvider)) //get the data from the cart
                {
                    connection.Open();
                    string sql = "select * from Order3DForm where username='" + userconfirm + "' AND orderstatus ='Complete Order'"; //get all the data from the shopping cart based on the user.
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Order3DForm order3d= new Order3DForm();
                                

                                order3d.ModelID = reader.GetInt32(0);
                               
                                order3d.instructions = reader.GetString(10);
                                order3d.status = reader.GetString(11);
                                order3d.order3DPrice = reader.GetInt32(12);
                                order3d.order3DQuantity = reader.GetInt32(13);
                                order3d.order3DShip = reader.GetInt32(14);
                                order3d.order3DDP = reader.GetInt32(15);
                                order3d.order3DPreferredD = reader.GetString(16);
                                order3d.order3DExpectedD = reader.GetString(17);
                                order3d.order3DExpectedT = reader.GetString(18);
                                order3d.ModelType = reader.GetString(19);
                                order3d.order3DDateCreated = reader.GetString(20);
                                order3d.receipt = reader.GetString(22);
                                order3d.paymentMethod = reader.GetString(23);
                                order3d.order3DDelivery = reader.GetString(24);
                                
                                listOrder3D.Add(order3d);

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading Orders: " + e.ToString());
                Console.WriteLine("Exception cart: " + e.Message);
            }

        }


        public IActionResult OnGetRemoveOrder()
        {
            OnGet();
            if (userconfirm == null)
            { //If not logged in, it will be redirected to login page
                return RedirectToPage("/Account");
            }
            else
            {

            }
            string osid = Request.Query["id"]; //name from the front end "?id=

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionProvider)) //static
                {
                    connection.Open();
                    string sql = "delete from OrderSimple where OrderID='" + osid + "' and username='" + userconfirm + "'"; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception e)
            {

            }

            return Redirect("/index");
        }

        public void OnGet()
        {
            userconfirm = HttpContext.Session.GetString("username");
            imgconfirm = HttpContext.Session.GetString("userimage");
            statusconfirm = HttpContext.Session.GetString("userstatus");
            notifCount = HttpContext.Session.GetInt32("NotificationCount") ?? 0;
            if (userconfirm == null)
            {
                Response.Redirect("/Account");
            }
            else
            {

            }
            GetOrders();
            //NAV COUNT
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
                totalnotifCount = notifCount + pnotifCount - pubnotifCount;

            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

    }

}
