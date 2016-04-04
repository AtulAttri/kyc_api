<%@ Page Language="C#" Debug="true" EnableViewStateMac="False" %>
<%@ Import Namespace="ecomm.util" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Payment Gateway</title>

    <style type="text/css">
        .modal {
            position: fixed;
            top: 0;
            left: 0;
            background-color: black;
            z-index: 99;
            opacity: 0.8;
            filter: alpha(opacity=80);
            -moz-opacity: 0.8;
            min-height: 100%;
            width: 100%;
        }

        .loading {
            font-family: Arial;
            font-size: 10pt;
            border: 5px solid #67CFF5;
            width: 200px;
            height: 100px;
            display: none;
            position: fixed;
            background-color: White;
            z-index: 999;
        }
    </style>

    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1/jquery.min.js"></script>
    <script type="text/javascript">
        var cntr = 0;
        function ShowProgress() {
            cntr = setTimeout(LoadProgress, 200);
        }

        function LoadProgress() {
                var modal = $('<div />');
                modal.addClass("modal");
                $('body').append(modal);
                var loading = $(".loading");
                loading.show();
                var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
                var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
                loading.css({ top: top, left: left });
                EndPageLoad();
        }

        function EndPageLoad() {
            clearTimeout(cntr);
            document.forms[0].submit();
        }

        window.onload = function () {
            ShowProgress();
        }

        window.onunload = function () {
            EndPageLoad();
        }



    </script>

</head>

