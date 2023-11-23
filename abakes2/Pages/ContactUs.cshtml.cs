using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient; 
namespace abakes2.Pages
{
    public class ContactUsModel : PageModel
    {
        public String userconfirm = "";
        public String imgconfirm = "";
        public String statusconfirm = "";

        public int notifCount = 0;
        public int pnotifCount = 0;
        public int cartCount = 0;
        public int totalnotifCount = 0;
        public int NotificationCount { get; set; }
        public string connectionProvider = "Server=tcp:eu-az-sql-serv4c4db8f1d3864bb6a62cb90162f811bf.database.windows.net,1433;Initial Catalog=dh60fwg8dvwho6h;Persist Security Info=False;User ID=uw1aiy40iqpmqas;Password=U6W5bT1pW3O37J#DEihhdV%p@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public void OnPost()
        {

            string Name = Request.Form["fullname"];
            string Email = Request.Form["email"];
            string Phone = Request.Form["phone"];
            string Shape = Request.Form["cake-shape"];
            string Tier = Request.Form["cake-tier"];
            string Type = Request.Form["cake_type"];
            string Size = Request.Form["cake_size"];
            string Instruction = Request.Form["special_instructions"];
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "insert into OrderForm (Name, Email, Phone, Shapes, Tier, CakeFlavors, CakeSizes, CakeInstruction, status)" +
                        "VALUES(@Name, @Email, @Phone, @Shapes, @Tier, @CakeFlavors, @CakeSizes, @CakeInstruction, 'true');";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Name", Name);
                        command.Parameters.AddWithValue("@Email", Email);
                        command.Parameters.AddWithValue("@Phone", Phone);
                        command.Parameters.AddWithValue("@Shapes", Shape);
                        command.Parameters.AddWithValue("@Tier", Tier);
                        command.Parameters.AddWithValue("@CakeFlavors", Type);
                        command.Parameters.AddWithValue("@CakeSizes", Size);
                        command.Parameters.AddWithValue("@CakeInstruction", Instruction);

                        command.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void OnGet()
        {
            userconfirm = HttpContext.Session.GetString("username");
            imgconfirm = HttpContext.Session.GetString("userimage");
            statusconfirm = HttpContext.Session.GetString("userstatus");
            notifCount = HttpContext.Session.GetInt32("NotificationCount") ?? 0;
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
    }
}
