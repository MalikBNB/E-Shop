using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(o => o.ShippingAddress, a => a.WithOwner());
            builder.OwnsOne(o => o.PaymentSummary, p => p.WithOwner());
            builder.Property(s => s.Status).HasConversion(o => o.ToString(), o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o));
            builder.Property(s => s.Subtotal).HasColumnType("decimal(18,2)");
            builder.Property(s => s.OrderDate).HasConversion(d => d.ToUniversalTime(), d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
            builder.HasMany(o => o.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
