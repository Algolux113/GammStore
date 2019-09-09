using System;
using System.Collections.Generic;

namespace GammStore.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }

        public int AccountId { get; set; }
        public Account Account { get; set; }

        public DateTimeOffset DateTime { get; set; }
        public OrderStatus Status { get; set; }

        List<OrderBody> OrderBodies { get; set; }
    }

    public enum OrderStatus
    {
        New = 1,
        Cancel = 2
    }
}
