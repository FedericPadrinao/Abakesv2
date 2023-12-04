using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace abakes2.Pages
{
    public class Account_Verif_SuccModel : PageModel
    {
        public String userconfirm = "";
        public String imgconfirm = "";
        public String statusconfirm = "";
        public string connectionProvider = "Data Source=eu-az-sql-serv8c295e6a1afc4f69be52fd159aeb63da.database.windows.net;Initial Catalog=drt6diqvzxczvbi;User ID=uhsk2j20jhg6qgk;Password=***********";
        public void OnGet()
        {
            userconfirm = HttpContext.Session.GetString("username");
            imgconfirm = HttpContext.Session.GetString("userimage");
            statusconfirm = HttpContext.Session.GetString("userstatus");
            userconfirm = HttpContext.Session.GetString("username");
            if (userconfirm != null)
            {
                Response.Redirect("/Index");

            }
            else
            {

            }
        }
    }
}
