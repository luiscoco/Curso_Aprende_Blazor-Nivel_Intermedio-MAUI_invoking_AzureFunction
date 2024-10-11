# How to invoke an Azure Function from a MAUI Blazor application

**IMPORTANT NOTE**: as prerequisite for this sample is to create an Azure Function, and deploy it to Azure Portal, following the steps stablished in this URL:

https://github.com/luiscoco/AzureFunctions_CreateFunctionInVisualStudio2022

## 1. Run Visual Studio 2022 Community Edition and create a MAUI Blazor application

![image](https://github.com/user-attachments/assets/923de41d-1791-4536-9d72-90ac114f66c9)

![image](https://github.com/user-attachments/assets/18c590ba-8701-40e8-b256-e596ee9fa3ac)

![image](https://github.com/user-attachments/assets/dc19e651-165e-4fec-82fe-3fd291d0464e)

![image](https://github.com/user-attachments/assets/3d6872ac-c752-4279-802c-974077a43fdf)

![image](https://github.com/user-attachments/assets/d510279f-c15b-4dbf-a275-c9e11e69c587)

## 2. Create a Service for invoking the Azure Function

This code is responsible for invoking an Azure Function by making an HTTP GET request and handling the result

It uses an injected HttpClient to send the request asynchronously, and depending on the outcome, it either returns the response content or an error message

**AzureFunctionService.cs**

```csharp
using System.Net.Http;
using System.Threading.Tasks;

namespace MAUIBlazorAzureFunctionInvoking.Services
{

    public class AzureFunctionService
    {
        private readonly HttpClient _httpClient;

        public AzureFunctionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> InvokeFunctionAsync()
        {
            try
            {
                // Replace with your actual Azure Function URL
                string functionUrl = "https://myfunctionforblazor.azurewebsites.net/api/Function1?code=YUKb4eMSrWqeFw2lxL0XJUzUgBfIw3Gh-pVTeELRtym8AzFuThTRwQ%3D%3D";

                var response = await _httpClient.GetAsync(functionUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return $"Error: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }
    }
}
```

**Namespace and Dependencies**:

The code is inside the MAUIBlazorAzureFunctionInvoking.Services namespace

It uses the System.Net.Http for making HTTP requests and System.Threading.Tasks to work with asynchronous operations

**AzureFunctionService Class**:

The class AzureFunctionService encapsulates the logic to invoke an Azure Function via an HTTP request

It has a constructor that receives an HttpClient object (_httpClient) via dependency injection, which is used to make HTTP requests

**InvokeFunctionAsync Method**:

The method InvokeFunctionAsync is asynchronous (Task<string> return type) and returns a string representing the response or an error message

It constructs a URL (functionUrl) that points to an Azure Function. The URL includes the function endpoint and an authentication code (code query parameter)

**Making the Request**:

The method calls _httpClient.GetAsync(functionUrl) to make a GET request to the Azure Function. This is an asynchronous operation (await keyword)

**Handling the Response**:

If the request is successful (response.IsSuccessStatusCode), the content of the response is read as a string using response.Content.ReadAsStringAsync()

If the request fails (e.g., server error), it returns an error message with the HTTP status code

**Error Handling**:

The method is wrapped in a try-catch block to handle any exceptions that might occur during the HTTP request. If an exception is caught, it returns the exception's message

## 3. Create a component for invoking the Service

This Blazor component provides a user interface where a button triggers an Azure Function call through the AzureFunctionService

It then displays the result of the Azure Function (or an error message) on the page

The interaction is asynchronous, ensuring smooth UI updates without blocking the app

**AzureFunctionInvokingComponent.razor**

```razor
@page "/AzureFunctionComponent"

@using MAUIBlazorAzureFunctionInvoking.Services

@inject AzureFunctionService azureFunctionService

<PageTitle>Invoke Azure Function</PageTitle>

<h3>Invoke Azure Function</h3>

<p>
    <button @onclick="InvokeAzureFunction">Invoke Function</button>
</p>

<p>@responseMessage</p>

@code {
    private string responseMessage = string.Empty;

    private async Task InvokeAzureFunction()
    {
        // Call the Azure Function through the service
        responseMessage = await azureFunctionService.InvokeFunctionAsync();
    }
}
```

This code is a Blazor component written in Razor syntax, used in a .NET MAUI Blazor app to invoke an Azure Function via a service (AzureFunctionService)

**Component Declaration and Dependencies**:

**@page "/AzureFunctionComponent"**: This line defines the URL route for this Blazor component. In this case, it's accessible via /AzureFunctionComponent

**@using MAUIBlazorAzureFunctionInvoking.Services**: This imports the namespace where the AzureFunctionService is defined, allowing the component to use the service

**@inject AzureFunctionService azureFunctionService**: This injects the AzureFunctionService into the component, making it available for invoking Azure Functions

**HTML Markup**:

**```<PageTitle>Invoke Azure Function</PageTitle>```**: This sets the title of the page in the browser or application tab

The markup contains a heading (```<h3>Invoke Azure Function</h3>```), a button (```<button @onclick="InvokeAzureFunction">Invoke Function</button>```), and a paragraph (```<p>@responseMessage</p>```) that displays the response from the Azure Function

**Button with Event Handling**:

The button element (```<button @onclick="InvokeAzureFunction">```) is wired up with the @onclick directive to trigger the InvokeAzureFunction method when clicked

This allows the button to initiate the process of invoking the Azure Function

**Code Section (@code)**:

This section contains the C# code that runs inside the Blazor component

**private string responseMessage = string.Empty;**: This is a field that stores the response message from the Azure Function. It's initially an empty string

**private async Task InvokeAzureFunction()**: This is an asynchronous method that calls the InvokeFunctionAsync method from the injected azureFunctionService

When the button is clicked, it invokes this method to call the Azure Function, and the response message is stored in the responseMessage field

The await keyword ensures that the component waits for the Azure Function to return a result before updating the responseMessage

## 4. Modify the middleware(Program.cs)

We register the service with this code:

```
builder.Services.AddHttpClient<AzureFunctionService>();
```

This is the middleware whole code:

**Program.cs**

```csharp
using MAUIBlazorAzureFunctionInvoking.Services;
using Microsoft.Extensions.Logging;

namespace MAUIBlazorAzureFunctionInvoking
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Register HttpClient and AzureFunctionService in DI
            builder.Services.AddHttpClient<AzureFunctionService>();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
```

## 5. Run the application and see the results

![image](https://github.com/user-attachments/assets/e8696012-e688-464d-b25f-8eea8b1a5eb5)

![image](https://github.com/user-attachments/assets/9a88e311-1df4-4e7c-a076-32eb257c7248)
