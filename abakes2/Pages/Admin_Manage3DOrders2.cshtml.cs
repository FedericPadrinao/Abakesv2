using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Admin_Manage3DOrders2Model : PageModel
    {
        public CustomerInfo customerInfo = new CustomerInfo();
        public UserInfo userInfo = new UserInfo();
        public List<OrderSimpleInfo> orderSimpleInfo = new List<OrderSimpleInfo>();
        public List<Order3DForm> order3DInfo = new List<Order3DForm>();
        public string status { get; set; }
        public String userconfirm = "";
        public String errorMessage = "";
        public String statusconfirm = "";
        public String successMessage = "";
        public string connectionString = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";

        public void GetOrders()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "select * from Order3DForm WHERE status ='true'"; //getting the data based from the odid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OrderSimpleInfo os = new OrderSimpleInfo();
                                Order3DForm order3DList = new Order3DForm();

                                order3DList.ModelID = reader.GetFieldValue<int>(reader.GetOrdinal("OrderId"));
                                order3DList.username = reader.GetFieldValue<string>(reader.GetOrdinal("username"));
                                order3DList.ModelType = reader.GetFieldValue<string>(reader.GetOrdinal("ModelType"));
                                order3DList.instructions = reader.GetFieldValue<string>(reader.GetOrdinal("instructions"));
                                order3DList.order3DPrice = reader.GetFieldValue<int>(reader.GetOrdinal("OrderPrice"));
                                order3DList.order3DShip = reader.GetFieldValue<int>(reader.GetOrdinal("ShippingPrice"));
                                order3DList.order3DDP = reader.GetFieldValue<int>(reader.GetOrdinal("Downpayment"));
                                order3DList.order3DPreferredD = reader.GetFieldValue<string>(reader.GetOrdinal("PreferredDelivery"));
                                order3DList.order3DExpectedD = reader.GetFieldValue<string>(reader.GetOrdinal("ExpectedDelivery"));
                                order3DList.order3DExpectedT = reader.GetFieldValue<string>(reader.GetOrdinal("ExpectedTime"));
                                order3DList.order3DDelivery = reader.GetFieldValue<string>(reader.GetOrdinal("OrderDelivery"));
                                orderSimpleInfo.Add(os);
                                order3DInfo.Add(order3DList);

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
        public void OnGet()
        {
            GetOrders();
        }

        public IActionResult OnGetReject()
        {
            GetOrders();
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
                    string sql = "delete from Order3DForm WHERE username='" + user + "'"; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    String sql2 = "UPDATE LoginCustomer SET ordermax3D='false' WHERE username='" + user + "'";
                    using (SqlCommand command2 = new SqlCommand(sql2, connection))
                    {
                        command2.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception e)
            {

            }

            return Redirect("/Index");
        }

        public void OnPostPauseOrder()
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "UPDATE LoginCustomer SET status='false'";


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Admin_ManageOrders");
        }
        public void OnPostResumeOrder()
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "UPDATE LoginCustomer SET status='true'";


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Admin_ManageOrders");
        }
    }
}

