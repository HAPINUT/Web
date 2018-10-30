using shanuMVCUserRoles.DTO;
using System;

namespace shanuMVCUserRoles.Models.Shop
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {

        }

        public OrderViewModel(Order order)
        {
            Id = order.Id;
            UserId = order.UserId;
            OrderDate = order.OrderDate;
        }

        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime? OrderDate { get; set; }


    }
}