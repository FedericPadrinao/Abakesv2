using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Customer_NotifModel : PageModel
    {
        public List<PrivateNotifInfo> listPrivateNotifInfo = new List<PrivateNotifInfo>();
        public List<NotificationInfo> listNotifications = new List<NotificationInfo>();
        public int NotificationCount { get; set; } // Property to store notification count
        public CustomerInfo customerInfo = new CustomerInfo();
        

        

        public String userconfirm = "";
        public String errorMessage = "";
        public String statusconfirm = "";
        public String successMessage = "";
        public String imageconfrim = "";
        public String imgconfirm = "";
        public int cartCount = 0;
        public string connectionString = "Data Source=ROVIC\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";

        public void OnGet()
        {
            userconfirm = HttpContext.Session.GetString("username");
            imgconfirm = HttpContext.Session.GetString("userimage");
            statusconfirm = HttpContext.Session.GetString("userstatus");

            if (userconfirm != null)
            {


            }
            else
            {
                Response.Redirect("/Index");
            }
            //NOTIFICATION
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select * from Notification where status = 'true'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                NotificationInfo notificationInfo = new NotificationInfo();
                                notificationInfo.NotifID = reader.GetFieldValue<int>(reader.GetOrdinal("NotificationID"));
                                notificationInfo.NotifTitle = reader.GetFieldValue<string>(reader.GetOrdinal("NotificationTitle"));
                                notificationInfo.NotifText = reader.GetFieldValue<string>(reader.GetOrdinal("NotificationText"));
                                notificationInfo.NotifImg = reader.GetFieldValue<string>(reader.GetOrdinal("NotificationImage"));
                                listNotifications.Add(notificationInfo);
                                NotificationCount = listNotifications.Count;


                            }
                            

                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select * from PrivateNotification where status = 'true' AND username = '" + userconfirm + "'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PrivateNotifInfo pni = new PrivateNotifInfo();
                                NotificationInfo notificationInfo = new NotificationInfo();
                                pni.NotifID = reader.GetFieldValue<int>(reader.GetOrdinal("NotificationID"));
                                pni.NotifName = reader.GetFieldValue<string>(reader.GetOrdinal("username"));
                                pni.NotifTitle = reader.GetFieldValue<string>(reader.GetOrdinal("NotificationTitle"));
                                pni.NotifText = reader.GetFieldValue<string>(reader.GetOrdinal("NotificationText"));
                                pni.NotifImg = reader.GetFieldValue<string>(reader.GetOrdinal("NotificationImage"));
                                listPrivateNotifInfo.Add(pni);

                                NotificationCount = listNotifications.Count + listPrivateNotifInfo.Count;
                            }
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
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
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

        }

    }
}
