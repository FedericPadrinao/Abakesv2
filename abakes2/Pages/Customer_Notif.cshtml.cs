using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Customer_NotifModel : PageModel
    {
        public List<PrivateNotifInfo> listPrivateNotifInfo = new List<PrivateNotifInfo>();
        public List<NotificationInfo> listNotifications = new List<NotificationInfo>();
        public int NotificationCount { get; set; }
        public CustomerInfo customerInfo = new CustomerInfo();
        public bool IsRead { get; set; }
        public String userconfirm = "";
        public String errorMessage = "";
        public String statusconfirm = "";
        public String successMessage = "";
        public String imageconfrim = "";
        public String imgconfirm = "";
        public int cartCount = 0;
        public int cartCount3D = 0;
        public int totalcartCount = 0;
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
                    string sql = "SELECT * FROM Notification WHERE status = 'true' ORDER BY DateCreated DESC";
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
                                notificationInfo.DateCreated = reader.GetFieldValue<string>(reader.GetOrdinal("DateCreated"));

                                listNotifications.Add(notificationInfo);
                                NotificationCount = listNotifications.Count;


                            }
                            

                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM PrivateNotification WHERE status = 'true' AND username = @username ORDER BY DateCreated DESC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@username", userconfirm);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PrivateNotifInfo pni = new PrivateNotifInfo();
                                NotificationInfo notificationInfo = new NotificationInfo();
                                pni.IsRead = reader.GetFieldValue<bool>(reader.GetOrdinal("isRead"));

                                if (!pni.IsRead)
                                {
                                    NotificationCount++;
                                }

                                pni.NotifID = reader.GetFieldValue<int>(reader.GetOrdinal("NotificationID"));
                                pni.NotifName = reader.GetFieldValue<string>(reader.GetOrdinal("username"));
                                pni.NotifTitle = reader.GetFieldValue<string>(reader.GetOrdinal("NotificationTitle"));
                                pni.NotifText = reader.GetFieldValue<string>(reader.GetOrdinal("NotificationText"));
                                pni.NotifImg = reader.GetFieldValue<string>(reader.GetOrdinal("NotificationImage"));
                                pni.DateCreated = reader.GetFieldValue<string>(reader.GetOrdinal("DateCreated"));
                                listPrivateNotifInfo.Add(pni);
                            }
                            int privateNotifCount = listPrivateNotifInfo.Count(pni => !pni.IsRead);
                            NotificationCount = listNotifications.Count + privateNotifCount;
                            foreach (var notification in listNotifications.ToList())
                            {
                                if (IsNotificationRead(notification.NotifID, notification.NotifTitle, userconfirm))
                                {
                                    NotificationCount--;
                                }
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
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select count(OrderID) from Order3dForm where status = 'true' AND username = '" + userconfirm + "'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cartCount3D = reader.GetInt32(0);
                            }
                        }
                    }
                }
                totalcartCount = cartCount3D + cartCount;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

        }

        public IActionResult OnPostMarkAsRead(int notificationId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE PrivateNotification SET isRead = 1 WHERE NotificationID = @notificationId";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@notificationId", notificationId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            return RedirectToPage("/Customer_Notif");
        }

        public IActionResult OnPostMarkPubAsRead(int notificationId)
        {
            try
            {
                string currentUsername = HttpContext.Session.GetString("username");

                if (string.IsNullOrEmpty(currentUsername))
                {
                    return RedirectToPage("/Index");
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string selectSql = "SELECT NotificationTitle FROM Notification WHERE NotificationID = @notificationId";
                    using (SqlCommand selectCommand = new SqlCommand(selectSql, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@notificationId", notificationId);
                        string notificationTitle = selectCommand.ExecuteScalar()?.ToString();
                        string insertSql = "INSERT INTO ReadPublicNotif (username, NotificationID, NotificationTitle) VALUES (@username, @notificationId, @notificationTitle)";
                        using (SqlCommand insertCommand = new SqlCommand(insertSql, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@username", currentUsername);
                            insertCommand.Parameters.AddWithValue("@notificationId", notificationId);
                            insertCommand.Parameters.AddWithValue("@notificationTitle", notificationTitle);

                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            return RedirectToPage("/Customer_Notif");
        }


        public bool IsNotificationRead(int notificationId, string notificationTitle, string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM ReadPublicNotif WHERE Username = @username AND NotificationID = @notificationId AND NotificationTitle = @notificationTitle";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@notificationId", notificationId);
                    command.Parameters.AddWithValue("@notificationTitle", notificationTitle);

                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        public IActionResult OnPostMarkAllAsRead()
        {
            try
            {
                string currentUsername = HttpContext.Session.GetString("username");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string privateSql = "UPDATE PrivateNotification SET isRead = 1 WHERE username = @username AND status = 'true' AND isRead = 0";
                    using (SqlCommand privateCommand = new SqlCommand(privateSql, connection))
                    {
                        privateCommand.Parameters.AddWithValue("@username", currentUsername);
                        privateCommand.ExecuteNonQuery();
                    }
                    string publicSql = "INSERT INTO ReadPublicNotif (username, NotificationID, NotificationTitle) SELECT @username, NotificationID, NotificationTitle FROM Notification WHERE status = 'true' AND NOT EXISTS (SELECT 1 FROM ReadPublicNotif WHERE Username = @username AND NotificationID = Notification.NotificationID)";
                    using (SqlCommand publicCommand = new SqlCommand(publicSql, connection))
                    {
                        publicCommand.Parameters.AddWithValue("@username", currentUsername);
                        publicCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            return RedirectToPage("/Customer_Notif");
        }

    }
}

