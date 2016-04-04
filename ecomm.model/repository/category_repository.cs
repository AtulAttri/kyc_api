using ecomm.dal;
using ecomm.util;
using ecomm.util.entities;
// to be removed
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ecomm.model.repository
{
    public class category_repository
    {

  
        public List<kycid> kyc_insert(List<kyc> kyc_list)
        {
            DataAccess da = new DataAccess();
            dal.DataAccess dal = new DataAccess();          
            List<kyc> kc = new List<kyc>();
            List<kycid> KYC_ID = new List<kycid>();
            string guid = "";
            //string MobNo = "";
            //string strMobileNo = "";
            kycid kid = new kycid();

            try
            {

                for (int i = 0; i < kyc_list.Count(); i++)
                {
                    kyc kyc = new kyc();
                    kyc = kyc_list[i];

                    kid = new kycid();
                    kid.id = kyc.device_rowid;
                    try
                    {
                        DataTable dt = da.ExecuteDataTable("usp_kyc_add"
                              , da.Parameter("_form_no", kyc.form_no)
                            , da.Parameter("_owner_name", kyc.owner_name)
                            , da.Parameter("_bussiness_name", kyc.bussiness_name)
                            , da.Parameter("_building_name", kyc.building_name)
                            , da.Parameter("_street_name", kyc.street_name)
                            , da.Parameter("_landmark", kyc.landmark)
                              , da.Parameter("_state", kyc.state)
                                , da.Parameter("_district", kyc.district)
                            , da.Parameter("_city", kyc.city)
                            , da.Parameter("_area", kyc.area)
                            , da.Parameter("_pincode", kyc.pincode)
                            , da.Parameter("_mobile_1", kyc.mobile_1)
                              , da.Parameter("_mobile_2", kyc.mobile_2)
                            , da.Parameter("_landline", kyc.landline)
                            , da.Parameter("_email", kyc.email)
                            , da.Parameter("_website", kyc.website)
                             , da.Parameter("_shop_size", kyc.shop_size)
                              , da.Parameter("_dob", kyc.dob)
                            , da.Parameter("_user_id", kyc.user_id)
                              , da.Parameter("_lat", kyc.lat)
                            , da.Parameter("_long", kyc.longt)
                            , da.Parameter("_device_id", kyc.device_id)
                            , da.Parameter("_device_rowid", kyc.device_rowid)
                             , da.Parameter("_brands", kyc.brandlist)
                             , da.Parameter("_industry", kyc.industry)
                              , da.Parameter("_smartphone", kyc.smartphone)
                               , da.Parameter("_competitive_brands", kyc.competitive_brands)
                            );

                        kid.kyc_id = long.Parse(dt.Rows[0][0].ToString());

                        KYC_ID.Add(kid);
                    }
                    catch (Exception ex)
                    {
                        ExternalLogger.LogError(ex, this, "#");
                        kid.kyc_id = 0;
                        KYC_ID.Add(kid);
                    }
                }
                return KYC_ID;
            }
            catch (Exception ex)
            {
                ExternalLogger.LogError(ex, this, "#");
                return KYC_ID;
            }
        }

        public List<kycidstatus> SelectKycByUserId(int user_id)
        {
            DataAccess dal = new DataAccess();
            List<kycidstatus> ky = new List<kycidstatus>();
            DataTable dtbl = dal.ExecuteDataTable("usp_SelectKycByUserId", dal.Parameter("_user_id", user_id.ToString())
                                                                       
                );
            if (dtbl != null && dtbl.Rows.Count > 0)
            {
                for (int i = 0; i < dtbl.Rows.Count; i++)
                {
                    kycidstatus kycsts = new kycidstatus();
                    kycsts.kyc_id = Convert.ToInt64(dtbl.Rows[i]["kyc_id"].ToString());
                    kycsts.approval_status = Convert.ToInt32(dtbl.Rows[i]["approval_status"].ToString());
                    kycsts.reject_reason = dtbl.Rows[i]["reject_reason"].ToString();
                    kycsts.reject_remarks = dtbl.Rows[i]["reject_remarks"].ToString();



                    ky.Add(kycsts);
                }
            }
            return ky;
        }
        public List<UserAllocation> SelectUserAllocation(int user_id, int areaid)
        {
            DataAccess dal = new DataAccess();
            List<UserAllocation> UA = new List<UserAllocation>();
            DataTable dtbl = dal.ExecuteDataTable("usp_SelectUserAllocation", dal.Parameter("p_userid", user_id)
                                                                              , dal.Parameter("p_userrole", areaid));
            if (dtbl != null && dtbl.Rows.Count > 0)
            {
                for (int i = 0; i < dtbl.Rows.Count; i++)
                {
                    UserAllocation usrall = new UserAllocation();
                    usrall.userAreaID = Convert.ToInt64(dtbl.Rows[i]["userAreaID"].ToString());
                    usrall.areaid = Convert.ToInt32(dtbl.Rows[i]["areaid"].ToString());
                    usrall.state = dtbl.Rows[i]["State"].ToString();
                    usrall.district = dtbl.Rows[i]["District"].ToString();
                    usrall.city = dtbl.Rows[i]["City"].ToString();
                    usrall.area = dtbl.Rows[i]["Area"].ToString();
                    usrall.pincode = dtbl.Rows[i]["PinCode"].ToString();



                    UA.Add(usrall);
                }
            }
            return UA;
                     
           
        }
        public string UserLogin(user_security us)
        {
            string dbpwd = "";
            string dbuser_id = "";

            try
            {
                DataAccess dal = new DataAccess();
                DataTable dt = dal.ExecuteDataTable("usp_UserSelect", dal.Parameter("p_userid", 0),
                                                dal.Parameter("p_username", us.user_name),
                                                dal.Parameter("p_QryTyp", 2));
               // cls_Staticvalues cl = new cls_Staticvalues();
              //  cl.UserRole = dt.Rows[0][3].ToString();
              //  cl.UserID = int.Parse(dt.Rows[0][0].ToString());
                if (dt.Rows.Count > 0)
                {
                    dbpwd = dt.Rows[0][2].ToString();
                    dbuser_id = dt.Rows[0][0].ToString();

                    //DataTable dtb = new DataTable();
                    //dtb = SelectKycByUserId(dbuser_id.ToString());


                }
                else
                {
                    return "Login Failure";
                }
            }
            catch (Exception ex)
            {
                return "Error: Connecting the Server" + ex.Message;
            }

            if (BCrypt.Net.BCrypt.Verify(us.password, dbpwd))
            {
                return "Success," + dbuser_id;
            }
            else
            {
                return "Login Failure";
            }
        }
      
 
    }
}