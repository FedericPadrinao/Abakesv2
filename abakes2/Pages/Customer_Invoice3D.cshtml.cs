using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Customer_Invoice3DModel : PageModel
    {
        public string FormattedDateTime { get; set; }
        public string userconfirm = "";
        public String statusconfirm = "";
        public string imgconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public int TotalCost = 0;
        public string connectionProvider = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";
        
        public Order3DForm order3d = new Order3DForm();
        public CustomerInfo customerInfo = new CustomerInfo();
        public void OnGet()
        {
            String id = Request.Query["Id"];

            userconfirm = HttpContext.Session.GetString("username");
            imgconfirm = HttpContext.Session.GetString("userimage");
            statusconfirm = HttpContext.Session.GetString("userstatus");
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Invoice WHERE InvoiceID=@id ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                order3d.ModelID = reader.GetInt32(0);

                                order3d.instructions = reader.GetString(10);
                                order3d.status = reader.GetString(11);
                                order3d.order3DPrice = reader.GetInt32(12);
                                order3d.order3DQuantity = reader.GetInt32(13);
                                order3d.order3DShip = reader.GetInt32(14);
                                order3d.order3DDP = reader.GetInt32(15);
                                order3d.order3DPreferredD = reader.GetString(16);
                                order3d.order3DExpectedD = reader.GetString(17);
                                order3d.order3DExpectedT = reader.GetString(18);
                                order3d.ModelType = reader.GetString(19);
                                order3d.order3DDateCreated = reader.GetString(20);
                                order3d.receipt = reader.GetString(22);
                                order3d.paymentMethod = reader.GetString(23);
                                order3d.order3DDelivery = reader.GetString(24);
                                TotalCost = order3d.order3DDP + order3d.order3DPrice + order3d.order3DShip;
                            }
                        }
                    }


                }

                //CUSTOMER
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM LoginCustomer WHERE username= '" + userconfirm + "'";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                customerInfo.username = reader.GetString(1);
                                customerInfo.lname = reader.GetString(2);
                                customerInfo.fname = reader.GetString(3);
                                customerInfo.email = reader.GetString(5);
                                customerInfo.address = reader.GetString(6);
                                customerInfo.phone = reader.GetString(7);
                                customerInfo.city = reader.GetString(9);
                                customerInfo.barangay = reader.GetString(10);


                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            DateTime now = DateTime.Now;

            // Format the date and time
            FormattedDateTime = now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
