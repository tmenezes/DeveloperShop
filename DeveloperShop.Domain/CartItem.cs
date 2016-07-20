namespace DeveloperShop.Domain
{
    public class CartItem
    {
        public Developer Developer { get; set; }
        public int AmountOfHours { get; set; }
        public decimal TotalPrice => (Developer?.Price ?? 0) * AmountOfHours;


        public CartItem()
        {

        }
        public CartItem(Developer developer, int amountOfHours)
        {
            Developer = developer;
            AmountOfHours = amountOfHours;
        }
    }
}