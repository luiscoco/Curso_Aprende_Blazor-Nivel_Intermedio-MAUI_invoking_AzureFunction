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
