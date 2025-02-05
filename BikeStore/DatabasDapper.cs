using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeStore.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BikeStore
{
    internal class DatabasDapper
    {
        // Lokalt
        static string connString = "data source=.\\SQLEXPRESS; initial catalog=BikeStore1; " +
            "TrustServerCertificate=true; persist security info=True; Integrated Security=True";

        // Azure
        //static string connString = "Server=tcp:daradb1.database.windows.net,1433;Initial Catalog = daradb1; Persist Security Info=False;User ID = daraad; Password=Qchpw9pc;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout = 30;";

        public static Models.Product GetProduct(int id)
        {
            string sql = $"SELECT * FROM Products Where ID = {id}";
            Models.Product product = new Models.Product();

            using (var connection = new SqlConnection(connString))
            {
                product = connection.QuerySingleOrDefault<Product>(sql);
            }

            return product;
        }
        public static void UpdateCartOwner(int oldId, int newId)
        {
            string sql = $"UPDATE Cart SET UserId = {newId} WHERE UserId = '{oldId}'";

            using (var connection = new SqlConnection(connString))
            {
                connection.Query(sql);
            }
        }
        public static void UpdateStock(int amountInStock, int productId)
        {
            string sql = $"UPDATE Products SET Stock = {amountInStock} WHERE ID = {productId}";

            using (var connection = new SqlConnection(connString))
            {
                connection.Query(sql);
            }
        }
        public static void UpdateStockAfterPurchase(int purchasedAmount, int productId)
        {
            string sql = $"UPDATE Products SET Stock -= {purchasedAmount} WHERE ID = {productId}";

            using (var connection = new SqlConnection(connString))
            {
                connection.Query(sql);
            }
        }
        public static List<Models.Product> GetProducts(int id)
        {
            string sql = $"SELECT * FROM Products WHERE CategoryId = {id}";
            List<Models.Product> product = new List<Models.Product>();

            using (var connection = new SqlConnection(connString))
            {
                product = connection.Query<Models.Product>(sql).ToList();
            }
            return product;
        }
        public static List<Models.OrderDetail> GetOrderDetails(int orderId)
        {
            string sql = $"SELECT * FROM OrderDetails WHERE OrderId = {orderId}";
            List<Models.OrderDetail> orderLines = new List<Models.OrderDetail>();

            using (var connection = new SqlConnection(connString))
            {
                orderLines = connection.Query<Models.OrderDetail>(sql).ToList();
            }
            return orderLines;
        }
        public static List<Models.Order> GetOrders(Customer customer)
        {
            string sql = $"SELECT * FROM Orders WHERE CustomerId = {customer.Id}";
            List<Models.Order> orders = new List<Models.Order>();

            using (var connection = new SqlConnection(connString))
            {
                orders = connection.Query<Models.Order>(sql).ToList();
            }
            return orders;
        }
        public static List<Models.Product> SearchProducts(string search)
        {
            string sql = $"SELECT * FROM Products WHERE Name like '%{search}%'";
            List<Models.Product> product = new List<Models.Product>();

            using (var connection = new SqlConnection(connString))
            {
                product = connection.Query<Models.Product>(sql).ToList();
            }
            return product;
        }
        public static void RemoveProductFromCart(int userId, int productId)
        {
            string sql = $"DELETE FROM Cart Where UserId = '{userId}' AND ProductId = {productId}";

            using (var connection = new SqlConnection(connString))
            {
                connection.Query(sql);
            }
        }
        public static void ClearCart(int userId)
        {
            string sql = $"DELETE FROM Cart Where UserId = '{userId}'";

            using (var connection = new SqlConnection(connString))
            {
                connection.Query(sql);
            }
        }
        public static List<Cart> GetCartList(int userId)
        {
            string sql = $"SELECT * FROM Cart Where UserId = {userId}";
            List<Models.Cart> cartList = new List<Models.Cart>();

            using (var connection = new SqlConnection(connString))
            {
                cartList = connection.Query<Models.Cart>(sql).ToList();
            }

            return cartList;
        }
        public static void ModifyQuantity(int userId, int productId, int newAmount)
        {
            string sql = $"UPDATE Cart SET Amount = {newAmount} WHERE UserId = '{userId}' AND ProductId = {productId}";

            using (var connection = new SqlConnection(connString))
            {
                connection.Query(sql);
            }
        }
        public static int GetNumberOfProductsInCart(int userId)
        {
            string sql = $"SELECT * FROM Cart Where UserId = {userId}";
            List<Models.Cart> product = new List<Models.Cart>();

            using (var connection = new SqlConnection(connString))
            {
                product = connection.Query<Models.Cart>(sql).ToList();
            }

            return product.Count;
        }
        public static async Task<List<Product>> GetAllProductsAsync()
        {
            string sql = $"SELECT * FROM Products";
            List<Models.Product> product = new List<Models.Product>();

            await using (var connection = new SqlConnection(connString))
            {
                product = connection.Query<Models.Product>(sql).ToList();
            }

            return product;
        }
        public static List<Models.Product> GetAllProducts()
        {
            string sql = $"SELECT * FROM Products";
            List<Models.Product> product = new List<Models.Product>();

            using (var connection = new SqlConnection(connString))
            {
                product = connection.Query<Models.Product>(sql).ToList();
            }

            return product;
        }
        public static List<Models.FeaturedProduct> GetFeaturedProducts()
        {
            string sql = $"SELECT * FROM FeaturedProducts";
            List<Models.FeaturedProduct> products = new List<Models.FeaturedProduct>();

            using (var connection = new SqlConnection(connString))
            {
                products = connection.Query<Models.FeaturedProduct>(sql).ToList();
            }

            return products;
        }
        public static Customer GetCustomer(int customerId)
        {
            string sql = $"SELECT * FROM Customers WHERE Id = {customerId}";
            Customer customer = new Customer();

            using (var connection = new SqlConnection(connString))
            {
                customer = connection.QuerySingleOrDefault<Customer>(sql);
            }
            return customer;
        }
        public static List<Models.Customer> GetAllCustomers()
        {
            string sql = $"SELECT * FROM Customers";
            List<Models.Customer> customers = new List<Models.Customer>();

            using (var connection = new SqlConnection(connString))
            {
                customers = connection.Query<Models.Customer>(sql).ToList();
            }
            return customers;
        }
        public static List<Models.Category> GetAllCategories()
        {
            string sql = $"SELECT * FROM Categories";
            List<Models.Category> categories = new List<Models.Category>();

            using (var connection = new SqlConnection(connString))
            {
                categories = connection.Query<Models.Category>(sql).ToList();
            }
            return categories;
        }
        public static Category GetCategory(int? id)
        {
            string sql = $"SELECT * FROM Categories WHERE ID = {id}";
            Category category = new Category();

            using (var connection = new SqlConnection(connString))
            {
                category = connection.QuerySingleOrDefault<Category>(sql);
            }
            if (category != null)
            {
                return category;
            }
            else
            {
                category = new Category();
                return category;
            }
        }
        public static Customer SignInAsGuest()
        {
            string sql = $"SELECT * FROM Customers WHERE Id = '1'";
            Customer guest = new Customer();

            using (var connection = new SqlConnection(connString))
            {
                guest = connection.QuerySingleOrDefault<Customer>(sql);
            }

            return guest;
        }
        public static Customer AuthenticateUser(string userName) // Change since username is not required to be unique?
        {
            string sql = $"SELECT * FROM Customers WHERE Name = '{userName}'";
            Customer user = new Customer();

            using (var connection = new SqlConnection(connString))
            {
                user = connection.QuerySingleOrDefault<Customer>(sql);
            }

            return user;
        }

        public static Admin AuthenticateAdmin(string userName)
        {
            string sql = $"SELECT * FROM Admins WHERE Name = '{userName}'";
            Admin user = new Admin();

            using (var connection = new SqlConnection(connString))
            {
                user = connection.QuerySingleOrDefault<Admin>(sql);
            }

            return user;
        }
    }
}
