namespace GoOnlineTodo.Entities.DbSet
{
    public class Todo
    {
        public Guid TodoId { get; set; }
        // For the sake of simplicity, DateTime is used over DateTimeOffset.
        public DateTime ExpiryDateTime { get; set; }
        public string Title { get; set; } = String.Empty;
        public string? Description { get; set; }
        public double PercentComplete { get; set; }
    }
}
