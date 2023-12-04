using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Customer_PNotifModel : PageModel
    {
        public List<NotificationInfo> listNotifications = new List<NotificationInfo>();
        public CustomerInfo customerInfo = new CustomerInfo();
        public List<PrivateNotifInfo> listPrivateNotifInfo = new List<PrivateNotifInfo>();
        public String userconfirm = "";
        public String errorMessage = "";
        public String imgconfirm = "";
        public String statusconfirm = "";
        public String successMessage = "";
        public String imageconfirm = "";
        public string connectionString = "Data Source=eu-az-sql-serv8c295e6a1afc4f69be52fd159aeb63da.database.windows.net;Initial Catalog=drt6diqvzxczvbi;User ID=uhsk2j20jhg6qgk;Password=***********";

        public void OnGet()
        {
            imgconfirm = HttpContext.Session.GetString("userimage");

            statusconfirm = HttpContext.Session.GetString("userstatus");
            userconfirm = HttpContext.Session.GetString("username");
            if (userconfirm != null)
            {


            }
            else
            {
                Response.Redirect("/Index");
            }
            try
            {
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
