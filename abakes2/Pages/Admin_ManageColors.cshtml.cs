using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Admin_ManageColorsModel : PageModel
    {
        public List<CakeColors> colorsList = new List<CakeColors>();
        public List<UserInfo> userInfo = new List<UserInfo>();
        public String userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public string connectionString = "Server=tcp:eu-az-sql-serv6e425a9865434acc8b7d6d8badb306b5.database.windows.net,1433;Initial Catalog=d58anpzl3kll8nf;Persist Security Info=False;User ID=ufy99xlgudx1fh3;Password=%g2q&#dV&ECBX6Oyf0%QkXHe5;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public void GetProducts(string sortProduct)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "select * from CakeColors"; //getting the data based from the pdid variable

                    string search = Request.Query["search"];
                    if (!String.IsNullOrEmpty(search))
                    {
                        sql = "SELECT * FROM CakeColors WHERE CakeColor LIKE '%" + search + "%'";
                    }

                    switch (sortProduct)
                    {
                        case "Sort Name":
                            sql += " ORDER BY CakeColor DESC";
                            break;
                        case "Sort Name2":
                            sql += " ORDER BY CakeColor ASC";
                            break;
                        case "Sort Status":
                            sql = "select * from CakeColors WHERE status = 'true'";
                            break;
                        case "Sort Status2":
                            sql = "select * from CakeColors WHERE status = 'false'";
                            break;
                    }
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CakeColors colors = new CakeColors();
                                colors.CakeID = reader.GetInt32(0);
                                colors.CakeColor = reader.GetString(1);
                                colors.status = reader.GetString(2);
                                colorsList.Add(colors);






                            }
                        }
                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading Colors: " + e.ToString());

            }
        }
        public IActionResult OnGetDelete()
        {
            string id = Request.Query["id"];
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "delete from CakeColors where ColorID='" + id + "'"; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                command.ExecuteNonQuery();

                            }
                        }




                    }
                }
            }
            catch (Exception err)
            {

            }

            return Redirect("/Admin_ManageColors");
        }

        public IActionResult OnGetArchive()
        {
            string id = Request.Query["id"];
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    String sql2 = "update CakeColors set status='true' where ColorID='" + id + "'";
                    using (SqlCommand command = new SqlCommand(sql2, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                command.ExecuteNonQuery();

                            }
                        }




                    }
                }
            }
            catch (Exception err)
            {

            }

            return Redirect("/Admin_ManageColors");
        }
        public IActionResult OnGetDisplay()
        {
            string id = Request.Query["id"];
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    String sql2 = "update CakeColors set status='false' where ColorID='" + id + "'";
                    using (SqlCommand command = new SqlCommand(sql2, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                command.ExecuteNonQuery();

                            }
                        }




                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("Exception: " + err.Message);
            }

            return Redirect("/Admin_ManageColors");
        }
        public void OnPostGenerate()
        {

            string colors = Request.Form["colors"];

            

            try
            {
               

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql2 = "INSERT INTO CakeColors " +
                                  "(CakeColor, status) VALUES " +
                                  "(@CakeColor,@status);";

                    using (SqlCommand command = new SqlCommand(sql2, connection))
                    {
                        command.Parameters.AddWithValue("@CakeColor", colors);
                        command.Parameters.AddWithValue("@status", "false");
                       

                        command.ExecuteNonQuery();

                    }

                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);

            }


            Response.Redirect("Admin_ManageColors");
        }
        public void OnGet(string sortProduct)
        {
            userconfirm = HttpContext.Session.GetString("useradmin");
            if (userconfirm != null)
            {

            }
            else
            {
                Response.Redirect("/index");
            }



            GetProducts(sortProduct);
        }
    }
}
