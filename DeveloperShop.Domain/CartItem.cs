namespace DeveloperShop.Domain
{
    public class CartItem
    {
        public Developer Developer { get; }
        public int AmountOfHours { get; }
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