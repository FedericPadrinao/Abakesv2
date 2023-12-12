using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Customer_Order3DFormModel : PageModel
    {
        public List<Feedbacks> listFeedback = new List<Feedbacks>();
        public List<UserInfo> userInfo = new List<UserInfo>();
        public List<Order3DInfo> order3DList = new List<Order3DInfo>();
        public Order3DForm order3DForm = new Order3DForm();
        public CustomerInfo customerInfo = new CustomerInfo();
        public List<Asset3DInfo> asset3DList = new List<Asset3DInfo>();
        public Asset3DForm asset3DForm = new Asset3DForm();
        public Boolean check = false;
        public String userconfirm = "";
        public String imgconfirm = "";
        public String statusconfirm = "";
        public int notifCount = 0;
        public int pnotifCount = 0;
        public int pubnotifCount = 0;
        public int cartCount = 0;
        public int cartCount3D = 0;
        public int totalcartCount = 0;
        public int totalnotifCount = 0;
        public int NotificationCount { get; set; }
        public string connectionString = "Server=tcp:eu-az-sql-serv6e425a9865434acc8b7d6d8badb306b5.database.windows.net,1433;Initial Catalog=d58anpzl3kll8nf;Persist Security Info=False;User ID=ufy99xlgudx1fh3;Password=%g2q&#dV&ECBX6Oyf0%QkXHe5;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public void GetModels()
        {
            String id = Request.Query["Id"];
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "select * from Order3D "; //getting the data based from the fbid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                Order3DInfo orderThree = new Order3DInfo();

                                orderThree.ModelID = reader.GetFieldValue<int>(reader.GetOrdinal("Id"));
                                orderThree.ModelName = reader.GetFieldValue<string>(reader.GetOrdinal("ModelName"));
                                orderThree.Scale = reader.GetFieldValue<int>(reader.GetOrdinal("Scale"));
                                orderThree.TColor = reader.GetFieldValue<string>(reader.GetOrdinal("Texture1"));
                                orderThree.TColor2 = reader.GetFieldValue<string>(reader.GetOrdinal("Texture2"));
                                orderThree.TColor3 = reader.GetFieldValue<string>(reader.GetOrdinal("Texture3"));
                                orderThree.TotalTexture = reader.GetFieldValue<int>(reader.GetOrdinal("TotalTexture"));
                                orderThree.Color = reader.GetFieldValue<string>(reader.GetOrdinal("TextureColor1"));
                                orderThree.Color2 = reader.GetFieldValue<string>(reader.GetOrdinal("TextureColor2"));
                                orderThree.Color3 = reader.GetFieldValue<string>(reader.GetOrdinal("TextureColor3"));
                                orderThree.ModelType = reader.GetFieldValue<string>(reader.GetOrdinal("ModelType"));
                                /* orderInfo.ModelID = reader.GetInt32(0);
                                 orderInfo.ModelName = reader.GetString(1);
                                 orderInfo.ScaleX = reader.GetInt32(1);
                                 orderInfo.ScaleY = reader.GetInt32(2);
                                 orderInfo.ScaleZ = reader.GetInt32(3); */

                                order3DList.Add(orderThree);


                            }
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "select * from Asset3D ORDER BY AssetName ASC"; //getting the data based from the fbid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Asset3DInfo assetThree = new Asset3DInfo();
                                assetThree.AssetID = reader.GetFieldValue<int>(reader.GetOrdinal("AssetID"));
                                assetThree.AssetName = reader.GetFieldValue<string>(reader.GetOrdinal("AssetName"));
                                assetThree.AssetPath = reader.GetFieldValue<string>(reader.GetOrdinal("AssetPath"));
                                assetThree.AssetScale = reader.GetFieldValue<int>(reader.GetOrdinal("AssetScale"));
                                assetThree.PositionX = reader.GetFieldValue<string>(reader.GetOrdinal("PositionX"));
                                assetThree.PositionY = reader.GetFieldValue<string>(reader.GetOrdinal("PositionY"));
                                assetThree.PositionZ = reader.GetFieldValue<string>(reader.GetOrdinal("PositionZ"));

                                asset3DList.Add(assetThree);

                            }
                        }
                    }
                }





            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading Models: " + e.ToString());

            }
        }

        public async Task<IActionResult> OnPostInsertCake(IFormFile file)
        {
            OnGet();
            order3DForm.ModelName1 = Request.Form["modelname"];
            order3DForm.Scale1 = int.Parse(Request.Form["scale"]);
            order3DForm.Texture1 = Request.Form["texture1"];
            order3DForm.Texture2 = Request.Form["texture2"];
            order3DForm.Texture3 = Request.Form["texture3"];
            order3DForm.Color = Request.Form["color1"];
            order3DForm.Color2 = Request.Form["color2"];
            order3DForm.Color3 = Request.Form["color3"];
            order3DForm.ModelType = Request.Form["ModelType"];
            string assetNameStr = Request.Form["assetname"];
            string assetPathStr = Request.Form["assetpath"];
            string assetScaleStr = Request.Form["assetscale"];
            string posX = Request.Form["positionx"];
            string posY = Request.Form["positiony"];
            string posZ = Request.Form["positionz"];
            
            string instructions = Request.Form["special_instructions"];
            string Delivery = Request.Form["delivery"];
            string Preferred = Request.Form["preferred"];
            string[] assetNameList = assetNameStr.Split(",");
            string[] assetPathList = assetPathStr.Split(",");
            string[] assetScaleList = assetScaleStr.Split(",");
            string[] posXList = posX.Split(",");
            string[] posYList = posY.Split(",");
            string[] posZList = posZ.Split(",");
            int insertedPrimaryKey = 0;
            try
            {
                if (file == null)
                {
                   
                        // Valid file format, proceed with upload
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                        
                            string fileName = "";
                           
                           

                            connection.Open();
                            string sql3 = "INSERT INTO Order3DForm " +
                                 "(username, Tier1, Scale1, Texture1, Texture2, Texture3, Color1, Color2, Color3, instructions, status, OrderPrice, OrderQuantity, ShippingPrice, Downpayment, PreferredDelivery, ExpectedDelivery, ExpectedTime, ModelType, DateCreated, orderstatus, receipt, PaymentMethod, OrderDelivery, Coupon, NetOrderPrice,picture,dedicationpic) " +
                                 "VALUES " +
                                 "(@username, @Tier1, @Scale1, @Texture1, @Texture2, @Texture3, @Color1, @Color2, @Color3, @instructions, 'false', '0' , '1', '0', '0', @preferred, '', '', @modeltype, @dateCreated, @orderstatus, @receipt, @paymentMethod, @delivery, '', 0, '', @dedicationpic); SELECT SCOPE_IDENTITY(); ";

                            using (SqlCommand insertcommand = new SqlCommand(sql3, connection))
                            {
                                insertcommand.Parameters.AddWithValue("@username", userconfirm);
                                insertcommand.Parameters.AddWithValue("@Tier1", order3DForm.ModelName1);
                                insertcommand.Parameters.AddWithValue("@Scale1", order3DForm.Scale1);
                                insertcommand.Parameters.AddWithValue("@Texture1", order3DForm.Texture1);
                                insertcommand.Parameters.AddWithValue("@Texture2", order3DForm.Texture2);
                                insertcommand.Parameters.AddWithValue("@Texture3", order3DForm.Texture3);
                                insertcommand.Parameters.AddWithValue("@Color1", order3DForm.Color);
                                insertcommand.Parameters.AddWithValue("@Color2", order3DForm.Color2);
                                insertcommand.Parameters.AddWithValue("@Color3", order3DForm.Color3);
                                insertcommand.Parameters.AddWithValue("@instructions", instructions);
                                insertcommand.Parameters.AddWithValue("@delivery", Delivery);
                                insertcommand.Parameters.AddWithValue("@preferred", Preferred);
                                insertcommand.Parameters.AddWithValue("@modeltype", order3DForm.ModelType);
                                insertcommand.Parameters.AddWithValue("@dateCreated", "");
                                insertcommand.Parameters.AddWithValue("@orderstatus", "Design Your Cake");
                                insertcommand.Parameters.AddWithValue("@receipt", "");
                                insertcommand.Parameters.AddWithValue("@paymentMethod", "");
                                insertcommand.Parameters.AddWithValue("@dedicationpic", fileName);


                                insertedPrimaryKey = Convert.ToInt32(insertcommand.ExecuteScalar());




                            }
                        }


                        for (int i = 0; i < assetNameList.Length - 1; i++)
                        {
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();
                                string sql4 = "INSERT INTO Asset3DForm " +
                                     "(OrderID, username, ModelName, ModelPath, ModelScale, PositionX, PositionY, PositionZ) " +
                                     "VALUES " +
                                     "(@orderID, @username, @assetname, @assetpath, @assetscale, @positionx, @positiony, @positionz) ";

                                using (SqlCommand insertcommand1 = new SqlCommand(sql4, connection))
                                {


                                    Console.WriteLine("LengtH: " + assetNameList.Length);
                                    insertcommand1.Parameters.AddWithValue("@orderid", insertedPrimaryKey);
                                    insertcommand1.Parameters.AddWithValue("@username", userconfirm);
                                    insertcommand1.Parameters.AddWithValue("@assetname", assetNameList[i]);
                                    insertcommand1.Parameters.AddWithValue("@assetpath", assetPathList[i]);
                                    insertcommand1.Parameters.AddWithValue("@assetscale", assetScaleList[i]);
                                    insertcommand1.Parameters.AddWithValue("@positionx", posXList[i]);
                                    insertcommand1.Parameters.AddWithValue("@positiony", posYList[i]);
                                    insertcommand1.Parameters.AddWithValue("@positionz", posZList[i]);


                                    insertcommand1.ExecuteNonQuery();

                                }
                            }
                        }
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            String sql = "UPDATE LoginCustomer SET ordermax3D = 'true' WHERE username = @user";
                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {

                                command.Parameters.AddWithValue("@user", userconfirm);
                                command.ExecuteNonQuery();



                                // Redirect to the success page after successful form submission
                                return RedirectToPage("/Customer_OrderSuccess3D_Prompt");

                            }
                        }
                    
                }
                if (file != null && file.Length > 0)
                {
                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                    {
                        // Valid file format, proceed with upload
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            string fileName = Path.GetFileName(file.FileName);
                            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "Account", fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            connection.Open();
                            string sql3 = "INSERT INTO Order3DForm " +
                                 "(username, Tier1, Scale1, Texture1, Texture2, Texture3, Color1, Color2, Color3, instructions, status, OrderPrice, OrderQuantity, ShippingPrice, Downpayment, PreferredDelivery, ExpectedDelivery, ExpectedTime, ModelType, DateCreated, orderstatus, receipt, PaymentMethod, OrderDelivery, Coupon, NetOrderPrice,picture,dedicationpic) " +
                                 "VALUES " +
                                 "(@username, @Tier1, @Scale1, @Texture1, @Texture2, @Texture3, @Color1, @Color2, @Color3, @instructions, 'false', '0' , '1', '0', '0', @preferred, '', '', @modeltype, @dateCreated, @orderstatus, @receipt, @paymentMethod, @delivery, '', 0, '', @dedicationpic); SELECT SCOPE_IDENTITY(); ";

                            using (SqlCommand insertcommand = new SqlCommand(sql3, connection))
                            {
                                insertcommand.Parameters.AddWithValue("@username", userconfirm);
                                insertcommand.Parameters.AddWithValue("@Tier1", order3DForm.ModelName1);
                                insertcommand.Parameters.AddWithValue("@Scale1", order3DForm.Scale1);
                                insertcommand.Parameters.AddWithValue("@Texture1", order3DForm.Texture1);
                                insertcommand.Parameters.AddWithValue("@Texture2", order3DForm.Texture2);
                                insertcommand.Parameters.AddWithValue("@Texture3", order3DForm.Texture3);
                                insertcommand.Parameters.AddWithValue("@Color1", order3DForm.Color);
                                insertcommand.Parameters.AddWithValue("@Color2", order3DForm.Color2);
                                insertcommand.Parameters.AddWithValue("@Color3", order3DForm.Color3);
                                insertcommand.Parameters.AddWithValue("@instructions", instructions);
                                insertcommand.Parameters.AddWithValue("@delivery", Delivery);
                                insertcommand.Parameters.AddWithValue("@preferred", Preferred);
                                insertcommand.Parameters.AddWithValue("@modeltype", order3DForm.ModelType);
                                insertcommand.Parameters.AddWithValue("@dateCreated", "");
                                insertcommand.Parameters.AddWithValue("@orderstatus", "Design Your Cake");
                                insertcommand.Parameters.AddWithValue("@receipt", "");
                                insertcommand.Parameters.AddWithValue("@paymentMethod", "");
                                insertcommand.Parameters.AddWithValue("@dedicationpic", "img/Account/" + fileName);


                                insertedPrimaryKey = Convert.ToInt32(insertcommand.ExecuteScalar());




                            }
                        }


                        for (int i = 0; i < assetNameList.Length - 1; i++)
                        {
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();
                                string sql4 = "INSERT INTO Asset3DForm " +
                                     "(OrderID, username, ModelName, ModelPath, ModelScale, PositionX, PositionY, PositionZ) " +
                                     "VALUES " +
                                     "(@orderID, @username, @assetname, @assetpath, @assetscale, @positionx, @positiony, @positionz) ";

                                using (SqlCommand insertcommand1 = new SqlCommand(sql4, connection))
                                {


                                    Console.WriteLine("LengtH: " + assetNameList.Length);
                                    insertcommand1.Parameters.AddWithValue("@orderid", insertedPrimaryKey);
                                    insertcommand1.Parameters.AddWithValue("@username", userconfirm);
                                    insertcommand1.Parameters.AddWithValue("@assetname", assetNameList[i]);
                                    insertcommand1.Parameters.AddWithValue("@assetpath", assetPathList[i]);
                                    insertcommand1.Parameters.AddWithValue("@assetscale", assetScaleList[i]);
                                    insertcommand1.Parameters.AddWithValue("@positionx", posXList[i]);
                                    insertcommand1.Parameters.AddWithValue("@positiony", posYList[i]);
                                    insertcommand1.Parameters.AddWithValue("@positionz", posZList[i]);


                                    insertcommand1.ExecuteNonQuery();

                                }
                            }
                        }
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            String sql = "UPDATE LoginCustomer SET ordermax3D = 'true' WHERE username = @user";
                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {

                                command.Parameters.AddWithValue("@user", userconfirm);
                                command.ExecuteNonQuery();



                                // Redirect to the success page after successful form submission
                                return RedirectToPage("/Customer_OrderSuccess3D_Prompt");

                            }
                        }
                    }
                }
            }
           
            catch (Exception ex)
            {
                //errorMessage = ex.Message;
                Console.WriteLine("Error: " + ex.ToString());
            }


            return RedirectToPage("/Customer_OrderSuccess3D_Prompt");
        }



        public void OnGet()
        {

            userconfirm = HttpContext.Session.GetString("username");
            imgconfirm = HttpContext.Session.GetString("userimage");
            statusconfirm = HttpContext.Session.GetString("userstatus");
            notifCount = HttpContext.Session.GetInt32("NotificationCount") ?? 0;
            if (userconfirm == null)
            {
                HttpContext.Session.SetString("ReturnUrl", Request.Path);
                Response.Redirect("/Account");
            }
            else
            {

            }
            //NAV COUNT
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
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

                using (SqlConnection connection = new SqlConnection(connectionString))
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
                                customerInfo.ordermax3D = reader.GetString(14);
                                customerInfo.accstatus = reader.GetString(12);

                            }
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
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

                using (SqlConnection connection = new SqlConnection(connectionString))
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

                using (SqlConnection connection = new SqlConnection(connectionString))
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
                using (SqlConnection connection = new SqlConnection(connectionString))
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
            GetModels();
        }


    }
}