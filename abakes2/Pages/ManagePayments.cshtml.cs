using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class ManagePaymentsModel : PageModel
    {
        public List<CustomerInfo> customerInfo = new List<CustomerInfo>();
        public List<InvoiceInfo> invoiceList = new List<InvoiceInfo>();
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
                    string sql = "select * from Invoice where status ='true' "; //getting the data based from the pdid variable

                    string search = Request.Query["search"];
                    if (!String.IsNullOrEmpty(search))
                    {
                        sql = "SELECT * FROM Invoice WHERE username LIKE '%" + search + "%'";
                    }
                   
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
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

                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string selectsql = "SELECT * FROM Invoice WHERE username = @username";
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

        public void OnGet(string sortUser)
        {
            GetUsers(sortUser);
        }
    }
}
