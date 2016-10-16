# About

`2016-10-16`



## Install

```powershell
Install-Package MienDev.AspNetCore.Identity.MongoDB
```



## Configuration

#### Add MongoDB connection string

To add config section, take `appsetttings.json` as example. 

```json
{
  // ...
  "mongoIdentity":{
  		"connectionString":"mongodb://127.0.0.1:27017",
    	"databaseName":"mongoIdentity"
	}
  // ...
}
```

#### Add user model

Add user model inherit from `IdentityUser` , using  the namespace `MienDev.AspNetCore.Identity.MongoDB`

```c#
public class ApplicationUser: IdentityUser{
  // ...
}
```

#### Config Identity option

Config identity option to `startup.cs` in the `ConfigureServices` 

```c#

// DI injection for MongoIdentity
services.AddMongoIdentity<ApplicationUser, IdentityRole>(Configuration.GetSection("mongoIdentity"));

// Inject IUserStore, IRoleStore, using .AddMongoIdentityStores() after IdentityBuilder
services.AddIdentity<ApplicationUser, IdentityRole>()
  .AddMongoIdentityStores()
  .AddDefaultTokenProviders();
```



`

`