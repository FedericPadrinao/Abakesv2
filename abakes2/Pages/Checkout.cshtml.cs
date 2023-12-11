using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class CheckoutModel : PageModel
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
        public int codechecker = 0;
        public string userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public CustomerInfo customerInfo = new CustomerInfo();
        public List<OrderSimpleInfo> listOrderSimple = new List<OrderSimpleInfo>();
        public OrderSimpleInfo os = new OrderSimpleInfo();
        public InvoiceInfo invoice = new InvoiceInfo();
        public code code = new code();
        public String statusconfirm = "";
        //public code codeInfo = new code();
        public int discountCode = 0;
        public decimal discountedPrice = 0;
        public decimal discountedPrice2 = 0;
        public int finaldiscountedPrice = 0;
        public int TotalNetCost = 0;
        public string connectionProvider = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";
       

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
                                orderstatus = reader.GetFieldValue<string>(reader.GetOrdinal("status"));
                                os.osPrice = reader.GetFieldValue<int>(reader.GetOrdinal("OrderPrice"));
                                orderPrice = reader.GetFieldValue<int>(reader.GetOrdinal("OrderPrice"));
                                os.osShip = reader.GetFieldValue<int>(reader.GetOrdinal("ShippingPrice"));
                                os.osDP = reader.GetFieldValue<int>(reader.GetOrdinal("Downpayment"));
                                os.osColor = reader.GetFieldValue<string>(reader.GetOrdinal("color"));
                                os.osDedication = reader.GetFieldValue<string>(reader.GetOrdinal("dedication"));
                                os.netOrderPrice = reader.GetFieldValue<int>(reader.GetOrdinal("NetOrderPrice"));
                                listOrderSimple.Add(os);

                                TotalCost = TotalCart + TotalDP + ShippingPrice;
                                TotalNetCost = os.netOrderPrice + TotalDP + ShippingPrice;
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
                                invoice.NetInvoicePrice = reader.GetFieldValue<int>(reader.GetOrdinal("NetOrderPrice"));
                                invoice.CouponCode = reader.GetFieldValue<string>(reader.GetOrdinal("Coupon"));

                                remainingFee = invoice.invoiceDP + invoice.invoiceShip;
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
                            string fileName = Path.GetFileName(file.FileName);
                            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "Account", fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }


                            connection.Open();


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
                                        invoice.NetInvoicePrice = reader.GetFieldValue<int>(reader.GetOrdinal("NetOrderPrice"));
                                        invoice.CouponCode = reader.GetFieldValue<string>(reader.GetOrdinal("Coupon"));

                                        using (SqlConnection connectionWrite = new SqlConnection(connectionProvider)) //get the data from the cart
                                        {
                                            connectionWrite.Open();
                                            string insertsql = "insert into Invoice (username, occasion, shapes, tier, flavors, sizes, instructions, delivery, status, OrderPrice, OrderQuantity, ShippingPrice, Downpayment, PreferredDelivery, ExpectedDelivery, ExpectedTime, color, dedication, DateCreated, orderstatus, receipt, PaymentMethod,Coupon,NetInvoicePrice,receipt2)" +
                                    "VALUES(@username, @occasion, @shapes, @tier, @flavors, @sizes, @instructions, @delivery, @status, @orderprice, @orderquantity, @shippingprice, @downpayment, @preferred, @expecteddelivery, @expectedtime, @color, @dedication, @currentDate, '50% Downpayment', @receipt, @paymentMethod,@coupon,@netInvoicePrice,'');";
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
                                                insertCommand.Parameters.AddWithValue("@receipt", "img/Account/" + fileName);
                                                insertCommand.Parameters.AddWithValue("@paymentMethod", paymentMethod);
                                                insertCommand.Parameters.AddWithValue("@coupon", invoice.CouponCode);
                                                insertCommand.Parameters.AddWithValue("@netInvoicePrice", invoice.NetInvoicePrice);
                  

                                                insertCommand.ExecuteNonQuery();
                                            }
                                        }

                                        //Remove All Items from Cart

                                        using (SqlConnection connectionDelete = new SqlConnection(connectionProvider)) //get the data from the cart
                                        {
                                            connectionDelete.Open();
                                            string sql = "delete from OrderSimple where username='" + userconfirm + "'"; //deletes all content from user's cat after checking out
                                            using (SqlCommand deleteCommand = new SqlCommand(sql, connectionDelete))
                                            {


                                                deleteCommand.ExecuteNonQuery();

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

     

        public async Task<IActionResult> OnPostDiscountAsync()
        {
            OnGet();
            string couponcode = Request.Form["coupon"];
            int checkSec = 0;
            int percentage = 100;
            try
            {
                    using (SqlConnection connection = new SqlConnection(connectionProvider))
                    {
                        connection.Open();
                        string sql2 = "SELECT * FROM GenerateCode WHERE code=@CouponCode AND status='true' AND avail2d='true'";

                        //getting the data based from the pdid variable
                        using (SqlCommand command = new SqlCommand(sql2, connection))
                        {
                        command.Parameters.AddWithValue("@CouponCode", couponcode);
                        using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    code.status = reader.GetString(2);
                                    code.avail2d = reader.GetString(5);
                                    
                                    discountCode = reader.GetFieldValue<int>(reader.GetOrdinal("Discount"));
                                    
                                    discountedPrice = ((decimal)discountCode/percentage)*orderPrice;
                                    discountedPrice2 = orderPrice - discountedPrice;
                                    finaldiscountedPrice = (int)Math.Round(discountedPrice2);

                                }
                            }
                     }
                    if (discountCode == 0 || code.status == "false" || code.avail2d == "false")
                    {
                        
                        TempData["DiscountErrorMessage"] = "Discount code is not valid.";
                        return RedirectToPage("/Checkout", new { user = userconfirm });
                    }
                   
                    else
                    {

                        checkSec++;
                        Console.WriteLine("Coupon" + discountCode + "discountedPrice" + discountedPrice + "finaldiscountedPrice" + finaldiscountedPrice + "OrderPrice" + orderPrice);
                        Console.WriteLine("check" + checkSec);
                        if (checkSec > 0)
                        {


                            string sql3 = "update GenerateCode set avail2d='false' where code='" + couponcode + "'";

                            using (SqlCommand command = new SqlCommand(sql3, connection))
                            {

                                command.ExecuteNonQuery();
                            }

                            string sql4 = "update OrderSimple set NetOrderPrice=@discountedPrice,Coupon=@coupon WHERE username = @username AND status = 'true'";

                            using (SqlCommand command = new SqlCommand(sql4, connection))
                            {
                                command.Parameters.AddWithValue("@username", userconfirm);
                                command.Parameters.AddWithValue("@coupon", couponcode);
                                command.Parameters.AddWithValue("@discountedPrice", finaldiscountedPrice);
                                command.ExecuteNonQuery();
                            }


                        }
                        else
                        {

                            return RedirectToPage("/Checkout", new { user = userconfirm });
                        }

                    }

                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToPage("/Checkout", new { user = userconfirm });
            }

            return RedirectToPage("/Checkout", new { user = userconfirm });
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
