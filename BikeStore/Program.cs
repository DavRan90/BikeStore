using System.ComponentModel;
using BikeStore.Models;

namespace BikeStore
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool loggedIn = false;
            Customer customer = DatabasDapper.SignInAsGuest();
            Admin admin = new Admin();
            DatabasDapper.ClearCart(1); // Clear guest cart before start
            while (true)
            {
                Draw.ShowAll();
                if (admin.Id > 0)
                {
                    Draw.ShowLoggedInUser(admin.Name);
                }
                else if (customer.Id > 0)
                {
                    Draw.ShowLoggedInUser(customer.Name);
                    var cart = DatabasDapper.GetCartList(customer.Id);
                    Draw.ShowCartBanner(customer.Id);
                }

                Draw.ResetLowerBar();
                Console.SetCursorPosition(8, 29);
                Console.Write("Ange bokstav eller siffra för att gå vidare: ");
                var input = Console.ReadKey();

                switch (input.KeyChar)
                {
                    case 'r':
                    case 'R':
                        Draw.ShowPage();
                        Draw.ShowFeatured(customer);
                        break;
                    case 's':
                    case 'S':
                        Draw.ResetLowerBar();
                        Console.Write("Sök efter produkt: ");
                        string search = Console.ReadLine();
                        if (search.Length > 2)
                        {
                            Draw.ShowSearchedProducts(search, customer);
                        }
                        else
                        {
                            Draw.ResetLowerBar();
                            Console.Write("Atleast 2 characters required for a search! ");
                            Console.ReadKey();
                        }
                        break;
                    // Some shortcuts to functionality for testing
                    //case 'w':
                    //case 'W':
                    //    Draw.ShowOrders(customer);
                    //    break;
                    //case 'z':
                    //    Draw.ShowShipping(customer);
                    //    break;
                    //case 'x':
                    //    Draw.ShowPayment(customer, 19);
                    //    break;
                    case 'a':
                    case 'A':
                        admin = Draw.AdminPage(admin);
                        break;
                    case 'l':
                    case 'L':
                        customer = Draw.CustomerPage(customer);
                        break;
                    case 'v':
                    case 'V':
                        Draw.ShowPage();
                        if (customer.Id > 0)
                        {
                            Draw.ShowCart(customer);
                        }
                        break;
                    default:
                        bool isNum = int.TryParse(input.KeyChar.ToString(), out int inputAsInt);
                        if (isNum)
                        {
                            try
                            {
                                Draw.ShowPage();
                                Draw.ShowCategoryPage(inputAsInt, customer);
                            }
                            catch
                            {
                                Draw.ResetLowerBar();
                                Console.Write("Invalid ID");
                            }
                        }
                        else
                        {
                            break;
                        }
                        break;


                }
            }
        }
    }
}