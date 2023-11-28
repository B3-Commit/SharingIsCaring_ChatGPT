using System.Net.Http.Json;
using System.Web;

using Microsoft.Extensions.Logging;

using SharingIsCaring.Core.Models.Search;

namespace SharingIsCaring.Core.Services;

public interface IMicrosoftLearnSearchService
{
    Task<SearchResponse> SearchAsync(
        string query,
        string scope,
        string locale,
        string[] facets,
        string filter,
        int take = 10,
        bool expandScope = true,
        string partnerId = "LearnSite",
        CancellationToken cancellationToken = default);

    Task<SearchResponse> SearchAsync(Action<SearchRequest> searchRequest);
    Task<SearchResponse> SearchAsync(SearchRequest searchRequest);
}

public class MicrosoftLearnSearchService(
    ILogger<MicrosoftLearnSearchService> logger,
    IHttpClientFactory httpClientFactory)
    : IMicrosoftLearnSearchService
{
    public async Task<SearchResponse> SearchAsync(
        string query,
        string scope, 
        string locale, 
        string[] facets, 
        string filter, 
        int take = 10, 
        bool expandScope = true, 
        string partnerId = "LearnSite", 
        CancellationToken cancellationToken = default)
    {
        return await SearchAsync(new SearchRequest
        {
            Query = query,
            Scope = scope,
            Locale = locale,
            Facets = facets,
            Filter = filter,
            Take = take,
            ExpandScope = expandScope,
            PartnerId = partnerId,
            CancellationToken = cancellationToken
        });
    }

    public async Task<SearchResponse> SearchAsync(Action<SearchRequest> searchRequestAction)
    {
        SearchRequest req = new();
        searchRequestAction(req);

        return await SearchAsync(req);
    }

    public async Task<SearchResponse> SearchAsync(SearchRequest searchRequest)
    {
        logger.BeginScope($"[{nameof(SearchRequest)}]");
        logger.LogInformation(
            message: "Searching with query '{query}' on MicrosoftLearn",
            searchRequest.Query);

        var httpClient = httpClientFactory.CreateClient(
            name: nameof(MicrosoftLearnSearchService));

        // Make search parameters HTML-safe
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        queryString["search"] = searchRequest.Query;
        queryString["scope"] = searchRequest.Scope;
        queryString["locale"] = searchRequest.Locale;

        // TODO: Implement facets
        
        queryString["$filter"] = searchRequest.Filter;
        queryString["$top"] = searchRequest.Take.ToString();

        queryString["expandScope"] = searchRequest.ExpandScope.ToString();
        queryString["partnerId"] = searchRequest.PartnerId;

        HttpResponseMessage response = await httpClient
            .GetAsync(
                requestUri: $"api/search?{queryString}",
                cancellationToken: searchRequest.CancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<SearchResponse>() ?? null!;
        }
        else
        {
            string errMessage = await response.Content.ReadAsStringAsync();
            logger.LogError($"Unable to parse JSON response: {errMessage}");

            throw new Exception(errMessage);
        }
    }
}