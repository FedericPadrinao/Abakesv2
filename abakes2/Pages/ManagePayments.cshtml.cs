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
        public string connectionString = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";

        public void GetUsers(string sortUser)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    //getting the data based from the pdid variable
                    string sql = "SELECT * FROM Invoice WHERE OrderStatus != @orderStatus";
                    string sql3d = "SELECT * FROM Order3DForm WHERE OrderStatus != 'Complete Order' and status != 'false'";
                    string search = Request.Query["search"];
                    if (!String.IsNullOrEmpty(search))
                    {
                        // Use parameterized query to avoid SQL injection
                        sql = "SELECT * FROM Invoice WHERE username LIKE @username AND OrderStatus != @orderStatus";
                        sql3d = "SELECT * FROM Order3DForm WHERE username LIKE @username AND OrderStatus != @orderStatus and status != 'false'";
                    }
                    else
                    {
                        // Use parameterized query to avoid SQL injection
                        sql = "SELECT * FROM Invoice WHERE OrderStatus != @orderStatus";
                        sql3d = "SELECT * FROM Order3DForm WHERE OrderStatus != 'Complete Order' and status != 'false'";
                    }

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@username", "%" + search + "%"); // Use parameterized values
                        command.Parameters.AddWithValue("@orderStatus", "Complete Order"); // Adjust as needed
                        command.Parameters.AddWithValue("@status", "false"); 

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
                                invoiceInfo.picture = reader.GetFieldValue<string>(reader.GetOrdinal("receipt2"));
                              
                                
                                
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
                                order3DInfo.picture = reader.GetFieldValue<string>(reader.GetOrdinal("picture"));
                                order3DList.Add(order3DInfo);
                            }
                        }
                    }

                }
                
                

            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading Payments: " + e.ToString());

            }
        }

        public void OnGet(string sortUser)
        {
            GetUsers(sortUser);
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
