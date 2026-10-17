using System;
using Microsoft.Data.Sqlite;
namespace Program
{

    class Product
    {
        public string Id {get; private set;}
        public string Name {get; private set;}
        public string Category {get; private set;}
        public double Price {get; private set;}
        public int Quantity {get; private set;}
        public override string ToString() => $"{Id} | {Name} | {Category} | ${Price} | Stock: {Quantity}";

        public Product (string Id, string Name, string Category, double Price, int Quantity)
        {

            if (string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Category))
            {
                throw new ArgumentException();
            }
            if(Price < 0 || Quantity < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            this.Id = Id;
            this.Name = Name;
            this.Category = Category;
            this.Price = Price;
            this.Quantity = Quantity;
        }

    }
    class InventoryManager
    {
        private const string connectionString = "Data Source=inventory.db";
        public static void PressF()
        {
            Console.WriteLine("\nPress Enter to continue");
            Console.ReadLine();
        } 
        public static void ClearTerminal()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine();
        }

        public static void InitializeDatabase()
        {
            using( var connection = new SqliteConnection(InventoryManager.connectionString))
            {
                connection.Open();

                string createTableSql = @"
                CREATE TABLE IF NOT EXISTS Products (
                Id TEXT PRIMARY KEY,
                Name TEXT NOT NULL,
                Category TEXT NOT NULL,
                Price REAL NOT NULL,
                Quantity INTEGER NOT NULL);";

                using (var command = new SqliteCommand(createTableSql, connection))
                {
                  command.ExecuteNonQuery();  
                };
            };            
        }

