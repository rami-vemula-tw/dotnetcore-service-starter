# Logging Best Practices


### Logging Providers in ASP.NET Core
    1. Console
    2. Debug
    3. EventSource
    4. EventLog
    5. Trace
    6. Event

### Log Levels in ASP.NET Core - The log level indicates the severity or importance.
    1. Trace
    2. Debug
    3. Info
    4. Warning
    5. Error
    6. Critical
    7. None

## Text Logging
Text logging is also called ‘printf debugging’ after the C printf() family of functions.
The problem with text logging files is they are unstructured text data. This makes it hard to query them for any sort of useful information.

Hence, as a best practice always use strucuted logging.


## What is Structured Logging

Structured logging can be thought of as a stream of key-value pairs for every event logged, instead of just the plain text line of conventional logging.

### Using Structured Logging

1. #### Adding the provider

    To add a provider in an app that uses Generic Host, call the provider's Add{provider name} extension method in Program.cs:

    ```public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
        ```

2. #### Logging Events
    Logging events for an API call should  produce the trace shown below :
    ```
    info: Microsoft.AspNetCore.Hosting.Diagnostics[1]
      Request starting HTTP/1.1 GET http://localhost:5000/api/todo/0
    info: Microsoft.AspNetCore.Hosting.Diagnostics[2]
      Request finished in 84.26180000000001ms 307
    info: Microsoft.AspNetCore.Hosting.Diagnostics[1]
      Request starting HTTP/2 GET https://localhost:5001/api/todo/0
    info: Microsoft.AspNetCore.Routing.EndpointMiddleware[0]
      Executing endpoint 'TodoApiSample.Controllers.TodoController.GetById (TodoApiSample)'
    info: Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker[3]
      Route matched with {action = "GetById", controller = "Todo", page = ""}. Executing controller action with signature Microsoft.AspNetCore.Mvc.IActionResult GetById(System.String) on controller TodoApiSample.Controllers.TodoController (TodoApiSample).
    info: TodoApiSample.Controllers.TodoController[1002]
      Getting item 0
    warn: TodoApiSample.Controllers.TodoController[4000]
      GetById(0) NOT FOUND
    info: Microsoft.AspNetCore.Mvc.StatusCodeResult[1]
      Executing HttpStatusCodeResult, setting HTTP status code 404
    ```


3. #### Logging contexts and correlation Ids


    Logging context allows you to define a scope, so you can trace and correlate a set of events, even across the boundaries of the applications involved.

    Correlation Ids are a mean to establish a link between two or more contexts or applications, but can get difficult to trace. At some point it might be better to handle contexts that cover business concepts or entities, such as an PaymentContext that can be easily identified across different applications, even when using different technologies.

    Here are the minimum context properties that is recommended in addition to the event properties

    These are some of the context properties :

        *ApplicationContext* or *CorrelationId* Is defined on application startup and adds the ApplicationContext or CorrelationId property to all events.

        *SourceContext* Identifies the full name of the class where the event is logged, it's usually defined when creating or injecting the logger.

        *RequestId* Is a typical context that covers all events while serving a request. It's defined by the ASP.NET Core request pipeline.

        *EventId* and event parameters. EventsIds can be defined in a LoggingEvents class


3. #### Setup and Configuration

    Logging provider configuration is provided by one or more configuration providers:

    File formats (INI, JSON, and XML).
    Command-line arguments.
    Environment variables.
    In-memory .NET objects.
    The unencrypted Secret Manager storage.
    An encrypted user store, such as Azure Key Vault.
    Custom providers (installed or created).

    The following example shows the contents of a typical appsettings.Development.json file for Console provider:
    ```
    {
        "Logging": {
                "LogLevel": {
                "Default": "Debug",
                "System": "Information",
                "Microsoft": "Information"
            },
            "Console":
            {
                "IncludeScopes": true
            }
        }
    }

    ```

## Create Application Logger to fecilitate Structured Logging
- Create [IApplicationLogger.cs](../PaymentService/Infrastructure/Contracts/IApplicationLogger.cs) and [ApplicationLogger.cs](../PaymentService/Infrastructure/Logging/ApplicationLogger.cs) which will provide wrapper methods to perform following activities.
  - Fetch the server and client context information and construct the logging `Attributes`.
  - Log the data with specified `LogLevel` and `Attributes` using default methods of `Microsoft.Extensions.Logging`.
  - IApplicationLogger is resolved with ApplicationLogger instance in Startup class.

