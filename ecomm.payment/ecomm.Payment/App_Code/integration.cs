namespace IntegrationKit
{
    using ecomm.util;
    using System;
    using System.Data;
    using ecomm.model.repository;

    public class libfuncs
    {
        
        public string getchecksum(string MerchantId, string OrderId, string Amount, string redirectUrl, string WorkingKey)
        {
            string str;
            long adler;
            str = MerchantId + "|" + OrderId + "|" + Amount + "|" + redirectUrl + "|" + WorkingKey;
            adler = 1;
            return adler32(adler, str);
        }

        public string verifychecksum(string MerchantId, string OrderId, string Amount, string AuthDesc, string WorkingKey, string checksum)
        {
            string str, retval, adlerResult;
            long adler;
            str = MerchantId + "|" + OrderId + "|" + Amount + "|" + AuthDesc + "|" + WorkingKey;
            adler = 1;
            adlerResult = adler32(adler, str);

            if (string.Compare(adlerResult, checksum, true) == 0)
            {
                retval = "true";
            }
            else
            {
                retval = "false";
            }
            return retval;
        }


        private string adler32(long adler, string strPattern)
        {
            long BASE;
            long s1, s2;
            char[] testchar;
            long intTest = 0;

            BASE = 65521;
            s1 = andop(adler, 65535);
            s2 = andop(cdec(rightshift(cbin(adler), 16)), 65535);

            for (int n = 0; n < strPattern.Length; n++)
            {

                testchar = (strPattern.Substring(n, 1)).ToCharArray();
                intTest = (long)testchar[0];
                s1 = (s1 + intTest) % BASE;
                s2 = (s2 + s1) % BASE;
            }
            return (cdec(leftshift(cbin(s2), 16)) + s1).ToString();
        }


        private long power(long num)
        {
            long result = 1;
            for (int i = 1; i <= num; i++)
            {
                result = result * 2;
            }
            return result;
        }


        private long andop(long op1, long op2)
        {
            string op, op3, op4;
            op = "";

            op3 = cbin(op1);
            op4 = cbin(op2);

            for (int i = 0; i < 32; i++)
            {
                op = op + "" + ((long.Parse(op3.Substring(i, 1))) & (long.Parse(op4.Substring(i, 1))));
            }
            return cdec(op);
        }

        private string cbin(long num)
        {
            string bin = "";
            do
            {
                bin = (((num % 2)) + bin).ToString();
                //num = (long) Math.Floor(num / 2);
                num = (long)Math.Floor(Convert.ToDouble(num) / 2);
            } while (!(num == 0));

            long tempCount = 32 - bin.Length;

            for (int i = 1; i <= tempCount; i++)
            {
                bin = "0" + bin;
            }
            return bin;
        }


        private string leftshift(string str, long num)
        {
            long tempCount = 32 - str.Length;

            for (int i = 1; i <= tempCount; i++)
            {

                str = "0" + str;
            }

            for (int i = 1; i <= num; i++)
            {
                str = str + "0";
                str = str.Substring(1, str.Length - 1);
            }
            return str;
        }


        private string rightshift(string str, long num)
        {

            for (int i = 1; i <= num; i++)
            {
                str = "0" + str;
                str = str.Substring(0, str.Length - 1);
            }
            return str;
        }

        private long cdec(string strNum)
        {
            long dec = 0;
            for (int n = 0; n < strNum.Length; n++)
            {
                dec = dec + (long)(long.Parse(strNum.Substring(n, 1)) * power(strNum.Length - (n + 1)));
            }
            return dec;
        }

        public Boolean InitiateOrder(string EmailID, string OrderID, string PaidAmt, string Points, string Points_INR, string strActualAmt, string egiftVouNo, string egiftVouAmt, string discount_coupon,string discount_amount, string status)
        {
            try
            {
                ecomm.model.repository.payment_repository pr = new ecomm.model.repository.payment_repository();
                bool blret = pr.InitiateOrder(EmailID, OrderID, PaidAmt, Points, Points_INR, strActualAmt, egiftVouNo, egiftVouAmt, discount_coupon,discount_amount,status);
                //call this method to remove cart data from mongodb collection  'cart' 
                ecomm.model.repository.store_repository sr = new ecomm.model.repository.store_repository();
                if (blret)
                {
                    if (sr.cart_exists(EmailID))
                    {
                        sr.del_cart(EmailID);
                    }
                }
                return blret;
            }
            catch (Exception ex)
            {
                ExternalLogger.LogError(ex, this, "#");
                return false;
            }
        }

        public string ProcessOrderStatus(string OrderID, string PaidAmt, string remarks, string status)
        {
            try
            {
                ecomm.model.repository.payment_repository pr = new ecomm.model.repository.payment_repository();
                return pr.ProcessOrderStatus(OrderID, PaidAmt, remarks, status);
            }
            catch (Exception ex)
            {
                ExternalLogger.LogError(ex, this, "#");
                return "";
            }
        }


        public string GetOrderAmount(string OrderID)
        {
            double total_amount = 0;
            try
            {
                ecomm.model.repository.payment_repository pr = new ecomm.model.repository.payment_repository();
                total_amount = pr.getOrderAmount(Convert.ToInt64(OrderID));

                return Convert.ToString(total_amount);
            }
            catch (Exception ex)
            {
                ExternalLogger.LogError(ex, this, "#");
                return "0";
            }
        }

        public string GetPointsBalance(string OrderID)
        {
            try
            {
                double point_balance = 0;
                ecomm.model.repository.payment_repository pr = new ecomm.model.repository.payment_repository();
                point_balance = pr.getUserPointBalance(Convert.ToInt64(OrderID));

                return Convert.ToString(point_balance);
            }
            catch (Exception ex)
            {
                ExternalLogger.LogError(ex, this, "#");
                return "0";
            }

        }

        public string GetCompanyURL(string OrderID)
        {
            try
            {
                ecomm.model.repository.payment_repository pr = new ecomm.model.repository.payment_repository();
                return pr.GetCompanyURLFromOrderID(OrderID);
            }
            catch (Exception ex)
            {
                ExternalLogger.LogError(ex, this, "#");
                return "";
            }
        }


    }			//End of Class libfuncs

}		//End of class IntegrationKit