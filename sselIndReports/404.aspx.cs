using System;
using System.Web.UI;

namespace sselIndReports
{
    public partial class _404 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            litMessage.Text = $"<strong>{Request.Url}</strong><br>The requested page was not found.";
        }
    }
}