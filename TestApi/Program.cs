using TestApi;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddAuthentication(o =>
//    {
//        o.DefaultScheme = "Cookies";
//    })
//    .AddCookie("Cookies", o =>
//    {
//        o.Cookie.HttpOnly= true;
//        o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//    });

builder.Services.AddControllers();

builder.Services.ConfigureSwagger();
builder.Services.ConfigureHsts();
builder.Services.AddResponseCaching();

builder.WebHost.ConfigureKestrel();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseDeveloperExceptionPage();

    //app.UseExceptionHandler("/error-development");
}
else
{
    app.UseHsts();
    //app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();
app.UseSecurityCookies();
app.UseDisableCaching();
app.UseResponseCaching();

//app.UseAuthorization();

app.MapControllers();


app.Run();
