using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Checkout2Model : PageModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public int notifCount = 0;
        public int pnotifCount = 0;
        public int pubnotifCount = 0;
        public int cartCount = 0;
        public int totalnotifCount = 0;
        public int TotalCart = 0;
        public int TotalDP = 0;
        public int ShippingPrice = 0;
        public int cartcount = 0;
        public int cartCount3D = 0;
        public int totalcartCount = 0;
        public int orderPrice = 0;
        public string orderstatus = "";
        public string imgconfirm = "";
        public string invoiceorderstatus = "";
        public int TotalCost = 0;
        public int remainingFee = 0;
        public string userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public CustomerInfo customerInfo = new CustomerInfo();
        public List<OrderSimpleInfo> listOrderSimple = new List<OrderSimpleInfo>();
        public OrderSimpleInfo os = new OrderSimpleInfo();
        public InvoiceInfo invoice = new InvoiceInfo();
        public String statusconfirm = "";
        //public code codeInfo = new code();
        public int discountCode = 0;
        public decimal discountedPrice = 0;
        public decimal discountedPrice2 = 0;
        public int finaldiscountedPrice = 0;
        public int TotalNetCost = 0;
        public string connectionProvider = "Server=tcp:eu-az-sql-serv6e425a9865434acc8b7d6d8badb306b5.database.windows.net,1433;Initial Catalog=d58anpzl3kll8nf;Persist Security Info=False;User ID=ufy99xlgudx1fh3;Password=%g2q&#dV&ECBX6Oyf0%QkXHe5;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


        public void GetProducts()
        { //will load getProducts everytime you launch the website
            string pdID = Request.Query["id"];
            try
            {

                //TOTAL PRODUCT
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "SELECT SUM(OrderPrice) FROM Invoice WHERE username = @username AND orderstatus = 'Order Confirmation'";
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
                    string sql = "select sum(Downpayment) from Invoice where username = @username AND orderstatus = 'Order Confirmation'";
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
                    string sql = "select ShippingPrice from Invoice where username = @username AND orderstatus = 'Order Confirmation'";
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
                    string sql = "select * from Invoice where username = @username AND orderstatus = 'Order Confirmation'"; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@username", userconfirm);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
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
                                TotalCost = TotalCart + TotalDP + ShippingPrice;
                                TotalNetCost = invoice.NetInvoicePrice + TotalDP + ShippingPrice;
                                remainingFee = invoice.invoiceDP + invoice.invoiceShip;
                            }
                        }
                    }
                }
               

            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading Checkout2: " + e.ToString());
                Console.WriteLine("Exception cart: " + e.Message);
            }



        }

        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            OnGet();
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string successMessageProfile = "";
            string errorMessageProfile = "";

            string paymentMethod = Request.Form["paymentMethod"];
            try
            {
                // File format validation
                if (file != null && file.Length > 0)
                {
                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                    {
                        // Valid file format, proceed with upload
                        using (SqlConnection connection = new SqlConnection(connectionProvider))
                        {
                             string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "Account", fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }


                            connection.Open();


                            string selectsql = "select * from Invoice where username = @username AND orderstatus = 'Order Confirmation'";
                            using (SqlCommand command = new SqlCommand(selectsql, connection))
                            {
                                command.Parameters.AddWithValue("@username", userconfirm);
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {

                                        InvoiceInfo invoice = new InvoiceInfo();
                                        invoice.invoiceID = reader.GetFieldValue<int>(reader.GetOrdinal("InvoiceID"));
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

                                        using (SqlConnection connectionWrite = new SqlConnection(connectionProvider)) //get the data from the cart
                                        {
                                            connectionWrite.Open();
                                            string insertsql = "UPDATE Invoice " +
                                                        "SET DateCreated = @currentDate, orderstatus = 'Full Payment', " +
                                                        "receipt2 = @receipt " +
                                                        "WHERE InvoiceID = @orderId;";
                                            using (SqlCommand updateCommand = new SqlCommand(insertsql, connectionWrite))
                                            {
                                                updateCommand.Parameters.AddWithValue("@currentDate", currentDate);
                                                updateCommand.Parameters.AddWithValue("@receipt", "img/Account/" + fileName);
                                                
                                                updateCommand.Parameters.AddWithValue("@orderId", invoice.invoiceID);

                                                updateCommand.ExecuteNonQuery();
                                            }
                                        }


                                      

                                    }
                                }
                            }


                            successMessageProfile = "Profile picture changed successfully! Please log in again to take effect";


                        }


                    }
                    else
                    {
                        // Invalid file format, set error message
                        errorMessageProfile = "Invalid file format. Please upload a JPEG, JPG, or PNG file.";
                        TempData["errorMessageProfile"] = errorMessageProfile; // Store the success message in TempData

                        return Redirect("/Index");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                errorMessageProfile = "Profile picture upload failed. Please try again."; // Set error message

                TempData["errorMessageProfile"] = errorMessageProfile; // Store the success message in TempData

                return Redirect("/Index");
            }


            return Redirect("/Index");
        }

  
       

        public void OnGet()
        {
            statusconfirm = HttpContext.Session.GetString("userstatus");
            imgconfirm = HttpContext.Session.GetString("userimage");

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
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Invoice WHERE username= '" + userconfirm + "'";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                invoiceorderstatus = reader.GetFieldValue<string>(reader.GetOrdinal("orderstatus"));


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