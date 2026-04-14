using System;

class Product;
[
  public int Id;
  public string Name;
  public double Price;
  public int RemainingStock;

  public void DisplayProduct()
  {
    Console.WriteLine($"{Id}, {Name}, -₱{Price} (Stock: {RemainingStock})");
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
