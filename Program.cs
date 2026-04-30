using System;
using System.Collections.Generic;
using System.Linq;

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
    static List<Transaction> orderHistory = new List<Transaction>();
    static int receiptCounter = 1;

    static void Main()
    {
        while (true)
        {
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
                case "5": return;
                default: Console.WriteLine("Invalid choice."); break;
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

        if (input == "1")
        {
            DisplayAllByCategory();
        }
        else if (int.TryParse(input, out int catIdx) && catIdx > 1 && catIdx <= categories.Count + 1)
        {
            string selectedCat = categories[catIdx - 2];
            DisplaySingleCategory(selectedCat);
        }
        else
        {
            Console.WriteLine("Invalid selection.");
        }
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
            Console.WriteLine("1. Checkout\n2. Update Quantity\n3. Remove Item\n4. Clear Cart\n5. Back to Main Menu");
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

            Console.Write($"Enter new quantity for {item.Product.Name} (Available: {item.Product.RemainingStock}): ");
            if (int.TryParse(Console.ReadLine(), out int newQty) && newQty > 0 && newQty <= item.Product.RemainingStock)
            {
                item.Quantity = newQty;
                item.Product.ReduceStock(newQty);
                Console.WriteLine("Quantity updated.");
            }
            else
            {
                Console.WriteLine("Invalid quantity. Restoring original amount.");
                item.Product.ReduceStock(item.Quantity);
            }
        }
    }

    static void RemoveFromCart()
    {
        Console.Write("Enter item number to remove: ");
        if (int.TryParse(Console.ReadLine(), out int idx) && idx > 0 && idx <= cart.Count)
        {
            cart[idx - 1].Product.RestoreStock(cart[idx - 1].Quantity);
            cart.RemoveAt(idx - 1);
            Console.WriteLine("Item removed from cart.");
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
            Console.WriteLine("Error: Insufficient payment or invalid format.");
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

        orderHistory.Add(new Transaction { ReceiptNumber = rNum, Date = DateTime.Now, FinalTotal = finalAmount });
        receiptCounter++;
        cart.Clear();

        Console.WriteLine("\n[System Notification]");
        var lowStock = products.Where(p => p.RemainingStock <= p.ReorderLevel).ToList();
        if (lowStock.Any())
        { 
            foreach (var p in lowStock) Console.WriteLine($"LOW STOCK: {p.Name} ({p.RemainingStock} left)");
        }
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
