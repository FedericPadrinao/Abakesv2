using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
namespace abakes2.Pages
{
    public class ManageFeedbackModel : PageModel
    {
        public List<Feedbacks> listFeedback = new List<Feedbacks>();
        public List<UserInfo> userInfo = new List<UserInfo>();
        public string userconfirm = "";

        public string connectionString = "Data Source=eu-az-sql-serv8c295e6a1afc4f69be52fd159aeb63da.database.windows.net;Initial Catalog=drt6diqvzxczvbi;User ID=uhsk2j20jhg6qgk;Password=***********";

        public void GetFeedbacks(string sortFeedback)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "select * from feedback WHERE status='true'"; //getting the data based from the fbid variable
                    string search = Request.Query["search"];
                    if (!String.IsNullOrEmpty(search))
                    {
                        sql = "SELECT * FROM feedback WHERE status ='true' AND name LIKE '%" + search + "%'";
                    }

                    switch (sortFeedback)
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
                                fb.status = reader.GetFieldValue<string>(reader.GetOrdinal("status"));

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


        public IActionResult OnGetRemove()
        {
            string id = Request.Query["id"];
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    string sql = "delete from feedback where id='" + id + "'"; //getting the data based from the fbid variable
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

            return Redirect("/Admin_ManageFeedback");
        }
        public IActionResult OnGetHide()
        {
            string id = Request.Query["id"];
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) //static
                {
                    connection.Open();
                    String sql2 = "update feedback set status='false' where id='" + id + "'";
                    using (SqlCommand command = new SqlCommand(sql2, connection))
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
            catch (Exception err)
            {

            }

            return Redirect("/Admin_ManageFeedback");
        }
        public void OnGet(string sortFeedback)
        {

            userconfirm = HttpContext.Session.GetString("username");

            if (userconfirm != null)
            {

            }
            else
            {
                Response.Redirect("/index");
            }

            GetFeedbacks(sortFeedback);
        }
    }
}
