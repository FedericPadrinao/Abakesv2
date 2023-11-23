using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
namespace abakes2.Pages
{
    public class OrdFormPModel : PageModel
    {
        public CustomerInfo customerInfo = new CustomerInfo();
        public List<OrderSimpleInfo> orderSimpleInfo = new List<OrderSimpleInfo>();
        public String userconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public String imgconfirm = "";
        public String statusconfirm = "";
        public int notifCount = 0;
        public int pnotifCount = 0;
        public int cartCount = 0;
        public int totalnotifCount = 0;
        public int NotificationCount { get; set; }

        public string connectionProvider = "Server=tcp:eu-az-sql-serv4c4db8f1d3864bb6a62cb90162f811bf.database.windows.net,1433;Initial Catalog=dh60fwg8dvwho6h;Persist Security Info=False;User ID=uw1aiy40iqpmqas;Password=U6W5bT1pW3O37J#DEihhdV%p@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


        public void OnGet()
        {
            userconfirm = HttpContext.Session.GetString("username");
            imgconfirm = HttpContext.Session.GetString("userimage");
            statusconfirm = HttpContext.Session.GetString("userstatus");
            notifCount = HttpContext.Session.GetInt32("NotificationCount") ?? 0;
            if (userconfirm == null)
            {
                Response.Redirect("/Account");
            }
            else
            {

            }
            //NAV COUNT
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(NotificationID) from Notification where status = 'true'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                notifCount = reader.GetInt32(0);

                            }


                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(NotificationID) from PrivateNotification where status = 'true' AND username = '" + userconfirm + "'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                pnotifCount = reader.GetInt32(0);
                            }
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(OrderID) from OrderSimple where status = 'true' AND username = '" + userconfirm + "'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cartCount = reader.GetInt32(0);
                            }
                        }
                    }
                }
                totalnotifCount = notifCount + pnotifCount;

            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        public IActionResult OnPost()
        {
            OnGet();

            if (userconfirm == null)
            {
                return RedirectToPage("/Account");
            }
            else
            {
                string Occasion = Request.Form["occasion"];
                string Shape = Request.Form["shapes"];
                string Tier = Request.Form["tier"];
                string Flavors = Request.Form["flavors"];
                string Sizes = Request.Form["sizes"];
                string Instruction = Request.Form["special_instructions"];
                string Delivery = Request.Form["delivery"];
                string Preferred = Request.Form["preferred"];
                string Color = Request.Form["color"];
                string Dedication = Request.Form["dedication"];

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionProvider))
                    {
                        connection.Open();
                        string sql = "insert into OrderSimple (username, occasion, shapes, tier, flavors, sizes, instructions, delivery, status, OrderPrice, OrderQuantity, ShippingPrice, Downpayment, PreferredDelivery, ExpectedDelivery, ExpectedTime, color, dedication)" +
                            "VALUES(@username, @occasion, @shapes, @tier, @flavors, @sizes, @instructions, @delivery, 'false' , '0' , '1', '0', '0', @preferred, 'N/A', 'N/A', @color, @dedication);";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@username", userconfirm);
                            command.Parameters.AddWithValue("@occasion", Occasion);
                            command.Parameters.AddWithValue("@shapes", Shape);
                            command.Parameters.AddWithValue("@tier", Tier);
                            command.Parameters.AddWithValue("@flavors", Flavors);
                            command.Parameters.AddWithValue("@sizes", Sizes);
                            command.Parameters.AddWithValue("@instructions", Instruction);
                            command.Parameters.AddWithValue("@delivery", Delivery);
                            command.Parameters.AddWithValue("@preferred", Preferred);
                            command.Parameters.AddWithValue("@color", Color);
                            command.Parameters.AddWithValue("@dedication", Dedication);
                            command.ExecuteNonQuery();

                            // Redirect to the success page after successful form submission
                            return RedirectToPage("/Customer_OrderSuccess_Prompt");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    // Handle the exception if needed
                }
            }

            // If the form submission fails, stay on the current page
            return Page();
        }
    }
}