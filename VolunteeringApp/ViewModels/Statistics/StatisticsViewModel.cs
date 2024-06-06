namespace VolunteeringApp.ViewModels.Statistics
{
    public class StatisticsViewModel
    {
            public string Title { get; set; }
            public string? Description { get; set; }
            public StatisticType Type { get; set; }
            public object Value { get; set; } // Using object to handle different data types
            public DateTime GeneratedAt { get; set; }
        public enum StatisticType
        {
            Count,
            Percentage,
            Average,
            List,
            Other
        }
    }
}