<body >

    <script lang="c#" runat="server">
    
        IntegrationKit.libfuncs myUtility = new IntegrationKit.libfuncs();
        void Display(Object sender, EventArgs e)
        {
            try
            {
                //string Merchant_Id, Amount, Order_Id, Redirect_Url, WorkingKey, intChecksum;
                string WorkingKey, intChecksum;
                string strActualAmt = "";
                string strPoints = "";
                string strPointsINR = "";
                string stregiftVouNo = "";
                string stregiftAmount = "";
                string stremail = "";
                string strPaidAmount = "";
                string final_amt = "";
                string strPointBal = "";
                string strDiscCoupon = "";
                string strDiscCouponAmount = "";

                Merchant_Id.Value = "M_acc27738_27738";// Request.Form["Merchant_Id"];				    //This id(also User Id)  available at "Generate Working Key" of "Settings & Options" 
                Amount.Value = Request.Form["Amount"];						  //your script should substitute the amount here in the quotes provided here
                Order_Id.Value = Request.Form["Order_Id"];							//your script should substitute the order description here in the quotes provided here
                stremail = Request.Form["billing_cust_email"];
                strPaidAmount = Request.Form["Amount"];
                strActualAmt = Request.Form["Actual_Amount"];
                strPoints = Request.Form["Points"];
                strPointsINR = Request.Form["Points_Inr"];
                stregiftVouNo = Request.Form["EGift_VNO"];
                stregiftAmount = Request.Form["EGift_Amt"];
                strDiscCoupon = Request.Form["Discount_Coupon"];
                strDiscCouponAmount = Request.Form["Discount_Coupon_Amount"];
                
                final_amt = myUtility.GetOrderAmount(Order_Id.Value);
                strPointBal = myUtility.GetPointsBalance(Order_Id.Value);

                if (string.IsNullOrEmpty(strPointsINR))
                    strPointsINR = "0";
                if (string.IsNullOrEmpty(stregiftAmount))
                    stregiftAmount = "0";
                if (string.IsNullOrEmpty(final_amt))
                    final_amt = "0";

                if (Convert.ToDouble(strPointsINR) > Convert.ToDouble(strPointBal))
                {
                    strPointsINR = strPointBal;
                }
                string strWrkAmount = Convert.ToString(Convert.ToDouble(final_amt) - Convert.ToDouble(strPointsINR) - Convert.ToDouble(stregiftAmount) - Convert.ToDouble(strDiscCouponAmount)); // Included strDiscCouponAmount Diwakar 4/2/2014

                strPaidAmount = strWrkAmount; //Amount.Value;// final_amt;
                strActualAmt = strWrkAmount; //Amount.Value;// final_amt;

                myUtility.InitiateOrder(stremail, Order_Id.Value, strPaidAmount, strPoints, strPointsINR, strActualAmt, stregiftVouNo, stregiftAmount,strDiscCoupon,strDiscCouponAmount, "1");
                //stregiftVouNo = Request.Form["Amount"];
                //stregiftAmount = Request.Form["Amount"];            

                Redirect_Url.Value = "http://54.208.211.125:8084/redirecturl.aspx";// Request.Form["Redirect_Url"];			 //your redirect URL where your customer will be redirected after authorisation from CCAvenue
                WorkingKey = "1d2y2kwcayo6sxqpcj";				  //put in the 32 bit alphanumeric key in the quotes provided here.Please note that get this key ,login to your CCAvenue merchant account and visit the "Generate Working Key" section at the "Settings & Options" page. 
                Checksum.Value = "";
                intChecksum = "";
                //Before Calling this method all parameters should have a value especially working key.
                intChecksum = myUtility.getchecksum(Merchant_Id.Value, Order_Id.Value, Amount.Value, Redirect_Url.Value, WorkingKey);

                //Assign Following fields to send it ahead.

                Checksum.Value = intChecksum;
                billing_cust_name.Value = Request.Form["billing_cust_name"];
                billing_cust_address.Value = Request.Form["billing_cust_address"];
                billing_cust_country.Value = Request.Form["billing_cust_country"];
                billing_cust_state.Value = Request.Form["billing_cust_state"];
                billing_cust_tel.Value = Request.Form["billing_cust_tel"];
                billing_cust_email.Value = Request.Form["billing_cust_email"];
                delivery_cust_name.Value = Request.Form["delivery_cust_name"];
                delivery_cust_address.Value = Request.Form["delivery_cust_address"];
                delivery_cust_country.Value = Request.Form["delivery_cust_country"];
                delivery_cust_state.Value = Request.Form["delivery_cust_state"];
                delivery_cust_tel.Value = Request.Form["delivery_cust_tel"];
                delivery_cust_notes.Value = "";// Request.Form["billing_cust_name"];
                Merchant_Param.Value = "";
                billing_zip_code.Value = Request.Form["billing_zip_code"];
                delivery_zip_code.Value = Request.Form["delivery_zip_code"];
                billing_cust_city.Value = Request.Form["billing_cust_city"];
                delivery_cust_city.Value = Request.Form["delivery_cust_city"];
            }
            catch (Exception ex)
            {
                ExternalLogger.LogError(ex, this, "#");
            }

        }
        void Page_Load(Object sender, EventArgs e)
        {
            Display(sender, e);
        }
    
    
    </script>

    <!-- Mention the URL of the page ("IN action attribute" ) to which data will be posted -->
    <form method="post" action="https://www.ccavenue.com/shopzone/cc_details.jsp">

        <input type="hidden" id="Merchant_Id" runat="server">
        <input type="hidden" id="Amount" runat="server">
        <input type="hidden" id="Order_Id" runat="server">
        <input type="hidden" id="Redirect_Url" runat="server">
        <input type="hidden" id="Checksum" runat="server">
        <input type="hidden" id="billing_cust_name" runat="server">
        <input type="hidden" id="billing_cust_address" runat="server">
        <input type="hidden" id="billing_cust_country" runat="server">
        <input type="hidden" id="billing_cust_state" runat="server">
        <input type="hidden" id="billing_cust_tel" runat="server">
        <input type="hidden" id="billing_cust_email" runat="server">
        <input type="hidden" id="delivery_cust_name" runat="server">
        <input type="hidden" id="delivery_cust_address" runat="server">
        <input type="hidden" id="delivery_cust_country" runat="server">
        <input type="hidden" id="delivery_cust_state" runat="server">
        <input type="hidden" id="delivery_cust_tel" runat="server">
        <input type="hidden" id="delivery_cust_notes" runat="server">
        <input type="hidden" id="Merchant_Param" runat="server">
        <input type="hidden" id="billing_cust_city" runat="server">
        <input type="hidden" id="billing_zip_code" runat="server">
        <input type="hidden" id="delivery_cust_city" runat="server">
        <input type="hidden" id="delivery_zip_code" runat="server">
        <%--<input type="submit" value="submit" runat="server"> --%>

        <div class="loading" align="center">
            Wait...the Order is Processed <br />
            <br />
            <img src="loader.gif" alt="" />
        </div>

    </form>
</body>
</html>
