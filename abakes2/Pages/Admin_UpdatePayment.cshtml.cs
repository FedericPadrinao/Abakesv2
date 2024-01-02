using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Admin_UpdatePaymentModel : PageModel
    {
        public List<UserInfo> listUser = new List<UserInfo>();
        public List<Products> listProduct = new List<Products>();

        public string userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public string connectionString = "Data Source=ROVIC\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";
      


        [HttpPost]
        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
           

            if (file != null && file.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);


                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "menu", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
    
                    try
                    {


                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            String sql2 = "INSERT INTO ShoppingCart " +
                                          "(Payment) VALUES " +
                                          "(@ProductImg);";

                            using (SqlCommand command = new SqlCommand(sql2, connection))
                            {

                              
                                command.Parameters.AddWithValue("@ProductImg", "/img/menu/" + fileName);



                                command.ExecuteNonQuery();

                            }

                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return Redirect("/Admin_ManagePaymentMethod");
                    }
              
            

            }

            return Redirect("/Admin_ManagePaymentMethod");
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
