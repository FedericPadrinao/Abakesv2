using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class ManagePayments2Model : PageModel
    {
        public List<CustomerInfo> customerInfo = new List<CustomerInfo>();
        public List<InvoiceInfo> invoiceList = new List<InvoiceInfo>();
        public CustomerInfo customer = new CustomerInfo();
        public String userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public string connectionString = "Data Source=ROVIC\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";

        public void GetUsers(string sortUser)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    //getting the data based from the pdid variable
                    string sql = "SELECT * FROM Invoice WHERE OrderStatus = @orderStatus";
                    string search = Request.Query["search"];
                    if (!String.IsNullOrEmpty(search))
                    {
                        // Use parameterized query to avoid SQL injection
                        sql = "SELECT * FROM Invoice WHERE username LIKE @username AND OrderStatus = @orderStatus";
                    }
                    else
                    {
                        // Use parameterized query to avoid SQL injection
                        sql = "SELECT * FROM Invoice WHERE OrderStatus = @orderStatus";
                    }

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@username", "%" + search + "%"); // Use parameterized values
                        command.Parameters.AddWithValue("@orderStatus", "Complete Order"); // Adjust as needed

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                

                                InvoiceInfo invoiceInfo = new InvoiceInfo();
                                invoiceInfo.invoiceID = reader.GetInt32(0);
                                invoiceInfo.invoiceUsername = reader.GetFieldValue<string>(reader.GetOrdinal("username"));
                                invoiceInfo.orderStatus = reader.GetFieldValue<string>(reader.GetOrdinal("orderstatus"));
                                invoiceInfo.receipt = reader.GetFieldValue<string>(reader.GetOrdinal("receipt"));

                                invoiceList.Add(invoiceInfo);







                            }
                        }
                    }

                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string selectsql = "SELECT * FROM Invoice WHERE username = @username AND OrderStatus = 'Complete Order'";
                    using (SqlCommand command = new SqlCommand(selectsql, connection))
                    {
                        command.Parameters.AddWithValue("@username", userconfirm);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {


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
        public IActionResult OnGetComplete()
        {
            string user = Request.Query["user"];
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    String sql2 = "update LoginCustomer set ordermax='false' where username='" + user + "'";
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
                Console.WriteLine("Error Removing Accounts: " + err.ToString());
            }

            return Redirect("/ManagePayments2");
        }

        public IActionResult OnGetPause()
        {
            string user = Request.Query["user"];
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    String sql2 = "update LoginCustomer set ordermax='true' where username='" + user + "'";
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
                Console.WriteLine("Error Removing Accounts: " + err.ToString());
            }

            return Redirect("/ManagePayments2");
        }
        public void OnGet(string sortUser)
        {
            GetUsers(sortUser);
            using (SqlConnection connection = new SqlConnection(connectionString)) //static
            {
                connection.Open();
                string sql = "select * FROM LoginCustomer"; //getting the data based from the fbid variable
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customer.ordermax = reader.GetFieldValue<string>(reader.GetOrdinal("ordermax"));
                        }
                    }
                }
            }
        }


    }
}
