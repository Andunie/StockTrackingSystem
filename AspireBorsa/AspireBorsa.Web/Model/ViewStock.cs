namespace AspireBorsa.Web.Model
{
    public class ViewStock
    {
        public string Symbol { get; set; } = string.Empty;
        public double LastPrice { get; set; }
        public double BuyPrice { get; set; }
        public double SellPrice { get; set; }
        public double HighPrice { get; set; }
        public double LowPrice { get; set; }
        public double AveragePrice { get; set; }
        public double PercentageChange { get; set; }
        public double VolumeLot { get; set; }
        public long VolumeTL { get; set; }
        public DateTime LastTransactionTime { get; set; }
        public bool isActive { get; set; }
    }
}
