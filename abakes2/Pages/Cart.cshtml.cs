using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class CartModel : PageModel
    {
        public List<CustomerInfo> listCustomer = new List<CustomerInfo>();
        public List<OrderSimpleInfo> listOrderSimple = new List<OrderSimpleInfo>();
        public OrderSimpleInfo os = new OrderSimpleInfo();
        public InvoiceInfo invoice = new InvoiceInfo();
        public Order3DForm order3D = new Order3DForm();
        public String userconfirm = "";
        public string connectionProvider = "Data Source=ROVIC\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";
        public String errorMessage = "";
        public String successMessage = "";
        public int TotalCart = 0;
        public int cartcount = 0;
        public String imgconfirm = "";
        public String statusconfirm = "";
        public int simpleEmpty = 0;
        public int threeEmpty = 0;
        public int invoiceEmpty = 0;
        public int threeinvoiceEmpty = 0;
        public int pubnotifCount = 0;
        public int notifCount = 0;
        public int pnotifCount = 0;
        public int cartCount = 0;
        public int cartCount3D = 0;
        public int totalcartCount = 0;
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
                    string sql = "select * from OrderSimple where username='" + userconfirm + "' AND status ='true'"; //get all the data from the shopping cart based on the user.
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                simpleEmpty++;
                                
                                Console.WriteLine("No orders found for user: " + simpleEmpty);
                            }
                            else
                            {
                                while (reader.Read())
                                {



                                    os.osID = reader.GetFieldValue<int>(reader.GetOrdinal("OrderID"));
                                    os.osUsername = reader.GetFieldValue<string>(reader.GetOrdinal("username"));
                                    os.osOccasion = reader.GetFieldValue<string>(reader.GetOrdinal("occasion"));
                                    os.osShapes = reader.GetFieldValue<string>(reader.GetOrdinal("shapes"));
                                    os.osTier = reader.GetFieldValue<string>(reader.GetOrdinal("tier"));
                                    os.osFlavors = reader.GetFieldValue<string>(reader.GetOrdinal("flavors"));
                                    os.osSizes = reader.GetFieldValue<string>(reader.GetOrdinal("sizes"));
                                    os.osInstruction = reader.GetFieldValue<string>(reader.GetOrdinal("instructions"));
                                    os.osDelivery = reader.GetFieldValue<string>(reader.GetOrdinal("delivery"));
                                    os.status = reader.GetFieldValue<string>(reader.GetOrdinal("status"));
                                    os.osPrice = reader.GetFieldValue<int>(reader.GetOrdinal("OrderPrice"));

                                  

                                }
                            }
                        }
                    }
                }
                using (SqlConnection connection = new SqlConnection(connectionProvider)) //get the data from the cart
                {
                    connection.Open();
                    string sql = "select * from Invoice where username='" + userconfirm + "' AND orderstatus ='Order Confirmation'"; //get all the data from the shopping cart based on the user.
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                invoiceEmpty++;

                                Console.WriteLine("No orders found for user: " + simpleEmpty);
                            }
                            while (reader.Read())
                                {
                                invoice.invoiceID = reader.GetInt32(0);
                                invoice.invoiceUsername = reader.GetFieldValue<string>(reader.GetOrdinal("username"));
                                invoice.invoiceOccasion = reader.GetFieldValue<string>(reader.GetOrdinal("occasion"));
                                invoice.invoiceShapes = reader.GetFieldValue<string>(reader.GetOrdinal("shapes"));
                                invoice.invoiceTier = reader.GetFieldValue<string>(reader.GetOrdinal("tier"));
                                invoice.invoiceFlavors = reader.GetFieldValue<string>(reader.GetOrdinal("flavors"));
                                invoice.invoiceSizes = reader.GetFieldValue<string>(reader.GetOrdinal("sizes"));
                                invoice.invoiceInstruction = reader.GetFieldValue<string>(reader.GetOrdinal("instructions"));
                                invoice.invoiceDelivery = reader.GetFieldValue<string>(reader.GetOrdinal("delivery"));
                                invoice.status = reader.GetFieldValue<string>(reader.GetOrdinal("status"));
                                invoice.invoicePrice = reader.GetFieldValue<int>(reader.GetOrdinal("OrderPrice"));
                                invoice.invoiceQuantity = reader.GetFieldValue<int>(reader.GetOrdinal("OrderQuantity"));
                                invoice.invoiceShip = reader.GetFieldValue<int>(reader.GetOrdinal("ShippingPrice"));
                                invoice.invoiceDP = reader.GetFieldValue<int>(reader.GetOrdinal("Downpayment"));
                                invoice.invoicePreferredD = reader.GetFieldValue<string>(reader.GetOrdinal("PreferredDelivery"));
                                invoice.invoiceExpectedD = reader.GetFieldValue<string>(reader.GetOrdinal("ExpectedDelivery"));
                                invoice.invoiceExpectedT = reader.GetFieldValue<string>(reader.GetOrdinal("ExpectedTime"));
                                invoice.invoiceColor = reader.GetFieldValue<string>(reader.GetOrdinal("color"));
                                invoice.invoiceDedication = reader.GetFieldValue<string>(reader.GetOrdinal("dedication"));
                                invoice.NetInvoicePrice = reader.GetFieldValue<int>(reader.GetOrdinal("NetInvoicePrice"));
                                invoice.CouponCode = reader.GetFieldValue<string>(reader.GetOrdinal("Coupon"));





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
        public IActionResult OnGetOrder()
        { //Check out Order
            
            if (userconfirm == null)
            {
                return RedirectToPage("/login");
            }
            else
            {

            }

            string order_id = "";
            string username = "";
            int pID = 0;
            string pName = "";
            string category = "";
            int quantity = 1;
            int total = 0;
            int price = 0;
            string pImg = "";

         


            //Remove All Items from Cart

            using (SqlConnection connection = new SqlConnection(connectionProvider)) //get the data from the cart
            {
                connection.Open();
                string sql = "delete from OrderSimple where username='" + userconfirm + "'"; //deletes all content from user's cat after checking out
                using (SqlCommand command = new SqlCommand(sql, connection))
                {


                    command.ExecuteNonQuery();

                }
            }


            return Redirect("/Thanks");
        }


        public IActionResult OnGetRemoveCart()
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

            return Redirect("/Cart");
        }

        public IActionResult OnGetRemove3D()
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
                    string sql = "delete from Order3DForm where OrderID='" + osid + "' and username='" + userconfirm + "'"; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception e)
            {

            }

            return Redirect("/Cart");
        }




        public void OnGet()
        {
            userconfirm = HttpContext.Session.GetString("username");
            imgconfirm = HttpContext.Session.GetString("userimage");
            statusconfirm = HttpContext.Session.GetString("userstatus");

            GetOrders();

            // String id = Request.Query["id"]; //SPECIFIES ANONG PRODUCT 

            if (userconfirm != null)
            {


            }
            else
            {
                Response.Redirect("/Index");
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select * from LoginCustomer";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CustomerInfo customerInfo = new CustomerInfo(); 
                                customerInfo.username = reader.GetFieldValue<string>(reader.GetOrdinal("username"));
                                customerInfo.password = reader.GetFieldValue<string>(reader.GetOrdinal("password"));
                                customerInfo.email = reader.GetFieldValue<string>(reader.GetOrdinal("email"));
                                customerInfo.phone = reader.GetFieldValue<string>(reader.GetOrdinal("phone"));
                                customerInfo.lname = reader.GetFieldValue<string>(reader.GetOrdinal("lname"));
                                customerInfo.fname = reader.GetFieldValue<string>(reader.GetOrdinal("fname"));
                                customerInfo.address = reader.GetFieldValue<string>(reader.GetOrdinal("address"));
                                customerInfo.city = reader.GetFieldValue<string>(reader.GetOrdinal("city"));
                                customerInfo.barangay = reader.GetFieldValue<string>(reader.GetOrdinal("barangay"));

                                listCustomer.Add(customerInfo);
                            }
                        }
                    }
                }

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

                using (SqlConnection connection = new SqlConnection(connectionProvider)) //get the data from the cart
                {
                    connection.Open();
                    string sql = "select * from Order3DForm where username='" + userconfirm + "' AND status ='true'"; //get all the data from the shopping cart based on the user.
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                threeEmpty++;
                                Console.WriteLine("No orders found for 3duser: " + threeEmpty);
                            }
                            else
                            {


                                while (reader.Read())
                                {



                                    order3D.ModelID = reader.GetFieldValue<int>(reader.GetOrdinal("OrderID"));
                                    order3D.ModelType = reader.GetFieldValue<string>(reader.GetOrdinal("ModelType"));
                                    order3D.instructions = reader.GetFieldValue<string>(reader.GetOrdinal("instructions"));
                                    order3D.order3DDelivery = reader.GetFieldValue<string>(reader.GetOrdinal("OrderDelivery"));
                                    order3D.order3DPrice = reader.GetFieldValue<int>(reader.GetOrdinal("OrderPrice"));
                             


                                }
                            }
                        }
                    }
                }
                using (SqlConnection connection = new SqlConnection(connectionProvider)) //get the data from the cart
                {
                    connection.Open();
                    string sql = "select * from Order3DForm where username='" + userconfirm + "' AND orderstatus ='Order Confirmation'"; //get all the data from the shopping cart based on the user.
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                threeinvoiceEmpty++;
                                Console.WriteLine("No orders found for 3duser: " + threeEmpty);
                            }
                            else
                            {


                                while (reader.Read())
                                {



                                    order3D.ModelID = reader.GetFieldValue<int>(reader.GetOrdinal("OrderID"));
                                    order3D.ModelType = reader.GetFieldValue<string>(reader.GetOrdinal("ModelType"));
                                    order3D.instructions = reader.GetFieldValue<string>(reader.GetOrdinal("instructions"));
                                    order3D.order3DDelivery = reader.GetFieldValue<string>(reader.GetOrdinal("OrderDelivery"));
                                    order3D.order3DPrice = reader.GetFieldValue<int>(reader.GetOrdinal("OrderPrice"));
                                    order3D.order3DDP = reader.GetFieldValue<int>(reader.GetOrdinal("Downpayment"));
                                    order3D.order3Dstatus = reader.GetFieldValue<string>(reader.GetOrdinal("orderstatus"));


                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }


    




        }

    }

}
