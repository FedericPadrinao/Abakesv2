using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Customer_ViewProductModel : PageModel
    {
        public List<UserInfo> listUser = new List<UserInfo>();
        public List<Products> listProduct = new List<Products>();
        public String userconfirm = "";
        public String imgconfirm = "";
        public String statusconfirm = "";
        public int notifCount = 0;
        public int pnotifCount = 0;
        public int cartCount = 0;
        public int totalnotifCount = 0;
        public String errorMessage = "";
        public String successMessage = "";

        public string connectionString = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";
        public int pdID = 0;
        public string pdName = "";
        public string pdCategory = "";
        public int pdPrice = 0;
        public string pdDescription = "";
        public string pdImg = "";

        public void GetProducts()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "select * from Product"; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Products pd = new Products();

                                pd.pdID = reader.GetFieldValue<int>(reader.GetOrdinal("ProductID"));
                                pd.pdName = reader.GetFieldValue<string>(reader.GetOrdinal("ProductName"));
                                pd.pdCategory = reader.GetFieldValue<string>(reader.GetOrdinal("ProductCategory"));
                                pd.pdPrice = reader.GetFieldValue<int>(reader.GetOrdinal("ProductPrice"));
                                pd.pdDescription = reader.GetFieldValue<string>(reader.GetOrdinal("ProductDesc"));
                                pd.pdImg = reader.GetFieldValue<string>(reader.GetOrdinal("ProductImg"));

                                listProduct.Add(pd);





                            }
                        }
                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading Products: " + e.ToString());

            }
        }

        public void OnGet()
        {
            
            userconfirm = HttpContext.Session.GetString("username");
            imgconfirm = HttpContext.Session.GetString("userimage");
            statusconfirm = HttpContext.Session.GetString("userstatus");
            notifCount = HttpContext.Session.GetInt32("NotificationCount") ?? 0;
            GetProducts();
            String id = Request.Query["id"];

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Product WHERE ProductID=@id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                pdID = reader.GetInt32(0);
                                pdName = reader.GetString(2);
                                pdCategory = reader.GetString(1);
                                pdPrice = reader.GetInt32(3);
                                pdDescription = reader.GetString(4);
                                pdImg = reader.GetString(5);

                            }

                        }
                    }


                }

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
                errorMessage = ex.Message;
            }
        }
    }
}