using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text;
using QRCoder;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;
namespace abakes2.Pages

{
    public class ManageQRModel : PageModel
    {
        public List<code> codeList = new List<code>();
        public string userconfirm = "";
        public String connectionProvider = "Server=tcp:eu-az-sql-serv6e425a9865434acc8b7d6d8badb306b5.database.windows.net,1433;Initial Catalog=d58anpzl3kll8nf;Persist Security Info=False;User ID=ufy99xlgudx1fh3;Password=%g2q&#dV&ECBX6Oyf0%QkXHe5;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
       
        public void OnGet(string sortProduct)
        {
            userconfirm = HttpContext.Session.GetString("useradmin");
            if (userconfirm != null)
            {

            }
            else
            {
                Response.Redirect("/index");
            }

            ReadCode(sortProduct);

        }



        public IActionResult OnGetDeactivate()
        {
            string id = Request.Query["id"];
            try
            {


                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql2 = "update GenerateCode set status='false' where id='" + id + "'";

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

                    String sql3 = "DELETE FROM GenerateCode WHERE id='" + id + "' AND status= 'false'";
                    using (SqlCommand command = new SqlCommand(sql3, connection))
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
                Console.WriteLine("Exception: " + e.Message);
            }

            return RedirectToPage("/Admin_ManageQR");

        }

        public void OnPostGenerate()
        {
            var rand = new Random();
            var code = new StringBuilder();
            var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var codeLength = 5;

            int discount = int.Parse(Request.Form["DiscountCoupon"]);

            for (int i = 0; i < codeLength; i++)
            {
                int index = rand.Next(0, characters.Length);
                code.Append(characters[index]);
            }
            var maincode = code.ToString();

            try
            {
                DateTime expiration = DateTime.Now.AddDays(5);

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql2 = "INSERT INTO GenerateCode " +
                                  "(code,status,date,Discount,avail2D,avail3D) VALUES " +
                                  "(@code,@status,@date,@discount,@avail2d,@avail3d);";

                    using (SqlCommand command = new SqlCommand(sql2, connection))
                    {
                        command.Parameters.AddWithValue("@code", maincode);
                        command.Parameters.AddWithValue("@status", "true");
                        command.Parameters.AddWithValue("@date", expiration);
                        command.Parameters.AddWithValue("@discount", discount);
                        command.Parameters.AddWithValue("@avail2d", "true");
                        command.Parameters.AddWithValue("@avail3d", "true");

                        command.ExecuteNonQuery();

                    }

                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);

            }


           Response.Redirect("/card?maincode=" + maincode);
        }


        public JsonResult OnGetExpiration(string sortProduct)
        {
            ReadCode(sortProduct);

            DateTime current = DateTime.Now;
            string currentDate = current.ToString("MMM dd yyyy h:mm tt").Replace(" ", "");
            string testdate = "";
            try
            {

                foreach (var i in codeList)
                {

                    if (currentDate.Contains(i.dateexpiry.Replace(" ", "")))
                    {
                        using (SqlConnection connection = new SqlConnection(connectionProvider))
                        {
                            connection.Open();
                            String sql2 = "update GenerateCode set status='false' where id='" + i.id + "'";

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
                    else
                    {

                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new JsonResult(new { success = false });
            }

            return new JsonResult(new { success = true });
        }

        public JsonResult OnGetSession()
        {

            userconfirm = HttpContext.Session.GetString("user");
            if (userconfirm != null)
            {

            }
            else
            {
                Response.Redirect("/index");

            }

            Console.WriteLine("Hello");



            return new JsonResult(new { success = true });
        }



        public void ReadCode(string sortProduct)
        {
            try
            {


                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "select * from GenerateCode";

                    switch (sortProduct)
                    {
                        case "Sort Name":
                            sql += " ORDER BY id DESC";
                            break;
                        case "Sort Name2":
                            sql += " ORDER BY id ASC";
                            break;
                        case "Sort Status":
                            sql = "select * from GenerateCode WHERE Discount = 0";
                            break;
                        case "Sort Status2":
                            sql = "select * from GenerateCode WHERE Discount !=0";
                            break;
                    }
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                code read = new code();


                                read.id = reader.GetInt32(0);
                                read.generatedCode = reader.GetString(1);
                                read.status = reader.GetString(2);
                                read.dateexpiry = reader.GetString(3);
                                read.discount = reader.GetInt32(4);
                                read.avail2d = reader.GetString(5);
                                read.avail3d = reader.GetString(6);
                                codeList.Add(read);

                            }
                        }
                    }

                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
    }

    public class code
    {
        public int id;
        public string generatedCode;
        public string status;
        public string dateexpiry;
        public int discount;
        public string avail2d;
        public string avail3d;
    }
}
