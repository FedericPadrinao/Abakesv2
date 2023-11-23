using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class DisplayTestModel : PageModel
    {
        public List<Feedbacks> listFeedback = new List<Feedbacks>();
        public List<UserInfo> userInfo = new List<UserInfo>();
        public List<Order3DInfo> order3DList = new List<Order3DInfo>();
        public Order3DForm order3DForm = new Order3DForm();
        public List<Asset3DInfo> asset3DList = new List<Asset3DInfo>();
        public Asset3DForm asset3DForm = new Asset3DForm();
        public Boolean check = false;
        public String userconfirm = "";
        public String imgconfirm = "";
        public String statusconfirm = "";

        public int notifCount = 0;
        public int pnotifCount = 0;
        public int cartCount = 0;
        public int totalnotifCount = 0;
        public int NotificationCount { get; set; }
        public string connectionString = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";

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
                    string sql = "select * from Asset3D "; //getting the data based from the fbid variable
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

        public IActionResult OnPostInsertCake()
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

            string assetNameStr = Request.Form["assetname"];
            string assetPathStr = Request.Form["assetpath"];
            string assetScaleStr = Request.Form["assetscale"];
            string posX = Request.Form["positionx"];
            string posY = Request.Form["positiony"];
            string posZ = Request.Form["positionz"];

            string[] assetNameList = assetNameStr.Split(",");
            string[] assetPathList = assetPathStr.Split(",");
            string[] assetScaleList = assetScaleStr.Split(",");
            string[] posXList = posX.Split(",");
            string[] posYList = posY.Split(",");
            string[] posZList = posZ.Split(",");
            int insertedPrimaryKey = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql3 = "INSERT INTO Order3DForm " +
                         "(username, Tier1, Scale1, Texture1, Texture2, Texture3, Color1, Color2, Color3) " +
                         "VALUES " +
                         "(@username, @Tier1, @Scale1, @Texture1, @Texture2, @Texture3, @Color1, @Color2, @Color3); SELECT SCOPE_IDENTITY(); ";

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
            }
            catch (Exception ex)
            {
                //errorMessage = ex.Message;
                Console.WriteLine("Error: " + ex.ToString());
            }


            return RedirectToPage("/Index");
        }



        public void OnGet()
        {

            userconfirm = HttpContext.Session.GetString("username");
            imgconfirm = HttpContext.Session.GetString("userimage");
            statusconfirm = HttpContext.Session.GetString("userstatus");
            notifCount = HttpContext.Session.GetInt32("NotificationCount") ?? 0;
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
                    string sql = "select count(NotificationID) from PrivateNotification where status = 'true' AND username = '" + userconfirm + "'";
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
                totalnotifCount = notifCount + pnotifCount;

            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            GetModels();
        }


    }
}
