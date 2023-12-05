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
        public List<Order3DForm> order3DList = new List<Order3DForm>();
        public String userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public string connectionString = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";

        public void GetUsers(string sortUser)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    //getting the data based from the pdid variable
                    string sql = "SELECT * FROM Invoice WHERE OrderStatus = @orderStatus";
                    string sql3d = "SELECT * FROM Order3DForm WHERE OrderStatus = @orderStatus";
                    string search = Request.Query["search"];
                    if (!String.IsNullOrEmpty(search))
                    {
                        // Use parameterized query to avoid SQL injection
                        sql = "SELECT * FROM Invoice WHERE username LIKE @username AND OrderStatus = @orderStatus";
                        sql3d = "SELECT * FROM Order3DForm WHERE OrderStatus = @orderStatus";
                    }
                    else
                    {
                        // Use parameterized query to avoid SQL injection
                        sql = "SELECT * FROM Invoice WHERE OrderStatus = @orderStatus";
                        sql3d = "SELECT * FROM Order3DForm WHERE OrderStatus = @orderStatus";
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
                    using (SqlCommand command = new SqlCommand(sql3d, connection))
                    {
                        command.Parameters.AddWithValue("@username", "%" + search + "%"); // Use parameterized values
                        command.Parameters.AddWithValue("@orderStatus", "Complete Order"); // Adjust as needed

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CustomerInfo customer = new CustomerInfo();
                                Order3DForm order3DInfo = new Order3DForm();
                                order3DInfo.ModelID = reader.GetInt32(0);
                                order3DInfo.username = reader.GetString(1);

                                order3DInfo.order3Dstatus = reader.GetString(21);
                                order3DInfo.receipt = reader.GetFieldValue<string>(reader.GetOrdinal("receipt"));
                                order3DList.Add(order3DInfo);
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

        public IActionResult OnGetFinish()
        {
            string user = Request.Query["user"];
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    String sql2 = "update LoginCustomer set ordermax3D='false' where username='" + user + "'";
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

        public IActionResult OnGetStop()
        {
            string user = Request.Query["user"];
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    String sql2 = "update LoginCustomer set ordermax3D='true' where username='" + user + "'";
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
                            customer.ordermax3D = reader.GetFieldValue<string>(reader.GetOrdinal("ordermax3D"));
                        }
                    }
                }
            }
        }


    }
}

