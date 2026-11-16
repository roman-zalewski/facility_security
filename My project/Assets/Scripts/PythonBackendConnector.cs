using UnityEngine;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class PythonBackendConnector
{

    // Reuse a single HttpClient instance across your application to avoid socket exhaustion

    public static async Task<string> BackendRequest(string requestSql)
    {
        using HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "romansServerName");
        request.Headers.TryAddWithoutValidation("application/json", requestSql);

        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        return null;
    }
}
