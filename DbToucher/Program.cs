using Persistence;
using Persistence.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbToucher
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintMenu();
        }

        private static void PrintMenu()
        {
            Console.WriteLine("Menu DB Toucher! Touch wherever you like!");
            Console.WriteLine("1. DOM");
            Console.WriteLine("2. Source");
            Console.WriteLine("3. User");
            Console.WriteLine("4. Releasable");
            Console.WriteLine("5. Source Subscription");
            Console.WriteLine("6. Item Subscription");
            Console.WriteLine("7. Release");
            Console.WriteLine("0. Close");
            int option = int.Parse(Console.ReadLine());
            /*switch (option)
            {
                case 1:
                    PrintABMDOM();
                    break;
                case 2:
                    PrintABMSource();
                    break;
                case 2:
                    PrintABMUser();
                    break;
                case 2:
                    PrintABMReleasable();
                    break;
                case 2:
                    PrintABMSource();
                    break;
                case 2:
                    PrintABMSource();
                    break;
                case 2:
                    PrintABMSource();
                    break;
                default:
                    break;
            }  */  
        }
    }
}