        public static void AddProduct(string id, string name, string category, double price, int quantity)
        {
            Product newProduct = new Product(id,name,category,price,quantity);
        
            using(var connection = new SqliteConnection(InventoryManager.connectionString))
            {
                connection.Open();

                string insertSql = @"
                INSERT INTO Products(Id, Name, Category, Price, Quantity)
                VALUES (@Id, @Name, @Category, @Price, @Quantity);";

                using(var command = new SqliteCommand(insertSql, connection))
                {
                    command.Parameters.AddWithValue("@Id", newProduct.Id);
                    command.Parameters.AddWithValue("@Name", newProduct.Name);
                    command.Parameters.AddWithValue("@Category", newProduct.Category);
                    command.Parameters.AddWithValue("@Price", newProduct.Price);
                    command.Parameters.AddWithValue("@Quantity", newProduct.Quantity);

                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<Product> GetAllProducts()
        {
            using(var connection = new SqliteConnection(InventoryManager.connectionString))
            {
                connection.Open();

                string selectSql = "SELECT Id, Name, Category, Price, Quantity FROM Products;";

                using(var command = new SqliteCommand(selectSql, connection))
                {
                        using(var reader = command.ExecuteReader())
                    {
                        return InventoryManager.ReadProducts(reader);
                    }
                    }
                }
            }

        public static void DeleteProductById(string id)
        {
            using(var connection = new SqliteConnection(InventoryManager.connectionString))
            {
                connection.Open();

                string deleteSql = "DELETE FROM Products WHERE Id = @Id";

                using(var command = new SqliteCommand(deleteSql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void SeedDataBase()
        {
            using(var connection = new SqliteConnection(InventoryManager.connectionString))
            {
                connection.Open();

                string selectSql = "SELECT COUNT(*) FROM Products";

                using(var command = new SqliteCommand(selectSql, connection))
                {
                    long count = (long)command.ExecuteScalar();

                    if(count == 0)
                    {
                        AddProduct("P001", "Wireless Mouse", "Electronics", 29.99, 15);
                        AddProduct("P002", "Mechanical Keyboard", "Electronics", 89.50, 8);
                        AddProduct("P003", "Espresso Coffee Beans", "Groceries", 18.20, 25);
                        AddProduct("P004", "Stainless Water Bottle", "Home & Kitchen", 14.99, 3);
                        AddProduct("P005", "USB-C Charging Cable", "Electronics", 9.99, 40);
                    }
                }
            }
            
        }

        public static bool ProductExists(string id)
        {
            using(var connection = new SqliteConnection(InventoryManager.connectionString))
            {
                connection.Open();

                string selectCountSql = "SELECT COUNT(*) FROM Products WHERE Id = @Id";
                using(var command = new SqliteCommand(selectCountSql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    long count = (long)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private static List<Product> ReadProducts(SqliteDataReader reader)
        {
                        List<Product> res = new List<Product>();

                        while (reader.Read())
                        {
                            string id = reader.GetString(0);
                            string name = reader.GetString(1);
                            string category = reader.GetString(2);
                            double price = reader.GetDouble(3);
                            int quantity = reader.GetInt32(4);

                            res.Add(new Product(id,name,category,price,quantity));
                        }

                        return res;
        }

        public static List<Product> SearchByName(string search)
        {
            using(var connection = new SqliteConnection(InventoryManager.connectionString))
            {
                connection.Open();

                string selectSql = "SELECT Id, Name, Category, Price, Quantity FROM Products WHERE Name LIKE @Search";
                using(var command = new SqliteCommand(selectSql, connection))
                {
                    command.Parameters.AddWithValue("@Search", $"%{search}%");

                    using(var reader = command.ExecuteReader())
                    {
                        return InventoryManager.ReadProducts(reader);
                    }
                }
            }
        }
        public static List<Product> SearchById(string search)
        {
            using(var connection = new SqliteConnection(InventoryManager.connectionString))
            {
                connection.Open();

                string selectSql = "SELECT Id, Name, Category, Price, Quantity FROM Products WHERE Id LIKE @Search";
                using(var command = new SqliteCommand(selectSql, connection))
                {
                    command.Parameters.AddWithValue("@Search", $"%{search}%");

                    using(var reader = command.ExecuteReader())
                    {
                        return InventoryManager.ReadProducts(reader);
                    }
                }
            }
        }

        public static void SearchingProducts(List<Product> result)
        {
            if (result.Count == 0)
                {
                    Console.WriteLine("No products found");
                    PressF();
                    return;
                }
                
                for(int i = 0; i < result.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {result[i]}");
                }

                Console.WriteLine("\nSelect the product");
                if(int.TryParse(Console.ReadLine(), out int productSelected) && productSelected > 0 && productSelected <= result.Count)
                {
                    ClearTerminal();
                    Product selected = result[productSelected - 1];
                    Console.WriteLine(selected);
                    Console.WriteLine("1. Delete");
                    if(int.TryParse(Console.ReadLine(), out int actionSelected) && actionSelected > 0 && actionSelected < 2)
                    {
                        Console.WriteLine("Deleting the product...");
                        DeleteProductById(result[productSelected - 1].Id);
                    } else
                    {
                        Console.WriteLine("Invalid input");
                    }
                } else
                {
                    Console.WriteLine("Invalid input");
                }
                PressF();
        }

        public static double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                if(double.TryParse(Console.ReadLine(), out double value) && value >= 0)
                {
                    return value;
                };

                ClearTerminal();
                Console.WriteLine("Incorrect input. Price must be a positive number.");
                PressF();
                ClearTerminal();
            }
        }
        public static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                if(int.TryParse(Console.ReadLine(), out int value) && value >= 0)
                {
                    return value;
                };

                ClearTerminal();
                Console.WriteLine("Incorrect input. Quantity must be a positive number.");
                PressF();
                ClearTerminal();
            }
        }

        public static string ReadString(string prompt, Func<string, bool>? validator = null, string errorMessage = "Input cannot be empty.")
        {
            while (true)
            {
                Console.WriteLine(prompt);
                string value = Console.ReadLine() ?? "";
                
                bool isValid = !string.IsNullOrWhiteSpace(value);

                if(isValid && validator != null)
                {
                    isValid = validator(value);
                }

                if (isValid)
                {
                    return value;
                }

                ClearTerminal();
                Console.WriteLine(errorMessage);
                PressF();
                ClearTerminal();
            }
        }
        public static void Main(string[] args)
        {
            InitializeDatabase();
            SeedDataBase();
            bool isActive = true;

            while(isActive)
            {
                ClearTerminal();
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
                        ClearTerminal();
                        Console.WriteLine("Let's start to add a new product");
                        string productId = ReadString("Please Enter an ID of product", id => !ProductExists(id), "This ID already exists or input incorrect, please choose another ID");
                        // Console.WriteLine("Please Enter an ID of product");
                        // string productId = Console.ReadLine() ?? "";
                        // while(ProductExists(productId) || string.IsNullOrWhiteSpace(productId))
                        // {
                        //     ClearTerminal();
                        //     Console.WriteLine("This ID already exists or input incorrect, please choose another ID");
                        //     productId = Console.ReadLine() ?? "";
                        // };
                        
                        string productName = ReadString("Please Enter a Name of product");
                        string productCategory = ReadString("Please Enter a Category of product");
                        // Console.WriteLine("Please Enter a Name of product");
                        // string productName = Console.ReadLine() ?? "";
                        // Console.WriteLine("Please Enter a category of product");
                        // string productCategory = Console.ReadLine() ?? "";

                        double productPrice = ReadDouble("Please Enter a price of product");
                        // Console.WriteLine("Please Enter a price of product");
                        // double.TryParse(Console.ReadLine(), out double productPrice);

                        // while(productPrice == 0)
                        // {
                        //     ClearTerminal();
                        //     Console.WriteLine("Incorrect input. Price must be a positive number.");
                        //     PressF();
                        //     ClearTerminal();
                        //     Console.WriteLine("Please Enter a price of product");
                        //     double.TryParse(Console.ReadLine(), out productPrice);
                        // };

                        int productQuantity = ReadInt("Please Enter a quantity of product");
                            // Console.WriteLine("Please Enter a quantity of product");
                            // int.TryParse(Console.ReadLine(), out int productQuantity);

                            // if(string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(productCategory))
                            // {
                            //     ClearTerminal();
                            //     Console.WriteLine("Incorrect input");
                            //     PressF();
                            //     break;
                            // };
                        try
                        {   
                            AddProduct(productId, productName, productCategory, productPrice, productQuantity);
                        }
                        catch(ArgumentOutOfRangeException)
                        {
                            Console.WriteLine("\n[Error] Price and quantity cannot be negative!");
                        }
                        catch(ArgumentException)
                        {
                            Console.WriteLine("\n[Error] Product fields cannot be empty!");
                        }

                        PressF();
                        break;
                    case 2:
                        ClearTerminal();
                        List<Product> productList = GetAllProducts();

                        if (productList.Count == 0)
                            {
                                Console.WriteLine("No products found");
                                PressF();
                                break;
                            };

                        foreach(Product product in productList)
                            {
                                Console.WriteLine(product);
                            };

                        PressF();
                        break;
                    case 3:
                        ClearTerminal();
                        Console.WriteLine("Choose the searching option");
                        Console.WriteLine("1. Searching for product Name");
                        Console.WriteLine("2. Searching for product ID");
                        int.TryParse(Console.ReadLine(), out int selectedNum);

                        if( selectedNum == 1)
                            {
                                ClearTerminal();
                                Console.WriteLine("Enter the Name of product");
                                string nameToSearch = Console.ReadLine() ?? "";
                                ClearTerminal();
                                List<Product> results = SearchByName(nameToSearch);
                                SearchingProducts(results);
                            } 
                            else if( selectedNum == 2 )
                            {
                                ClearTerminal();
                                Console.WriteLine("Enter the ID of product");
                                string idToSearch = Console.ReadLine() ?? "";
                                ClearTerminal();
                                List<Product> results = SearchById(idToSearch);
                                SearchingProducts(results);
                            };

                            break;
                    case 4:
                        ClearTerminal();
                        isActive = false;
                        break;
                }
            }
            }
        }
    }
}