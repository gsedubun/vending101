using VendingData.Models;

namespace VendingData.Repositories
{
    public interface IPaymentRepository
    {
        int CreatePayment(List<PaymentDenos> payments, int orderId);
    }

    public class PaymentRepository : IPaymentRepository
    {
        private VendingDbContext _context;

        public PaymentRepository(VendingDbContext context)
        {
            _context = context;
        }

        public int CreatePayment(List<PaymentDenos> payments, int orderId)
        {
            decimal total  = payments.Sum(x=> x.Quantity* x.BankNotes);
            var payment = new Payment { OrderID=orderId, TotalAmount=total };
            _context.Payments.Add(payment);
           int re= _context.SaveChanges();
            return re;
        }
    }
}
