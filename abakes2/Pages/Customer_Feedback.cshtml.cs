using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class Customer_FeedbackModel : PageModel
    {
        public List<Feedbacks> listFeedback = new List<Feedbacks>();
        public List<UserInfo> userInfo = new List<UserInfo>();
        public int fbID = 0;
        public string connectionString = "Data Source=orange\\sqlexpress;Initial Catalog=Abakes;Integrated Security=True";
        public String userconfirm = "";
        public String imgconfirm = "";
        public String statusconfirm = "";
        public int notifCount = 0;
        public int pnotifCount = 0;
        public int cartCount = 0;
        public int totalnotifCount = 0;
        public string spageid;
        public double totalItems;
        public int pagecurrent;
        public int pageid;
        public int totals;
        public int NotificationCount { get; set; }
        public void GetFeedbacks(int start, int totals, string sortOrder)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "SELECT TOP(" + totals + ") * FROM feedback WHERE status='true' and id NOT IN (SELECT TOP(" + (start - 1) + ") id FROM feedback)";
                    string search = Request.Query["search"];
                    //string sql = "select * from feedback where status='true'"; //getting the data based from the fbid variable
                    if (!String.IsNullOrEmpty(search))
                    {
                        sql = "SELECT * FROM feedback WHERE name and status ='true' LIKE '%" + search + "%'";
                    }
                    switch (sortOrder)
                    {
                        case "Sort Name":
                            sql += "ORDER BY name DESC";
                            break;
                        case "Sort Name2":
                            sql += "ORDER BY name ASC";
                            break;
                        case "Sort Rating":
                            sql += "ORDER BY rating DESC";
                            break;
                        case "Sort Rating2":
                            sql += "ORDER BY rating ASC";
                            break;

                    }
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                Feedbacks fb = new Feedbacks();
                                fb.fbID = reader.GetFieldValue<int>(reader.GetOrdinal("id"));
                                fb.fbName = reader.GetFieldValue<string>(reader.GetOrdinal("name"));
                                fb.fbRating = reader.GetString(reader.GetOrdinal("rating"));
                                fb.fbMessage = reader.GetFieldValue<string>(reader.GetOrdinal("message"));
                                fb.fbImg = reader.GetFieldValue<string>(reader.GetOrdinal("image"));


                                listFeedback.Add(fb);





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
        public void OnGet(string sortOrder)
        {
            userconfirm = HttpContext.Session.GetString("username");
            imgconfirm = HttpContext.Session.GetString("userimage");
            statusconfirm = HttpContext.Session.GetString("userstatus");
            notifCount = HttpContext.Session.GetInt32("NotificationCount") ?? 0;

            spageid = Request.Query["page"];
            pageid = int.Parse(spageid);
            totals = 6;
            totalItems = 0.0;
            if (pageid == 1) { }

            else
            {
                pageid = pageid - 1;
                pageid = pageid * totals + 1;
            }


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "select * from Product WHERE status ='true'";
                    //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {


                                totalItems++;




                            }
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
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

                using (SqlConnection connection = new SqlConnection(connectionString))
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
                totalnotifCount = notifCount + pnotifCount;

            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading Products: " + e.ToString());

            }

            pagecurrent = int.Parse(spageid);

            int varId = 0;
            userconfirm = HttpContext.Session.GetString("user");
            GetFeedbacks(pageid, totals, sortOrder);
        }
    }
}