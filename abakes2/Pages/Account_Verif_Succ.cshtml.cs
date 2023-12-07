using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace abakes2.Pages
{
    public class Account_Verif_SuccModel : PageModel
    {
        public String userconfirm = "";
        public String imgconfirm = "";
        public String statusconfirm = "";
        public string connectionProvider = "Data Source=orange\\sqlexpress;Initial Catalog=Abakes;Integrated Security=True";
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
