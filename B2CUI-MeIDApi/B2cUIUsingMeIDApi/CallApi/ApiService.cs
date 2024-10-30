﻿using Microsoft.Identity.Web;
using System.Net.Http.Headers;

namespace B2cUIUsingMeIDApi.CallApi;

public class ApiService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IConfiguration _configuration;

    public ApiService(IHttpClientFactory clientFactory,
        ITokenAcquisition tokenAcquisition,
        IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _tokenAcquisition = tokenAcquisition;
        _configuration = configuration;
    }

    public async Task<string> GetApiDataAsync()
    {
        var client = _clientFactory.CreateClient();

        var scope = _configuration["CallApi:ScopeForAccessToken"];

        if (scope == null)
            throw new ArgumentNullException("scope configuration not set ");

        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync([scope],
            "f611d805-cf72-446f-9a7f-68f2746e4724", null, null, null);

        string? url = _configuration["CallApi:ApiBaseAddress"];
        if (url != null)
            client.BaseAddress = new Uri(url);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync("Business");
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            return responseContent;
        }

        throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
    }
}
