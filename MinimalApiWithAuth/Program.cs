using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddSession(options =>
// {
//     options.IdleTimeout = TimeSpan.FromSeconds(10);
//     options.Cookie.HttpOnly = true;
//     options.Cookie.IsEssential = true;
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// app.UseSession();

var sessions = new Dictionary<string, Session>();
const string sessionCookie = "eu.recom.session";

app.MapGet("/login", (string username, string password, HttpContext ctx) =>
    {
        if (username == "admin" && password == "catmin")
        {
            var sessionId = Guid.NewGuid().ToString();
            if (!sessions.ContainsKey(sessionId))
                sessions[sessionId] = new Session();
            sessions[sessionId].Authenticated = true;

            ctx.Response.Cookies.Append(sessionCookie, sessionId, new ()
            {
                Expires = DateTimeOffset.Now.AddYears(2),
            });
            return "Authorized :)";
        }
        else
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return "Unauthorized :(";
        }
    })
    .WithName("Login")
    .WithOpenApi();

app.MapGet("/needs-auth", (HttpContext ctx) =>
    {
        ctx.Request.Cookies.TryGetValue(sessionCookie, out var sessionId);
        if (
            sessionId == null || 
            !sessions.TryGetValue(sessionId, out Session? session) || 
            !session.Authenticated
        )
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return "Unauthenticated :(";
        }

        return "Authenticated :)";
    })
    .WithName("Authorized")
    .WithOpenApi();

app.MapGet("/logout", (HttpContext ctx) =>
{
    // if (ctx.Request.Cookies.ContainsKey(sessionCookie))
    ctx.Response.Cookies.Delete(sessionCookie);
    sessions.Remove(sessionCookie);
});

app.Run();

class Session
{
    public bool Authenticated { get; set; }
}
