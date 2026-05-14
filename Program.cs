using System;
using System.Collections.Generic;
using System.Linq;

class Product
{
    private int _id;
    private string _name = "";
    private string _category = "";
    private double _price;
    private int _remainingStock;
    private int _reorderLevel = 5;

    public int Id => _id;
    public string Name => _name;
    public string Category => _category;
    public double Price => _price;
    public int RemainingStock => _remainingStock;
    public int ReorderLevel => _reorderLevel;

    public Product(int id, string name, string category, double price, int stock)
    {
        _id = id;
        _name = name;
        _category = category;
        _price = price;
        _remainingStock = stock;
    }

    public void DisplayProduct()
    {
        Console.WriteLine($"{_id,-3} | {_name,-15} | Php {_price,-8} | Stock: {_remainingStock}");
    }

    public void ReduceStock(int quantity) => _remainingStock -= quantity;
    public void RestoreStock(int quantity) => _remainingStock += quantity;
}

class CartItem
{
    public Product Product { get; private set; }
    public int Quantity { get; set; }

    public double Subtotal => Product.Price * Quantity;

    public CartItem(Product product, int quantity)
    {
        Product = product;
        Quantity = quantity;
    }
}

class Transaction
{
    public string ReceiptNumber { get; }
    public DateTime Date { get; }
    public double FinalTotal { get; }

    public Transaction(string receiptNumber, DateTime date, double finalTotal)
    {
        ReceiptNumber = receiptNumber;
        Date = date;
        FinalTotal = finalTotal;
    }

    public void DisplayHistory()
    {
        Console.WriteLine($"Receipt #{ReceiptNumber} - {Date:MMMM dd, yyyy hh:mm tt} - Final Total: Php {FinalTotal}");
    }
}

class Program
{
    private static List<Product> products = new List<Product>
    {
        new Product(1, "Lilies", "Flowers", 450, 25),
        new Product(2, "Daisies", "Flowers", 350, 21),
        new Product(3, "Roses", "Flowers", 400, 18),
        new Product(4, "Tulips", "Flowers", 300, 19),
        new Product(5, "Baby's breath", "Fillers", 200, 35),
        new Product(6, "Fern Leaves", "Fillers", 200, 30)
    };

    private static List<CartItem> cart = new List<CartItem>();
    private static List<Transaction> orderHistory = new List<Transaction>();
    private static int receiptCounter = 1;

    static void Main()
    {
        while (true)
        {
            CheckLowStock();

            Console.WriteLine("\n--- FLOWER SHOP MAIN MENU ---");
            Console.WriteLine("1. View Products to Order");
            Console.WriteLine("2. Search Product");
            Console.WriteLine("3. Cart Management (" + cart.Count + " items)");
            Console.WriteLine("4. View Order History");
            Console.WriteLine("5. Exit");
            Console.Write("\nSelect option: ");

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1": ShowProductMenu(); break;
                case "2": SearchProduct(); break;
                case "3": CartMenu(); break;
                case "4": ViewHistory(); break;
                case "5": Console.WriteLine("\nEnding Transaction... Thank you."); return;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }
    }

    static void CheckLowStock()
    {
        var lowStock = products.Where(p => p.RemainingStock <= p.ReorderLevel).ToList();
        if (lowStock.Any())
        {
            Console.WriteLine("\n[System Notification: Low Stock Alert]");
            foreach (var p in lowStock)
            {
                Console.WriteLine($"{p.Name} ({p.RemainingStock} left)");
            }
        }
    }

    static void ShowProductMenu()
    {
        Console.WriteLine("\n--- PRODUCT/CATEGORY DISPLAY ---");
        Console.WriteLine("1. View All Products");

        var categories = products.Select(p => p.Category).Distinct().ToList();
        for (int i = 0; i < categories.Count; i++)
        {
            Console.WriteLine($"{i + 2}. {categories[i]}");
        }

        Console.Write("Select option: ");
        string input = Console.ReadLine() ?? "";

        if (input == "1") DisplayAllByCategory();
        else if (int.TryParse(input, out int catIdx) && catIdx > 1 && catIdx <= categories.Count + 1)
        {
            DisplaySingleCategory(categories[catIdx - 2]);
        }
        else Console.WriteLine("Invalid selection.");
    }

    static void DisplaySingleCategory(string category)
    {
        Console.WriteLine($"\n--- {category.ToUpper()} ---");
        Console.WriteLine("ID  | Name            | Price        | Stock");
        Console.WriteLine("------------------------------------------------");
        var items = products.Where(p => p.Category == category);
        foreach (var item in items) item.DisplayProduct();

        if (GetYN("\nWould you like to add an item to your cart?")) AddToCart();
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
            foreach (var item in itemsInCategory) item.DisplayProduct();
        }

