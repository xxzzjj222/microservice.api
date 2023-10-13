namespace MicroService.Api.BusinessService.Models.Dtos
{
    /// <summary>
    /// 订单创建Dto
    /// </summary>
    public class OrderCreateDto
    {
        public int ProductName { set; get; }
        public int ProductPrice { set; get; }
    }
}
