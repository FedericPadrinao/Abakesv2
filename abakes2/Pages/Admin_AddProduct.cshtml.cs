using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Admin_AddProductModel : PageModel
    {
        public List<UserInfo> listUser = new List<UserInfo>();
        public List<Products> listProduct = new List<Products>();

        public string userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public string connectionString = "Server=tcp:eu-az-sql-serv8c295e6a1afc4f69be52fd159aeb63da.database.windows.net,1433;Initial Catalog=drt6diqvzxczvbi;Persist Security Info=False;User ID=uhsk2j20jhg6qgk;Password=3CZlMPeUY7D3yleRYezMeodZ2;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public int pdID = 0;
        public string pdName = "";
        public string pdCategory = "";
        public int pdPrice = 0;
        public string pdDescription = "";
        public string pdImg = "";


        [HttpPost]
        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            string pdname = Request.Form["name"];
           
            string price = Request.Form["price"];
            string desc = Request.Form["description"];

            if (file != null && file.Length > 0)
            {
                string fileName = Path.GetFileName(file.FileName);


                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "menu", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                int counter = GetProducts(pdname);

                if (counter == 0)
                {
                    try
                    {


                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            String sql2 = "INSERT INTO Product " +
                                          "(ProductCategory,ProductName,ProductPrice,ProductDesc,ProductImg,status) VALUES " +
                                          "('Cake',@ProductName,@ProductPrice,@ProductDesc,@ProductImg,'true');";

                            using (SqlCommand command = new SqlCommand(sql2, connection))
                            {
                            
                                command.Parameters.AddWithValue("@ProductName", pdname);
                                command.Parameters.AddWithValue("@ProductPrice", price);
                                command.Parameters.AddWithValue("@ProductDesc", desc);
                                command.Parameters.AddWithValue("@ProductImg", "/img/menu/" + fileName);



                                command.ExecuteNonQuery();

                            }

                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return Redirect("/Admin_ManageCakes");
                    }
                }
                else
                {
                    errorMessage = "This Product is Already Added!";
                    return Page();
                }


            }

            return Redirect("/Admin_ManageCakes");
        }




        public int GetProducts(string pdname)
        {
            int count = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "select * from Product where ProductName='" + pdname + "'"; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                count++;
                            }
                        }
                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading Products: " + e.ToString());

            }
            return count;
        }
        public void OnGet()
        {
            userconfirm = HttpContext.Session.GetString("user");
      
        }
    }
}