using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VendingData.Models;
using VendingData.Repositories;

namespace VendingConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var vendingMachine= host.Services.GetService<IVendingMachineRepository>();
            var orderRepo = host.Services.GetService<IOrderRepository>();


            while (true)
            {

                Console.WriteLine("\n" +
                    "1. Monhtly Report.\n" +
                    "2. Vending Machine.\n" +
                    "3. Exit Program.");

                Console.Write("Menu : ");
                string menu = Console.ReadLine();

                switch (menu)
                {
                    case "2":
                        Console.WriteLine("*****Vending Machine 101*****");
                        VendingMachine(vendingMachine,orderRepo);
                        break;
                    case "1":
                        MonthlyReport(vendingMachine, orderRepo);
                            break;
                    case "3":
                        Environment.Exit(0);
                        break;

                    default:
                        break;
                }
               
                Console.Clear();
            }
            
        }
        static void MonthlyReport(IVendingMachineRepository vendingMachine, IOrderRepository orderRepo)
        {
            
                var data  = vendingMachine.MonthlyReport();
                System.Console.WriteLine($@"{"Date".PadLeft(12)}|{"Item".PadLeft(15)}|{"Quantity".PadLeft(10)}|{"SubTotal".PadLeft(12)}|{"Total".PadLeft(12)}|{"Paid".PadLeft(12)}|{"Change".PadLeft(12)}|");
                System.Console.WriteLine("".PadLeft(91,'_'));
                foreach(var item in data){
                    string rows =$@"{item.Date.PadLeft(12)}|{item.Item.PadLeft(15)}|{item.Quantity.PadLeft(10)}|{item.SubTotal.PadLeft(12)}|{item.Total.PadLeft(12)}|{item.Paid.PadLeft(12)}|{item.Change.PadLeft(12)}|";
                System.Console.WriteLine(rows);
                }
                System.Console.WriteLine("".PadLeft(91,'_'));

                Console.WriteLine("\n Press Any key to continue!");

                Console.ReadKey();

        }
        static void VendingMachine(IVendingMachineRepository vendingMachine,IOrderRepository orderRepo)
        {
            var productlist = vendingMachine.AvailableProducts();
            ShowProductList(productlist);
            Dictionary<int, int> orderProdIds = new Dictionary<int, int>();
            while (true)
            {
                Console.Write("Select which product id to order :");
                string prodid = Console.ReadLine();
                Console.Write("How many (1,2..) :");
                string quantity = Console.ReadLine();
                if (quantity == "0")
                    continue;

                int qty=int.Parse(quantity);

                if (orderProdIds.ContainsKey(int.Parse(prodid)))
                {
                    int existingval;
                    orderProdIds.TryGetValue(int.Parse(prodid), out existingval);

                    orderProdIds.Remove(int.Parse(prodid));
                    qty +=existingval;
                    
                }

                bool orderValid=false;
                orderRepo.Validateorder(int.Parse(prodid), qty, out orderValid);
                if (orderValid)
                {
                    //    orderProdIds.Add(int.Parse(prodid), existingval);
                    orderProdIds.Add(int.Parse(prodid), qty);
                }
                else
                {
                    Console.WriteLine($"Stock for product {prodid} is not enough.");
                    continue;
                }


                Console.Write("Finish order (Y/N)? ");
                string finish = Console.ReadLine();
                if (finish.ToLower() == "y")
                    break;
            }

            decimal totalAmount = orderRepo.GetTotalAmount(orderProdIds);
            Console.WriteLine($"Your total order amount :{totalAmount.ToString("C")}");

            Console.WriteLine($"=================PAYMENT====================");
            Console.WriteLine($"Accepted denomination are :100000, 50000, 20000, 10000 ");
            Dictionary<decimal, int> paymentdenos = new Dictionary<decimal, int>();
            while (true)
            {
                string[] alloweddenom = new[] { "100000", "50000", "20000", "10000" };
                Console.Write("What denomination : ");
                string denomination = Console.ReadLine();
                if (!alloweddenom.Contains(denomination))
                {
                    Console.WriteLine($"{denomination} enomination is not accepted.");
                    continue;
                }
                Console.Write("How many : ");
                string qty = Console.ReadLine();
                if (qty == "0")
                    continue;
                if (paymentdenos.ContainsKey(decimal.Parse(denomination)))
                {
                    int existingval;
                    paymentdenos.TryGetValue(decimal.Parse(denomination), out existingval);
                    paymentdenos.Remove(decimal.Parse(denomination));
                    existingval += int.Parse(qty);
                    paymentdenos.Add(decimal.Parse(denomination), existingval);
                }
                else
                {
                    paymentdenos.Add(decimal.Parse(denomination), int.Parse(qty));
                }
                if (CalCulateTotalPay(paymentdenos) >= totalAmount)
                {
                    break;
                }
            }



            Order order = new Order() { OrderDate = DateTime.Now };
            order.OrderDetails = new List<OrderDetail>();
            foreach (var item in orderProdIds)
            {
                decimal price = 0;
                var prod = productlist.FirstOrDefault(x => x.ID == item.Key);
                if (prod != null)
                    price = prod.Price;
                order.OrderDetails.Add(new OrderDetail() { ProductID = item.Key, Quantity = item.Value, Price = price });
            }

            List<PaymentDenos> payment = new List<PaymentDenos>();
            foreach (var item in paymentdenos)
            {
                payment.Add(new PaymentDenos() { BankNotes = item.Key, Quantity = item.Value });
                System.Console.WriteLine($"Denomination : {item.Key}, Quantity :{item.Value}");
            }

            vendingMachine.Transaction(order, payment);

            System.Console.WriteLine("=================Denominations====================");
            IEnumerable<MoneyBox> denomList = vendingMachine.DenominationList();
            PrintDenomination(denomList);
            
            Console.WriteLine("Thank you.\n Press Any key to continue!");

            Console.ReadKey();
        }
        

        private static decimal CalCulateTotalPay(Dictionary<decimal, int> paymentdenos)
        {
            decimal totalAmount = 0;
            foreach (var item in paymentdenos)
            {
                totalAmount+= item.Key*item.Value;
            }
            return totalAmount;
        }
        private static void PrintDenomination(IEnumerable<MoneyBox> moneyBoxes){
            Console.WriteLine($"|{"Denomination".PadLeft(12)}|{"Quanity".PadLeft(10)}|");
            Console.WriteLine("".PadLeft(25,'_'));
    foreach (var item in moneyBoxes)
    {
string rows=$@"|{item.Denomination.ToString().PadLeft(12)}|{item.Quantity.ToString().PadLeft(10)}|";
System.Console.WriteLine(rows);
    }
            Console.WriteLine("".PadLeft(25,'_'));


        }
        private static void ShowProductList(IEnumerable<MasterProduct> productlist)
        {
            //Console.WriteLine($"|Product ID|Product.Name|Price|Quantity|");
            Console.WriteLine($"|{"Product ID".PadLeft(12)}|{"Product Name".PadLeft(15)}|{"Price".PadLeft(12)}|{"Quantity".PadLeft(10)}|");
            Console.WriteLine("_".PadLeft(52,'_'));

            foreach (var item in productlist)
            {
                Console.WriteLine($"|{item.ID.ToString().PadLeft(12)} |{item.Name.ToString().PadLeft(15)}|{item.Price.ToString().PadLeft(12)}|{item.Quantity.ToString().PadLeft(10)}|");
            }

        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddJsonFile("appsettings.json");
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                })
                .ConfigureServices((context, services) =>
                {
                    //add your service registrations

                    services.AddDbContext<VendingDbContext>(opt =>
                        opt.UseSqlServer(AppSettings.ConnectionString));
                    
                    services.AddSingleton<IPaymentRepository, PaymentRepository>();
                    services.AddSingleton<IOrderRepository, OrderRepository>();
                    services.AddSingleton<IMoneyBoxRepository, MoneyBoxRepository>();
                    services.AddSingleton<IVendingMachineRepository, VendingMachineRepository>();

                });

            return hostBuilder;
        }
    }
}