using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class ManagePaymentsModel : PageModel
    {
        public List<CustomerInfo> customerInfo = new List<CustomerInfo>();
        public List<InvoiceInfo> invoiceList = new List<InvoiceInfo>();
        public List<Order3DForm> order3DList = new List<Order3DForm>();
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
                    //getting the data based from the pdid variable
                    string sql = "SELECT * FROM Invoice WHERE OrderStatus != @orderStatus";
                    string sql3d = "SELECT * FROM Order3DForm WHERE OrderStatus != 'Complete Order'";
                    string search = Request.Query["search"];
                    if (!String.IsNullOrEmpty(search))
                    {
                        // Use parameterized query to avoid SQL injection
                        sql = "SELECT * FROM Invoice WHERE username LIKE @username AND OrderStatus != @orderStatus";
                        sql3d = "SELECT * FROM Order3DForm WHERE username LIKE @username AND OrderStatus != @orderStatus";
                    }
                    else
                    {
                        // Use parameterized query to avoid SQL injection
                        sql = "SELECT * FROM Invoice WHERE OrderStatus != @orderStatus";
                        sql3d = "SELECT * FROM Order3DForm WHERE OrderStatus != 'Complete Order'";
                    }

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@username", "%" + search + "%"); // Use parameterized values
                        command.Parameters.AddWithValue("@orderStatus", "Complete Order"); // Adjust as needed

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CustomerInfo customer = new CustomerInfo();

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

        public void OnGet(string sortUser)
        {
            GetUsers(sortUser);
        }


    }
}
