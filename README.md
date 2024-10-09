# How to invoke an Azure Function from a MAUI Blazor application

## 1. Run Visual Studio 2022 Community Edition and create a MAUI Blazor application



## 2. Create a Service for invoking the Azure Function

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


## 3. Create a component for invoking the Service

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

## 4. Modify the middleware(Program.cs)

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