- Start logging using `IApplicationLogger` instance as shown below to achieve structured logging.
```
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PaymentService.Infrastructure.Contracts;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IApplicationLogger<PaymentsController> _logger;
        private readonly IConfiguration _configuration;

        public PaymentsController(IApplicationLogger<PaymentsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public string Get()
        {
            _logger.LogInformation($"The payment database connection string : { _configuration.GetConnectionString("PaymentConnection") }");
            _logger.LogCritical($"The Elastic Search Endpoint : { _configuration["ElasticConfiguration:Uri"] }");
            _logger.LogCritical($"The Logstash Endpoint : { _configuration["LogstashConfiguration:Uri"] }");
            _logger.LogWarning($"The Current Environment : { Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") }");
            return "Payment Service Up ad running";
        }
    }
}

```

## Structured Logging with Azure Application Insights

- Create an instance of Azure Application Insights using the Azure Portal.
  - Get the `Instrumentation Key` from the Application Insights overview.
- Create `ApplicationInsights` section in **appsettings.json** file under **PaymentService** folder as shown below.

```
  "ApplicationInsights": {
    "InstrumentationKey": "4fa80d68-6bb8-4968-b43e-dfdc67fb18d0"
  },
```
- Add `Microsoft.ApplicationInsights.AspNetCore` Nuget package to the **PaymentService.csproj** as shown below.

```
 <PackageReference Include="Microsoft.ApplicationInsights.AspNetCore" Version="2.13.1" />
 ```
 - Run below command to restore the packages.
 ```
 $>> dotnet restore
 ```
 - Configure Application insights at ConfigureServices method of Startup.cs as shown below.
 ```
 services.AddLogging(config =>
 {
     config.Services.AddApplicationInsightsTelemetry();
 });
```
- With the above configuration, [ApplicationLogger.cs](../PaymentService/Infrastructure/Logging/ApplicationLogger.cs) instance will not stream logs to Azure Application Insights.


## Request Response Logging using Serilog middleware
The *UseSerilogRequestLogging()* extension method adds the Serilog RequestLoggingMiddleware to the ASP.NET pipeline. 

```
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    
    app.UseStaticFiles();

    app.UseSerilogRequestLogging(); // <-- Add this line

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
}

```

You can also call an overload to configure an instance of RequestLoggingOptions. This class has several properties that let you customise how the request logger generates the log statements
Serilog.AspNetCore adds the interface IDiagnosticContext to the DI container as a singleton, so you can access it from any of your classes. You can then use it to attach additional properties to the request log message by calling Set(). 

The static helper class below retrieves values from the current HttpContext and sets them if they're available.: 
```
 public static class RequestLogHelper 
{
    public static  void EnrichFromRequestResponse(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        var request = httpContext.Request;

        // Set all the common properties available for every request
        diagnosticContext.Set("Host", request.Host);
        diagnosticContext.Set("Protocol", request.Protocol);
        diagnosticContext.Set("Scheme", request.Scheme);

        // Only set it if available. You're not sending sensitive data in a querystring right?!
        if(request.QueryString.HasValue)
        {
            diagnosticContext.Set("QueryString", request.QueryString.Value);
        }

        // Set the content-type of the Response at this point
        diagnosticContext.Set("ContentType", httpContext.Response.ContentType);


        
        // Retrieve the IEndpointFeature selected for the request
        var endpoint = httpContext.GetEndpoint();
        if (endpoint is object) // endpoint != null
        {
            diagnosticContext.Set("EndpointName", endpoint.DisplayName);
        }
    }
}
}

```
*NOTE*: ASP.NET core 3.1 does not allow the response content to be logged. Request.Body is a forward only stream that doesn't support seeking or reading the stream a second time and hence Response body cannot be logged using Serilog request middleware.

Also, as a best practice, it is not recommended to log Response.Body due to performance considerations
        

## References

1. [Logging in .NET Core and ASP.NET Core](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1
)



