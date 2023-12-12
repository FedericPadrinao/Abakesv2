using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Admin_EditProductModel : PageModel
    {
        public List<UserInfo> listUser = new List<UserInfo>();
        public List<Products> listProduct = new List<Products>();
        public Products productInfo = new Products();
        public string userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public string connectionProvider = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";
        public string connectionString = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";
        public int pdID = 0;
        public string pdName = "";
        public string pdCategory = "";
        public int pdPrice = 0;
        public string pdDescription = "";
        public string pdImg = "";


        [HttpPost]
        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            OnGet();
            string pdname = Request.Form["name"];

            string price = Request.Form["price"];
            string desc = Request.Form["description"];
            string id = Request.Query["id"];
            if (file != null && file.Length > 0)
            {
                string fileName = Path.GetFileName(file.FileName);


                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "menu", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                

               
                    try
                    {


                        using (SqlConnection connection = new SqlConnection(connectionProvider))
                        {
                            connection.Open();
                            String sql2 = "UPDATE Product " +
                               "SET " +
                               "ProductCategory = 'Cake', " +
                               "ProductName = @ProductName, " +
                               "ProductPrice = @ProductPrice, " +
                               "ProductDesc = @ProductDesc, " +
                               "ProductImg = @ProductImg, " +
                               "status = 'true' " +
                               "WHERE " +
                               "ProductID = @ProductID;";

                           
                            using (SqlCommand command = new SqlCommand(sql2, connection))
                            {
                                command.Parameters.AddWithValue("@ProductID", id);
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
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionProvider))
                    {
                        connection.Open();
                        String sql2 = "UPDATE Product " +
                           "SET " +
                           "ProductCategory = 'Cake', " +
                           "ProductName = @ProductName, " +
                           "ProductPrice = @ProductPrice, " +
                           "ProductDesc = @ProductDesc, " +
                           "status = 'true' " +
                           "WHERE " +
                           "ProductID = @ProductID;";

                        using (SqlCommand command = new SqlCommand(sql2, connection))
                        {
                            command.Parameters.AddWithValue("@ProductID", id);
                            command.Parameters.AddWithValue("@ProductName", pdname);
                            command.Parameters.AddWithValue("@ProductPrice", price);
                            command.Parameters.AddWithValue("@ProductDesc", desc);

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



            return Redirect("/Admin_ManageCakes");
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
            string pdid = Request.Query["id"];
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "select * from Product where ProductID='" + pdid + "'"; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                productInfo.pdID = reader.GetInt32(0);
                                productInfo.pdName = reader.GetString(2);
                                productInfo.pdPrice = reader.GetInt32(3);
                                productInfo.pdDescription = reader.GetString(4);
                                productInfo.pdImg = reader.GetString(5);
                            }
                        }
                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("Error Editing Products: " + e.ToString());

            }
           
        }
    }
}