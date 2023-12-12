using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
namespace abakes2.Pages
{
    public class Customer_AddFeedbackModel : PageModel
    {
        public string userconfirm = "";
        public string connectionProvider = "Server=tcp:eu-az-sql-serv6e425a9865434acc8b7d6d8badb306b5.database.windows.net,1433;Initial Catalog=d58anpzl3kll8nf;Persist Security Info=False;User ID=ufy99xlgudx1fh3;Password=%g2q&#dV&ECBX6Oyf0%QkXHe5;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public string errorMessage = "";
        public String imgconfirm = "";
        public String statusconfirm = "";
       
      
       

        public int notifCount = 0;
        public int pnotifCount = 0;
        public int pubnotifCount = 0;
        public int cartCount = 0;
        public int cartCount3D = 0;
        public int totalcartCount = 0;
        public int totalnotifCount = 0;
        public int NotificationCount { get; set; }


        public string code = "";
        public void OnGet()
        {
            userconfirm = HttpContext.Session.GetString("username");
            imgconfirm = HttpContext.Session.GetString("userimage");
            statusconfirm = HttpContext.Session.GetString("userstatus");
            notifCount = HttpContext.Session.GetInt32("NotificationCount") ?? 0;

            
            if (userconfirm == null)
            {
                HttpContext.Session.SetString("ReturnUrl", Request.Path);
                Response.Redirect("/Account");

            }
            else
            {

            }
            //NAV COUNT
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
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

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(NotificationID) from PrivateNotification where status = 'true' AND isRead = 'false'  AND username = '" + userconfirm + "'";
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

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select count(NotificationID) from ReadPublicNotif where username = '" + userconfirm + "'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                pubnotifCount = reader.GetInt32(0);
                            }
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionProvider))
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
                using (SqlConnection connection = new SqlConnection(connectionProvider))
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
                totalnotifCount = notifCount + pnotifCount - pubnotifCount;

            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }


        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {

            userconfirm = HttpContext.Session.GetString("username");

            string star = Request.Form["rating"];
            string secCode = Request.Form["securitycode"];
            string message = Request.Form["textmessage"];
            int checkSec = 0;
            try
            {
                //image upload
                if (file != null && file.Length > 0)
                {

                    using (SqlConnection connection = new SqlConnection(connectionProvider))
                    {
                        connection.Open();
                        string sql2 = "select * from GenerateCode where code='" + secCode + "' and status='true'";

                        //getting the data based from the pdid variable
                        using (SqlCommand command = new SqlCommand(sql2, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    checkSec++;





                                }
                            }
                        }
                        Console.WriteLine(checkSec);
                        if (checkSec > 0)
                        {

                            string fileName = Path.GetFileName(file.FileName);


                            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "feedback", fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }




                            string sql = "insert into feedback (name,rating,message,image,status) values (@name,@rating,@message,@image,'false')";

                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                command.Parameters.AddWithValue("@name", userconfirm);
                                command.Parameters.AddWithValue("@rating", star);
                                command.Parameters.AddWithValue("@message", message);
                                command.Parameters.AddWithValue("@image", "/img/feedback/" + fileName);

                                command.ExecuteNonQuery();
                            }

                            string sql3 = "update GenerateCode set status='false' where code='" + secCode + "'";

                            using (SqlCommand command = new SqlCommand(sql3, connection))
                            {

                                command.ExecuteNonQuery();
                            }

                            String sql4 = "DELETE FROM GenerateCode WHERE status= 'false'";
                            using (SqlCommand command = new SqlCommand(sql4, connection))
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
                        else
                        {
                            errorMessage = "The code you used is invalid or has expired!";
                            TempData["errorMessageProfile"] = errorMessage;
                            return Redirect("/Customer_AddFeedback");
                        }



                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Page();
            }

            return Redirect("/");
        }
    }
}
