using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BikeStore.Models;

namespace BikeStore
{
    /// <summary>
    /// Class for drawing the elements and changes to the store
    /// </summary>
    internal class Draw
    {

        #region PageElements
        public static void ShowAll()
        {
            ShowPage();
            ShowCategories();
            ShowRecommended();
            ShowMenu();
        }
        public static void ShowPage()
        {
            Console.Clear();
            List<string> border = new List<string> { "                                                                  " };
            var windowBorder = new Window("", 1, 1, border);
            windowBorder.DrawBorder(30);

            List<string> topText = new List<string> { "# BikeStore #", "Allt inom cykling" };
            var windowTop = new Window("", 4, 3, topText);
            windowTop.Draw();

            List<string> divider = new List<string> { "                                                            " };
            var windowDivider = new Window("", 4, 8, divider);
            windowDivider.Draw();

            var windowDivider2 = new Window("", 4, 28, divider);
            windowDivider2.Draw();

            windowTop.Left = 47;
            windowTop.Draw();
        }
        public static void ShowCartBanner(int id)
        {
            int numberOfProducts = DatabasDapper.GetNumberOfProductsInCart(id);
            List<string> cartList = new List<string> { $"[V]arukorg: {numberOfProducts} " };
            var cartWindow = new Window("", 50, 8, cartList);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            cartWindow.Draw();
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void ShowLoggedInUser(string name)
        {
            List<string> cart = new List<string> { $"{name}" };
            var cartWindow = new Window("", 4, 8, cart);
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            cartWindow.Draw();
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void SetTitle(string title)
        {
            int centerPos = 35 - (title.Length / 2);
            Console.SetCursorPosition(centerPos, 9);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"{title}");
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void ResetLowerBar()
        {
            Console.SetCursorPosition(8, 29);
            Console.Write($"{"",-58}");
            Console.SetCursorPosition(8, 29);
        }
        public static void ShowMenu()
        {
            List<string> menuText = new List<string> { "[L]ogin", "[A]dmin", "[S]ök" };
            var windowMenu = new Window($"{"Meny",-8}", 52, 14, menuText);
            windowMenu.Draw();
        }
        #endregion

        #region Cart
        public static void ShowCart(Customer customer)
        {
            bool inCart = true;
            while (inCart)
            {
                SetTitle("Varukorg");
                ShowLoggedInUser(customer.Name);
                List<string> buyButton = new List<string> { "Tryck 'K' för att gå vidare med köp" };
                var buyButtonWindow = new Window("", 29, 25, buyButton);
                buyButtonWindow.Draw();
                var cart = DatabasDapper.GetCartList(customer.Id);
                List<string> cartList = new List<string>();
                decimal? cartSum = 0;
                int selectedItem = 0;
                foreach (var item in cart)
                {
                    var product = DatabasDapper.GetProduct(int.Parse(item.ProductId));
                    selectedItem++;
                    cartList.Add($"{selectedItem,-5} {item.Amount,-8} {product.Name,-25} {String.Format("{0:###,###.00}", product.Price),-19}");
                    cartSum += product.Price * item.Amount;
                }
                cartList.Add("_______________________________________________________");

                cartList.Add($"{"",-14} {"Total",-25} {String.Format("{0:###,###.00}", cartSum),-19}");
                cartSum = (cartSum * 75) / 100; // utan moms
                cartList.Add($"{"",-14} {"Total exkl. moms",-25} {String.Format("{0:###,###.00}", cartSum),-19}");

                var windowCategories = new Window($"{"Id",-5} {"Antal",-8} {"Produkt",-25} {"Pris",-9}", 4, 14, cartList);
                windowCategories.Draw();

                ResetLowerBar();
                Console.Write("Ange id för att ändra antal: ");
                var input = Console.ReadKey();
                bool num = int.TryParse(input.KeyChar.ToString(), out int cartIndex);
                if (num)
                {
                    Console.SetCursorPosition(12, 14 + cartIndex);
                    var productsInCart = DatabasDapper.GetCartList(customer.Id);
                    if (productsInCart.Count >= cartIndex)
                    {
                        Cart cartItem = productsInCart[cartIndex - 1];
                        Product product = DatabasDapper.GetProduct(int.Parse(cartItem.ProductId));
                        bool numberInput = int.TryParse(Console.ReadLine(), out int amountSelect);
                        if (numberInput && amountSelect == 0) // Remove product from cart
                        {
                            DatabasDapper.RemoveProductFromCart(customer.Id, int.Parse(cartItem.ProductId));
                        }
                        else if (amountSelect >= product.Stock)
                        {
                            ResetLowerBar();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("Not enough in stock");
                            Console.ForegroundColor = ConsoleColor.White;
                            Thread.Sleep(800);
                        }
                        else if (numberInput && amountSelect > 0)
                        {
                            DatabasDapper.ModifyQuantity(customer.Id, int.Parse(cartItem.ProductId), amountSelect);
                        }
                        //else
                        //{
                        //    return;
                        //}
                    }
                    else
                    {
                        ResetLowerBar();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Invalid ID");
                        Console.ForegroundColor = ConsoleColor.White;
                        Thread.Sleep(800);
                    }
                }
                else if (input.KeyChar == 'K' || input.KeyChar == 'k' && selectedItem > 0)
                {
                    ShowCustomerInformation(customer);
                    inCart = false;
                }
                else
                {
                    inCart = false;
                    return;
                }
            }
        }
        #endregion

        #region Shipping
        public static void ShowShipping(Customer customer)
        {
            ShowPage();
            SetTitle("Fraktval");
            ShowLoggedInUser(customer.Name);
            List<string> postNord = new List<string> { $"{"1. PostNord",-20}", "19kr" };
            var postNordWindow = new Window("Fraktalternativ 1", 4, 14, postNord);
            postNordWindow.Draw();


            List<string> instaBox = new List<string> { $"{"2. Instabox",-20}", "29kr" };
            var instaBoxWindow = new Window("Fraktalternativ 2", 4, 20, instaBox);
            instaBoxWindow.Draw();

            Console.SetCursorPosition(8, 29);
            Console.Write("Vilket leverensalternativ väljer du? ");
            bool inputNum = int.TryParse(Console.ReadLine(), out int deliveryOption);
            if (inputNum && deliveryOption > 0 && deliveryOption < 3)
            {
                int shippingCost = (deliveryOption == 1) ? shippingCost = 19 : shippingCost = 29;
                ResetLowerBar();
                Console.Write($"Du har valt alternativ {deliveryOption}");
                Thread.Sleep(1000);
                ShowPayment(customer, shippingCost);
                // Add deliverychoice
            }
            Console.ReadKey();
        }
        #endregion

        #region Payment
        public static void ShowSummary(Customer customer, int shippingCost)
        {
            ShowPage();
            SetTitle("Sammanfattning");
            ShowLoggedInUser(customer.Name);
            ShowCartBanner(customer.Id);

            var cart = DatabasDapper.GetCartList(customer.Id);
            List<string> cartList = new List<string>();
            List<OrderDetail> orderdetails = new List<OrderDetail>();
            OrderDetail orderDetail = new OrderDetail();
            
            decimal? cartSum = 0;
            int selectedItem = 0;
            foreach (var item in cart)
            {
                var product = DatabasDapper.GetProduct(int.Parse(item.ProductId));
                selectedItem++;
                cartList.Add($"{selectedItem,-5} {item.Amount,-8} {product.Name,-25} {String.Format("{0:###,###.00}", product.Price),-19}");
                orderDetail = new OrderDetail { ProductId = product.Id, UnitPrice = product.Price, Quantity = item.Amount };
                orderdetails.Add(orderDetail);
                cartSum += product.Price * item.Amount;
            }
            cartSum += shippingCost;
            cartList.Add($"{"",-14} {"Frakt",-25} {String.Format("{0:###,###.00}", shippingCost),-19}");
            cartList.Add("_______________________________________________________");

            cartList.Add($"{"",-14} {"Total",-25} {String.Format("{0:###,###.00}", cartSum),-19}");
            cartSum = (cartSum * 75) / 100; // utan moms
            cartList.Add($"{"",-14} {"Total exkl. moms",-25} {String.Format("{0:###,###.00}", cartSum),-19}");

            var windowCategories = new Window($"{"Id",-5} {"Antal",-8} {"Produkt",-25} {"Pris",-9}", 4, 14, cartList);
            windowCategories.Draw();

            ResetLowerBar();
            Console.Write("Sluför köp? (j/n) ");
            var input = Console.ReadKey();
            if (input.KeyChar == 'j' || input.KeyChar == 'J')
            {
                DatabasDapper.ClearCart(customer.Id);
                Order order = new Order { OrderDate = DateTime.Now, ShippedDate = DateTime.Now, CustomerId = customer.Id };
                order = Db.AddOrder(order);
                foreach (var detail in orderdetails)
                {
                    detail.OrderId = order.Id;
                    Db.AddOrderDetails(detail);
                    DatabasDapper.UpdateStockAfterPurchase((int)detail.Quantity, (int)detail.ProductId);
                }
                
                ResetLowerBar();
                Console.Write($"Köp slufört med orderID {order.Id}");
                Console.ReadKey();
            }
        }
        public static void ShowPayment(Customer customer, int shippingCost)
        {
            ShowPage();
            SetTitle("Betalning");
            ShowLoggedInUser(customer.Name);
            ShowCartBanner(customer.Id);
            List<string> swish = new List<string> { $"{"1. Swish",-20}" };
            var swishWindow = new Window("Betalningsmetod 1", 4, 14, swish);
            swishWindow.Draw();


            List<string> card = new List<string> { $"{"2. Visa/MasterCard",-20}" };
            var cardWindow = new Window("Betalningsmetod 2", 4, 20, card);
            cardWindow.Draw();

            Console.SetCursorPosition(8, 29);
            Console.Write("Vilket betalningssätt väljer du? ");
            bool inputNum = int.TryParse(Console.ReadLine(), out int paymentOption);


            ShowPage();
            SetTitle("Betalning");
            ShowLoggedInUser(customer.Name);
            if (inputNum)
            {
                if (paymentOption == 1)
                {
                    List<string> swishPay = new List<string> { $"{"",-20}" };
                    var swishPayWindow = new Window("Swish", 4, 14, swishPay);
                    swishPayWindow.Draw();
                    ResetLowerBar();
                    Console.Write("Skriv in ett telefonnummer");
                    Console.SetCursorPosition(6, 15);
                    Console.ReadKey();
                    ShowSummary(customer, shippingCost);
                }
                else if (paymentOption == 2)
                {
                    List<string> cardPay = new List<string> { $"{"",-20}" };
                    var cardPayWindow = new Window("Visa/MasterCard", 4, 14, cardPay);
                    cardPayWindow.Draw();
                    ResetLowerBar();
                    Console.Write("Skriv in ett kreditkort");
                    Console.SetCursorPosition(6, 15);
                    Console.ReadKey();
                    ShowSummary(customer, shippingCost);
                }
                else
                {
                    return;
                }
            }
        }
        #endregion

        #region Orders
        public static void ShowOrders(Customer customer)
        {
            ShowPage();
            SetTitle($"Orderhistorik");
            ShowLoggedInUser(customer.Name);
            ShowCartBanner(customer.Id);
            var orders = DatabasDapper.GetOrders(customer);
            List<string> showOrders = new List<string> { $"{"OrderID",-8} {"BeställningsDatum",-25} {"Skickat",-25}" };
            foreach (var order in orders)
            {
                string productString = $"{order.Id,-8} {order.OrderDate,-25} {order.ShippedDate,-25}";
                showOrders.Add(productString);
            }
            var productsWindow = new Window("Ordrar", 4, 15, showOrders);
            productsWindow.Draw();
            ResetLowerBar();
            Console.Write("Ange id för att ta kolla närmare på utvald order: ");
            string input = Console.ReadLine();
            bool inputIsInt = int.TryParse(input, out int inputAsInt);
            if (inputIsInt)
            {
                ShowOrder(inputAsInt, customer);
            }
        }

        public static void ShowOrder(int orderId, Customer customer)
        {
            ShowPage();
            SetTitle($"Order {orderId}");
            ShowLoggedInUser(customer.Name);
            ShowCartBanner(customer.Id);
            var orders = DatabasDapper.GetOrderDetails(orderId);
            
            List<string> productsInOrder = new List<string> { $"{"ID",-5} {"Antal",-8} {"Namn",-26} {"Pris",-18}" };
            decimal? orderSum = 0;
            foreach (var product in orders)
            {
                var prod = DatabasDapper.GetProduct((int)product.ProductId);
                string productString = $"{product.ProductId,-5} {product.Quantity,-8} {prod.Name,-26} {String.Format("{0:###,###.00}", product.UnitPrice),-18}";
                productsInOrder.Add(productString);
                orderSum += product.UnitPrice * product.Quantity;
                
            }
            productsInOrder.Add("_______________________________________________________");
            productsInOrder.Add($"{"",-14} {"Total",-26} {String.Format("{0:###,###.00}", orderSum),-19}");
            var productsWindow = new Window($"Order {orderId}", 4, 15, productsInOrder);
            productsWindow.Draw();
            
            ResetLowerBar();
            Console.Write("Tryck valfri knapp för att gå vidare ");
            Console.ReadKey();
        }
        #endregion

        #region Categories
        public static void AddOrRemoveCategory()
        {
            ResetLowerBar();
            Console.Write("Ange id för att ta bort, + för att lägga till ny: ");

            string addOrDelete = Console.ReadLine();
            if (addOrDelete == "+")
            {
                AddCategory();
            }
            else if (int.TryParse(addOrDelete, out int categoryId))
            {
                try
                {
                    Db.RemoveCategory(categoryId);
                }
                catch
                {
                    ResetLowerBar();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Category contains products");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else
            {
                return;
            }
        }
        public static void ShowCategories()
        {
            var categories = DatabasDapper.GetAllCategories();
            int counter = 0;
            List<string> categoriesText = new List<string>();
            foreach (var category in categories)
            {
                counter++;
                categoriesText.Add($"{counter}. {category.Name}");
            }
            var windowCategories = new Window("Kategorier", 4, 14, categoriesText);
            windowCategories.Draw();
        }
        public static void ShowCategoryPage(int categoryId, Customer customer)
        {
            ShowPage();
            ShowLoggedInUser(customer.Name);
            ShowCartBanner(customer.Id);
            
            var category = DatabasDapper.GetCategory(categoryId);
            if(category.Id > 0)
            {
                SetTitle(category.Name);
            }
            else
            {
                SetTitle("Invalid category ID");
            }
            var categorisedProducts = DatabasDapper.GetProducts(categoryId);


            List<string> productsByCategory = new List<string>();

            foreach (var product in categorisedProducts)
            {
                productsByCategory.Add($"{product.Id,-5} {product.Stock,-8} {product.Name,-30} {String.Format("{0:### ###.00}", product.Price),-14}");
            }

            var productsByCategoryWindow = new Window($"{"Id",-5} {"Antal",-8} {"Namn",-30} {"Pris",-10}", 4, 14, productsByCategory);
            productsByCategoryWindow.Draw();

            ResetLowerBar();
            Console.Write("Välj ett id för att se mer av utvald produkt: ");
            var input = Console.ReadLine();
            bool inputIsInt = int.TryParse(input, out int select);
            if (inputIsInt)
            {
                ShowProduct(select, customer);
            }
            else
            {
                return;
            }
        }
        public static void AddCategory()
        {
            ShowPage();
            SetTitle("Lägg till kategori");
            bool invalidInput = true;
            Category newCategory = new Category();
            while (invalidInput)
            {
                List<string> productName = new List<string> { "" };
                var productNameWindow = new Window("Category Name", 4, 14, productName);
                productNameWindow.Draw();
                Console.SetCursorPosition(6, 15);

                string name = Console.ReadLine();
                if (name != "")
                {
                    newCategory.Name = name;
                    invalidInput = false;
                }
                else
                {
                    return;
                }
            }

            bool addCategory = false;
            while (addCategory == false)
            {
                ResetLowerBar();
                Console.Write("Lägg till kategori (J/N)? ");
                string input = Console.ReadLine();
                if (input == "j" || input == "J")
                {
                    ResetLowerBar();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"{newCategory.Name} added!");
                    Console.ForegroundColor = ConsoleColor.White;
                    Db.AddCategory(newCategory);
                    ShowPage();
                    ShowCategories();
                    addCategory = true;
                }
                else if (input == "n" || input == "N")
                {
                    return;
                }
            }
        }
        public static void ShowCategoriesList()
        {
            var categories = DatabasDapper.GetAllCategories();
            List<string> categoriesList = new List<string> { $"{"ID",-5} {"Name",-20}" };
            foreach (var category in categories)
            {
                string cat = $"{category.Id,-5} {category.Name,-20}";
                categoriesList.Add(cat);
            }
            var categoryWindow = new Window("Categories", 75, 3, categoriesList);
            categoryWindow.Draw();
        }
        #endregion

        #region Products
        public static void ManageProducts()
        {
            bool managingProducts = true;
            while (managingProducts)
            {
                ShowPage();
                ShowCategoriesList();
                ShowProducts();
                string addOrDelete = Console.ReadLine();
                if (addOrDelete == "+")
                {
                    AddProduct();
                }

                else if (int.TryParse(addOrDelete, out int productId))
                {
                    ResetLowerBar();
                    Console.Write($"Ta bort produkt ID {productId} (j/n)? Siffra för att ändra antal: ");
                    var input = Console.ReadLine();
                    bool inputIsInt = int.TryParse(input, out int inputAsInt);
                    if (input == "j" || input == "J")
                    {
                        Db.RemoveProduct(productId);
                    }
                    else if (inputIsInt)
                    {
                        DatabasDapper.UpdateStock(inputAsInt, productId);
                    }
                    else
                    {
                        managingProducts = false;
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            
        }
        public static void ShowProduct(int id, Customer customer)
        {
            ShowPage();
            var product = DatabasDapper.GetProduct(id);
            var category = DatabasDapper.GetCategory(product.CategoryId);
            SetTitle(product.Name);
            ShowLoggedInUser(customer.Name);
            ShowCartBanner(customer.Id);

            List<string> listProduct = new List<string> { $"{"Pris:", -10} {String.Format("{0:###,###.00}", product.Price)}",
                                                        $"{"I lager:", -10} {product.Stock}",
                                                        $"{"Kategori:", -10} {category.Name}"};
            var productWindow = new Window($"{product.Name}", 5, 16, listProduct);
            productWindow.Draw();

            ResetLowerBar();
            Console.Write("Tryck 'l' för att lägga till, annars valfri knapp: ");
            // Add to cart table
            var input = Console.ReadKey();
            switch (input.KeyChar)
            {
                case 'l':
                case 'L':
                    if(product.Stock > 0)
                    {
                        ResetLowerBar();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"{product.Name} tillagd i vagnen!");
                        Console.ForegroundColor = ConsoleColor.White;
                        Db.AddProductToCart(product, customer.Id);
                    }
                    else
                    {
                        ResetLowerBar();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"{product.Name} slut i lager!");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.ReadKey();
                    }
                    break;
                default:
                    return;
            }
        }
        public static void AddProduct()
        {
            bool invalidInput = true;
            Product newProduct = new Product();
            while (invalidInput)
            {
                List<string> productName = new List<string> { "" };
                var productNameWindow = new Window("Product Name", 8, 14, productName);
                productNameWindow.Draw();
                Console.SetCursorPosition(10, 15);

                string name = Console.ReadLine();
                if (name != "")
                {
                    newProduct.Name = name;
                    invalidInput = false;
                }
            }
            invalidInput = true;
            while (invalidInput)
            {
                List<string> catId = new List<string> { "" };
                var catIdWindow = new Window("CategoryID", 8, 20, catId);
                catIdWindow.Draw();
                Console.SetCursorPosition(10, 21);
                bool isInt = int.TryParse(Console.ReadLine(), out int categoryId);

                if (isInt)
                {
                    newProduct.CategoryId = categoryId;
                    invalidInput = false;
                }
            }
            invalidInput = true;
            while (invalidInput)
            {
                List<string> priceList = new List<string> { "" };
                var priceWindow = new Window("Price", 40, 14, priceList);
                priceWindow.Draw();
                Console.SetCursorPosition(42, 15);
                bool isDouble = double.TryParse(Console.ReadLine(), out double price);

                if (isDouble)
                {
                    newProduct.Price = (decimal)price;
                    invalidInput = false;
                }
            }
            invalidInput = true;
            while (invalidInput)
            {
                List<string> stockList = new List<string> { "" };
                var stockWindow = new Window("Stock", 40, 20, stockList);
                stockWindow.Draw();
                Console.SetCursorPosition(42, 21);
                bool isInt = int.TryParse(Console.ReadLine(), out int stock);

                if (isInt)
                {
                    newProduct.Stock = stock;
                    invalidInput = false;
                }
            }
            bool addProduct = false;
            while (addProduct == false)
            {
                ResetLowerBar();
                Console.Write("Lägg till produkt (J/N)? ");
                string input = Console.ReadLine();
                if (input == "j" || input == "J")
                {
                    try
                    {
                        Db.AddProduct(newProduct);
                    }
                    catch
                    {
                        ResetLowerBar();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Invalid entry");
                        Console.ForegroundColor = ConsoleColor.White;
                        Thread.Sleep(500);
                    }
                    addProduct = true;
                }
                else if (input == "n" || input == "N")
                {
                    addProduct = true;
                }
            }
        }
        public static void ShowFeaturedProd()
        {
            ResetLowerBar();
            Console.Write("Ange id för att lägga till en ny produkt: ");
            string input = Console.ReadLine(); // kolla att det inte är en tom sträng
            bool num = int.TryParse(input, out int prodId);
            if (num)
            {
                var prod = DatabasDapper.GetProduct(prodId);
                ResetLowerBar();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{prod.Name} tillagd i rekommenderas!");
                Console.ForegroundColor = ConsoleColor.White;
                Db.AddFeaturedProduct(prod.Name, prodId);
            }
        }
        public static void ShowSearchedProducts(string search, Customer customer)
        {
            ShowPage();
            SetTitle($"Sökresultat för '{search}'");
            ShowLoggedInUser(customer.Name);
            ShowCartBanner(customer.Id);
            var products = DatabasDapper.SearchProducts(search);
            List<string> showProducts = new List<string> { $"{"ID",-5} {"Name",-20} {"Price",-10} {"Stock",-10} {"Category"}" };
            foreach (var product in products)
            {
                string productString = $"{product.Id,-5} {product.Name,-20} {String.Format("{0:###,###.00}", product.Price),-10} {product.Stock,-10} {product.CategoryId}";
                showProducts.Add(productString);
            }
            var productsWindow = new Window("Products", 4, 15, showProducts);
            productsWindow.Draw();
            ResetLowerBar();
            Console.Write("Ange id för att kolla närmare på utvald produkt: ");
            string input = Console.ReadLine();
            bool inputIsInt = int.TryParse(input, out int inputAsInt);
            if (inputIsInt)
            {
                ShowProduct(inputAsInt, customer);
            }
        }
        public static void ShowProducts()
        {
            SetTitle("Hantera produkter");
            var products = DatabasDapper.GetAllProducts();
            List<string> showProducts = new List<string> { $"{"ID",-5} {"Namn",-20} {"Pris",-10} {"Antal",-10} {"Kategori"}" };
            foreach (var product in products)
            {
                string productString = $"{product.Id,-5} {product.Name,-20} {String.Format("{0:###,###.00}", product.Price),-10} {product.Stock,-10} {product.CategoryId}";
                showProducts.Add(productString);
            }
            var productsWindow = new Window("Products", 75, 15, showProducts);
            productsWindow.Draw();
            ResetLowerBar();
            Console.Write("Ange id för att ändra, + för att lägga till ny: ");
        }
        #endregion

        #region CustomerPages
        public static void ShowCustomerInformation(Customer customer)
        {
            ShowPage();
            SetTitle("Kunduppgifter");
            if (customer.Id == 1)
            {
                customer = new Customer();
            }
            List<string> personNummer = new List<string> { $"{customer.PersonNummer,-20}" };
            var personNummerWindow = new Window("Personnummer", 4, 14, personNummer);
            personNummerWindow.Draw();

            List<string> name = new List<string> { $"{customer.Name,-20}" };
            var nameWindow = new Window("Namn", 4, 19, name);
            nameWindow.Draw();

            List<string> phone = new List<string> { $"{customer.PhoneNumber,-20}" };
            var phoneWindow = new Window("Telefon", 4, 24, phone);
            phoneWindow.Draw();

            List<string> email = new List<string> { $"{customer.Email,-34}" };
            var emailWindow = new Window("Email", 30, 14, email);
            emailWindow.Draw();

            List<string> zipCode = new List<string> { $"{customer.ZipCode,-10}" };
            var zipCodeWindow = new Window("ZipCode", 30, 19, zipCode);
            zipCodeWindow.Draw();

            List<string> city = new List<string> { $"{customer.City,-18}" };
            var cityWindow = new Window("Stad", 46, 19, city);
            cityWindow.Draw();

            List<string> address = new List<string> { $"{customer.Street,-34}" };
            var addressWindow = new Window("Gata", 30, 24, address);
            addressWindow.Draw();

            Console.SetCursorPosition(8, 29);
            Console.Write("Fyll i dina uppgifter ");

            //Read information
            if (customer.Id < 1)
            {
                customer = ReadCustomerInformation(customer);
            }
            else
            {
                Console.SetCursorPosition(8, 29);
                Console.Write("Vill du ändra din information (j/n)? ");
                string inp = Console.ReadLine();
                if (inp == "j" || inp == "J")
                {
                    customer = ReadCustomerInformation(customer);
                }
            }


            Console.SetCursorPosition(8, 29);
            Console.Write("Fortsätt med inmatad information (j/n)? ");
            string input = Console.ReadLine();
            if (input == "j" || input == "J")
            {
                if (customer.Id < 1)
                {
                    Console.SetCursorPosition(8, 29);
                    Console.Write("Vill du skapa ett konto med ifylld information (j/n)? ");
                    input = Console.ReadLine();
                    if (input == "j" || input == "J")
                    {
                        bool invalidInput = true;
                        while (invalidInput)
                        {
                            ResetLowerBar();
                            Console.Write($"{"Välj ett lösenord: "}");
                            string password = Console.ReadLine();
                            if (password != "")
                            {
                                ResetLowerBar();
                                Console.Write($"{"Bekräfta lösenordet: "}");
                                string confirmPassword = Console.ReadLine();

                                if (password == confirmPassword)
                                {
                                    customer.Password = password;
                                    Db.AddCustomer(customer);
                                    ResetLowerBar();
                                    Console.Write($"{"Konto skapat! "}");
                                    DatabasDapper.UpdateCartOwner(1, customer.Id); // Convert guest cart to user cart
                                    Thread.Sleep(1000);
                                    ShowShipping(customer);

                                    invalidInput = false;
                                }
                                else
                                {
                                    ResetLowerBar();
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write("Lösenorden stämmer inte överens");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Thread.Sleep(500);
                                }
                            }
                        }
                    }
                }
                else
                {
                    ResetLowerBar();
                    Console.Write($"{"Konto uppdaterat! "}");
                    Db.EditCustomer(customer);
                    Thread.Sleep(500);
                    ResetLowerBar();
                    Console.Write("Fortsätt till fraktval (j/n)? ");
                    var inp = Console.ReadKey();
                    if (inp.KeyChar == 'j' || inp.KeyChar == 'J')
                    {
                        ShowShipping(customer);
                    }
                }
            }
        }
        public static Customer ReadCustomerInformation(Customer customer)
        {
            bool invalidInput = true;
            while (invalidInput)
            {
                Console.SetCursorPosition(6, 15);
                string readInput = Console.ReadLine();
                if (readInput != "" && double.TryParse(readInput, out double number) && readInput.Length == 12)
                {
                    customer.PersonNummer = readInput;
                    invalidInput = false;
                    break;
                }
                else if (readInput == "" && customer.Id > 1)
                {
                    invalidInput = false;
                    break;
                }
                else
                {
                    Console.SetCursorPosition(6, 15);
                    Console.Write("Invalid input");
                }
            }
            invalidInput = true;
            while (invalidInput)
            {
                Console.SetCursorPosition(6, 20);
                string readInput = Console.ReadLine();
                if (readInput != "")
                {
                    customer.Name = readInput;
                    invalidInput = false;
                }
                else if (readInput == "" && customer.Id > 1)
                {
                    invalidInput = false;
                    break;
                }
                else
                {
                    Console.SetCursorPosition(6, 20);
                    Console.Write("Invalid input");
                }
            }
            invalidInput = true;
            while (invalidInput)
            {
                Console.SetCursorPosition(6, 25);
                string readInput = Console.ReadLine();
                if (readInput != "")
                {
                    customer.PhoneNumber = readInput;
                    invalidInput = false;
                }
                else if (readInput == "" && customer.Id > 1)
                {
                    invalidInput = false;
                    break;
                }
                else
                {
                    Console.SetCursorPosition(6, 25);
                    Console.Write("Invalid input");
                }
            }
            invalidInput = true;
            while (invalidInput)
            {
                Console.SetCursorPosition(32, 15);
                string readInput = Console.ReadLine();
                if (readInput != "")
                {
                    customer.Email = readInput;
                    invalidInput = false;
                }
                else if (readInput == "" && customer.Id > 1)
                {
                    invalidInput = false;
                    break;
                }
                else
                {
                    Console.SetCursorPosition(32, 15);
                    Console.Write("Invalid input");
                }
            }
            invalidInput = true;
            while (invalidInput)
            {
                Console.SetCursorPosition(32, 20);
                string readInput = Console.ReadLine();
                if (readInput != "" && int.TryParse(readInput, out int number))
                {
                    customer.ZipCode = readInput;
                    invalidInput = false;
                }
                else if (readInput == "" && customer.Id > 1)
                {
                    invalidInput = false;
                    break;
                }
                else
                {
                    Console.SetCursorPosition(32, 20);
                    Console.Write("Invalid input");
                }
            }
            invalidInput = true;
            while (invalidInput)
            {
                Console.SetCursorPosition(48, 20);
                string readInput = Console.ReadLine();
                if (readInput != "")
                {
                    customer.City = readInput;
                    invalidInput = false;
                }
                else if (readInput == "" && customer.Id > 1)
                {
                    invalidInput = false;
                    break;
                }
                else
                {
                    Console.SetCursorPosition(48, 20);
                    Console.Write("Invalid input");
                }
            }
            invalidInput = true;
            while (invalidInput)
            {
                Console.SetCursorPosition(32, 25);
                string readInput = Console.ReadLine();
                if (readInput != "")
                {
                    customer.Street = readInput;
                    invalidInput = false;
                }
                else if (readInput == "" && customer.Id > 1)
                {
                    invalidInput = false;
                    break;
                }
                else
                {
                    Console.SetCursorPosition(32, 25);
                    Console.Write("Invalid input");
                }
            }
            return customer;
        }
        public static Customer CustomerPage(Customer customer)
        {
            ShowPage();

            if (customer.Id < 2) // 1 är gäst
            {
                customer = LoginUser(customer);
            }
            else
            {
                ShowPage();
                ShowLoggedInUser($"{customer.Name}");
                ShowCartBanner(customer.Id);

                List<string> customerMenu = new List<string> { $"{"1. Uppgifter",-15}", { "2. Ordrar" }, { "3. Erbjudanden" }, { "4. Kundkonton" }, { "5. Logga ut" } };
                var customerMenuWindow = new Window("Meny", 4, 14, customerMenu);
                customerMenuWindow.Draw();
                Console.SetCursorPosition(8, 29);
                Console.Write("Ange vad du vill titta närmare på: ");
                bool numInput = int.TryParse(Console.ReadLine(), out int menuSelect);
                if (numInput)
                {
                    switch (menuSelect)
                    {
                        case 1:
                            ShowCustomerInformation(customer);
                            break;
                        case 2:
                            ShowPage();
                            ShowOrders(customer);
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                        case 5:
                            Console.SetCursorPosition(8, 29);
                            Console.Write($"Logged out as {customer.Name,-30}");
                            Thread.Sleep(1000);
                            return DatabasDapper.SignInAsGuest();
                    }
                }
                Console.ReadKey();
            }
            return customer;
        }
        public static void ShowCustomers()
        {
            var customers = DatabasDapper.GetAllCustomers();
            List<string> customerString = new List<string> { $"{"Id",-4} {"PersonNummer",-14} {"Namn",-19} {"Email",-20}" };
            foreach (var customer in customers)
            {
                customerString.Add($"{customer.Id,-4} {customer.PersonNummer,-14} {customer.Name,-19} {customer.Email,-20}");
            }
            var windowCategories = new Window("Users", 4, 14, customerString);
            windowCategories.Draw();

            ResetLowerBar();
            Console.Write("Skriv in ett Id om du vill ändra en användare: ");
            bool inputIsInt = int.TryParse(Console.ReadLine(), out int inputAsInt);
            if (inputIsInt)
            {
                Customer selectedCustomer = DatabasDapper.GetCustomer(inputAsInt);
                ShowCustomerInformation(selectedCustomer);
            }
        }
        #endregion

        #region Recommended products
        public static void ShowRecommended()
        {
            var products = DatabasDapper.GetFeaturedProducts();
            List<string> featuredProducts = new List<string>();
            if (products.Count > 0)
            {
                foreach (var product in products)
                {
                    featuredProducts.Add($"{product.Name}");
                }
            }
            else
            {
                featuredProducts.Add($"List is empty");
            }

            var windowFeatured = new Window($"{"[R]ekommenderas",-20}", 23, 14, featuredProducts);
            windowFeatured.Draw();
        }
        public static void ShowFeatured(User user)
        {
            SetTitle("Rekommenderade varor");
            ShowLoggedInUser(user.Name);
            var featuredProducts = DatabasDapper.GetFeaturedProducts();
            List<string> featuredProduct = new List<string>();
            int counter = 0;
            int position = 4;
            if (featuredProducts.Count > 0)
            {
                foreach (var product in featuredProducts)
                {
                    featuredProduct = new List<string>();
                    counter++;
                    var prod = DatabasDapper.GetProduct(product.ProductId);
                    featuredProduct.AddRange(
                        new List<string> { prod.Name, String.Format("{0:###,###.00 SEK}", prod.Price), prod.Stock.ToString() + " i lager" });

                    var windowFeatured = new Window($"{"Vara " + counter,-15}", position, 14, featuredProduct);
                    windowFeatured.Draw();
                    position += 20;
                }
            }
            else
            {
                featuredProduct.Add($"List is empty");
            }
            if (user is Customer)
            {
                ResetLowerBar();
                Console.Write("Vilken vara vill du kolla närmare på? ");
                bool isNumber = int.TryParse(Console.ReadLine(), out int index);
                index -= 1; // för att få rätt index
                if (isNumber)
                {
                    Product selectedProduct = DatabasDapper.GetProduct(featuredProducts[index].ProductId);
                    ShowProduct(selectedProduct.Id, (Customer)user);
                }
            }
            else if (user is Admin)
            {
                bool invalidChoice = true;
                while (invalidChoice)
                {
                    ResetLowerBar();
                    Console.Write("Ange en vara för att ta bort, + för att lägga till: ");
                    string inp = Console.ReadLine();
                    bool isNumber = int.TryParse(inp, out int index);

                    if (isNumber)
                    {
                        ResetLowerBar();
                        Console.Write($"Vill du ta bort vara {index} (j/n)? ");
                        var input = Console.ReadKey();
                        index -= 1; // för att få rätt index
                        if (input.KeyChar == 'j' || input.KeyChar == 'J')
                        {
                            //try
                            //{
                                ResetLowerBar();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write($"Vara {index + 1}, {featuredProducts[index].Name} borttagen");
                                Console.ForegroundColor = ConsoleColor.White;
                                Db.RemoveFeaturedProduct(featuredProducts[index].Id);
                                invalidChoice = false;
                            //}
                            //catch
                            //{
                            //    ResetLowerBar();
                            //    Console.Write("Ogilitgt ID");
                            //}
                            
                        }
                    }
                    else if (inp == "")
                    {
                        return;
                    }
                    else if (featuredProducts.Count >= 3)
                    {
                        ResetLowerBar();
                        Console.Write($"Max 3 produkter kan vara rekommenderade!");
                        Console.ReadKey();
                    }
                    else if (inp == "+")
                    {
                        ShowProducts();
                        ShowFeaturedProd();
                        invalidChoice = false;
                    }
                }
            }
        }
        #endregion

        #region Login
        public static Admin LoginAdmin(Admin admin)
        {
            SetTitle("Admin-inloggning");
            List<string> username = new List<string> { "Username" };
            var usernameWindow = new Window("", 8, 14, username);
            usernameWindow.Draw();

            List<string> password = new List<string> { "Password" };
            var passwordWindow = new Window("", 8, 17, password);
            passwordWindow.Draw();

            bool incorrectUserName = true;
            while (incorrectUserName)
            {
                ResetLowerBar();
                Console.Write("Ange användarnamn: ");
                string input = Console.ReadLine();
                var administrator = DatabasDapper.AuthenticateAdmin(input);
                if (administrator != null && administrator.Name == input)
                {
                    //incorrectUserName = false;
                    Console.SetCursorPosition(24, 15);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(input);
                    Console.ForegroundColor = ConsoleColor.White;

                    bool incorrectPassword = true;
                    while (incorrectPassword)
                    {
                        ResetLowerBar();
                        Console.Write("Ange lösenord : ");
                        string inputPw = Console.ReadLine();
                        if (administrator.Password == inputPw)
                        {
                            Console.SetCursorPosition(24, 18);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(inputPw);
                            Console.ForegroundColor = ConsoleColor.White;
                            Thread.Sleep(1000);
                            return administrator;
                        }
                        else
                        {
                            Console.SetCursorPosition(24, 18);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(inputPw);
                            Console.ForegroundColor = ConsoleColor.White;
                            Thread.Sleep(1000);
                        }
                    }
                }
                else if (input == "q" || input == "Q")
                {
                    return new Admin();
                }
                else
                {
                    ResetLowerBar();
                    Console.Write("Invalid user name!");
                    Thread.Sleep(500);
                }
            }
            return new Admin();
        }
        public static Customer LoginUser(Customer customer)
        {
            SetTitle("Kundinloggning");
            List<string> username = new List<string> { "Username" };
            var usernameWindow = new Window("", 8, 14, username);
            usernameWindow.Draw();

            List<string> password = new List<string> { "Password" };
            var passwordWindow = new Window("", 8, 17, password);
            passwordWindow.Draw();

            bool incorrectUserName = true;
            while (incorrectUserName)
            {
                ResetLowerBar();
                Console.Write("Ange användarnamn: ");
                string input = Console.ReadLine();
                var user = DatabasDapper.AuthenticateUser(input);
                if (user != null && user.Name == input)
                {
                    //incorrectUserName = false;
                    Console.SetCursorPosition(24, 15);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(input);
                    Console.ForegroundColor = ConsoleColor.White;

                    bool incorrectPassword = true;
                    while (incorrectPassword)
                    {
                        ResetLowerBar();
                        Console.Write("Ange lösenord : ");
                        string inputPw = Console.ReadLine();
                        if (user.Password == inputPw)
                        {
                            Console.SetCursorPosition(24, 18);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(inputPw);
                            Console.ForegroundColor = ConsoleColor.White;
                            Thread.Sleep(1000);
                            return user;
                        }
                        else
                        {
                            Console.SetCursorPosition(24, 18);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(inputPw);
                            Console.ForegroundColor = ConsoleColor.White;
                            Thread.Sleep(1000);
                        }
                    }
                }
                else if (input == "q" || input == "Q")
                {
                    return DatabasDapper.SignInAsGuest();
                }
                else
                {
                    ResetLowerBar();
                    Console.Write("Invalid user name!");
                    Thread.Sleep(500);
                }
            }
            return new Customer();
        }
        #endregion

        #region Admin
        public static Admin AdminPage(Admin admin)
        {
            ShowPage();

            if (admin.Id < 1)
            {
                admin = LoginAdmin(admin);
            }
            else
            {
                ShowPage();
                SetTitle("Administratör");
                ShowLoggedInUser(admin.Name);
                List<string> adminMenu = new List<string> { $"{"1. Produkter",-15}", { "2. Kategorier" }, { "3. Erbjudanden" }, { "4. Kundkonton" }, { "5. Logga ut" } };
                var adminMenuWindow = new Window("Meny", 4, 14, adminMenu);
                adminMenuWindow.Draw();
                Console.SetCursorPosition(8, 29);
                Console.Write("Ange vad du vill titta närmare på: ");
                bool numInput = int.TryParse(Console.ReadLine(), out int menuSelect);
                if (numInput)
                {
                    switch (menuSelect)
                    {
                        case 1:
                            ManageProducts();
                            break;
                        case 2:
                            ShowPage();
                            ShowCategories();
                            AddOrRemoveCategory();
                            break;
                        case 3:
                            ShowPage();
                            ShowFeatured(admin);
                            break;
                        case 4:
                            ShowCustomers();
                            break;
                        case 5:
                            Console.SetCursorPosition(8, 29);
                            Console.Write($"Logged out as {admin.Name,-30}");
                            Thread.Sleep(1000);
                            return new Admin();
                    }
                }
                Console.ReadKey();
            }
            return admin;
        }
        #endregion
    }
}
