using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class CartModel : PageModel
    {
        public List<CustomerInfo> listCustomer = new List<CustomerInfo>();
        public List<OrderSimpleInfo> listOrderSimple = new List<OrderSimpleInfo>();
        public OrderSimpleInfo os = new OrderSimpleInfo();
        public Order3DForm order3D = new Order3DForm();
        public String userconfirm = "";
        public string connectionProvider = "Data Source=ROVIC\\SQLEXPRESS;Initial Catalog=Abakes;Integrated Security=True";
        public String errorMessage = "";
        public String successMessage = "";
        public int TotalCart = 0;
        public int cartcount = 0;
        public String imgconfirm = "";
        public String statusconfirm = "";


        public int notifCount = 0;
        public int pnotifCount = 0;
        public int cartCount = 0;
        public int totalnotifCount = 0;
        public int NotificationCount { get; set; }
        public void GetOrders()
        {   //will load getProducts everytime you launch the website
            string pdID = Request.Query["id"];
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider)) //static
                {
                    connection.Open();
                    string sql = "select sum(OrderPrice) from OrderSimple where username='" + userconfirm + "' AND status ='true'"; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TotalCart = reader.GetInt32(0);

                            }
                        }
                    }
                }
                using (SqlConnection connection = new SqlConnection(connectionProvider)) //get the data from the cart
                {
                    connection.Open();
                    string sql = "select * from OrderSimple where username='" + userconfirm + "' AND status ='true'"; //get all the data from the shopping cart based on the user.
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                


                                os.osID = reader.GetFieldValue<int>(reader.GetOrdinal("OrderID"));
                                os.osUsername = reader.GetFieldValue<string>(reader.GetOrdinal("username"));
                                os.osOccasion = reader.GetFieldValue<string>(reader.GetOrdinal("occasion"));
                                os.osShapes = reader.GetFieldValue<string>(reader.GetOrdinal("shapes"));
                                os.osTier = reader.GetFieldValue<string>(reader.GetOrdinal("tier"));
                                os.osFlavors = reader.GetFieldValue<string>(reader.GetOrdinal("flavors"));
                                os.osSizes = reader.GetFieldValue<string>(reader.GetOrdinal("sizes"));
                                os.osInstruction = reader.GetFieldValue<string>(reader.GetOrdinal("instructions"));
                                os.osDelivery = reader.GetFieldValue<string>(reader.GetOrdinal("delivery"));
                                os.status = reader.GetFieldValue<string>(reader.GetOrdinal("status"));
                                os.osPrice = reader.GetFieldValue<int>(reader.GetOrdinal("OrderPrice"));
                               


                            }
                        }
                    }
                }
                using (SqlConnection connection = new SqlConnection(connectionProvider)) //get the data from the cart
                {
                    connection.Open();
                    string sql = "select * from Order3DForm where username='" + userconfirm + "' AND status ='true'"; //get all the data from the shopping cart based on the user.
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {



                                order3D.ModelID = reader.GetFieldValue<int>(reader.GetOrdinal("OrderID"));
                                order3D.ModelType = reader.GetFieldValue<string>(reader.GetOrdinal("ModelType"));
                                order3D.instructions = reader.GetFieldValue<string>(reader.GetOrdinal("instructions"));
                                order3D.order3DDelivery = reader.GetFieldValue<string>(reader.GetOrdinal("OrderDelivery"));
                                order3D.order3DPrice = reader.GetFieldValue<int>(reader.GetOrdinal("OrderPrice"));



                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading Orders: " + e.ToString());
                Console.WriteLine("Exception cart: " + e.Message);
            }
            
        }
        public IActionResult OnGetOrder()
        { //Check out Order
            
            if (userconfirm == null)
            {
                return RedirectToPage("/login");
            }
            else
            {

            }

            string order_id = "";
            string username = "";
            int pID = 0;
            string pName = "";
            string category = "";
            int quantity = 1;
            int total = 0;
            int price = 0;
            string pImg = "";

         


            //Remove All Items from Cart

            using (SqlConnection connection = new SqlConnection(connectionProvider)) //get the data from the cart
            {
                connection.Open();
                string sql = "delete from OrderSimple where username='" + userconfirm + "'"; //deletes all content from user's cat after checking out
                using (SqlCommand command = new SqlCommand(sql, connection))
                {


                    command.ExecuteNonQuery();

                }
            }


            return Redirect("/Thanks");
        }


        public IActionResult OnGetRemoveCart()
        {
            OnGet();
            if (userconfirm == null)
            { //If not logged in, it will be redirected to login page
                return RedirectToPage("/Account");
            }
            else
            {

            }
            string osid = Request.Query["id"]; //name from the front end "?id=

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionProvider)) //static
                {
                    connection.Open();
                    string sql = "delete from OrderSimple where OrderID='" + osid + "' and username='" + userconfirm + "'"; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception e)
            {

            }

            return Redirect("/index");
        }

     

        public IActionResult OnGetIncrease()
        {
            OnGet();
            if (userconfirm == null)
            {
                return RedirectToPage("/Login");
            }
            else
            {

            }
            string pdID = Request.Query["id"];
            int quantity = 1;
            int total = 0; //quantity*price
            int price = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider)) //static
                {
                    connection.Open();
                    string sql = "select * from OrderSimple where orderID='" + pdID + "'"; //getting the data based from the pdid variable from the Product Class(line 299)
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                price = reader.GetFieldValue<int>(reader.GetOrdinal("OrderPrice"));

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider)) //dynamic
                {
                    connection.Open();
                    string sql = "select * from ShoppingCart where username='" + userconfirm + "' and ProductID='" + pdID + "'"; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                quantity = reader.GetFieldValue<int>(reader.GetOrdinal("Quantity"));
                                quantity = quantity + 1;
                                total = (price * quantity);


                            }
                        }
                    }
                }


                using (SqlConnection connection = new SqlConnection(connectionProvider)) //dynamic because total and quantity only changes.
                {
                    connection.Open();
                    string sql = "update ShoppingCart set Quantity='" + quantity + "',TotalPrice='" + total + "' where username='" + userconfirm + "' and productID='" + pdID + "'"; ; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {


                        command.ExecuteNonQuery();

                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception cart: " + ex.Message);
            }
            return Redirect("/Cart");

        }

        public IActionResult OnGetDecrease()
        {
            OnGet();

            if (userconfirm == null)
            {
                return RedirectToPage("/Login");
            }
            else
            {

            }
            string pdID = Request.Query["id"];
            int quantity = 1;
            int total = 0; //quantity*price
            int price = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider)) //static
                {
                    connection.Open();
                    string sql = "select * from OrderSimple where OrderID='" + pdID + "'"; //getting the data based from the pdid variable from the Product Class(line 299)
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                price = reader.GetFieldValue<int>(reader.GetOrdinal("OrderPrice"));

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider)) //dynamic
                {
                    connection.Open();
                    string sql = "select * from ShoppingCart where username='" + userconfirm + "' and ProductID='" + pdID + "'"; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                quantity = reader.GetFieldValue<int>(reader.GetOrdinal("Quantity"));
                                quantity = quantity - 1;
                                total = (price * quantity);


                            }
                        }
                    }
                }


                using (SqlConnection connection = new SqlConnection(connectionProvider)) //dynamic because total and quantity only changes.
                {
                    connection.Open();
                    string sql = "update ShoppingCart set Quantity='" + quantity + "',TotalPrice='" + total + "' where username='" + userconfirm + "' and productID='" + pdID + "'"; ; //getting the data based from the pdid variable
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {


                        command.ExecuteNonQuery();

                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception cart: " + ex.Message);
            }
            return Redirect("/Cart");

        }


        public void OnGet()
        {
            userconfirm = HttpContext.Session.GetString("username");
            imgconfirm = HttpContext.Session.GetString("userimage");
            statusconfirm = HttpContext.Session.GetString("userstatus");

            GetOrders();

            // String id = Request.Query["id"]; //SPECIFIES ANONG PRODUCT 

            if (userconfirm != null)
            {


            }
            else
            {
                Response.Redirect("/Index");
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionProvider))
                {
                    connection.Open();
                    string sql = "select * from LoginCustomer";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CustomerInfo customerInfo = new CustomerInfo(); 
                                customerInfo.username = reader.GetFieldValue<string>(reader.GetOrdinal("username"));
                                customerInfo.password = reader.GetFieldValue<string>(reader.GetOrdinal("password"));
                                customerInfo.email = reader.GetFieldValue<string>(reader.GetOrdinal("email"));
                                customerInfo.phone = reader.GetFieldValue<string>(reader.GetOrdinal("phone"));
                                customerInfo.lname = reader.GetFieldValue<string>(reader.GetOrdinal("lname"));
                                customerInfo.fname = reader.GetFieldValue<string>(reader.GetOrdinal("fname"));
                                customerInfo.address = reader.GetFieldValue<string>(reader.GetOrdinal("address"));
                                customerInfo.city = reader.GetFieldValue<string>(reader.GetOrdinal("city"));
                                customerInfo.barangay = reader.GetFieldValue<string>(reader.GetOrdinal("barangay"));

                                listCustomer.Add(customerInfo);
                            }
                        }
                    }
                }

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
                totalnotifCount = notifCount + pnotifCount;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }


    




        }

    }

}
