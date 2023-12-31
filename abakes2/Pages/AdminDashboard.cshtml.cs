using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
namespace abakes2.Pages
{
    public class AdminDashboardModel : PageModel
    {
        public List<Products> listProduct = new List<Products>();
        public List<UserInfo> userInfo = new List<UserInfo>();
        public List<MonthlySales> MonthlySalesData { get; set; }
        public string gender { get; set; }
        public String userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public int feedbackcount = 0;
        public int feedbackpendingcount = 0;
        public string formatrating = "";
        public double feedbackrating = 0;
        public int usercount = 0;
        public int blockedusercount = 0;
        public int ordersimplecount = 0;
        public int simplepending = 0;
        public int threepending = 0;
        public int threesimplecount = 0;
        public int threecompleteorder = 0;
        public int simplecompleteorder = 0;
        public int simplependingpayment = 0;
        public int threependingpayment = 0;
        public int totalpendingpayment = 0;
        public int earnings = 0;
        public int earnings3D = 0;
        public int totalearnings = 0;
        public int totalcompleteorder = 0;
        public string connectionProvider = "Data Source=ROVIC\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";
        public void OnGet()
        {
            MonthlySalesData = GetMonthlySalesData();
            userconfirm = HttpContext.Session.GetString("useradmin");
            if (userconfirm != null)
            {

            }
            else
            {
                Response.Redirect("/index");
            }
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(id) from feedback where status='true'";
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
                    string sql = "select count(id) from feedback where status='false'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                feedbackpendingcount = reader.GetInt32(0);

                            }
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(Id) from LoginCustomer where accstatus='true'";
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
                    string sql = "select count(Id) from LoginCustomer where accstatus='false'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                blockedusercount = reader.GetInt32(0);

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
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(OrderID) from Order3DForm where status='false'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                threepending = reader.GetInt32(0);

                            }
                        }
                    }
                }
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(OrderID) from Order3DForm where orderstatus='Complete Order'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                threecompleteorder = reader.GetInt32(0);

                            }
                        }
                    }
                }
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(InvoiceID) from Invoice where orderstatus='Complete Order'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                simplecompleteorder = reader.GetInt32(0);

                            }
                        }
                    }
                }
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(InvoiceID) from Invoice where orderstatus!='Complete Order'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                simplependingpayment = reader.GetInt32(0);

                            }
                        }
                    }
                }
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(OrderID) from Order3DForm where orderstatus!='Complete Order'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                threependingpayment = reader.GetInt32(0);

                            }
                        }
                    }
                }
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(OrderID) from Order3DForm where status!='false'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                threesimplecount = reader.GetInt32(0);

                            }
                        }
                    }
                }
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select sum(OrderPrice) from Invoice where orderstatus='Complete Order'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                earnings = reader.GetInt32(0);

                            }
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select sum(OrderPrice) from Order3DForm where orderstatus='Complete Order'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                earnings3D = reader.GetInt32(0);

                            }
                        }
                    }
                }
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select AVG(CAST(rating AS FLOAT)) AS avgrating from feedback where status='true'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                feedbackrating = reader.GetDouble(0);
                                formatrating = feedbackrating.ToString("F2");

                            }
                        }
                    }
                }
                totalearnings = earnings + earnings3D;
                totalcompleteorder = threecompleteorder + simplecompleteorder;
                totalpendingpayment = threependingpayment + simplependingpayment;

            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading Dashboard: " + e.Message);
            }
        }
        public JsonResult OnGetGetFlavorData(int? selectedMonth)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "SELECT flavors, COUNT(*) as count FROM Invoice WHERE orderstatus = 'Complete Order'";

                    if (selectedMonth.HasValue && selectedMonth != 0)
                    {
                        sql += $" AND MONTH(ExpectedDelivery) = {selectedMonth}";
                    }

                    sql += " GROUP BY flavors";

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

        public JsonResult OnGetGetShapeData(int? selectedMonth)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "SELECT shapes, COUNT(*) as count FROM Invoice WHERE orderstatus = 'Complete Order'";

                    if (selectedMonth.HasValue && selectedMonth != 0)
                    {
                        sql += $" AND MONTH(ExpectedDelivery) = {selectedMonth}";
                    }

                    sql += " GROUP BY shapes";

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

        public JsonResult OnGetGetOccasionData(int? selectedMonth)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "SELECT occasion, COUNT(*) as count FROM Invoice WHERE orderstatus = 'Complete Order'";

                    if (selectedMonth.HasValue && selectedMonth != 0)
                    {
                        sql += $" AND MONTH(ExpectedDelivery) = {selectedMonth}";
                    }

                    sql += " GROUP BY occasion";

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

        public JsonResult OnGetGetGenderData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "SELECT gender, COUNT(*) as gender FROM LoginCustomer WHERE is_verified = 1 GROUP BY gender";

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

        public JsonResult OnGetGetAgeData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();

                    string sql = "SELECT DATEDIFF(YEAR, birthdate, GETDATE()) AS age, COUNT(*) AS count " +
                                 "FROM LoginCustomer " +
                                 "WHERE is_verified = 1 " +
                                 "GROUP BY DATEDIFF(YEAR, birthdate, GETDATE())";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<int> data = new List<int>();
                            List<string> labels = new List<string>();

                            while (reader.Read())
                            {
                                int age = reader.GetInt32(0);
                                int ageGroup = (int)Math.Floor((double)age / 5) * 5;
                                labels.Add($"{ageGroup}-{ageGroup + 4}");
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
        private List<MonthlySales> GetMonthlySalesData()
        {
            List<MonthlySales> monthlySalesData = new List<MonthlySales>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();

                    for (int month = 1; month <= 12; month++)
                    {
                        string sql = $"SELECT ISNULL(SUM(OrderPrice), 0) AS MonthlySales " +
                                     $"FROM (" +
                                     $"    SELECT OrderPrice, MONTH(ExpectedDelivery) AS SaleMonth " +
                                     $"    FROM Invoice " +
                                     $"    WHERE orderstatus = 'Complete Order' AND MONTH(ExpectedDelivery) = {month} " +
                                     $"    UNION ALL " +
                                     $"    SELECT OrderPrice, MONTH(ExpectedDelivery) AS SaleMonth " +
                                     $"    FROM Order3DForm " +
                                     $"    WHERE orderstatus = 'Complete Order' AND MONTH(ExpectedDelivery) = {month} " +
                                     $") AS MonthlyData";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    monthlySalesData.Add(new MonthlySales
                                    {
                                        Month = GetMonthName(month),
                                        Sales = reader.GetInt32(0)
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error retrieving monthly sales data: " + e.Message);
            }

            return monthlySalesData;
        }

        private string GetMonthName(int month)
        {
            return new DateTime(2023, month, 1).ToString("MMMM");
        }

        public class MonthlySales
        {
            public string Month { get; set; }
            public int Sales { get; set; }
        }
    }
}
