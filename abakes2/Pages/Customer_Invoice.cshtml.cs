using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Customer_InvoiceModel : PageModel
    {
        public string userconfirm = "";
        public String statusconfirm = "";
        public string imgconfirm = "";
        public String errorMessage = "";
        public String successMessage = "";
        public int TotalCost = 0;
        public int TotalNetCost = 0;
        public string connectionProvider = "Server=tcp:eu-az-sql-serv6e425a9865434acc8b7d6d8badb306b5.database.windows.net,1433;Initial Catalog=d58anpzl3kll8nf;Persist Security Info=False;User ID=ufy99xlgudx1fh3;Password=%g2q&#dV&ECBX6Oyf0%QkXHe5;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public string FormattedDateTime { get; set; }
        public InvoiceInfo invoiceInfo = new InvoiceInfo();
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
                                invoiceInfo.invoiceID = reader.GetInt32(0);
                                invoiceInfo.invoiceOccasion = reader.GetString(2);
                                invoiceInfo.invoiceShapes = reader.GetString(3);
                                invoiceInfo.invoiceTier = reader.GetString(4);
                                invoiceInfo.invoiceFlavors = reader.GetString(5);
                                invoiceInfo.invoiceSizes = reader.GetString(6);
                                invoiceInfo.invoiceInstruction = reader.GetString(7);
                                invoiceInfo.invoiceDelivery = reader.GetString(8);
                                invoiceInfo.status = reader.GetString(9);
                                invoiceInfo.invoicePrice = reader.GetInt32(10);
                                invoiceInfo.invoiceQuantity = reader.GetInt32(11);
                                invoiceInfo.invoiceShip = reader.GetInt32(12);
                                invoiceInfo.invoiceDP = reader.GetInt32(13);
                                invoiceInfo.invoicePreferredD = reader.GetString(14);
                                invoiceInfo.invoiceExpectedD = reader.GetString(15);
                                invoiceInfo.invoiceExpectedT = reader.GetString(16);
                                invoiceInfo.invoiceColor = reader.GetString(17);
                                invoiceInfo.invoiceDedication = reader.GetString(18);
                                invoiceInfo.invoiceDateCreated = reader.GetString(19);
                                invoiceInfo.orderStatus = reader.GetString(20);
                                invoiceInfo.receipt = reader.GetString(21);
                                invoiceInfo.paymentMethod = reader.GetString(22);
                                invoiceInfo.CouponCode = reader.GetString(23);
                                invoiceInfo.NetInvoicePrice = reader.GetInt32(24);
                                TotalNetCost = invoiceInfo.NetInvoicePrice + invoiceInfo.invoiceDP + invoiceInfo.invoiceShip;
                                TotalCost = invoiceInfo.invoiceDP + invoiceInfo.invoicePrice + invoiceInfo.invoiceShip;
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
            // Get the current date and time
            DateTime now = DateTime.Now;

            // Format the date and time
            FormattedDateTime = now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}