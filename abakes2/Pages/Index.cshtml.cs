﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace abakes2.Pages
{
    public class IndexModel : PageModel
    {
        public List<CustomerInfo> customerInfo = new List<CustomerInfo>();
        public List<NotificationInfo> listNotifications = new List<NotificationInfo>();

        public List<PrivateNotifInfo> listPrivateNotifInfo = new List<PrivateNotifInfo>();
        private readonly ILogger<IndexModel> _logger;
        public String userconfirm = "";
        public String imgconfirm = "";
        public String statusconfirm = "";
        public int notifCount = 0;
        public int pnotifCount = 0;
        public int cartCount = 0;
        public int totalnotifCount = 0;
        public string connectionString = "Server=tcp:eu-az-sql-serv4c4db8f1d3864bb6a62cb90162f811bf.database.windows.net,1433;Initial Catalog=dh60fwg8dvwho6h;Persist Security Info=False;User ID=uw1aiy40iqpmqas;Password=U6W5bT1pW3O37J#DEihhdV%p@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public int NotificationCount { get; set; } // Property to store notification count
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            userconfirm = HttpContext.Session.GetString("username");
            imgconfirm = HttpContext.Session.GetString("userimage");
            statusconfirm = HttpContext.Session.GetString("userstatus");
            notifCount = HttpContext.Session.GetInt32("NotificationCount") ?? 0;
            //NAV COUNT
            try
            {
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

            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        public IActionResult OnGetLogOut()
        {
            HttpContext.Session.Remove("username");
            userconfirm = HttpContext.Session.GetString("username");
            return RedirectToPage("/Index");
        }
    }
    public class UserInfo //admin
    {
        public String id = "";
        public String username = "";
        public String password = "";
        public String secAnswer = "";
    }

    public class Products
    {
        public int pdID = 0;
        public string pdName = "";
        public string pdCategory = "";
        public int pdPrice = 0;
        public string pdDescription = "";
        public string pdImg = "";
        public string pdStatus = "";
    }

    public class Asset3DInfo
    {
        public int AssetID = 0;
        public string AssetName = "";
        public string AssetPath = "";
        public int AssetScale = 0;
        public string PositionX = "";
        public string PositionY = "";
        public string PositionZ = "";

    }

    public class Asset3DForm
    {
        public int AssetID = 0;
        public int OrderID = 0;
        public string username = "";
        public string AssetName = "";
        public string AssetPath = "";
        public int AssetScale = 0;
        public string PositionX = "";
        public string PositionY = "";
        public string PositionZ = "";

    }
    public class OrderInfo
    {
        public int odID = 0;
        public string odName = "";
        public string odEmail = "";
        public string odPhone = "";
        public string odShapes = "";
        public string odTier = "";
        public string odFlavor = "";
        public string odSize = "";
        public string odInstruction = "";
        public string status = "";
    }

    public class Order3DInfo
    {
        public int ModelID = 0;
        public string ModelName = "";
        public int Scale = 0;
        public string TColor = "";
        public string TColor2 = "";
        public string TColor3 = "";
        public string Color = "";
        public string Color2 = "";
        public string Color3 = "";
        public int TotalTexture = 0;
        public string ModelType = "";
    }

    public class Order3DForm
    {
        public int ModelID = 0;
        public string username = "";
        public string ModelName1 = "";
        public string ModelName2 = "";
        public string ModelName3 = "";
        public int Scale1 = 0;
        public int Scale2 = 0;
        public int Scale3 = 0;
        public string Texture1 = "";
        public string Texture2 = "";
        public string Texture3 = "";
        public string Color = "";
        public string Color2 = "";
        public string Color3 = "";
    }

    public class OrderSimpleInfo
    {
        public int osID = 0;
        public string osUsername = "";
        public string osOccasion = "";
        public string osShapes = "";
        public string osTier = "";
        public string osFlavors = "";
        public string osSizes = "";
        public string osInstruction = "";
        public string osDelivery = "";
        public string status = "";
        public int osPrice = 0;
        public int osQuantity = 0;
        public int osShip = 0;
        public int osDP = 0;
        public string osPreferredD = "";
        public string osExpectedD = "";
        public string osExpectedT = "";
        public string osColor = "";
        public string osDedication = "";
    }

    public class InvoiceInfo
    {
        public int invoiceID = 0;
        public string invoiceUsername = "";
        public string invoiceOccasion = "";
        public string invoiceShapes = "";
        public string invoiceTier = "";
        public string invoiceFlavors = "";
        public string invoiceSizes = "";
        public string invoiceInstruction = "";
        public string invoiceDelivery = "";
        public string status = "";
        public int invoicePrice = 0;
        public int invoiceQuantity = 0;
        public int invoiceShip = 0;
        public int invoiceDP = 0;
        public string invoicePreferredD = "";
        public string invoiceExpectedD = "";
        public string invoiceExpectedT = "";
        public string invoiceColor = "";
        public string invoiceDedication = "";
        public string invoiceDateCreated = "";
        public string orderStatus = "";
        public string receipt = "";
    }
    public class Feedbacks
    {
        public int fbID = 0;
        public string fbName = "";
        public string fbRating = "";
        public string fbMessage = "";
        public string fbImg = "";
        public string status = "";

    }
    public class CustomerInfo
    {
        public int id = 0;
        public String username = "";
        public String password = "";
        public String email = "";
        public String phone = "";
        public String lname = "";
        public String fname = "";
        public String address = "";
        public String img = "";
        public String city = "";
        public String barangay = "";
        public String status = "";
        public String accstatus = "";
        public String ordermax = "";
        public String ordermax3D = "";
    }

    public class NotificationInfo
    {
        public int NotifID = 0;
        public String NotifTitle = "";
        public String NotifText = "";
        public String NotifImg = "";
        public String status = "";
    }

    public class PrivateNotifInfo
    {
        public int NotifID = 0;
        public String NotifName = "";
        public String NotifTitle = "";
        public String NotifText = "";
        public String NotifImg = "";
        public String status = "";
    }
}