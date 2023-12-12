using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Admin_Manage3DCakeModel : PageModel
    {
        public List<Products> listProduct = new List<Products>();
        public List<UserInfo> userInfo = new List<UserInfo>();
        public List<Asset3DInfo> assetInfo = new List<Asset3DInfo>();
        public List<Order3DInfo> order3DInfo = new List<Order3DInfo>();
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
                    string sql = "select * from Order3D"; //getting the data based from the pdid variable

                    string search = Request.Query["search"];
                    if (!String.IsNullOrEmpty(search))
                    {
                        sql = "SELECT * FROM Order3D WHERE ModelType LIKE '%" + search + "%'";
                    }

                    switch (sortProduct)
                    {
                        case "Sort Name":
                            sql += "ORDER BY ModelType DESC";
                            break;
                        case "Sort Name2":
                            sql += "ORDER BY ModelType ASC";
                            break;


                    }
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Order3DInfo order3D = new Order3DInfo();
                                order3D.ModelID = reader.GetInt32(0);
                                order3D.ModelType = reader.GetString(10);
                                order3DInfo.Add(order3D);

                             







                            }
                        }
                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading 3D Models: " + e.ToString());

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
                    string sql = "delete from Order3D where Id='" + id + "'"; //getting the data based from the pdid variable
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

            return Redirect("/Admin_Manage3DCake");
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
