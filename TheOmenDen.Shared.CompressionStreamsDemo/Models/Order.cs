namespace TheOmenDen.Shared.CompressionStreamsDemo.Models;

public sealed class Order
{
    public int OrderId { get; set; }
    public string Item { get; set; }
    public int Quantity { get; set; }
    public int? LotNumber { get; set; }
}
