namespace UBS.ReportManager.Service
{
    using System;
    using System.Threading.Tasks;
    using LendFoundry.Foundation.Client;
    using LendFoundry.Security.Tokens;
    using RestSharp;

    public static class ReportUtil
    {
        private const string QUERY_PARAM_NAME = "reportParams";

        /// <summary>
        /// Invokes the datasource endpoint reads the resulting Json data sent
        /// </summary>
        /// <param name="serviceProvider">Used to obtain a service-client</param>
        /// <param name="tokenReader">Used for creating a service-client</param>
        /// <param name="dsUri">URI of endpoint which can be invoked to fetch data</param>
        /// <param name="reportParams"></param>
        /// <returns>Json data received after invoking data-source URI</returns>
        public static async Task<string> FetchJsonData(IServiceProvider serviceProvider, ITokenReader tokenReader,
            Uri dsUri, string reportParams)
        {
            var baseUri = new Uri(dsUri.GetLeftPart(UriPartial.Authority));
            var sClient = serviceProvider.GetServiceClient(tokenReader, baseUri);
     
            var request = new RestRequest(baseUri.MakeRelativeUri(dsUri));

            request.AddQueryParameter(QUERY_PARAM_NAME, reportParams);

            var reportJsonData = await sClient.ExecuteAsync<object>(request) ?? "{}";
            // TODO if result is null, then convert it to empty document, and use for validation with the schema
            // Should we throw an exception if result is null
            // validate result with schema
            
            return reportJsonData.ToString();
        }
    }
}