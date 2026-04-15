using System;

class Product
{
    public int Id;
    public string Name = "";
    public double Price;
    public int RemainingStock;

    public void DisplayProduct()
    {
        Console.WriteLine($"{Id}. {Name} - Php {Price} (Stock: {RemainingStock})");
    }

    public double ComputeTotal(int quantity)
    {
        return Price * quantity;
    }

    public void ReduceStock(int quantity)
    {
        RemainingStock -= quantity;
    }
}

class Cart
{
    public Product Product = new Product();
    public int Quantity;
    public double Subtotal;

    public void Update(int quantity)
    {
        Quantity += quantity;
        Subtotal += Product.Price * quantity;
    }
}

class Program
{
    static void Main()
    {
        Product[] products = new Product[]
        {
            new Product { Id = 1, Name = "Lilies", Price = 300, RemainingStock = 11 },
            new Product { Id = 2, Name = "Daisies", Price = 200, RemainingStock = 6 },
            new Product { Id = 3, Name = "Roses", Price = 250, RemainingStock = 7 },
            new Product { Id = 4, Name = "Tulips", Price = 150, RemainingStock = 9 },
            new Product { Id = 5, Name = "Baby's breath", Price = 100, RemainingStock = 15 }
        };

        Cart[] cart = new Cart[10];
        int cartCount = 0;
        string again = "YES";

        do
        {
            Console.WriteLine("\n--- Flower Shop Store Menu ---");
            foreach (var p in products)
            {
                p.DisplayProduct();
            }

            Console.Write("\nEnter Product Id: ");
            if (!int.TryParse(Console.ReadLine(), out int prodNum) || prodNum < 1 || prodNum > products.Length)
            {
                Console.WriteLine("Invalid Product Id.");
                continue;
            }

            Product selected = products[prodNum - 1];
            if (selected.RemainingStock == 0)
            {
                Console.WriteLine("Item out of stock.");
                continue;
            }

            Console.Write("Enter quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0 || quantity > selected.RemainingStock)
            {
                Console.WriteLine("Invalid quantity or inventory shortage.");
                continue;
            }

            bool exists = false;
            for (int i = 0; i < cartCount; i++)
            {
                if (cart[i].Product.Id == selected.Id)
                {
                    cart[i].Update(quantity);
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                if (cartCount >= cart.Length)
                {
                    Console.WriteLine("Your cart is full.");
                    continue;
                }
                cart[cartCount] = new Cart
                {
                    Product = selected,
                    Quantity = quantity,
                    Subtotal = selected.ComputeTotal(quantity)
                };
                cartCount++;
            }

            selected.ReduceStock(quantity);
            Console.WriteLine("Item successfully added to cart!");

            Console.Write("\nDo you want to add more items? (Yes/No): ");
            string? input = Console.ReadLine();
            again = input != null ? input.Trim().ToUpper() : "NO";

        } while (again == "YES");

        Console.WriteLine("\n--- Final Receipt ---");
        double grandTotal = 0;
        for (int i = 0; i < cartCount; i++)
        {
            Console.WriteLine($"{cart[i].Product.Name} x{cart[i].Quantity} - Php {cart[i].Subtotal}");
            grandTotal += cart[i].Subtotal;
        }

        double discount = 0;
        if (grandTotal >= 500)
        {
            discount = grandTotal * 0.05;
            Console.WriteLine($"\nGrand Total: Php {grandTotal}");
            Console.WriteLine($"Discount (5%): -Php {discount}");
        }

        double finalTotal = grandTotal - discount;
        Console.WriteLine($"Final amount to pay: Php {finalTotal}");

        Console.WriteLine("\n--- Remaining Stock Update ---");
        foreach (var p in products)
        {
            Console.WriteLine($"{p.Name}: {p.RemainingStock}");
        }

        Console.WriteLine("\nThank you for shopping!");
    }
}
