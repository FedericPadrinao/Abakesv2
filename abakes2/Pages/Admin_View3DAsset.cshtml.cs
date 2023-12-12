using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Admin_View3DAssetModel : PageModel
    {
        public UserInfo userInfo = new UserInfo();
        public Asset3DInfo asset3D = new Asset3DInfo();
      
        public string userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public string connectionProvider = "Server=tcp:eu-az-sql-serv6e425a9865434acc8b7d6d8badb306b5.database.windows.net,1433;Initial Catalog=d58anpzl3kll8nf;Persist Security Info=False;User ID=ufy99xlgudx1fh3;Password=%g2q&#dV&ECBX6Oyf0%QkXHe5;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public void OnGet()
        {
            userconfirm = HttpContext.Session.GetString("useradmin");
            if (userconfirm != null)
            {

            }
            else
            {
                Response.Redirect("/index");
            }
            String id = Request.Query["Id"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Asset3D WHERE AssetID=@id ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                
                                asset3D.AssetID = reader.GetInt32(0);
                                asset3D.AssetName = reader.GetString(1);
                                asset3D.AssetPath = reader.GetString(2);
                                asset3D.AssetScale = reader.GetInt32(3);
                                asset3D.PositionX = reader.GetString(4);
                                asset3D.PositionY = reader.GetString(5);
                                asset3D.PositionZ = reader.GetString(6);

                                
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
