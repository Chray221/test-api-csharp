# TestAPI 

TestAPI is a C# .Net MVC API using Visual Studio as the IDE.

### What is this repository for? ###

* Quick summary
* Version
* Framework
* Plugins
* Codes

## Quick Summary 
-TestAPI uses SQLite as the database for storing user, and images and etc.

## Version
- 1.2

## Framework
- Microsoft.AspNetCore.App 3.1.2
- Microsoft.NETCode.App 3.1.0

## Plugins
- [EntityFrameWork](https://docs.microsoft.com/en-us/ef/)
    - using QUERY Loggin
        - add in `Context` class    
``` csharp
    -------------------------------------------------------------------------------------------------------------------------------
        public static readonly LoggerFactory DbCommandDebugLoggerFactory
          = new LoggerFactory(new[] {
          new DebugLoggerProvider()
          });
    -------------------------------------------------------------------------------------------------------------------------------
```    

        - add in `Context's OnConfiguring()`:    
``` csharp
    -------------------------------------------------------------------------------------------------------------------------------
        optionsBuilder
                .UseLoggerFactory(DbCommandDebugLoggerFactory) // to set the logger for DB query
                .EnableSensitiveDataLogging(); // enable logging
    -------------------------------------------------------------------------------------------------------------------------------
```    
    - SQL property options add in `OnModelCreating(ModelBuilder modelBuilder)`
        - using SQLite:    
``` csharp
    -------------------------------------------------------------------------------------------------------------------------------
                modelBuilder.Entity<User>()
                    .Property(b => b.CreatedAt)
                    .HasDefaultValueSql("datetime('now')");
    -------------------------------------------------------------------------------------------------------------------------------
```    
            default value in creating entry:
            add this in model:    
``` csharp
    -------------------------------------------------------------------------------------------------------------------------------
            [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    -------------------------------------------------------------------------------------------------------------------------------
```     
``` csharp
    -------------------------------------------------------------------------------------------------------------------------------
            modelBuilder.Entity<User>().Property(d => d.Id)
                .ValueGeneratedOnAdd();
    -------------------------------------------------------------------------------------------------------------------------------
```    
            
        
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
    - Add in Startup.cs ConfigureServices()     
``` csharp
    -------------------------------------------------------------------------------------------------------------------------------
        services.AddControllers()
            .AddNewtonsoftJson();
    -------------------------------------------------------------------------------------------------------------------------------
```
    - To change the casing to camel:    
``` csharp
    -------------------------------------------------------------------------------------------------------------------------------
        services.AddMvc()
            .AddNewtonsoftJson(options =>
                   options.SerializerSettings.ContractResolver =
                        new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() });
    -------------------------------------------------------------------------------------------------------------------------------
```
- [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp)

### How do I get set up? ###

* Summary of set up
* Configuration
* Dependencies
* Database configuration
* How to run tests
* Deployment instructions

#### How To Add Local Server ? ####
- Goto Program.cs
- Create method GetLocalIPv4()  
``` csharp
    -------------------------------------------------------------------------------------------------------------------------------
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    
    internal static string GetLocalIPv4(NetworkInterfaceType _type = NetworkInterfaceType.Ethernet)
        {  // Checks your IP adress from the local network connected to a gateway. This to avoid issues with double network cards
            string output = "";  // default output
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces()) // Iterate over each network interface
            {  // Find the network interface which has been provided in the arguments, break the loop if found
                //if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                //{   // Fetch the properties of this adapter
                    IPInterfaceProperties adapterProperties = item.GetIPProperties();
                    // Check if the gateway adress exist, if not its most likley a virtual network or smth
                    if (adapterProperties.GatewayAddresses.FirstOrDefault() != null)
                    {   // Iterate over each available unicast adresses
                        foreach (UnicastIPAddressInformation ip in adapterProperties.UnicastAddresses)
                        {   // If the IP is a local IPv4 adress
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {   // we got a match!
                                output = ip.Address.ToString();
                                break;  // break the loop!!
                            }
                        }
                    }
                //}
                // Check if we got a result if so break this method
                if (output != "") { break; }
            }
            // Return results
            return output;
        }
    -------------------------------------------------------------------------------------------------------------------------------
```
- Add to CreateHostBuilder property:    
```  csharp
    -------------------------------------------------------------------------------------------------------------------------------
       webBuilder.UseUrls("http://localhost:5000",
                        $"http://{GetLocalIPv4()}:5000"); // my IP Address
    -------------------------------------------------------------------------------------------------------------------------------
```
- To run
    - in terminal   
``` sh
    -------------------------------------------------------------------------------------------------------------------------------
        dotnet run
    -------------------------------------------------------------------------------------------------------------------------------
```
    - Visual Studio
        Click Run


#### How To Add Database
##### Using EntityFramework
- Add in `Startup.cs` `ConfigureServices()`
    - using In Memory   
``` csharp
    -------------------------------------------------------------------------------------------------------------------------------
        services.AddDbContext<TestContext>((opt) =>
            opt.UseInMemoryDatabase("UserList"));
    -------------------------------------------------------------------------------------------------------------------------------
```
    - using SQLite  
``` csharp
        services.AddDbContext<TestContext>(options =>
        {
            options.UseSqlite("Filename=./test_context.db"); //create sqlite db in project
        });
```    
    - using SQL     
``` csharp
    -------------------------------------------------------------------------------------------------------------------------------
        services.AddDbContext<ConfigurationContext>(options => {
            options.UseSqlServer(Configuration.GetConnectionString("MyConnection"));
        });
    -------------------------------------------------------------------------------------------------------------------------------
```    
        Then ADD in `appsettings.json` or `appsettings.Development.json`    
``` csharp
    -------------------------------------------------------------------------------------------------------------------------------
        ,"AllowedHosts": "*",
            "ConnectionStrings": {
                "MyConnection": "server=.;database=myDb;trusted_connection=true;"
            }
    -------------------------------------------------------------------------------------------------------------------------------
```

#### How To Migrate
##### Using EntityFramework 6.4.4
- First     
``` sh
        dotnet ef migrations add InitialCreate
```
- Then Add to `Context's Constructor()`     
```  csharp
    -------------------------------------------------------------------------------------------------------------------------------
       if (Database.GetPendingMigrations().Any())
        {
            Database.Migrate();
        }
    -------------------------------------------------------------------------------------------------------------------------------
```
### Contribution guidelines ###

* Writing tests
* Code review
* Other guidelines

### Who do I talk to? ###

* Repo owner or admin
* Other community or team contact