using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Admin_Manage3DOrdersModel : PageModel
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
        public string connectionString = "Server=tcp:eu-az-sql-serv4c4db8f1d3864bb6a62cb90162f811bf.database.windows.net,1433;Initial Catalog=dh60fwg8dvwho6h;Persist Security Info=False;User ID=uw1aiy40iqpmqas;Password=U6W5bT1pW3O37J#DEihhdV%p@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public void GetOrders()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "select * from Order3DForm"; //getting the data based from the odid variable
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
            string osid = Request.Query["id"]; //name from the front end "?id=

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "delete from Order3DForm where OrderID='" + osid + "' and username='" + userconfirm + "'"; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
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