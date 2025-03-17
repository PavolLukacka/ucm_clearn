var builder = WebApplication.CreateBuilder(args);

// Register ChatGptService and its required HttpClient BEFORE calling builder.Build
builder.Services.AddHttpClient<ChatGptService>();
builder.Services.AddScoped<ChatGptService>();



// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
