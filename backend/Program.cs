using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppContext>(options => options.UseSqlite($"Data Source=./data.sqlite3"));
builder.Services.Configure<JsonOptions>(j => 
    j.SerializerOptions.Converters.Add(new DateTimeConverterUsingDateTimeParse())
);

var app = builder.Build();


new AppContext().Setup(c =>
{
    c.Database.Migrate();
});


// app.MapGet("/", () => "Hello World!");

app.MapGet("/boms", (AppContext ctx) =>
    ctx.BillOfMaterials
        ?.Include(b => b.BillOfMaterialDetails)
);
app.MapPost("/boms", (AppContext ctx, BillOfMaterial b) =>
{
    ctx.Update(b);
    ctx.SaveChangesAsync();
    return b;
});


app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();

public class DateTimeConverterUsingDateTimeParse : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(typeToConvert == typeof(DateTime));
        return DateTime.Parse(reader.GetString() ?? string.Empty);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUniversalTime().ToString("o"));
    }
}

public class AppContext : DbContext
{
    public AppContext()
    {

    }

    public AppContext(DbContextOptions<AppContext> options)
    : base(options)
    { }

    public DbSet<BillOfMaterial>? BillOfMaterials { get; set; }
    public DbSet<BillOfMaterialDetail>? BillOfMaterialDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source=./data.sqlite3");

    public override int SaveChanges()
    {
        var now = DateTime.UtcNow;

        foreach (var changedEntity in ChangeTracker.Entries())
        {
            if (changedEntity.Entity is BaseModel entity)
            {
                switch (changedEntity.State)
                {
                    case EntityState.Added:
                        entity.CreatedAt = now;
                        entity.UpdatedAt = now;
                        break;

                    case EntityState.Modified:
                        Entry(entity).Property(x => x.CreatedAt).IsModified = false;
                        entity.UpdatedAt = now;
                        break;
                }
            }
        }

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var changedEntity in ChangeTracker.Entries())
        {
            if (changedEntity.Entity is BaseModel entity)
            {
                switch (changedEntity.State)
                {
                    case EntityState.Added:
                        entity.CreatedAt = now;
                        entity.UpdatedAt = now;
                        break;

                    case EntityState.Modified:
                        Entry(entity).Property(x => x.CreatedAt).IsModified = false;
                        entity.UpdatedAt = now;
                        break;
                }
            }
        }

        return base.SaveChangesAsync();
    }


}

public class BaseModel
{
    public long? Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class BillOfMaterial : BaseModel
{
    public string? Name { get; set; }
    public List<BillOfMaterialDetail>? BillOfMaterialDetails { get; set; }
}
public class BillOfMaterialDetail : BaseModel
{
    string? Name { get; set; }
    public long? BillOfMaterialId { get; set; }

    [JsonIgnore]
    public BillOfMaterial? BillOfMaterial { get; set; }
}
