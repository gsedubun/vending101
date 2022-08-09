using VendingData.Models;
using System.Collections.Generic;

namespace VendingData.Repositories
{
    public interface IMoneyBoxRepository
    {
        int UpdateMoneyBox(List<PaymentDenos> payment, decimal totalOrderAmount);

    }
    public class MoneyBoxRepository : IMoneyBoxRepository
    {
        private VendingDbContext _context;

        public MoneyBoxRepository( VendingDbContext context )
        {
            _context=context;
        }
        

        public int UpdateMoneyBox(List<PaymentDenos> payment, decimal totalOrderAmount)
        {
            var totalpayment = payment.Sum(x=> x.BankNotes * x.Quantity);
            if ((totalpayment) >= totalOrderAmount)
            {

                int re = 0;
                foreach (var p in payment)
                {
                    // ADD MONEYBOX DENOMINATIONS QUANTITY
                    var addMoneyBox = _context.MoneyBox.Where(x => x.Denomination == p.BankNotes).FirstOrDefault();
                    if (addMoneyBox != null)
                    {
                        addMoneyBox.Quantity += p.Quantity;
                        _context.MoneyBox.Attach(addMoneyBox);
                        var ent = _context.Entry(addMoneyBox);
                        ent.Property(x => x.Quantity).IsModified = true;
                    }
                }
                re += _context.SaveChanges();


               
                // DEDUCT MONEYBOX DENOM QUANTITY
                decimal change = totalpayment - totalOrderAmount;
                if (change > 0)
                {

                    foreach (var p in payment)
                    {
                        while (change > 0) { 
                            var existingmoneybox = _context.MoneyBox.OrderByDescending(x=> x.Denomination).ToList();
                            foreach (var item in existingmoneybox)
                            {
                                if (change - item.Denomination >= 0)
                                {
                                    item.Quantity -= 1;
                                    _context.MoneyBox.Attach(item);
                                    var ent = _context.Entry(item);
                                    ent.Property(x => x.Quantity).IsModified = true;
                                    change=change-item.Denomination;
                                    break;
                                }
                            }

                            re += _context.SaveChanges();

                        }
                    }

                }
                return re;
            }
            throw new Exception($"Total payment is not enough.");
        }
    }
}
