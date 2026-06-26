namespace BloggingSystem.Models;

/// <summary>
/// Carries the paged + filtered post list to the Index view.
/// </summary>
public class PostListViewModel
{
    public IReadOnlyList<Post> Posts { get; set; } = new List<Post>();
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int TotalCount { get; set; }

    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}
