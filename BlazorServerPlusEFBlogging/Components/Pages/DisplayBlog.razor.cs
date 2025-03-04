using BlazorApp3.Persistence;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp3.Components.Pages;

public partial class DisplayBlog : ComponentBase
{
    private Task<List<Post>> GetPosts()
    {
        using var ctx = new BloggingContext();
        return ctx.Posts.ToListAsync();
    }

    private async Task DeletePost(Post post)
    {
        await using var ctx = new BloggingContext();
        ctx.Remove(post);
        await ctx.SaveChangesAsync();
    }
}