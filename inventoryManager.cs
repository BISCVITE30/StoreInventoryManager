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

        public Product (string Id, string Name, string Category, double Price, int Quantity)
        {
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
        
            using(var connection = new SqliteConnection(InventoryManager.connectionString))
            {
                connection.Open();

                string insertSql = @"
                INSERT INTO Products(Id, Name, Category, Price, Quantity)
                VALUES (@Id, @Name, @Category, @Price, @Quantity);";

                using(var command = new SqliteCommand(insertSql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@Price", price);
                    command.Parameters.AddWithValue("@Quantity", quantity);

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void GetAllProducts()
        {
            using(var connection = new SqliteConnection(InventoryManager.connectionString))
            {
                connection.Open();

                string selectSql = "SELECT Id, Name, Category, Price, Quantity FROM Products;";

                using(var command = new SqliteCommand(selectSql, connection))
                {
                    using(var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string id = reader.GetString(0);
                            string name = reader.GetString(1);
                            string category = reader.GetString(2);
                            double price = reader.GetDouble(3);
                            int quantity = reader.GetInt32(4);

                            Console.WriteLine($"{id} | {name} | {category} | {price} | Stock: {quantity}");
                        }
                    }
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

        public static void SearchByName(string search)
        {
            using(var connection = new SqliteConnection(InventoryManager.connectionString))
            {
                connection.Open();

                string selectSql = "SELECT * FROM Products WHERE Name LIKE @Search";
                using(var command = new SqliteCommand(selectSql, connection))
                {
                    command.Parameters.AddWithValue("@Search", $"%{search}%");

                    using(var reader = command.ExecuteReader())
                    {
                        bool found = false;
                        while (reader.Read())
                        {
                            string id = reader.GetString(0);
                            string name = reader.GetString(1);
                            string category = reader.GetString(2);
                            double price = reader.GetDouble(3);
                            int quantity = reader.GetInt32(4);

                            Console.WriteLine($"{id} | {name} | {category} | {price} | Stock: {quantity}");
                            found = true;
                        }
                        if(!found)
                        {
                            Console.WriteLine("No products found");
                        }
                    }
                }
            }
        }
        public static void SearchById(string search)
        {
            using(var connection = new SqliteConnection(InventoryManager.connectionString))
            {
                connection.Open();

                string selectSql = "SELECT * FROM Products WHERE Id LIKE @Search";
                using(var command = new SqliteCommand(selectSql, connection))
                {
                    command.Parameters.AddWithValue("@Search", $"%{search}%");

                    using(var reader = command.ExecuteReader())
                    {
                        bool found = false;
                        while (reader.Read())
                        {
                            string id = reader.GetString(0);
                            string name = reader.GetString(1);
                            string category = reader.GetString(2);
                            double price = reader.GetDouble(3);
                            int quantity = reader.GetInt32(4);

                            Console.WriteLine($"{id} | {name} | {category} | {price} | Stock: {quantity}");
                            found = true;
                        }

                        if (!found)
                        {
                            Console.WriteLine("No products found");
                        }
                    }
                }
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
                        Console.WriteLine("Please Enter an ID of product");
                        string productId = Console.ReadLine() ?? "";
                            while(ProductExists(productId) || string.IsNullOrWhiteSpace(productId))
                            {
                                ClearTerminal();
                                Console.WriteLine("This ID already exists or input incorrect, please choose another ID");
                                productId = Console.ReadLine() ?? "";
                            }
                        Console.WriteLine("Please Enter a Name of product");
                        string productName = Console.ReadLine() ?? "";
                        Console.WriteLine("Please Enter a category of product");
                        string productCategory = Console.ReadLine() ?? "";
                        Console.WriteLine("Please Enter a price of product");
                        Double.TryParse(Console.ReadLine(), out double productPrice);
                        Console.WriteLine("Please Enter a quantity of product");
                        int.TryParse(Console.ReadLine(), out int productQuantity);
                        if(string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(productCategory))
                            {
                                ClearTerminal();
                                Console.WriteLine("Incorrect input");
                                PressF();
                                break;
                            }
                        AddProduct(productId, productName, productCategory, productPrice, productQuantity);
                        PressF();
                        break;
                    case 2:
                        ClearTerminal();
                        GetAllProducts();
                        PressF();
                        break;
                    case 3:
                        ClearTerminal();
                        Console.WriteLine("Choose the searching option");
                        Console.WriteLine("1. Searching for product Name");
                        Console.WriteLine("2. Searching for product ID");
                        int.TryParse(Console.ReadLine(), out int ChoosenNum);
                        if( ChoosenNum == 1)
                            {
                                ClearTerminal();
                                Console.WriteLine("Enter the Name of product");
                                string nameToCompare = Console.ReadLine() ?? "";
                                ClearTerminal();
                                SearchByName(nameToCompare);
                                PressF();
                            } else if( ChoosenNum == 2 )
                            {
                                ClearTerminal();
                                Console.WriteLine("Enter the ID of product");
                                string idToCompare = Console.ReadLine() ?? "";
                                ClearTerminal();
                                SearchById(idToCompare);
                                PressF();
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