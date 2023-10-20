namespace MicroService.Api.OrderService.Models.Dtos
{
    /// <summary>
    /// 订单库存 Dto
    /// </summary>
    public class OrderStockDto
    {
        public int ProductId { set; get; }
        public int Stock { set; get; }
    }
}
