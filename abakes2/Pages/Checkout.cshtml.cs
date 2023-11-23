using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class CheckoutModel : PageModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public int TotalCart = 0;
        public int TotalDP = 0;
        public int ShippingPrice = 0;
        public int cartcount = 0;
        public int TotalCost = 0;
        public string userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public CustomerInfo customerInfo = new CustomerInfo();
        public List<OrderSimpleInfo> listOrderSimple = new List<OrderSimpleInfo>();
        public String statusconfirm = "";

        public string connectionProvider = "Server=tcp:eu-az-sql-serv4c4db8f1d3864bb6a62cb90162f811bf.database.windows.net,1433;Initial Catalog=dh60fwg8dvwho6h;Persist Security Info=False;User ID=uw1aiy40iqpmqas;Password=U6W5bT1pW3O37J#DEihhdV%p@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
       

        public void GetProducts()
        { //will load getProducts everytime you launch the website
            string pdID = Request.Query["id"];
            try
            {

                //TOTAL PRODUCT
                using (SqlConnection connection = new SqlConnection(connectionProvider)) 
                {
                    connection.Open();
                    string sql = "SELECT SUM(OrderPrice) FROM OrderSimple WHERE username = @username AND status = 'true'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@username", userconfirm);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TotalCart = reader.GetInt32(0);

                            }
                        }
                    }
                }
                //TOTAL DOWNPAYMENT
                using (SqlConnection connection = new SqlConnection(connectionProvider)) //static
                {
                    connection.Open();
                    string sql = "select sum(Downpayment) from OrderSimple where username = @username AND status = 'true'"; 
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@username", userconfirm);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TotalDP = reader.GetInt32(0);

                            }
                        }
                    }
                }
                //SHIPPING FEE
                using (SqlConnection connection = new SqlConnection(connectionProvider)) //static
                {
                    connection.Open();
                    string sql = "select ShippingPrice from OrderSimple where username = @username AND status = 'true'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@username", userconfirm);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ShippingPrice = reader.GetInt32(0);

                            }
                        }
                    }
                }
               

                using (SqlConnection connection = new SqlConnection(connectionProvider)) //static
                {
                    connection.Open();
                    string sql = "select * from OrderSimple where username = @username AND status = 'true'"; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@username", userconfirm);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OrderSimpleInfo os = new OrderSimpleInfo();


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
                                os.osShip = reader.GetFieldValue<int>(reader.GetOrdinal("ShippingPrice"));
                                os.osDP = reader.GetFieldValue<int>(reader.GetOrdinal("Downpayment"));
                                os.osColor = reader.GetFieldValue<string>(reader.GetOrdinal("color"));
                                os.osDedication = reader.GetFieldValue<string>(reader.GetOrdinal("dedication"));
                                listOrderSimple.Add(os);

                                TotalCost = TotalCart + ShippingPrice;
                            }
                        }
                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading Products: " + e.ToString());
                Console.WriteLine("Exception cart: " + e.Message);
            }



        }

        public IActionResult OnGetOrder()
        {
            OnGet();
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            using (SqlConnection connection = new SqlConnection(connectionProvider))
            {
                connection.Open();

                // Step 1: Select data from OrderSimple
                string selectsql = "SELECT * FROM OrderSimple WHERE username = @username AND status = 'true'";
                using (SqlCommand command = new SqlCommand(selectsql, connection))
                {
                    command.Parameters.AddWithValue("@username", userconfirm);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            
                            InvoiceInfo invoice = new InvoiceInfo();
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


                            using (SqlConnection connectionWrite = new SqlConnection(connectionProvider)) //get the data from the cart
                            {
                                connectionWrite.Open();
                                string insertsql = "insert into Invoice (username, occasion, shapes, tier, flavors, sizes, instructions, delivery, status, OrderPrice, OrderQuantity, ShippingPrice, Downpayment, PreferredDelivery, ExpectedDelivery, ExpectedTime, color, dedication, DateCreated)" +
                        "VALUES(@username, @occasion, @shapes, @tier, @flavors, @sizes, @instructions, @delivery, @status, @orderprice, @orderquantity, @shippingprice, @downpayment, @preferred, @expecteddelivery, @expectedtime, @color, @dedication, @currentDate);";
                                using (SqlCommand insertCommand = new SqlCommand(insertsql, connectionWrite))
                                {
                                    insertCommand.Parameters.AddWithValue("@username", invoice.invoiceUsername);
                                    insertCommand.Parameters.AddWithValue("@occasion", invoice.invoiceOccasion);
                                    insertCommand.Parameters.AddWithValue("@shapes", invoice.invoiceShapes);
                                    insertCommand.Parameters.AddWithValue("@tier", invoice.invoiceTier);
                                    insertCommand.Parameters.AddWithValue("@flavors", invoice.invoiceFlavors);
                                    insertCommand.Parameters.AddWithValue("@sizes", invoice.invoiceSizes);
                                    insertCommand.Parameters.AddWithValue("@instructions", invoice.invoiceInstruction);
                                    insertCommand.Parameters.AddWithValue("@delivery", invoice.invoiceDelivery);
                                    insertCommand.Parameters.AddWithValue("@status", invoice.status);
                                    insertCommand.Parameters.AddWithValue("@orderPrice", invoice.invoicePrice);
                                    insertCommand.Parameters.AddWithValue("@orderquantity", invoice.invoiceQuantity);
                                    insertCommand.Parameters.AddWithValue("@shippingprice", invoice.invoiceShip);
                                    insertCommand.Parameters.AddWithValue("@downpayment", invoice.invoiceDP);
                                    insertCommand.Parameters.AddWithValue("@preferred", invoice.invoicePreferredD);
                                    insertCommand.Parameters.AddWithValue("@expecteddelivery", invoice.invoiceExpectedD);
                                    insertCommand.Parameters.AddWithValue("@expectedtime", invoice.invoiceExpectedT);
                                    insertCommand.Parameters.AddWithValue("@color", invoice.invoiceColor);
                                    insertCommand.Parameters.AddWithValue("@dedication", invoice.invoiceDedication);
                                    insertCommand.Parameters.AddWithValue("@currentDate", currentDate);
                                    // Add more parameters as needed

                                    insertCommand.ExecuteNonQuery();
                                }
                            }

                           /* //Remove All Items from Cart

                            using (SqlConnection connectionDelete = new SqlConnection(connectionProvider)) //get the data from the cart
                            {
                                connectionDelete.Open();
                                string sql = "delete from ShoppingCart where username='" + userconfirm + "'"; //deletes all content from user's cat after checking out
                                using (SqlCommand deleteCommand = new SqlCommand(sql, connectionDelete))
                                {


                                    deleteCommand.ExecuteNonQuery();

                                }
                            }
                            */
                        }
                    }
                }
            }

            return Redirect("/Index");
        }

       

        public void OnGet()
        {
            statusconfirm = HttpContext.Session.GetString("userstatus");

            userconfirm = HttpContext.Session.GetString("username");
            GetProducts();
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

            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.ToString());
            }
        }

    }
}
