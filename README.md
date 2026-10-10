# Inventory Manager

A C# console application for managing product stock in a warehouse. I built it to practice object-oriented programming and working with a SQLite database.

## Features

- **Add products:** enter an ID, name, category, price, and quantity. Duplicate IDs are rejected.
- **View all products:** list every product with its price and stock level.
- **Search:** find products by name or ID (partial matches work).
- **Delete:** choose a product from the numbered search results and delete it.
- **Persistent storage:** data is saved in a local SQLite database, so it stays after the program closes. The first run adds 5 sample products.

## Planned

- Update the stock quantity of a product
- Low-stock alerts
- Search by category
- Move the database code into its own class

## Built With

- C# / .NET
- SQLite (`Microsoft.Data.Sqlite`)

## Getting Started

1. Install the .NET SDK.
2. Clone the repository.
3. Run `dotnet run` in the project folder. The database file `inventory.db` is created automatically.
