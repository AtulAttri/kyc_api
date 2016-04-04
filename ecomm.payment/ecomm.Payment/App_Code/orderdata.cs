using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationKit
{

    public class size
    {
        public string id { get; set; }
        public string size_name { get; set; }
        public string sku { get; set; }
        public int stock { get; set; }
    }
    public class orderdata
    {
        public string cart_item_id { get; set; }
        public string id { get; set; }
        public string sku { get; set; }
        public string brand { get; set; }
        public string quantity { get; set; }
        public string mrp { get; set; }
        public string final_offer { get; set; }
        public string name { get; set; }
        public imgurl image { get; set; }
        public string discount { get; set; }
        public string express { get; set; }
        public List<size> sizes { get; set; }
    }

    public class imgurl
    {
        public string display_name { get; set; }
        public string link { get; set; }
        public string thumb_link { get; set; }
        public string zoom_link { get; set; }
    }
}