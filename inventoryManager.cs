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

        public Product (string id, string name, string category, double price, int quantity)
        {

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentException();
            }
            if(price < 0 || quantity < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            this.Id = id;
            this.Name = name;
            this.Category = category;
            this.Price = price;
            this.Quantity = quantity;
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

        public static bool UpdateProductQuantity(string id, int quantity)
        {
            if(quantity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity));
            }

            using(var connection = new SqliteConnection(InventoryManager.connectionString))
            {
                connection.Open();

                string updateSql = "UPDATE Products SET Quantity = @Quantity WHERE Id = @Id";

                using(var command = new SqliteCommand(updateSql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Quantity", quantity);

                    return command.ExecuteNonQuery() > 0;
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

        public static bool DeleteProductById(string id)
        {
            using(var connection = new SqliteConnection(InventoryManager.connectionString))
            {
                connection.Open();

                string deleteSql = "DELETE FROM Products WHERE Id = @Id";

                using(var command = new SqliteCommand(deleteSql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);


                    return command.ExecuteNonQuery() > 0;
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

        public static void PrintProducts(List<Product> prodList)
        {
            for(int i = 0; i < prodList.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {prodList[i]}");
            }
            Console.WriteLine("\nSelect the product");
        }

        public static Product? SelectProduct(List<Product> prodList)
        {
            if(int.TryParse(Console.ReadLine(), out int productSelected) && productSelected > 0 && productSelected <= prodList.Count)
            {
                ClearTerminal();
                Product selected = prodList[productSelected - 1];

                return selected;
            };

            return null;
        }

        public static void DeletionProduct(Product product)
        {
            if( ConfirmAction("Are you sure you want to delete this product? (Y/N)") && DeleteProductById(product.Id))
                {
                    Console.WriteLine("Deletion complete");
                } 
                else
                {
                    Console.WriteLine("Deletion canceled");
                }
        }

        public static void UpdateProduct(Product productToUpdate)
        {
            int updatedQuantity = ReadInt("What quantity would you like to update this product to?");

                if( ConfirmAction("Are you sure you want to update this product? (Y/N)") && UpdateProductQuantity(productToUpdate.Id, updatedQuantity))
                {
                    Console.WriteLine("Update complete");
                } 
                else
                {
                    Console.WriteLine("Update canceled");
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
            
            PrintProducts(result);

            Product? selected = SelectProduct(result);

            if(selected == null)
            {
                Console.WriteLine("Invalid choice. Please try again.");
                PressF();
                return;
            }

            Console.WriteLine(selected);
            Console.WriteLine("1. Delete");
            Console.WriteLine("2. Update quantity");

            if(int.TryParse(Console.ReadLine(), out int actionSelected))
            {
                if(actionSelected == 1)
                {
                    DeletionProduct(selected);
                }
                else if(actionSelected == 2)
                {
                    UpdateProduct(selected);
                } else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                    PressF();
                }
            } 
            else
            {
                Console.WriteLine("Invalid choice. Please try again.");
                PressF();
            }
            PressF();
        }

        public static bool ConfirmAction(string prompt)
        {
            Console.WriteLine(prompt);
            string answer = Console.ReadLine() ?? "";
            return string.Equals( answer, "Y", StringComparison.OrdinalIgnoreCase);
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
                Console.WriteLine("Incorrect input. Price must be a zero or a positive number.");
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
                Console.WriteLine("Incorrect input. Quantity must be a zero or a positive number.");
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
                            string productName = ReadString("Please Enter a Name of product");
                            string productCategory = ReadString("Please Enter a Category of product");
                            double productPrice = ReadDouble("Please Enter a price of product");
                            int productQuantity = ReadInt("Please Enter a quantity of product");

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
                            }
                            else
                            {
                                Console.WriteLine("Invalid choice. Please try again.");
                            }
                            break;

                        case 4:
                            ClearTerminal();
                            isActive = false;
                            break;

                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            PressF();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }
            }
        }
    }
}