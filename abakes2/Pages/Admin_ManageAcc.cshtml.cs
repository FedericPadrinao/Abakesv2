using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
namespace abakes2.Pages
{
    public class ManageAccModel : PageModel
    {
        public List<CustomerInfo> customerInfo = new List<CustomerInfo>();
       
        public String userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public string connectionString = "Data Source=eu-az-sql-serv5434154f0e9a4d00a109437d48355b69.database.windows.net;Initial Catalog=d5rw6jsfzbuks4y;Persist Security Info=True;User ID=uqqncmi3rkbksbc;Password=***********";

        public void GetUsers(string sortUser)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "select * from LoginCustomer where accstatus ='true' "; //getting the data based from the pdid variable

                    string search = Request.Query["search"];
                    if (!String.IsNullOrEmpty(search))
                    {
                        sql = "SELECT * FROM LoginCustomer WHERE username LIKE '%" + search + "%'";
                    }

                    switch (sortUser)
                    {
                        case "Sort ID":
                            sql += "ORDER BY Id DESC";
                            break;
                        case "Sort ID2":
                            sql += "ORDER BY Id ASC";
                            break;
                        case "Sort Name":
                            sql += "ORDER BY username DESC";
                            break;
                        case "Sort Name2":
                            sql += "ORDER BY username ASC";
                            break;

                    }
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CustomerInfo customer = new CustomerInfo();
                                
                                customer.id = reader.GetFieldValue<int>(reader.GetOrdinal("Id"));
                                customer.username = reader.GetFieldValue<string>(reader.GetOrdinal("username"));
                                customer.lname = reader.GetFieldValue<string>(reader.GetOrdinal("lname"));
                                customer.fname = reader.GetFieldValue<string>(reader.GetOrdinal("fname"));
                                customer.email = reader.GetFieldValue<string>(reader.GetOrdinal("email"));
                                customer.address = reader.GetFieldValue<string>(reader.GetOrdinal("address"));
                                customer.phone = reader.GetFieldValue<string>(reader.GetOrdinal("phone"));
                                customer.img = reader.GetFieldValue<string>(reader.GetOrdinal("picture"));
                                customer.city = reader.GetFieldValue<string>(reader.GetOrdinal("city"));
                                customer.barangay = reader.GetFieldValue<string>(reader.GetOrdinal("barangay"));
                                customer.accstatus = reader.GetFieldValue<string>(reader.GetOrdinal("accstatus"));
                               
                                customerInfo.Add(customer);

                               





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

        public IActionResult OnGetRemove()
        {
            string id = Request.Query["id"];
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "delete from LoginCustomer where Id='" + id + "'"; //getting the data based from the fbid variable
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
            catch (Exception e)
            {
                Console.WriteLine("Error Removing Accounts: " + e.ToString());

            }

            return Redirect("/Admin_ManageAcc");
        }

        public IActionResult OnGetBlock()
        {
            string id = Request.Query["id"];
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    String sql2 = "update LoginCustomer set accstatus='false' where Id='" + id + "'";
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

            return Redirect("/Admin_ManageAcc");
        }

        public void OnGet(string sortUser)
        {
            GetUsers(sortUser);
        }
    }
}
