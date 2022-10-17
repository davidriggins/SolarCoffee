namespace SolarCoffee.Web.ViewModels
{
    public class InvoiceModel
    {
        /// <summary>
        /// View model for open SalesOrders
        /// </summary>
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int CustomerId { get; set; }
        public List<SalesOrderItemModel> LineItems { get; set; }
    }


    /// <summary>
    /// View model for SalesOrderItems
    /// </summary>
    public class SalesOrderItemModel
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public ProductViewModel Product { get; set; }
    }
}
