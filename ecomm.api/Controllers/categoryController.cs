﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AttributeRouting;
using AttributeRouting.Web.Http;
using ecomm.util.entities;
using System.Web;
using System.IO;



namespace ecomm.api.Controllers
{
    public class categoryController : ApiController
    {

       

        [POST("category/kyc/kyc_add/")]
        public List<kycid> kyc_insert(List<kyc> kyc)
        {
            ecomm.model.repository.category_repository cr = new model.repository.category_repository();
            return cr.kyc_insert(kyc);
        }

        [POST("category/admin/kyc_login/")]
        public string UserLogin(user_security us)
        {
            ecomm.model.repository.category_repository cr = new model.repository.category_repository();
            return cr.UserLogin(us);
        }

        [POST("/category/admin/SelectKycByUserId/")]
        public List<kycidstatus> SelectKycByUserId(int  user_id)
        {
            ecomm.model.repository.category_repository cr = new ecomm.model.repository.category_repository();
            return cr.SelectKycByUserId(user_id);
        }
        [POST("/category/admin/SelectUserAllocation/")]
        public List<UserAllocation> SelectUserAllocation(int user_id, int areaid)
        {
            ecomm.model.repository.category_repository cr = new ecomm.model.repository.category_repository();
            return cr.SelectUserAllocation(user_id, areaid);
        }

      

    }
}
