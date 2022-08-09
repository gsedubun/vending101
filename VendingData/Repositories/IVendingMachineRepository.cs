using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VendingData.Models;

namespace VendingData.Repositories
{
    public interface IVendingMachineRepository 
    {
         IEnumerable<MasterProduct> AvailableProducts();
        IEnumerable<MonthlyReport> MonthlyReport();
        int Transaction(Order order, List<PaymentDenos> payment);
        IEnumerable<MoneyBox> DenominationList();
    }

    public class VendingMachineRepository : IVendingMachineRepository
    {
        private VendingDbContext _context;
        private IMoneyBoxRepository _moneyBoxRepository;
        private IOrderRepository _orderRepository;
        private IPaymentRepository _paymentRepository;

        public VendingMachineRepository(
            VendingDbContext context, 
            IMoneyBoxRepository moneyBoxRepository, 
            IOrderRepository orderRepository,
            IPaymentRepository paymentRepository)
        {
            _context = context;
            _moneyBoxRepository = moneyBoxRepository;
            _orderRepository = orderRepository;
            _paymentRepository= paymentRepository;
        }

        public IEnumerable<MasterProduct> AvailableProducts()
        {
            var _fr = (from c in _context.MasterProducts
                        where c.Quantity>0
                        select new MasterProduct { ID=c.ID, Name=c.Name, Quantity=c.Quantity, Price=c.Price}
                       ).ToList();

            return _fr;
        }

        public int Transaction(Order order, List<PaymentDenos> payment)
        {
            int re = 0;
            re = _orderRepository.CreateOrder(order);

            decimal totalorderamt = _orderRepository.GetTotalAmount(order);

            

            re += _moneyBoxRepository.UpdateMoneyBox(payment, totalorderamt);
            

            re+= _paymentRepository.CreatePayment(payment, order.ID);
            
            return re;
        }

        public IEnumerable<MonthlyReport> MonthlyReport()
        {
            
            SqlConnection myConnection = new SqlConnection(AppSettings.ConnectionString);
            myConnection.StatisticsEnabled = true;
            myConnection.Open();
            string query = $@"select Format(o.OrderDate,'dd/MM/yyyy') as [Date]
,mp.[Name] as [Item]
,SUM(os.Quantity ) as Quantity
,(os.Quantity * os.Price) SubTotal
,Sum(o.TotalAmount) as Total
,p.TotalAmount as Paid
,p.TotalAmount-o.TotalAmount as Change
 from orders o
join orderdetails os on o.id=os.OrderID
join MasterProducts mp on os.ProductID=mp.ID
join payments p on o.ID=p.OrderID
group by Format(o.OrderDate,'dd/MM/yyyy') 
,mp.[Name]
,(os.Quantity * os.Price)
,p.TotalAmount,o.TotalAmount ;";
            SqlCommand myCommand = new SqlCommand(query, myConnection);
            try
            {
               myCommand.CommandTimeout = 60;
               SqlDataReader myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
               var stats = myConnection.RetrieveStatistics();
               //logger.Debug($"Executed Query : {myCommand.CommandText}\nFor {stats["ExecutionTime"]} ms.");

               List<MonthlyReport> res = new List<MonthlyReport>();

               while( myReader.Read()){
                MonthlyReport mr = new MonthlyReport();
                mr.Date=myReader["Date"].ToString();
                mr.Item=myReader["Item"].ToString();

                mr.Quantity = myReader["Quantity"].ToString();
                mr.SubTotal=myReader["SubTotal"].ToString();
                mr.Total=myReader["Total"].ToString();
                mr.Paid=myReader["Paid"].ToString();
                mr.Change=myReader["Change"].ToString();
                res.Add(mr);
               }
               return res;
            }
            catch (Exception e)
            {
               throw e;
            }
            finally
            {
               myConnection.Close();
            }
        }

        public IEnumerable<MoneyBox> DenominationList()
        {
            var data = _context.MoneyBox.ToList();
            return data;
        }
    }
    
}
