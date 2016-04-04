using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecomm.util.entities
{
   

    public class    kyc
    {
        public DateTime create_time { get; set; }
        public Int64 id { get; set; }
        public string form_no { get; set; }
        public string owner_name { get; set; }
        public string bussiness_name { get; set; }
        public string building_name { get; set; }
        public string street_name { get; set; }
        public string landmark { get; set; }
        public string state { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public string area { get; set; }
        public int pincode { get; set; }
        public long mobile_1 { get; set; }
        public long mobile_2 { get; set; }
        public long landline { get; set; }
        public string email { get; set; }
        public string website { get; set; }
        public string shop_size { get; set; }
        public string dob { get; set; }
        public string user_id { get; set; }
        public string status { get; set; }
      //  public bit ignore_flag { get; set; }

        public string ignore_user_id { get; set; }
        public DateTime ignore_date { get; set; }
        public Decimal lat { get; set; }
        public Decimal longt { get; set; }
        public string device_id { get; set; }
        public int device_rowid { get; set; }
        public string brandlist { get; set; }
        public string industry { get; set; }
        public int approval_status { get; set; }
        public string reject_reason { get; set; }
        public string reject_remarks { get; set; }
        public string smartphone { get; set; }
        public string competitive_brands { get; set; }
      
    }

    public class kycid
    {
        public Int64 id { get; set; }
        public Int64 kyc_id { get; set; }

    }

    public class kycidstatus
    {
       
        public Int64 kyc_id { get; set; }
        public int approval_status { get; set; }
        public string reject_reason { get; set; }
        public string reject_remarks { get; set; }

    }

    public class UserAllocation
    {

        public Int64 userAreaID { get; set; }
        public Int64 areaid { get; set; }
        public string state { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public string area { get; set; }
        public string pincode { get; set; }

    }

    public class user_security
    {
        public long Id { get; set; }
        public string user_name { get; set; }
        public long company_id { get; set; }
        public string password { get; set; }
        public string user_type { get; set; }
    }
}
