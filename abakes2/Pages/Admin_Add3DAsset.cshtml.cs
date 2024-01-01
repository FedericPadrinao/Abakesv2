using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Admin_Add3DAssetModel : PageModel
    {
        public List<UserInfo> listUser = new List<UserInfo>();
        public List<Products> listProduct = new List<Products>();

        public string userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public string connectionString = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";
      

        [HttpPost]
        
        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            userconfirm = HttpContext.Session.GetString("useradmin");
            if (userconfirm != null)
            {

            }
            else
            {
                Response.Redirect("/index");
            }
            string successMessageProfile = "";
            string errorMessageProfile = "";
            string name = Request.Form["name"];

            try
            {
                // File format validation
                if (file != null && file.Length > 0)
                {
                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (fileExtension == ".json")
                    {
                        // Valid file format, proceed with upload
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            string fileName = Path.GetFileName(file.FileName);
                            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "models", fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            connection.Open();
                            string sql = "INSERT INTO Asset3D " +
                                          "(AssetName,AssetPath,AssetScale,PositionX,PositionY,PositionZ) VALUES " +
                                          "(@AssetName,@AssetPath,@AssetScale,@PositionX,@PositionY,@PositionZ);";
                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                command.Parameters.AddWithValue("@AssetName", name);
                                command.Parameters.AddWithValue("@AssetPath", "models/" + fileName);
                                command.Parameters.AddWithValue("@AssetScale", '1');
                                command.Parameters.AddWithValue("@PositionX", '0');
                                command.Parameters.AddWithValue("@PositionY", '0');
                                command.Parameters.AddWithValue("@PositionZ", '0');
                        
                                command.ExecuteNonQuery();
                            }



                        }
                    }
                    else
                    {
                        // Invalid file format, set error message
                        
                        TempData["InvalidJSONMessage"] = "Invalid file format. Please upload a .JSON file.";

                        return Redirect("/Admin_Add3DAsset");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
               
                return Redirect("/Admin_Manage3DAsset");
            }


       
            return Redirect("/Admin_Manage3DAsset");
        }

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

        }
    }
}