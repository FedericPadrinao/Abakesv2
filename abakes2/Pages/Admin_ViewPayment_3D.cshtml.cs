using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Admin_ViewPayment_3DModel : PageModel
    {
        public string successMessage = "";
        public string errorMessage = "";
        public string connectionProvider = "Data Source=ROVIC\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";
        public NotificationInfo notifInfo = new NotificationInfo();

        public UserInfo userInfo = new UserInfo();
        public OrderSimpleInfo orderSimple = new OrderSimpleInfo();
        public CustomerInfo customerInfo = new CustomerInfo();
        public InvoiceInfo invoiceInfo = new InvoiceInfo();

        public string userconfirm = "";

        public OrderSimpleInfo orderInfo = new OrderSimpleInfo();
        public Order3DForm order3D = new Order3DForm();
        public List<Asset3DForm> listAsset3D = new List<Asset3DForm>();

        public void OnGet()
        {
            String user = Request.Query["user"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Order3DForm WHERE username=@user ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user", userconfirm);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                order3D.ModelID = reader.GetInt32(0);
                                order3D.username = reader.GetString(1);
                                order3D.ModelName1 = reader.GetString(2);
                                order3D.Scale1 = reader.GetInt32(3);
                                order3D.Texture1 = reader.GetString(4);
                                order3D.Texture2 = reader.GetString(5);
                                order3D.Texture3 = reader.GetString(6);
                                order3D.Color = reader.GetString(7);
                                order3D.Color2 = reader.GetString(8);
                                order3D.Color3 = reader.GetString(9);
                                order3D.status = reader.GetString(11);
                                order3D.order3DPrice = reader.GetInt32(12);
                                order3D.order3DQuantity = reader.GetInt32(13);
                                order3D.order3DShip = reader.GetInt32(14);
                                order3D.order3DDP = reader.GetInt32(15);
                                order3D.order3DPreferredD = reader.GetString(16);
                                order3D.order3DExpectedD = reader.GetString(17);
                                order3D.order3DExpectedT = reader.GetString(18);
                                order3D.order3Dstatus = reader.GetString(21);
                            }
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Asset3DForm WHERE username=@user ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user", userconfirm);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Asset3DForm asset3D = new Asset3DForm();
                                asset3D.AssetID = reader.GetInt32(0);
                                asset3D.OrderID = reader.GetInt32(1);
                                asset3D.username = reader.GetString(2);
                                asset3D.AssetName = reader.GetString(3);
                                asset3D.AssetPath = reader.GetString(4);
                                asset3D.AssetScale = reader.GetInt32(5);
                                asset3D.PositionX = reader.GetString(6);
                                asset3D.PositionY = reader.GetString(7);
                                asset3D.PositionZ = reader.GetString(8);

                                listAsset3D.Add(asset3D);
                                Console.WriteLine("Assets" + asset3D.AssetPath);
                                Console.WriteLine("Assets" + asset3D.AssetName);
                            }
                        }
                    }
                }

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

                    // CUSTOMER
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
            Response.Redirect("/Admin_ViewPayment?user=" + user);
        }
    }
}
