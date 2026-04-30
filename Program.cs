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
