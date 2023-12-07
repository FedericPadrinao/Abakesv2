using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace abakes2.Pages
{
    public class Customer_InvoiceModel : PageModel
    {
        public string FormattedDateTime { get; set; }

        public void OnGet()
        {
            // Get the current date and time
            DateTime now = DateTime.Now;

            // Format the date and time
            FormattedDateTime = now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}