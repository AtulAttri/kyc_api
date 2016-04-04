<%@ Page Language="C#" Debug="true" EnableViewStateMac="False" %>

<%@ Import Namespace="IntegrationKit" %>
<html>
<head>
</head>
<script language="C#" runat="server">
    libfuncs myUtility = new libfuncs();

    void Page_Load(Object sender, EventArgs e)
    {
        populate(sender, e);
    }

    void populate(Object sender, EventArgs e)
    {
        string WorkingKey, Order_Id, Merchant_Id, Amount, AuthDesc, Checksum, newChecksum, status;
        string URL = "";
        
        //Assign following values to send it to verifychecksum function.
        //put in the 32 bit working key in the quotes provided here
        WorkingKey = "1d2y2kwcayo6sxqpcj";
        Merchant_Id = "M_acc27738_27738";
        Order_Id = Request.Form["Order_Id"];
        Amount = Request.Form["Amount"];
        AuthDesc = Request.Form["AuthDesc"];

        ////////////////////////  ERROR...This variable(status) is not declared anywhere 

        status = Request.Form["Status"];

        ////////////////////// This comment is given by Majestic People, Coimbatore
        ////////////////////// The following variable "checksum" is declared as "Checksum" at the top

        Checksum = Request.Form["Checksum"];

        /*
        Response.Write(Merchant_Id + "<br>");
        Response.Write(Order_Id + "<br>");
        Response.Write(Amount + "<br>");
        Response.Write(AuthDesc + "<br>");
        Response.Write(status + "<br>");
        Response.Write(Checksum + "<br>");
        */
        //Checksum = verifychecksum(Merchant_Id , Order_Id, Amount , AuthDesc ,WorkingKey, Checksum);
        Checksum = myUtility.verifychecksum(Merchant_Id, Order_Id, Amount, AuthDesc, WorkingKey, Checksum);
        //Checksum.Value = intChecksum;
        Response.Write(Checksum + "<br>");

        URL = myUtility.GetCompanyURL(Order_Id); //For getting the Company URL
        URL = URL + ConfigurationManager.AppSettings["annectosURL"];
        if ((Checksum == "true") && (AuthDesc == "Y"))
        {
            
            Message.Text = "<br>Thank you for shopping with us. Your credit card has been charged and your transaction is successful. We will be shipping your order to you soon.";
            /* 
                Here you need to put in the routines for a successful 
                 transaction such as sending an email to customer,
                 setting database status, informing logistics etc etc
            */
            //Diwakar Clear cart 2/2/2014
            //start
            ecomm.model.repository.store_repository sr = new ecomm.model.repository.store_repository();
            ecomm.model.repository.payment_repository pr = new ecomm.model.repository.payment_repository();
            System.Data.DataTable dtShip = pr.GetDataFromUserID(Convert.ToInt64(Order_Id));
            //Response.Write(Order_Id + " " +  dtShip.Rows[0]["user_id"].ToString());

            if ((dtShip != null) || (dtShip.Rows.Count != 0))
            {
                // Response.Write("ind");         
                if (sr.cart_exists(dtShip.Rows[0]["user_id"].ToString()))
                {
                    //Response.Write("cart Exists?");
                    sr.del_cart(dtShip.Rows[0]["user_id"].ToString());
                }

            }
    
            string strMessageOutput=myUtility.ProcessOrderStatus(Order_Id, Amount, "Transaction is successful", "2");
            if (strMessageOutput == "Success")
                Response.Redirect(URL + "/2");
            else
                Response.Write("Error: " + strMessageOutput); 
        }
        else if ((Checksum == "true") && (AuthDesc == "N"))
        {
            Message.Text = "<br>Thank you for shopping with us. However,the transaction has been declined.";
            /*
                Here you need to put in the routines for a failed
                transaction such as sending an email to customer
                setting database status etc etc
            */
            Response.Redirect(URL + "/3");
        }
        else if ((Checksum == "true") && (AuthDesc == "B"))
        {
            Message.Text = "<br>Thank you for shopping with us.We will keep you posted regarding the status of your order through e-mail";
            /*
                Here you need to put in the routines/e-mail for a  "Batch Processing" order
                This is only if payment for this transaction has been made by an American Express Card
                since American Express authorisation status is available only after 5-6 hours by mail from ccavenue and at the "View Pending Orders"
         */
            myUtility.ProcessOrderStatus(Order_Id, Amount, "Will keep you posted regarding the status of your order through e-mail", "4");
            Response.Redirect(URL + "/4");
        }
        else
        {
            Message.Text = "<br>Security Error. Illegal access detected";
            /*
                Here you need to simply ignore this and dont need
                to perform any operation in this condition
            */
            //myUtility.ProcessOrderStatus(Order_Id, Amount, AuthDesc, "9");
            Response.Redirect(URL + "/9");
        }
    }

</script>
<body>
    <center>
       <form  method="post" runat="server">
           <asp:label id="Message" runat="server"/>
       </form>
       </center>
</body>
</html>

