namespace DeveloperShop.Domain
{
    public class DiscountCoupon
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Key { get; set; }
        public double DiscountPercentage { get; set; }

        public DiscountCoupon()
        {
        }

        public DiscountCoupon(string key)
        {
            Key = key;
        }
    }
}
