using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Checkout3DModel : PageModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public int TotalCart = 0;
        public int TotalDP = 0;
        public int ShippingPrice = 0;
        public int cartcount = 0;
        public int TotalCost = 0;
        public string userconfirm = "";
        public string imgconfirm = "";
        public int notifCount = 0;
        public int pnotifCount = 0;
        public int pubnotifCount = 0;
        public int cartCount = 0;
        public int cartCount3D = 0;
        public int totalcartCount = 0;
        public int totalnotifCount = 0;
        public String errorMessage = "";
        public String successMessage = "";
        public CustomerInfo customerInfo = new CustomerInfo();
        public OrderSimpleInfo os = new OrderSimpleInfo();
        public Order3DForm order3D = new Order3DForm();
        public code code = new code();
        public List<Asset3DForm> asset3DList = new List<Asset3DForm>();
        public List<CartPayment> cartPayments = new List<CartPayment>();
        public String statusconfirm = "";
        public int discountCode = 0;
        public decimal discountedPrice = 0;
        public decimal discountedPrice2 = 0;
        public int finaldiscountedPrice = 0;
        public int orderPrice = 0;
        public int TotalNetCost = 0;
        public string connectionProvider = "Data Source=ROVIC\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";


        public void GetProducts()
        { 
            //will load getProducts everytime you launch the website
            string pdID = Request.Query["id"];
            try
            {

                //TOTAL PRODUCT
              
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "SELECT SUM(OrderPrice) FROM Order3DForm WHERE username = @username AND status = 'true'";
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
                    string sql = "select sum(Downpayment) from Order3DForm where username = @username AND status = 'true'";
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
               
                using (SqlConnection connection = new SqlConnection(connectionProvider)) //static
                {
                    connection.Open();
                    string sql = "select ShippingPrice from Order3DForm where username = @username AND status = 'true'";
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

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Order3DForm WHERE username=@user AND status = 'true'";

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
                                orderPrice = reader.GetFieldValue<int>(reader.GetOrdinal("OrderPrice"));
                                order3D.order3DQuantity = reader.GetInt32(13);
                                order3D.order3DShip = reader.GetInt32(14);
                                order3D.order3DDP = reader.GetInt32(15);
                                order3D.order3DPreferredD = reader.GetString(16);
                                order3D.order3DExpectedD = reader.GetString(17);
                                order3D.order3DExpectedT = reader.GetString(18);
                                order3D.order3Dstatus = reader.GetString(11);
                                order3D.ModelType = reader.GetString(19);
                                order3D.netOrder3DPrice = reader.GetInt32(26);
                                TotalCost = TotalCart + ShippingPrice;
                                TotalNetCost = order3D.netOrder3DPrice + order3D.order3DDP + order3D.order3DShip;
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
                        command.Parameters.AddWithValue("@id", order3D.ModelID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Asset3DForm asset = new Asset3DForm();
                                asset.AssetName = reader.GetString(3);

                                asset3DList.Add(asset);
                            }
                        }
                    }


                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading Checkout3D: " + e.ToString());
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


                            string selectsql = "SELECT * FROM Order3DForm WHERE username = @username AND status = 'true'";
                            using (SqlCommand command = new SqlCommand(selectsql, connection))
                            {
                                command.Parameters.AddWithValue("@username", userconfirm);
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
                                        order3D.netOrder3DPrice = reader.GetInt32(26);

                                        using (SqlConnection connectionWrite = new SqlConnection(connectionProvider)) //get the data from the cart
                                        {
                                            connectionWrite.Open();
                                            string insertsql = "UPDATE Order3DForm " +
                                                        "SET status = @status, " +
                                                        "DateCreated = @currentDate, orderstatus = '50% Downpayment', " +
                                                        "receipt = @receipt, PaymentMethod = @paymentMethod " +
                                                        "WHERE OrderID = @orderId;";
                                            using (SqlCommand updateCommand = new SqlCommand(insertsql, connectionWrite))
                                            {
                                                updateCommand.Parameters.AddWithValue("@status", "Paid");
                                                updateCommand.Parameters.AddWithValue("@currentDate", currentDate);
                                                updateCommand.Parameters.AddWithValue("@receipt", "img/Account/" + fileName);
                                                updateCommand.Parameters.AddWithValue("@paymentMethod", paymentMethod);
                                                updateCommand.Parameters.AddWithValue("@orderId", order3D.ModelID);

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

                        return Redirect("/Checkout3D");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                errorMessageProfile = "Profile picture upload failed. Please try again."; // Set error message

                TempData["errorMessageProfile"] = errorMessageProfile; // Store the success message in TempData

                return Redirect("/Checkout3D");
            }


            return Redirect("/Customer_OrderStatus3D");
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
                    string sql2 = "SELECT * FROM GenerateCode WHERE code=@CouponCode AND status='true' AND avail3D='true'";

                    //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql2, connection))
                    {
                        command.Parameters.AddWithValue("@CouponCode", couponcode);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                
                                discountCode = reader.GetFieldValue<int>(reader.GetOrdinal("Discount"));
                                code.status = reader.GetString(2);
                                code.avail2d = reader.GetString(5);
                                discountedPrice = ((decimal)discountCode / percentage) * orderPrice;
                                discountedPrice2 = orderPrice - discountedPrice;
                                finaldiscountedPrice = (int)Math.Round(discountedPrice);

                            }
                        }
                    }
                    if (discountCode == 0 || code.status == "false" || code.avail2d == "false")
                    {

                        TempData["DiscountErrorMessage"] = "Discount code is not valid.";
                        return RedirectToPage("/Checkout3D", new { user = userconfirm });
                    }

                    else
                    {

                        checkSec++;
                        Console.WriteLine("Coupon" + discountCode + "discountedPrice" + discountedPrice + "finaldiscountedPrice" + finaldiscountedPrice + "OrderPrice" + orderPrice);
                        Console.WriteLine("check" + checkSec);
                        if (checkSec > 0)
                        {


                            string sql3 = "update GenerateCode set avail3D='false' where code='" + couponcode + "'";

                            using (SqlCommand command = new SqlCommand(sql3, connection))
                            {

                                command.ExecuteNonQuery();
                            }

                            string sql4 = "update Order3DForm set NetOrderPrice=@discountedPrice, Coupon=@coupon WHERE username = @username AND status = 'true'";

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
                           
                            
                            return RedirectToPage("/Checkout3D", new { user = userconfirm });
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
                    String sql = "SELECT * FROM ShoppingCart";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CartPayment cart = new CartPayment();
                                cart.CartID = reader.GetInt32(0);
                                cart.PaymentImg = reader.GetString(1);
                                cartPayments.Add(cart);
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

