using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Admin_ManageNotifModel : PageModel
    {
        public List<Feedbacks> listFeedback = new List<Feedbacks>();
        public List<NotificationInfo> listNotification = new List<NotificationInfo>();
        public List<UserInfo> userInfo = new List<UserInfo>();
        public string userconfirm = "";

        public string connectionString = "Server=tcp:eu-az-sql-serv6e425a9865434acc8b7d6d8badb306b5.database.windows.net,1433;Initial Catalog=d58anpzl3kll8nf;Persist Security Info=False;User ID=ufy99xlgudx1fh3;Password=%g2q&#dV&ECBX6Oyf0%QkXHe5;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public void GetNotifications()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();
                    string sql = "select * from Notification order by NotificationID desc"; 
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                NotificationInfo ni = new NotificationInfo();
                                Feedbacks fb = new Feedbacks();
                                ni.NotifID = reader.GetFieldValue<int>(reader.GetOrdinal("NotificationID"));
                                ni.NotifTitle = reader.GetFieldValue<string>(reader.GetOrdinal("NotificationTitle"));
                                ni.NotifText = reader.GetString(reader.GetOrdinal("NotificationText"));
                                ni.NotifImg = reader.GetString(reader.GetOrdinal("NotificationImage"));
                                ni.status = reader.GetString(reader.GetOrdinal("status"));

                                listNotification.Add(ni);





                            }
                        }
                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading Feedbacks: " + e.ToString());

            }
        }


        public IActionResult OnGetRemove()
        {
            string id = Request.Query["id"];
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "delete from Notification where NotificationID='" + id + "'"; //getting the data based from the fbid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                command.ExecuteNonQuery();






                            }
                        }
                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("Error Removing Feedbacks: " + e.ToString());

            }

            return Redirect("/Admin_ManageNotif");
        }
        public void OnGet()
        {

            userconfirm = HttpContext.Session.GetString("useradmin");
            if (userconfirm != null)
            {

            }
            else
            {
                Response.Redirect("/index");
            }
            GetNotifications();
        }
    }
}
