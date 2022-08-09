using VendingData.Models;

namespace VendingData.Repositories
{
    public interface IOrderRepository
    {
        int CreateOrder(Order order);
        decimal GetTotalAmount(Order order);
        void Validateorder(int productId, int quantity, out bool orderValid);

        void Validateorder(Order order, out bool orderValid);
        decimal GetTotalAmount(Dictionary<int, int> orderProdIds);
    }
    public class OrderRepository : IOrderRepository
    {
        private VendingDbContext _context;

        public OrderRepository(VendingDbContext context)
        {
            _context= context;
        }
        public void Validateorder(int productId, int quantity, out bool orderValid)
        {
            orderValid=false;
            var prods = (from p in _context.MasterProducts
                         where p.ID==productId && p.Quantity>=quantity
                         select p).ToList();
            orderValid=prods.Any();
        }
        public int CreateOrder(Order order)
        {
            var productids = order.OrderDetails.Select(x => x.ProductID).ToArray();

            bool orderValid;
            Validateorder(order, out orderValid);
           
            if (!orderValid)
            {
                throw new Exception($"Product that you order is out of stock, please check your order again.");
            }


            List<MasterProduct> validateProducts;

            validateProducts = (from p in _context.MasterProducts
                                where productids.Contains(p.ID)
                                && p.Quantity > 0
                                select p
                                    ).ToList();
            
            order.TotalAmount= order.OrderDetails.Sum(x=> x.Quantity* x.Price);

            _context.Orders.Add(order);
            int re = _context.SaveChanges();
            if (re > 0)
            {
                foreach (var item in validateProducts)
                {
                    var orderForthisproduct = order.OrderDetails.FirstOrDefault(x => x.ProductID == item.ID);
                    if (orderForthisproduct != null)
                    {
                        item.Quantity = item.Quantity - orderForthisproduct.Quantity;
                    }
                }
                _context.MasterProducts.AttachRange(validateProducts);
                re += _context.SaveChanges();
            }
            return re;

        }

        public void Validateorder(Order order, out bool orderValid)
        {
            var productids = order.OrderDetails.Select(x => x.ProductID).ToArray();

           var validateProducts = (from p in _context.MasterProducts
                                where productids.Contains(p.ID)
                                && p.Quantity > 0
                                select p
                                    ).ToList();
            if (validateProducts.Count != productids.Length)
            {
                orderValid=false;
                return ;
            }


            orderValid = true;
            foreach (var item in validateProducts)
            {
                var whatsOnOrder = order.OrderDetails.FirstOrDefault(x => x.ProductID == item.ID);
                if (item.Quantity < whatsOnOrder.Quantity)
                {
                    orderValid = false;
                    break;
                }
            }
        }

        public decimal GetTotalAmount(Order order)
        {
            decimal re = 0;

            re=order.OrderDetails.Sum(x=> x.Quantity * x.Price);
            return re;
        }

        public decimal GetTotalAmount(Dictionary<int,int> orderProdIds)
        {
            decimal amt = 0;
            foreach (var item in orderProdIds)
            {
               var product = _context.MasterProducts.Where(x=> x.ID== item.Key ).FirstOrDefault();
               amt+= product.Price*item.Value;
            }
            return amt;
        }
    }
}
