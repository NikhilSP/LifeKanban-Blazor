namespace LifeKanban.Model.Home;

public class DashboardColumn
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
    public int Order { get; set; }
    
    public DashboardColumn Clone()
    {
        return new DashboardColumn()
        {
            Id = Id, Title = Title, IsVisible = IsVisible, Order = Order
        };
    }
}