using UnityEngine;
using System.Net.Http.Json;

public class PythonBackendConnector
{

    // Reuse a single HttpClient instance across your application to avoid socket exhaustion
    using HttpClient client = new HttpClient();

    public static async string BackendRequest(string requestSql){
    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "romansServerName");
    request.Headers.TryAddWithoutValidation("application/json", requestSql);

    using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

    if (response.IsSuccessStatusCode)
    {
        return await response.Content.ReadAsStringAsync();
    }
    }
}
