# Introduction 
TODO: Give a short introduction of your project. Let this section explain the objectives or the motivation behind this project. 

# Getting Started
TODO: Guide users through getting your code up and running on their own system. In this section you can talk about:
1.	Installation process
2.	Software dependencies
3.	Latest releases
4.	API references

# Build and Test
TODO: Describe and show how to build your code and run the tests. 

# Contribute
TODO: Explain how other users and developers can contribute to make your code better. 

If you want to learn more about creating good readme files then refer the following [guidelines](https://docs.microsoft.com/en-us/azure/devops/repos/git/create-a-readme?view=azure-devops). You can also seek inspiration from the below readme files:
- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)

# References
- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-3.1)
- [Visual Studio](https://visualstudio.microsoft.com/)
- [IdentityServer4](https://identityserver.io/)

- https://medium.com/@matthew.bajorek/configuring-serilog-in-asp-net-core-2-2-web-api-5e0f4d89749c
- https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-3.1


# init migration
-  -OutputDir Data/Migrations

# Cachce table SQL
```
CREATE TABLE [dbo].[CacheTable](
        [Id] [nvarchar](449) NOT NULL,
        [Value] [varbinary](max) NOT NULL,
        [ExpiresAtTime] [datetimeoffset](7) NOT NULL,
        [SlidingExpirationInSeconds] [bigint] NULL,
        [AbsoluteExpiration] [datetimeoffset](7) NULL,
     CONSTRAINT [pk_Id] PRIMARY KEY CLUSTERED 
    (
        [Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, 
           IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, 
           ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
 
    CREATE NONCLUSTERED INDEX [Index_ExpiresAtTime] ON [dbo].[CacheTable]
    (
        [ExpiresAtTime] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, 
           SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, 
           ONLINE = OFF, ALLOW_ROW_LOCKS = ON, 
           ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
```


#host to IIS

  1/ IIS fall back to index.html by adding a rewrite rule.  https://www.iis.net/downloads/microsoft/url-rewrite
  2/ Add a rewrite rule to web.config

  ```
     <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <system.webServer>
        <rewrite>
          <rules>
            <rule name="AngularJS Routes" stopProcessing="true">
              <match url=".*" />
              <conditions logicalGrouping="MatchAll">
                <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
                <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />   
              </conditions>
              <action type="Rewrite" url="/" />
            </rule>
          </rules>
        </rewrite>
      </system.webServer>
    </configuration>
```