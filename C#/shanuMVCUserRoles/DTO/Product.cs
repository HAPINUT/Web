using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace shanuMVCUserRoles.DTO
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Decription { get; set; }
        public decimal? Price { get; set; }
        public string CategoryName { get; set; }
        public int? CategoryId { get; set; }
        public string Image { get; set; }
    }
}