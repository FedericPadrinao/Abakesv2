using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Checkout3D2Model : PageModel
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
        public List<Asset3DForm> asset3DList = new List<Asset3DForm>();
        public String statusconfirm = "";
        public int discountCode = 0;
        public decimal discountedPrice = 0;
        public int finaldiscountedPrice = 0;
        public int remainingFee = 0;
        public int orderPrice = 0;
        public int TotalNetCost = 0;
        public string connectionProvider = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";


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
                    string sql = "SELECT SUM(OrderPrice) FROM Order3DForm WHERE username = @username AND orderstatus = 'Order Confirmation'";
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
                    string sql = "select sum(Downpayment) from Order3DForm where username = @username AND orderstatus = 'Order Confirmation'";
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
                    string sql = "select ShippingPrice from Order3DForm where username = @username AND orderstatus = 'Order Confirmation'";
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
                    String sql = "SELECT * FROM Order3DForm WHERE username=@user AND orderstatus = 'Order Confirmation'";

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
                                remainingFee = order3D.order3DDP + order3D.order3DShip;
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


                            string selectsql = "SELECT * FROM Order3DForm WHERE username = @username AND orderstatus = 'Order Confirmation'";
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
                                                        "SET DateCreated = @currentDate, orderstatus = 'Full Payment', " +
                                                        "picture = @receipt " +
                                                        "WHERE OrderID = @orderId;";
                                            using (SqlCommand updateCommand = new SqlCommand(insertsql, connectionWrite))
                                            {
                                                
                                                updateCommand.Parameters.AddWithValue("@currentDate", currentDate);
                                                updateCommand.Parameters.AddWithValue("@receipt", "img/Account/" + fileName);
                                               
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

                        return Redirect("/Checkout3D2");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                errorMessageProfile = "Profile picture upload failed. Please try again."; // Set error message

                TempData["errorMessageProfile"] = errorMessageProfile; // Store the success message in TempData

                return Redirect("/Checkout3D2");
            }


            return Redirect("/Customer_OrderStatus3D");
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