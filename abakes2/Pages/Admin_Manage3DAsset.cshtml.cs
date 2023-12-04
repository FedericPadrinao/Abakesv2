using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Admin_Manage3DAssetModel : PageModel
    {
        public List<Products> listProduct = new List<Products>();
        public List<UserInfo> userInfo = new List<UserInfo>();
        public List<Asset3DInfo> assetInfo = new List<Asset3DInfo>();
        public String userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public string connectionString = "Server=tcp:eu-az-sql-serv8c295e6a1afc4f69be52fd159aeb63da.database.windows.net,1433;Initial Catalog=drt6diqvzxczvbi;Persist Security Info=False;User ID=uhsk2j20jhg6qgk;Password=3CZlMPeUY7D3yleRYezMeodZ2;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public void GetProducts(string sortProduct)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "select * from Asset3D"; //getting the data based from the pdid variable

                    string search = Request.Query["search"];
                    if (!String.IsNullOrEmpty(search))
                    {
                        sql = "SELECT * FROM Asset3D WHERE AssetName LIKE '%" + search + "%'";
                    }

                    switch (sortProduct)
                    {
                        case "Sort Name":
                            sql += "ORDER BY AssetName DESC";
                            break;
                        case "Sort Name2":
                            sql += "ORDER BY AssetName ASC";
                            break;
                       

                    }
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Asset3DInfo asset = new Asset3DInfo();
                                asset.AssetID = reader.GetInt32(0);
                                asset.AssetName = reader.GetString(1);
                                asset.AssetPath = reader.GetString(2);
                              
                                assetInfo.Add(asset);

                          







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
        public IActionResult OnGetDelete()
        {
            string id = Request.Query["id"];
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "delete from Asset3D where AssetID='" + id + "'"; //getting the data based from the pdid variable
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

            return Redirect("/Admin_Manage3DAsset");
        }
        public void OnGet(string sortProduct)
        {
            userconfirm = HttpContext.Session.GetString("user");



            GetProducts(sortProduct);
        }
    }
}
