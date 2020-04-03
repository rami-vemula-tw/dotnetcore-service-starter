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

Hence, as a best practice always use strucuted logging


## Structured Logging

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



## References

1. [Logging in .NET Core and ASP.NET Core](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1
)



