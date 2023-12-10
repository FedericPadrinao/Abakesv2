using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Admin_ManageSimpleOrders2Model : PageModel
    {
        public CustomerInfo customerInfo = new CustomerInfo();
        public UserInfo userInfo = new UserInfo();
        public List<OrderSimpleInfo> orderSimpleInfo = new List<OrderSimpleInfo>();
        public String userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public string connectionString = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";

        public void GetOrders(string sortOrder)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "select * from OrderSimple WHERE status ='true' "; //getting the data based from the odid variable
                    string search = Request.Query["search"];
                    if (!String.IsNullOrEmpty(search))
                    {
                        sql = "SELECT * FROM OrderSimple WHERE status ='true' AND username LIKE '%" + search + "%' ";
                    }

                    switch (sortOrder)
                    {
                        case "Sort Name":
                            sql += "ORDER BY username DESC";
                            break;
                        case "Sort Name2":
                            sql += "ORDER BY usernameame ASC";
                            break;
                        case "Sort Flavor":
                            sql += "ORDER BY flavors DESC";
                            break;
                        case "Sort Flavor2":
                            sql += "ORDER BY flavors ASC";
                            break;

                    }
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OrderSimpleInfo os = new OrderSimpleInfo();


                                os.osID = reader.GetFieldValue<int>(reader.GetOrdinal("OrderID"));
                                os.osUsername = reader.GetFieldValue<string>(reader.GetOrdinal("username"));
                                os.osOccasion = reader.GetFieldValue<string>(reader.GetOrdinal("occasion"));
                                os.osShapes = reader.GetFieldValue<string>(reader.GetOrdinal("shapes"));
                                os.osTier = reader.GetFieldValue<string>(reader.GetOrdinal("tier"));
                                os.osFlavors = reader.GetFieldValue<string>(reader.GetOrdinal("flavors"));
                                os.osSizes = reader.GetFieldValue<string>(reader.GetOrdinal("sizes"));
                                os.osInstruction = reader.GetFieldValue<string>(reader.GetOrdinal("instructions"));
                                os.osDelivery = reader.GetFieldValue<string>(reader.GetOrdinal("delivery"));
                                os.status = reader.GetFieldValue<string>(reader.GetOrdinal("status"));
                                os.osPrice = reader.GetFieldValue<int>(reader.GetOrdinal("OrderPrice"));
                                os.osShip = reader.GetFieldValue<int>(reader.GetOrdinal("ShippingPrice"));
                                os.osDP = reader.GetFieldValue<int>(reader.GetOrdinal("Downpayment"));
                                os.osPreferredD = reader.GetFieldValue<string>(reader.GetOrdinal("PreferredDelivery"));
                                os.osExpectedD = reader.GetFieldValue<string>(reader.GetOrdinal("ExpectedDelivery"));
                                os.osExpectedT = reader.GetFieldValue<string>(reader.GetOrdinal("ExpectedTime"));
                                os.osColor = reader.GetFieldValue<string>(reader.GetOrdinal("color"));
                                os.osDedication = reader.GetFieldValue<string>(reader.GetOrdinal("dedication"));
                                orderSimpleInfo.Add(os);

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
        public void OnGet(string sortOrder)
        {
            GetOrders(sortOrder);
        }

        public IActionResult OnGetReject(string sortOrder)
        {
            GetOrders(sortOrder);
            if (userconfirm == null)
            { //If not logged in, it will be redirected to login page
                return RedirectToPage("/Account");
            }
            else
            {

            }
            String user = Request.Query["user"];
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "delete from OrderSimple WHERE username='" + user + "'"; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    String sql2 = "UPDATE LoginCustomer SET ordermax='false' WHERE username='" + user + "'";
                    using (SqlCommand command2 = new SqlCommand(sql2, connection))
                    {
                        command2.ExecuteNonQuery();
                    }
                }
               

            }
            catch (Exception e)
            {

            }

            return Page();
        }
       
    }
}
