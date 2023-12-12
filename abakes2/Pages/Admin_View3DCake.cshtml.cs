using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Admin_View3DCakeModel : PageModel
    {
        public UserInfo userInfo = new UserInfo();
       
        public Order3DInfo order3D = new Order3DInfo();
        public string userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public string connectionProvider = "Server=tcp:eu-az-sql-serv6e425a9865434acc8b7d6d8badb306b5.database.windows.net,1433;Initial Catalog=d58anpzl3kll8nf;Persist Security Info=False;User ID=ufy99xlgudx1fh3;Password=%g2q&#dV&ECBX6Oyf0%QkXHe5;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public void OnGet()
        {
            String id = Request.Query["Id"];
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
                    String sql = "SELECT * FROM Order3D WHERE Id=@id ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                               
                                order3D.ModelID = reader.GetInt32(0);
                                order3D.ModelName = reader.GetString(1);
                                order3D.Scale = reader.GetInt32(2);
                                order3D.TColor = reader.GetString(3);
                                order3D.TColor2 = reader.GetString(4);
                                order3D.TColor3 = reader.GetString(5);
                                order3D.TotalTexture = reader.GetInt32(6);
                                order3D.Color = reader.GetString(7);
                                order3D.Color = reader.GetString(8);
                                order3D.Color = reader.GetString(9);


                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

    }
}