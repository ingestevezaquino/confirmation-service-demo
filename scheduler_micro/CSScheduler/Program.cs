using CSScheduler;
using CSScheduler.Services.ConfIPTV;
using CSScheduler.Services.CSCore;
using CSScheduler.Services.DiagADSL;
using Hangfire;
using Hangfire.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddTransient<ICSCoreService, CSCoreService>();
builder.Services.AddHttpClient();
builder.Services.AddHangfire(config =>
{
    config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(Environment.GetEnvironmentVariable("CONNECTION_DB"), new PostgreSqlStorageOptions());
});
builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Disable Hangfire Dashboard Authentication -
// IMPORTANT NOTE: we do this just because we're dealing with a demo app.
app.UseHangfireDashboard("/hangfire", new DashboardOptions()
{
    Authorization = new[] { new DashboardNoAuthorizationFilter() }
});

app.UseRouting();

app.UseAuthorization();

ICSCoreService cscoreService = app.Services.GetRequiredService<ICSCoreService>();
IRecurringJobManager recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();

recurringJobManager.RemoveIfExists("load_tickets");
recurringJobManager.AddOrUpdate<CSCoreService>("load_tickets", e => e.LoadTickets(), "*/1 * * * *");

recurringJobManager.RemoveIfExists("exec_diag_adsl_process");
recurringJobManager.AddOrUpdate<DiagADSLService>("exec_diag_adsl_process", e => e.ExecProcessDiagADSL(), "*/3 * * * *");

recurringJobManager.RemoveIfExists("exec_conf_iptv_process");
recurringJobManager.AddOrUpdate<ConfIPTVService>("exec_conf_iptv_process", e => e.ExecProcessConfIPTV(), "*/3 * * * *");

app.MapRazorPages();

app.Run();
