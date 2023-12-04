using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
namespace abakes2.Pages
{
    public class AdminDashboardModel : PageModel
    {
        public List<Products> listProduct = new List<Products>();
        public List<UserInfo> userInfo = new List<UserInfo>();
        public String userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public int feedbackcount = 0;
        public int usercount = 0;
        public int ordersimplecount = 0;
        public int simplepending = 0;
        public string connectionProvider = "Data Source=orange\\sqlexpress;Initial Catalog=Abakes;Integrated Security=True";
        public void OnGet()
        {
            userconfirm = HttpContext.Session.GetString("username");
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionProvider)) 
                {
                    connection.Open();
                    string sql = "select count(id) from feedback";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                              feedbackcount = reader.GetInt32(0);

                            }
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(Id) from LoginCustomer";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                usercount = reader.GetInt32(0);

                            }
                        }
                    }
                }
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(OrderID) from OrderSimple where status='true'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ordersimplecount = reader.GetInt32(0);

                            }
                        }
                    }
                }
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(OrderID) from OrderSimple where status='false'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                simplepending = reader.GetInt32(0);

                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading Products: " + e.Message);
            }
        }
        public JsonResult OnGetGetFlavorData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "SELECT flavors, COUNT(*) as count FROM Invoice WHERE orderstatus = 'Complete Order' GROUP BY flavors";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<int> data = new List<int>();
                            List<string> labels = new List<string>();

                            while (reader.Read())
                            {
                                labels.Add(reader.GetString(0));
                                data.Add(reader.GetInt32(1));
                            }

                            return new JsonResult(new { labels, data });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return new JsonResult(new { error = e.Message });
            }
        }
    }
}
