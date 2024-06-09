using System.Reflection;
using DddService.Aggregates;
using DddService.Common;
using DddService.Dto;
using DddService.EventBus;
using DddService.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddDbContext<UserAggregateDbContext>(options =>
{
    options.UseNpgsql("Host=localhost;Port=5432;Database=userAggregate;Username=postgres;Password=12345",
        b => b.MigrationsAssembly("DddService"));
});

// Add Kafka
builder.Services.AddSingleton<KafkaProducerService>();

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

SeedData(app);

// Seed data
void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        using (var db = scope.ServiceProvider.GetRequiredService<UserAggregateDbContext>())
        {
            db.Database.EnsureCreated();
            if (!db.Users.Any() || !db.Passports.Any())
            {
                var passport = Passport.Create(PassportId.Of(Guid.NewGuid()), PassportNumber.Of("763890932"), Surename.Of("Ivanov"), Name.Of("Ivan"), 
                DateOfBirth.Of(new DateTime(1978, 5, 15, 0, 0, 0, DateTimeKind.Utc)), DateOfExpiry.Of(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc)));
                var ticket = Ticket.Of(new DateTime(2025, 2, 10, 0, 0, 0, DateTimeKind.Utc), passport);
                db.Passports.Add(passport);
                db.Users.Add(User.Create(UserId.Of(Guid.NewGuid()), passport, ticket));
                db.SaveChanges();
            }
        }
    }
}


app.MapPost("api/users", async (UserInputModel model, IMediator mediator) =>
{
    var command = new CreateUserCommand(model.Surename, model.Name, model.PassportNumber, model.DateOfBirth, model.DateOfPassportExpiry);
    var response = await mediator.Send(command);
    return Results.Created($"api/users/{response.Id}", response);
});

app.MapGet("api/users", async (IMediator mediator) =>
{
    return await mediator.Send(new GetAllUsersQuery());
});
app.MapGet("api/users/{passportnumber}", async (string passportnumber, IMediator mediator) =>
{
    return await mediator.Send(new GetUserByPassport(passportnumber));
});
app.Run();

public class UserAggregateDbContext : DbContext
{
    public DbSet<Passport> Passports { get; set; }
    public DbSet<User> Users { get; set; }

    private IMediator _mediator;
    public UserAggregateDbContext(DbContextOptions<UserAggregateDbContext> options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

    public UserAggregateDbContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().HasKey(r => r.Id);         
        modelBuilder.Entity<User>().ToTable(nameof(User));
        modelBuilder.Entity<User>().Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(userId => userId.Value, dbId => UserId.Of(dbId));
        
        modelBuilder.Entity<User>().OwnsOne(
            x => x.Ticket,
            a =>
            {
                a.Property(p => p.Price)
                    .HasColumnName("TicketPrice")
                    .HasMaxLength(50);
                a.Property(p => p.DateOfTicketExpiry)
                    .HasColumnName("DateOfTicketExpiry")
                    .HasMaxLength(50);
            }
        );
        
        modelBuilder.Entity<Passport>().HasKey(r => r.Id);
        modelBuilder.Entity<Passport>().ToTable(nameof(Passport));
        modelBuilder.Entity<Passport>().Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(passportId => passportId.Value, dbId => PassportId.Of(dbId));
        
        modelBuilder.Entity<Passport>().ComplexProperty(p=>p.PassportNumber , p =>
        {
            p.Property(x=>x.Value).HasColumnName(nameof(Passport.PassportNumber))
                .IsRequired()
                .HasMaxLength(50);
        });
        modelBuilder.Entity<Passport>().ComplexProperty(p=>p.Surename , p =>
        {
            p.Property(x=>x.Value).HasColumnName(nameof(Passport.Surename))
                .IsRequired()
                .HasMaxLength(50);
        });
        modelBuilder.Entity<Passport>().ComplexProperty(p=>p.Name , p =>
        {
            p.Property(x=>x.Value).HasColumnName(nameof(Passport.Name))
                .IsRequired()
                .HasMaxLength(50);
        });
        modelBuilder.Entity<Passport>().ComplexProperty(p=>p.DateOfBirth , p =>
        {
            p.Property(x=>x.Value).HasColumnName(nameof(Passport.DateOfBirth))
                .IsRequired()
                .HasMaxLength(50);
        });
        modelBuilder.Entity<Passport>().ComplexProperty(p=>p.DateOfExpiry , p =>
        {
            p.Property(x=>x.Value).HasColumnName(nameof(Passport.DateOfExpiry))
                .IsRequired()
                .HasMaxLength(50);
        });
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        // Dispatch Domain Events collection.
        await DispatchEvents(cancellationToken);

        return result;
    }

    private async Task DispatchEvents(CancellationToken cancellationToken)
    {
        var domainEntities = ChangeTracker
            .Entries<IAggregate>()
            .Where(x => x.Entity.GetDomainEvents() != null && x.Entity.GetDomainEvents().Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.GetDomainEvents())
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await _mediator.Publish(domainEvent, cancellationToken);
    }

}
