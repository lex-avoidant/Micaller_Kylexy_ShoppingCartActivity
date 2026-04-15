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
