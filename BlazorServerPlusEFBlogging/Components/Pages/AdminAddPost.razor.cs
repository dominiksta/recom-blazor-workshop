using BlazorApp3.Persistence;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp3.Components.Pages;

public partial class AdminAddPost : ComponentBase
{
    private DbContext _ctx = new BloggingContext();

    public async Task AddPost(string title, string content)
    {
        await _ctx.AddAsync(new Post()
        {
            Title = title, Content = content,
        });
        await _ctx.SaveChangesAsync();
    }
}