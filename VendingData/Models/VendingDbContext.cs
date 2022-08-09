using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VendingData.ModelConfigs;

namespace VendingData.Models
{
    public class VendingDbContext : DbContext
    {
        public VendingDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<MasterProduct> MasterProducts { get;set;}
        public DbSet<MoneyBox>  MoneyBox { get; set; }

        public DbSet<Order> Orders{ get;set;}
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new MoneyBoxConfiguration());
            modelBuilder.ApplyConfiguration(new MasterProductConfiguration());
        }
    
        
    }


    public class Order
    {
        public int ID { get;set;}
        public DateTime OrderDate { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get;set;}

        public decimal TotalAmount { get;set;}

    }
    public class OrderDetail
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public decimal Price { get;set;}
        public int Quantity { get;set;}

    }

    public class Payment
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public decimal TotalAmount { get;set;}
        //public int Quantity { get;set;}
    }
public class PaymentDenos { 
    public decimal BankNotes { get; set; }
    public int Quantity { get; set; }
}

    public class MonthlyReport
    {
        public string? Date { get;  set; }
        public string?Item { get;  set; }
        public string Quantity { get;  set; }
        public string SubTotal { get;  set; }
        public string Total { get;  set; }
        
        public string Paid { get;  set; }
        public string Change { get;  set; }


    }


    public class MoneyBox {
        public int ID { get; set; }
        public decimal Denomination { get; set; }
        public int Quantity { get; set; }
    }
    public class MasterProduct
    {
        public int ID { get;set;}
        public string Name { get;set;}
        public decimal Price { get;set;}
        public int Quantity { get;set;}
    }
}
