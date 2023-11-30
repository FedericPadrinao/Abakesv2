using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace abakes2.Pages
{
    public class Customer_OrderSuccess3D_PromptModel : PageModel
    {
        public String userconfirm = "";
        public String imgconfirm = "";
        public String statusconfirm = "";
        public string connectionProvider = "Data Source=eu-az-sql-serv5434154f0e9a4d00a109437d48355b69.database.windows.net;Initial Catalog=d5rw6jsfzbuks4y;Persist Security Info=True;User ID=uqqncmi3rkbksbc;Password=***********";
        public void OnGet()
        {
            userconfirm = HttpContext.Session.GetString("username");
            imgconfirm = HttpContext.Session.GetString("userimage");
            statusconfirm = HttpContext.Session.GetString("userstatus");
        }
    }
}