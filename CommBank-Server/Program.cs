using System.Text.Json;
using CommBank.Models;
using CommBank.Services;
using MongoDB.Driver;
using System.Text.Json;
using Tag = CommBank.Models.Tag;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("Secrets.json");

var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("CommBank"));
var mongoDatabase = mongoClient.GetDatabase("CommBank");

IAccountsService accountsService = new AccountsService(mongoDatabase);
IAuthService authService = new AuthService(mongoDatabase);
IGoalsService goalsService = new GoalsService(mongoDatabase);
ITagsService tagsService = new TagsService(mongoDatabase);
ITransactionsService transactionsService = new TransactionsService(mongoDatabase);
IUsersService usersService = new UsersService(mongoDatabase);

builder.Services.AddSingleton(accountsService);
builder.Services.AddSingleton(authService);
builder.Services.AddSingleton(goalsService);
builder.Services.AddSingleton(tagsService);
builder.Services.AddSingleton(transactionsService);
builder.Services.AddSingleton(usersService);

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(builder => builder
   .AllowAnyOrigin()
   .AllowAnyMethod()
   .AllowAnyHeader());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


List<Account>? accounts = JsonSerializer.Deserialize<List<Account>>( "../Data/Accounts.json");
List<Goal>? goals = JsonSerializer.Deserialize<List<Goal>>( "../Data/Goals.json");
List<Tag>? tags = JsonSerializer.Deserialize<List<Tag>>( "../Data/Tags.json");
List<Transaction>? transactions = JsonSerializer.Deserialize<List<Transaction>>( "../Data/Transactions.json");
List<User>? users = JsonSerializer.Deserialize<List<User>>( "../Data/Users.json");

foreach(Account account in accounts){
    accountsService.CreateAsync(account);
}
foreach(Goal goal in goals)
{
    goalsService.CreateAsync(goal);
}
foreach(Tag tag in tags){
    tagsService.CreateAsync(tag);
}
foreach(Transaction transaction in transactions)
{
    transactionsService.CreateAsync(transaction);
}
foreach(User user in users)
{
    usersService.CreateAsync(user);
}

app.Run();

