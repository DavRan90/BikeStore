using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeStore.Models;

namespace BikeStore
{
    internal class Db
    {
        public static void RemoveFeaturedProduct(int id)
        {
            using (var db = new BikeStoreContext())
            {
                var productList = db.FeaturedProducts;
                var scrapFeaturedProduct = (from c in productList
                                            where c.Id == id
                                            select c).SingleOrDefault();
                if (scrapFeaturedProduct != null)
                {
                    productList.Remove(scrapFeaturedProduct);
                }
                db.SaveChanges();
            }
        }
        public static void RemoveFeaturedProductByProductId(int id)
        {
            using (var db = new BikeStoreContext())
            {
                var productList = db.FeaturedProducts;
                var scrapFeaturedProduct = (from c in productList
                                            where c.ProductId == id
                                            select c).SingleOrDefault();
                if (scrapFeaturedProduct != null)
                {
                    productList.Remove(scrapFeaturedProduct);
                }
                db.SaveChanges();
            }
        }
        public static void AddFeaturedProduct(string name, int id)
        {
            using (var db = new BikeStoreContext())
            {
                db.AddRange(
                    new FeaturedProduct
                    {
                        Name = name,
                        ProductId = id
                    });
                db.SaveChanges();
            }
        }
        public static Order AddOrder(Order order)
        {
            using(var db = new BikeStoreContext())
            {
                db.Add(order);
                db.SaveChanges();
                return order;
            }
        }
        public static void AddOrderDetails(OrderDetail orderDetail)
        {
            using (var db = new BikeStoreContext())
            {
                db.Add(orderDetail);
                db.SaveChanges();
            }
            
        }
        public static void AddCustomer(string personNr, string name, string city, string street, string zipCode, string email, string password)
        {
            using (var db = new BikeStoreContext())
            {
                db.AddRange(
                    new Customer
                    {
                        PersonNummer = personNr,
                        Name = name,
                        City = city,
                        Street = street
                    });
                db.SaveChanges();
            }
        }
        public static void AddCustomer(Customer customer)
        {
            using (var db = new BikeStoreContext())
            {
                db.Add(customer);
                db.SaveChanges();
            }
        }
        public static void AddCategory(Category category)
        {
            using (var db = new BikeStoreContext())
            {
                db.Add(category);
                db.SaveChanges();
            }
        }
        public static void AddProduct(Product product)
        {
            using (var db = new BikeStoreContext())
            {
                db.Add(product);
                db.SaveChanges();
            }
        }
        public static void EditCustomer(Customer customer)
        {
            using (var db = new BikeStoreContext())
            {
                db.Update(customer);
                db.SaveChanges();
            }
        }
        public static void AddProductToCart(Product product, int userId)
        {
            using (var db = new BikeStoreContext())
            {
                
                db.Add(
                    new Cart
                    {
                        ProductId = product.Id.ToString(),
                        UserId = userId,
                        Amount = 1
                    });
                db.SaveChanges();
            }
        }
        public static void RemoveCategory(int id)
        {
            using (var db = new BikeStoreContext())
            {
                var categoriesList = db.Categories;
                var scrapProduct = (from c in categoriesList
                                    where c.Id == id
                                    select c).SingleOrDefault();
                if (scrapProduct != null)
                {
                    categoriesList.Remove(scrapProduct);
                }
                db.SaveChanges();
            }
        }
        public static void RemoveProduct(int id)
        {
            using (var db = new BikeStoreContext())
            {
                var productList = db.Products;
                var scrapProduct = (from c in productList
                                            where c.Id == id
                                            select c).SingleOrDefault();
                if (scrapProduct != null)
                {
                    productList.Remove(scrapProduct);
                }
                db.SaveChanges();
            }
        }
        public static void AddCart(Cart cart)
        {
            using (var db = new BikeStoreContext())
            {
                db.Add(cart);
                db.SaveChanges();
            }
        }
        public static void AddProduct(string name, int categoryId, decimal? price, int stock)
        {
            using (var db = new BikeStoreContext())
            {
                db.AddRange(
                    new Product
                    {
                        Name = name,
                        CategoryId = categoryId,
                        Price = price,
                        Stock = stock
                    });
                db.SaveChanges();
            }
        }
    }
}
