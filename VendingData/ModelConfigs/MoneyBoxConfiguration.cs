using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VendingData.Models;

namespace VendingData.ModelConfigs
{
    public class MoneyBoxConfiguration : IEntityTypeConfiguration<MoneyBox>
    {
        public void Configure(EntityTypeBuilder<MoneyBox> builder)
        {
            builder.HasData(new MoneyBox[]
            {
                new MoneyBox{ ID=1, Denomination=100000, Quantity=10},
                new MoneyBox{ ID=2, Denomination=50000, Quantity=10},
                new MoneyBox{ ID=3, Denomination=20000, Quantity=100},

                new MoneyBox{ ID=4, Denomination=10000, Quantity=100},
                new MoneyBox{ ID=5, Denomination=5000, Quantity=100},
                new MoneyBox{ ID=6, Denomination=2000, Quantity=100},
                new MoneyBox{ ID=7, Denomination=1000, Quantity=100},
            });
        }
    }

    public class MasterProductConfiguration : IEntityTypeConfiguration<MasterProduct>
    {
        public void Configure(EntityTypeBuilder<MasterProduct> builder)
        {
            builder.HasData(new MasterProduct[]
            {
                new MasterProduct{ ID=1, Name="Mineral Water", Price=4000, Quantity=10},
                new MasterProduct{ ID=2, Name="Soft Drink", Price=8000, Quantity=10},
                new MasterProduct{ ID=3, Name="Milk", Price=6000, Quantity=10},
                new MasterProduct{ ID=4, Name="Coffee", Price=10000, Quantity=10}
            });
        }
    }
}
