using System;
namespace Program
{

    class Product
    {
        // add Id, Name, Category, Price, Quantity
        public string Id {get; private set;}
        public string Name {get; private set;}
        public string Category {get; private set;}
        public double Price {get; private set;}
        public int Quantity {get; private set;}

        public Product (string Id, string Name, string Category, double Price, int Quantity)
        {
            this.Id = Id;
            this.Name = Name;
            this.Category = Category;
            this.Price = Price;
            this.Quantity = Quantity;
        }
    }
    class inventoryManager
    {
        static public void Main(string[] args)
        {
            bool isActive = true;
            List<Product> productsList= new List<Product>()
            {
                new Product("P001", "Wireless Mouse", "Electronics", 29.99, 15), 
                new Product("P002", "Mechanical Keyboard", "Electronic", 89.50, 8),
                new Product("P003", "Espresso Coffee Beans", "Groceries", 18.20, 25),
                new Product("P004", "Stainless Water Bottle", "Home & Kitchen", 14.99, 3),
                new Product("P005", "USB-C Charging Cable", "Electronic", 9.99, 40)
            };

            while(isActive)
            {
            string[] programMenu =
            {
                "1. Add a new product",
                "2. View all products",
                "3. Search for a product",
                "4. Exit the program"
            };

            foreach( string option in programMenu)
            {
                Console.WriteLine(option);
            }
            

            if (int.TryParse(Console.ReadLine() ?? "", out int userChoice))
            {
                switch (userChoice)
                {
                    case 1:
                        Console.WriteLine("Let's start to add a new product");
                        Console.WriteLine("Please Enter an ID of product");
                        string productId = Console.ReadLine() ?? "";
                        Console.WriteLine("Please Enter a Name of product");
                        string productName = Console.ReadLine() ?? "";
                        Console.WriteLine("Please Enter a category of product");
                        string productCategory = Console.ReadLine() ?? "";
                        Console.WriteLine("Please Enter a price of product");
                        Double.TryParse(Console.ReadLine(), out double productPrice);
                        // double productPrice = Convert.ToDouble(Console.ReadLine());
                        Console.WriteLine("Please Enter a quantity of product");
                        int.TryParse(Console.ReadLine(), out int productQuantity);
                        // int productQuantity = Console.ReadLine();
                        productsList.Add(new Product(productId, productName, productCategory, productPrice, productQuantity ));
                        break;
                    case 2:
                    foreach ( Product product in productsList)
                        {
                            Console.WriteLine($"{product.Id} | {product.Name} | {product.Category} | ${product.Price} | {product.Quantity}pc.");
                        }
                        break;
                    case 3:
                        break;
                    case 4:
                        isActive = false;
                        break;
                        // return;
                }
            }
            }
        }
    }
}