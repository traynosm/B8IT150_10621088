namespace core_strength_yoga_products_api.Data
{
    public class DataGenerationSettings
    {
        public int SimulateOrdersPerDay { get; set; }

        public int SimulateQtyMaximumPerOrder { get; set; }

        public int SimulateForDays { get; set; }

        public int ReplenishWhenReachesBelow { get; set; }

        public int ReplenishTo { get; set; }
    }
}
