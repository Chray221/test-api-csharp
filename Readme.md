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
        public static readonly LoggerFactory DbCommandDebugLoggerFactory
          = new LoggerFactory(new[] {
          new DebugLoggerProvider()
          });
        ```    

        - add in `Context's OnConfiguring()`:    
        
        
        ``` csharp
    
        optionsBuilder
                .UseLoggerFactory(DbCommandDebugLoggerFactory) // to set the logger for DB query
                .EnableSensitiveDataLogging(); // enable logging
    
        ```    
    - SQL property options add in `OnModelCreating(ModelBuilder modelBuilder)`
        - using SQLite:    
        
        ``` csharp
        modelBuilder.Entity<User>()
            .Property(b => b.CreatedAt)
            .HasDefaultValueSql("datetime('now')");
    
        ```  
        
        default value in creating entry:
        add this in model:    
        
        ``` csharp
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]    
        ```     
        ``` csharp    
            modelBuilder.Entity<User>().Property(d => d.Id)
                .ValueGeneratedOnAdd();    
        ```    
            
        
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
    - Add in Startup.cs ConfigureServices()     
        ``` csharp
    
        services.AddControllers()
            .AddNewtonsoftJson();
    
        ```
    - To change the casing to camel:    
        ``` csharp    
        services.AddMvc()
            .AddNewtonsoftJson(options =>
                   options.SerializerSettings.ContractResolver =
                        new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() });    
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
    
    ```
- Add to CreateHostBuilder property:    
    ```  csharp    
   webBuilder.UseUrls("http://localhost:5000",
                    $"http://{GetLocalIPv4()}:5000"); // my IP Address    
    ```
- To run
    - in terminal   
    ``` sh    
    dotnet run    
    ```
    - Visual Studio
        Click Run

#### How To Add Server ? ####

#### How To Add Database
##### Using EntityFramework
- Add in `Startup.cs` `ConfigureServices()`
    - using In Memory   
        ``` csharp    
        services.AddDbContext<TestContext>((opt) =>
            opt.UseInMemoryDatabase("UserList"));    
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
    
        services.AddDbContext<ConfigurationContext>(options => {
            options.UseSqlServer(Configuration.GetConnectionString("MyConnection"));
        });    
        ```    
        Then ADD in `appsettings.json` or `appsettings.Development.json`    
        ``` csharp    
        ,"AllowedHosts": "*",
            "ConnectionStrings": {
                "MyConnection": "server=.;database=myDb;trusted_connection=true;"
            }    
        ```
    - For SQL Database using docker (SQL FOR MAC)
        - https://hub.docker.com/editions/community/docker-ce-desktop-mac
        - https://www.c-sharpcorner.com/article/entity-framework-core-with-sql-server-in-docker-container/
        - FOR DOCKER 
            - Open Docker
            - Goto folder
            - enter in terminal to build project
                ``` sh
                docker build -t [dockerfilename]
                ```
            - enter in terminal to run project server
                ``` sh
                docker run -d -p 80:80 --name docker-tutorial docker1 01tutorial
                ```
##### [Using Posgresql in C#](https://medium.com/@agavatar/webapi-with-net-core-and-postgres-in-visual-studio-code-8b3587d12823) or [this](https://medium.com/@RobertKhou/asp-net-core-mvc-identity-using-postgresql-database-bc52255f67c4) 
- install PostgreSQL
    ``` sh
    brew install postgresql
    ```
- ### postgresql ###
    - `To migrate existing data` from a previous major version of PostgreSQL run:
        ``` sh
        brew postgresql-upgrade-database
        ```
    - To have `launchd start postgresql now and restart at login`:
        ``` sh 
        brew services start postgresql
        ```
    - Or, if you don't want/need a `background service` you can just run:
        ``` sh
        pg_ctl -D /usr/local/var/postgres start
        ```
- using psql
    - Creating database
        ``` sh 
        psql template1
        ```
    - to create database
        ``` sh
        create database [name of database];
        ```
    - Creating role 
        ``` sh
        create ROLE [role_name] WITH SUPERUSER CREATEDB CREATEROLE LOGIN ENCRYPTED PASSWORD '[role_password]';
        ```
    - Change Role
        ``` sh
        psql -d [database_name]] -U [role_name]]
        ```