        if (GetYN("\nWould you like to add an item to your cart?")) AddToCart();
    }

    static void SearchProduct()
    {
        Console.Write("\nEnter product name to search: ");
        string search = Console.ReadLine()?.ToLower() ?? "";
        var filtered = products.Where(p => p.Name.ToLower().Contains(search) || p.Category.ToLower().Contains(search)).ToList();

        if (filtered.Count == 0) { Console.WriteLine("No matches found."); return; }

        Console.WriteLine("\nSearch Results:");
        foreach (var p in filtered) { Console.Write($"[{p.Category}] "); p.DisplayProduct(); }

        if (GetYN("Add one of these to cart?")) AddToCart();
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
                else cart.Add(new CartItem(selected, qty));

                selected.ReduceStock(qty);
                Console.WriteLine($"Success! {qty} {selected.Name} added to cart.");
            }
            else Console.WriteLine("Invalid quantity or inventory shortage.");
        }
    }

    static void CartMenu()
    {
        while (true)
        {
            Console.WriteLine("\n--- YOUR SHOPPING CART ---");
            if (cart.Count == 0) { Console.WriteLine("Your cart is empty."); break; }

            double total = 0;
            for (int i = 0; i < cart.Count; i++)
            {
                Console.WriteLine($"{i + 1}. [{cart[i].Product.Category}] {cart[i].Product.Name} (x{cart[i].Quantity}) - Php {cart[i].Subtotal}");
                total += cart[i].Subtotal;
            }
            Console.WriteLine($"\nSubtotal: Php {total}");
            Console.WriteLine("1. Checkout\n2. Update Quantity\n3. Remove Item\n4. Clear Cart\n5. Back");
            Console.Write("Selection: ");
            string c = Console.ReadLine() ?? "";

            if (c == "1") { Checkout(total); break; }
            if (c == "2") UpdateCartItem();
            if (c == "3") RemoveFromCart();
            if (c == "4")
            {
                foreach (var item in cart) item.Product.RestoreStock(item.Quantity);
                cart.Clear();
                Console.WriteLine("Cart emptied.");
                break;
            }
            if (c == "5") break;
        }
    }

    static void UpdateCartItem()
    {
        Console.Write("Enter item number to update: ");
        if (int.TryParse(Console.ReadLine(), out int idx) && idx > 0 && idx <= cart.Count)
        {
            var item = cart[idx - 1];
            item.Product.RestoreStock(item.Quantity);
            Console.Write($"Enter new quantity for {item.Product.Name}: ");
            if (int.TryParse(Console.ReadLine(), out int newQty) && newQty > 0 && newQty <= item.Product.RemainingStock)
            {
                item.Quantity = newQty;
                item.Product.ReduceStock(newQty);
                Console.WriteLine("Quantity updated.");
            }
            else item.Product.ReduceStock(item.Quantity);
        }
    }

    static void RemoveFromCart()
    {
        Console.Write("Enter item number to remove: ");
        if (int.TryParse(Console.ReadLine(), out int idx) && idx > 0 && idx <= cart.Count)
        {
            cart[idx - 1].Product.RestoreStock(cart[idx - 1].Quantity);
            cart.RemoveAt(idx - 1);
            Console.WriteLine("Item removed.");
        }
    }

    static void Checkout(double grandTotal)
    {
        double discount = grandTotal >= 5000 ? grandTotal * 0.10 : 0;
        double finalAmount = grandTotal - discount;
        Console.WriteLine($"\nFinal Amount Due: Php {finalAmount}");

        double payment = 0;
        while (true)
        {
            Console.Write("Enter payment: ");
            if (double.TryParse(Console.ReadLine(), out payment) && payment >= finalAmount) break;
            Console.WriteLine("Error: Insufficient payment.");
        }

        string rNum = receiptCounter.ToString("D4");
        Console.WriteLine("\n******************************");
        Console.WriteLine($"   OFFICIAL RECEIPT: {rNum}");
        Console.WriteLine($"   DATE: {DateTime.Now:MM/dd/yyyy HH:mm}");
        Console.WriteLine("******************************");
        foreach (var item in cart)
            Console.WriteLine($"{item.Product.Name,-15} x{item.Quantity}  Php {item.Subtotal}");

        Console.WriteLine("------------------------------");
        Console.WriteLine($"Grand Total:   Php {grandTotal}");
        Console.WriteLine($"Discount:      -Php {discount}");
        Console.WriteLine($"FINAL TOTAL:   Php {finalAmount}");
        Console.WriteLine("------------------------------");
        Console.WriteLine($"Total Paid:    Php {payment}");
        Console.WriteLine($"Change:        Php {payment - finalAmount}");
        Console.WriteLine("******************************");
        Console.WriteLine("   Thank you for shopping!");


        orderHistory.Add(new Transaction(rNum, DateTime.Now, finalAmount));
        receiptCounter++;
        cart.Clear();
    }

    static void ViewHistory()
    {
        Console.WriteLine("\n--- COMPLETED TRANSACTIONS ---");
        if (orderHistory.Count == 0) Console.WriteLine("No history found.");
        else foreach (var t in orderHistory) t.DisplayHistory();
    }

    static bool GetYN(string msg)
    {
        while (true)
        {
            Console.Write($"{msg} (Y/N): ");
            string input = Console.ReadLine()?.Trim().ToUpper() ?? "";
            if (input == "Y") return true;
            if (input == "N") return false;
            Console.WriteLine("Please enter only 'Y' or 'N'.");
        }
    }
}
