using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
namespace abakes2.Pages
{
    public class ChangePaswordModel : PageModel //ChangePaswordModel to IndexModel
    {
        //public List<CustomerInfo> customerInfo = new List<CustomerInfo>();
        public CustomerInfo customerInfo = new CustomerInfo();
        public string userconfirm = "";
        public string username { get; set; }
        public String errorMessage = "";
        public String successMessage = "";
        public String imgconfirm = "";
        public String statusconfirm = "";

        public string connectionProvider = "Data Source=DESKTOP-ABF48JR\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";
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

            String user = Request.Query["user"];

            try
            {
                //CUSTOMER
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "SELECT * FROM LoginCustomer WHERE username=@user" ;

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user", user);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                customerInfo.username = reader.GetString(1);
                                
                                
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
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.ToString());
            }
        }

        public void OnPost()
        {
            string username = Request.Form["username"];
            string phone = Request.Form["phone"];
            string email = Request.Form["email"];
            string address = Request.Form["address"];
            string city = Request.Form["city"];
            string barangay = Request.Form["barangay"];

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    String sql = "UPDATE LoginCustomer SET username='" + username + "',phone='" + phone + "',email='" + email + "',address='" + address + "',city='" + city + "',barangay='" + barangay + "' where username='" + username + "'";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
            Response.Redirect("/Index");
        }
    }
}
