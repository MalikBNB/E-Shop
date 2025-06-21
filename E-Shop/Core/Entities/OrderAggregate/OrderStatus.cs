using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.OrderAggregate
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,

        [EnumMember(Value = "Payment Recieved")]
        PaymentRecieved,

        [EnumMember(Value = "Payment Faild")]
        PaymentFaild,

        // [EnumMember(Value = "")] --> returns the specified value when we call the enum insted of 0, 1, 2...
    }
}
