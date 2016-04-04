using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ecomm.Payment
{
    public partial class makePayment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            NameValueCollection nvc = Request.Form;

            if (!string.IsNullOrEmpty(nvc["txttest"]))
            {

            }
        }
    }
}