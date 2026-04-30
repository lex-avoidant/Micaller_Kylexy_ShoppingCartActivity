using System;

class Product
{
    public int Id;
    public string Name = "";
    public string Category = "";
    public double Price;
    public int RemainingStock;
    public int ReorderLevel = 5;

    public void DisplayProduct()
    {
        Console.WriteLine($"{Id,-3} | {Name,-15} | Php {Price,-8} | Stock: {RemainingStock}");
    }

    public void ReduceStock(int quantity) => RemainingStock -= quantity;
    public void RestoreStock(int quantity) => RemainingStock += quantity;
}

class CartItem
{
    public Product Product { get; set; } = new Product();
    public int Quantity { get; set; }
    public double Subtotal => Product.Price * Quantity;
}

class Transaction
{
    public string ReceiptNumber = "";
    public DateTime Date;
    public double FinalTotal;

    public void DisplayHistory()
    {
        Console.WriteLine($"Receipt #{ReceiptNumber} - {Date:MMMM dd, yyyy hh:mm tt} - Final Total: Php {FinalTotal}");
    }
}

class Program
{
    static List<Product> products = new List<Product>
    {
        new Product { Id = 1, Name = "Lilies", Category = "Flowers", Price = 450, RemainingStock = 25 },
        new Product { Id = 2, Name = "Daisies", Category = "Flowers", Price = 350, RemainingStock = 21 },
        new Product { Id = 3, Name = "Roses", Category = "Flowers", Price = 400, RemainingStock = 18 },
        new Product { Id = 4, Name = "Tulips", Category = "Flowers", Price = 300, RemainingStock = 19 },
        new Product { Id = 5, Name = "Baby's breath", Category = "Fillers", Price = 200, RemainingStock = 35 },
        new Product { Id = 6, Name = "Fern Leaves", Category = "Fillers", Price = 200, RemainingStock = 30 }
    };

    static List<CartItem> cart = new List<CartItem>();
    static List<Transaction> orderHistory = new List<Transaction>(); //
    static int receiptCounter = 1;

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("--- FLOWER SHOP MAIN MENU ---");
            Console.WriteLine("1. View Products to Order");
            Console.WriteLine("2. Search Product");
            Console.WriteLine("3. Cart Management (" + cart.Count + " items)");
            Console.WriteLine("4. View Order History");
            Console.WriteLine("5. Exit");
            Console.Write("\nSelect option: ");

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1": DisplayAllByCategory(); break;
                case "2": SearchProduct(); break;
                case "3": CartMenu(); break;
                case "4": ViewHistory(); break;
                case "5": return;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }
    }
  
    static void DisplayAllByCategory()
    {
        var categories = products.Select(p => p.Category).Distinct();

        foreach (var category in categories)
        {
            Console.WriteLine($"\n--- {category.ToUpper()} ---");
            Console.WriteLine("ID  | Name            | Price        | Stock");
            Console.WriteLine("------------------------------------------------");
            var itemsInCategory = products.Where(p => p.Category == category);
            foreach (var item in itemsInCategory)
            {
                item.DisplayProduct();
            }
        }

        if (GetYN("\nWould you like to add an item to your cart?"))
        {
            AddToCart();
        }
    }

    static void SearchProduct()
    {
        Console.Write("\nEnter product name to search: ");
        string search = Console.ReadLine()?.ToLower() ?? "";

        var filtered = products.Where(p => p.Name.ToLower().Contains(search) || p.Category.ToLower().Contains(search)).ToList();

        if (filtered.Count == 0)
        {
            Console.WriteLine("No matches found.");
            return;
        }

        Console.WriteLine("\nSearch Results:");
        foreach (var p in filtered)
        {
            Console.Write($"[{p.Category}] ");
            p.DisplayProduct();
        }

        if (GetYN("Add one of these to cart?"))
        {
            AddToCart();
        }
    }

    static void AddToCart()
    {
        Console.Write("Enter Product Id: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var selected = products.FirstOrDefault(p => p.Id == id);
            if (selected == null) { Console.WriteLine("Product not found."); return; }
            if (selected.RemainingStock <= 0) { Console.WriteLine("Out of stock."); return; }

            Console.Write($"Enter quantity: ");
            if (int.TryParse(Console.ReadLine(), out int qty) && qty > 0 && qty <= selected.RemainingStock)
            {
                var existing = cart.FirstOrDefault(c => c.Product.Id == id);
                if (existing != null) existing.Quantity += qty;
                else cart.Add(new CartItem { Product = selected, Quantity = qty });

                selected.ReduceStock(qty);
                Console.WriteLine($"Success! {qty} {selected.Name} added to cart.");
            }
            else Console.WriteLine("Invalid quantity or inventory shortage.");
        }
    }
