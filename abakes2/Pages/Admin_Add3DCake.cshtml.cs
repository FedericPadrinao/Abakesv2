using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Admin_Add3DCakeModel : PageModel
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
                            string sql = "INSERT INTO Order3D " +
                                          "(ModelName,Scale,Texture1,Texture2,Texture3,TotalTexture,TextureColor1,TextureColor2,TextureColor3,ModelType) VALUES " +
                                          "(@ModelName,@Scale,@Texture1,@Texture2,@Texture3,@TotalTexture,@TextureColor1,@TextureColor2,@TextureColor3,@ModelType);";
                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                command.Parameters.AddWithValue("@ModelName", "models/" + fileName);
                                command.Parameters.AddWithValue("@ModelType", name);
                                command.Parameters.AddWithValue("@Scale", '1');
                                command.Parameters.AddWithValue("@Texture1", "");
                                command.Parameters.AddWithValue("@Texture2", "");
                                command.Parameters.AddWithValue("@Texture3", "");
                                command.Parameters.AddWithValue("@TextureColor1", "");
                                command.Parameters.AddWithValue("@TextureColor2", "");
                                command.Parameters.AddWithValue("@TextureColor3", "");
                                command.Parameters.AddWithValue("@TotalTexture", '0');
                                
                                

                                command.ExecuteNonQuery();
                            }



                        }
                    }
                    else
                    {
                        // Invalid file format, set error message
                        TempData["InvalidJSON3DMessage"] = "Invalid file format. Please upload a .JSON file.";

                        
                        return Redirect("/Admin_Manage3DCake");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return Redirect("/Admin_Manage3DCake");
            }



            return Redirect("/Admin_Manage3DCake");
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