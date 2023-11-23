using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
namespace abakes2.Pages
{
    public class ManageCakesModel : PageModel
    {
        public List<Products> listProduct = new List<Products>();
        public List<UserInfo> userInfo = new List<UserInfo>();
        public String userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public string connectionString = "Server=tcp:eu-az-sql-serv4c4db8f1d3864bb6a62cb90162f811bf.database.windows.net,1433;Initial Catalog=dh60fwg8dvwho6h;Persist Security Info=False;User ID=uw1aiy40iqpmqas;Password=U6W5bT1pW3O37J#DEihhdV%p@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public void GetProducts(string sortProduct)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "select * from Product WHERE status='true'"; //getting the data based from the pdid variable

                    string search = Request.Query["search"];
                    if (!String.IsNullOrEmpty(search))
                    {
                        sql = "SELECT * FROM Product WHERE status ='true' AND ProductName LIKE '%" + search + "%'";
                    }

                    switch (sortProduct)
                    {
                        case "Sort Name":
                            sql += "ORDER BY ProductName DESC";
                            break;
                        case "Sort Name2":
                            sql += "ORDER BY ProductName ASC";
                            break;
                        case "Sort Price":
                            sql += "ORDER BY ProductPrice DESC";
                            break;
                        case "Sort Price2":
                            sql += "ORDER BY ProductPrice ASC";
                            break;

                    }
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Products pd = new Products();

                                pd.pdID = reader.GetFieldValue<int>(reader.GetOrdinal("ProductID"));
                                pd.pdName = reader.GetFieldValue<string>(reader.GetOrdinal("ProductName"));
                                pd.pdCategory = reader.GetFieldValue<string>(reader.GetOrdinal("ProductCategory"));
                                pd.pdPrice = reader.GetFieldValue<int>(reader.GetOrdinal("ProductPrice"));
                                pd.pdDescription = reader.GetFieldValue<string>(reader.GetOrdinal("ProductDesc"));
                                pd.pdImg = reader.GetFieldValue<string>(reader.GetOrdinal("ProductImg"));

                                listProduct.Add(pd);







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
                    string sql = "delete from Product where ProductID='" + id + "'"; //getting the data based from the pdid variable
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

            return Redirect("/ProductList");
        }

        public IActionResult OnGetArchive()
        {
            string id = Request.Query["id"];
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    String sql2 = "update Product set status='false' where ProductID='" + id + "'";
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

            return Redirect("/Admin_ManageCakes");
        }
        public void OnGet(string sortProduct)
        {
            userconfirm = HttpContext.Session.GetString("user");



            GetProducts(sortProduct);
        }
    }
}