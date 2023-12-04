using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Admin_ViewPaymentModel : PageModel
    {
        public string successMessage = "";
        public string errorMessage = "";
        public string connectionProvider = "Server=tcp:eu-az-sql-serv8c295e6a1afc4f69be52fd159aeb63da.database.windows.net,1433;Initial Catalog=drt6diqvzxczvbi;Persist Security Info=False;User ID=uhsk2j20jhg6qgk;Password=3CZlMPeUY7D3yleRYezMeodZ2;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public NotificationInfo notifInfo = new NotificationInfo();
        public UserInfo userInfo = new UserInfo();
        public OrderSimpleInfo orderSimple = new OrderSimpleInfo();
        public CustomerInfo customerInfo = new CustomerInfo();
        public InvoiceInfo invoiceInfo = new InvoiceInfo();

        public string userconfirm = "";


        public void OnGet()
        {
            String user = Request.Query["user"];

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Invoice WHERE username=@user";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user", user);
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

                            }
                        }
                    }


                }

                //CUSTOMER
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM LoginCustomer WHERE username= '" + user + "'";

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
        }  
        public void OnPost()
        {
            string orderstatus = Request.Form["orderstatus"];
            String user = Request.Query["user"];
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "UPDATE Invoice SET orderstatus='" + orderstatus + "' WHERE username='" + user + "'";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
               
            }
            Response.Redirect("/Admin_ViewPayment?user=" +user);
        }
    }
}

    