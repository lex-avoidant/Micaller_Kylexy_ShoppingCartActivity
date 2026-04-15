using System;

class Product;
[
  public int Id;
  public string Name = "";
  public double Price;
  public int RemainingStock;

  public void DisplayProduct()
  {
    Console.WriteLine($"{Id}, {Name}, - Php{Price} (Stock: {RemainingStock})");
  }
  public double ComputeTotal(int quantity)
  {
    return Price * quantity;
  }
  public bool IsAvailable(int quantity)
  {
    return quantity <= RemainingStock;
  }
  public void ReduceStock(int quantity)
  {
    RemainingStock -= quantity;
  }

  class Cart
  {
    public Product Product = new Product ();
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
        new Product { Id = 1, Name = "Lilies", Price = 300, RemaininStock = 11 },
        new Product { Id = 2, Name = "Daisies", Price = 200, RemaininStock = 6 },
        new Product { Id = 3, Name = "Roses", Price = 250, RemaininStock = 7 },
        new Product { Id = 4, Name = "Tulips", Price = 150, RemaininStock = 9 }
      };
      Cart[] cart = new Cart[10];
      int cartCount = 0;

      string again = "Yes";
      do
      {
        Console.WriteLine("\nFlower Shop Store Menu");
        foreach (var p in products)
        {
          p.Display();
        }
        
        Console.Write("\nEnter Product Id: ");
        if (!int.TryParse(Console.ReadLine(), out int prodNum) ||
            prodNum < 1 || prodNum > products.Length)
        {
          Console.WriteLine("Invalid Product Id.");
          continue;
        }
        
        Product selected = products[prodNum - 1];
        if (selected.RemainingSock == 0)
        {
          Console.WriteLine("This product is out of stock.");
          cotinue;
        }

        Console.Write("Enter quantity: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
        {
          Console.WriteLine("Inventory shortage.");
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
