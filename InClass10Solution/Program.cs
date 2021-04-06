using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InClass10
{
    public class Product
    {
        public string ID { get; set; }
        public string productname { get; set; }
        public double price { get; set; }
        public ICollection<OrderDetail> relatedorders { get; set; }
    }

    public class Order
    {
        public string ID { get; set; }
        public DateTime orderdate { get; set; }
        public string customerName { get; set; }
        public ICollection<OrderDetail> orderedproducts { get; set; }
    }


    public class OrderDetail
    {
        public string ID { get; set; }
        public Order order { get; set; }
        public Product product { get; set; }
        public int quantity { get; set; }
    }

    class ApplicationDbContext : DbContext
    {
        public DbSet<Product> products { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderDetail> orderDetails { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=InClass10;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Database.EnsureCreated();
                Product[] plist = new Product[]
                {
                    new Product{ID="P01",productname="Basmati Rice 20 pounds",price=20.00},
                    new Product{ID="P02",productname="Sugar 10 pounds",price=8.00},
                    new Product{ID="P03",productname="Oats 1 pound",price=9.00},
                    new Product{ID="P04",productname="Masoor dal whole 2 pound",price=5.00},
                    new Product{ID="P05",productname="Banana 12 piece",price=3.00}
                };

                Order[] olist = new Order[]
                {
                    new Order{ID="O01",orderdate=DateTime.Parse("2021-03-01"),customerName="customer1"},
                    new Order{ID="O02",orderdate=DateTime.Parse("2021-02-26"),customerName="customer2"},
                    new Order{ID="O03",orderdate=DateTime.Parse("2021-03-12"),customerName="customer3"},
                    new Order{ID="O04",orderdate=DateTime.Parse("2021-04-01"),customerName="customer3"},
                    new Order{ID="O05",orderdate=DateTime.Parse("2021-03-20"),customerName="customer4"}
                };

                OrderDetail[] dlist = new OrderDetail[]
                {
                    new OrderDetail{ID="O01P01",order=olist[0],product=plist[0],quantity=2},
                    new OrderDetail{ID="O01P03",order=olist[0],product=plist[2],quantity=1},
                    new OrderDetail{ID="O01P04",order=olist[0],product=plist[3],quantity=5},
                    new OrderDetail{ID="O02P01",order=olist[1],product=plist[0],quantity=2},
                    new OrderDetail{ID="O02P05",order=olist[1],product=plist[4],quantity=1},
                    new OrderDetail{ID="O03P02",order=olist[2],product=plist[1],quantity=4},
                    new OrderDetail{ID="O03P04",order=olist[2],product=plist[3],quantity=1},
                    new OrderDetail{ID="O03P05",order=olist[2],product=plist[4],quantity=5},
                    new OrderDetail{ID="O04P01",order=olist[3],product=plist[0],quantity=1},
                    new OrderDetail{ID="O04P02",order=olist[3],product=plist[1],quantity=4},
                    new OrderDetail{ID="O04P03",order=olist[3],product=plist[2],quantity=3},
                    new OrderDetail{ID="O04P04",order=olist[3],product=plist[3],quantity=4},
                    new OrderDetail{ID="O04P05",order=olist[3],product=plist[4],quantity=5},
                };
                if (!context.orders.Any())
                {
                    foreach (Order o in olist)
                    {
                        context.orders.Add(o);
                    }
                    context.SaveChanges();
                }

                if (!context.products.Any())
                {
                    foreach (Product p in plist)
                    {
                        context.products.Add(p);
                    }
                    context.SaveChanges();
                }

                if (!context.orderDetails.Any())
                {
                    foreach (OrderDetail d in dlist)
                    {
                        context.orderDetails.Add(d);
                    }
                    context.SaveChanges();
                }
                context.SaveChanges();
                // Display all orders where a product is sold
                var a = context.orders
                    .Include(c => c.orderedproducts)
                    .Where(c => c.orderedproducts.Count != 0);
                Console.WriteLine("................Order where a product is sold................");
                foreach (var i in a)
                {
                    Console.WriteLine("OrderID={0},OrderDate={1},CustomerName={2}", i.ID, i.orderdate, i.customerName);
                }

                // For a given product, find the order where it is sold the maximum.
                Order output = context.orderDetails
                    .Where(c => c.product.productname == "Masoor dal whole 2 pound")
                    .OrderByDescending(c => c.quantity)
                    .Select(c => c.order)
                    .First();
                Console.WriteLine("................Order where maximum amount of Masoor Dal has been sold............");
                Console.WriteLine("OrderID={0},OrderDate={1},CustomerName={2}", output.ID, output.orderdate, output.customerName);

                // Find the orders where a given product is sold.
                var orders = context.orderDetails
                    .Where(c => c.product.productname == "Sugar 10 pounds");
                Console.WriteLine("Sugar is sold in the following orders ");
                foreach (var i in orders)
                {
                    Console.Write(i.order.ID+" ");
                }
            }
        }
    }
}