- Adding in c# WEB API
    - add Packages if .net version 3.1.2 or Up
        - [Npgsql.EntityFrameworkCore.PostgreSQL](https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL/3.1.2)
        - [Npgsql.EntityFrameworkCore.PostgreSQL.Design v1.1.0](https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL.Design/1.1.0)
    - add to `appsettings.json` or `appsettings.development.json`
        ``` json
        "ConnectionStrings": {
            "MyPostgresqlConn": "Server=localhost; Port=5432; Database=mydb; Username=csharpuser; Password=csharpuserpassword"
        }
        ```
        where
        ``` json
        "MyPostgresqlConn": "Server=localhost; Port=5432; Database=[name of database]; Username=[role_name]; Password=[role_password]"
        ```
    - add in `Startup.cs` `ConfigureServices(IServiceCollection services)`
        ``` csharp        
        services.AddDbContext<TestContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("MyPostgresqlConn")));
        ```
#### EntityFramework Model attributes
    - [Required] for Dto request required checking
    - [Key] for primary key
    - [ForeignKey] for foreign key can be add to object property as reference for fetch via primary key
        - _dbContext.Entry(dataObject).Reference(u => u.RefObject).Load(); to use relationship data.
        - or _dbContext.Table.Include(u => u.RefObject);
    - [Index(nameof(propName),IsUnique =true)] //add in class
    - [Column] use for defining column options

#### How To Migrate
##### Using EntityFramework 6.4.4
- First     
    ``` sh
    dotnet ef migrations add InitialCreate
    ```
- Then Add to `Context's Constructor()`     
    ```  csharp    
    if (Database.GetPendingMigrations().Any())
    {
        Database.Migrate();
    }    
    ```

##### Attribute for Controller
    - [Authorize] to add authorization in controller or controller method or actions
    - [AllowAnonymous] add in action to even if controller is [Authorize] it can still be access with out authorization
    - 

#### How to add Authentication
##### JWT Token Bearer
- First install nugger
    - Microsoft.AspNetCore.Identity
    - Microsoft.AspNetCore.Authentication.JwtBearer
- Then add in appsettings.json
    ``` json
        ,  
          "JWT": {  
            "ValidAudience": "http://localhost:4200",  
            "ValidIssuer": "http://localhost:61955",  
            "Secret": "ByYM000OLlMQG6VVVp1OH7Xzyr7gHuw1qvUC5dcGt3SNM"  
          }  
    ```
- Then add UserRoles *optional
    ``` csharp
        public static class UserRoles  
        {  
            public const string Admin = "Admin";  
            public const string User = "User";  
        }  
    ```
- Then add in StartUp's ConfigureServices
``` csharp
// Adding Authentication  
            services.AddAuthentication(options =>  
            {  
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;  
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;  
            })  
  
            // Adding Jwt Bearer  
            .AddJwtBearer(options =>  
            {  
                options.SaveToken = true;  
                options.RequireHttpsMetadata = false;  
                options.TokenValidationParameters = new TokenValidationParameters()  
                {  
                    ValidateIssuer = true,  
                    ValidateAudience = true,  
                    ValidAudience = Configuration["JWT:ValidAudience"],  
                    ValidIssuer = Configuration["JWT:ValidIssuer"],  
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))  
                };  
            });  
```
- Then add in StartUp's Configure
``` csharp      
    //after app.UseRouting()
            app.UseAuthentication()
               .UseAuthorization()
```
- Then use to create token
``` csharp
    public string CreateToker(string username)
    {
        var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "User"),
            };

        //if user has many roles
        //foreach (var userRole in userRoles)
        //{
        //    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        //}

        SymmetricSecurityKey authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
```

#### How to add Swagger
- First
    insall nuget
        - Swashbuckle.AspNetCore
        - Swashbuckle.AspNetCore.Annotations
        - Swashbuckle.AspNetCore.Newtonsoft
- Then add to `StartUp's Configure()'
    ``` csharp
    app.UseSwagger()
        .UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Test Api");
        });
    ```
- Then add to `StartUp's ConfigureServices()'
    ``` csharp
        //NOTE: To use swagger
        services
            .AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1.0", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "TestC# API", Version = "v1.0" });
                c.ResolveConflictingActions(apidescription => apidescription.First());
            })
        //NOTE: added Newtonsoft for Swagger
            .AddSwaggerGenNewtonsoftSupport()
            .AddLogging();        
    ```

#### Random
- To get exception in Controller Action
    ``` csharp        
        var exceptionHandlerFeature =
            HttpContext.Features.Get<IExceptionHandlerFeature>()!;
    ```