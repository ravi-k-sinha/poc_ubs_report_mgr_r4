namespace UBS.ReportManager.Service
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abstractions.Model.Domain;
    using LendFoundry.Foundation.Client;
    using LendFoundry.Security.Tokens;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Schema;
    using RestSharp;

    public static class ReportUtil
    {
        private const string QueryParamName = "reportParams";

        /// <summary>
        /// Invokes the datasource endpoint reads the resulting Json data sent
        /// </summary>
        /// <param name="serviceProvider">Used to obtain a service-client</param>
        /// <param name="tokenReader">Used for creating a service-client</param>
        /// <param name="dsUri">URI of endpoint which can be invoked to fetch data</param>
        /// <param name="reportParams">Parameters sent to the endpoint for fetching report data</param>
        /// <returns>Json data received after invoking data-source URI</returns>
        public static async Task<string> FetchJsonData(IServiceProvider serviceProvider, ITokenReader tokenReader,
            Uri dsUri, string reportParams)
        {
            
            var baseUri = new Uri(dsUri.GetLeftPart(UriPartial.Authority));
            var sClient = serviceProvider.GetServiceClient(tokenReader, baseUri);
     
            var request = new RestRequest(baseUri.MakeRelativeUri(dsUri));
            
            request.AddQueryParameter(QueryParamName, reportParams);

            var reportJsonData = await sClient.ExecuteAsync<object>(request) ?? "{}";
            
            return reportJsonData.ToString();
        }


        public static bool ValidAsPerSchema(string strInput, string schemaStr, out IList<string> errors)
        {
            errors = new List<string>();
            
            if (!IsValidJson(strInput) || !IsValidJson(schemaStr))
            {
                errors.Add("Input or schema is not a valid json");
                return false;
            }
            
            var schema = JSchema.Parse(schemaStr);
            var input = JObject.Parse(strInput);

            var valid = input.IsValid(schema, out errors);
            return valid;
        }
        
        public static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput))
            {
                return false;
            }
            
            var trimmedInput = strInput.Trim();

            if (!(trimmedInput.StartsWith("{") && trimmedInput.EndsWith("}")
                  || trimmedInput.StartsWith("{") && trimmedInput.EndsWith("}")))
            {
                return false;
            }

            try
            {
                JToken.Parse(trimmedInput);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Generates a message to be used for ReportNotFoundException when the input could be either id or code of a report
        /// </summary>
        /// <param name="idOrCode"></param>
        /// <returns></returns>
        public static string GetNotFoundExMsgForIdOrCode(string idOrCode)
        {
            return $"A report was not found with id/code={idOrCode}";
        }

        public static bool ValidatePatchedReport(Report original, Report patched, out List<string> errors)
        {
            errors = new List<string>();

            if (! patched.Code.Equals(original.Code))
            {
                errors.Add("'Code' cannot be updated");
            }

            if (! patched.CreatedOn.Equals(original.CreatedOn))
            {
                errors.Add("'CreatedOn' date cannot be updated");
            }
            
            if (! patched.UpdatedOn.Equals(original.UpdatedOn))
            {
                errors.Add("'UpdatedOn' date cannot be updated");
            }
            
            if (! patched.DeletedOn.Equals(original.DeletedOn))
            {
                errors.Add("'DeletedOn' date cannot be updated");
            }
            
            // TODO Other validations to be done related to length, and type of data
            
            return errors.Count == 0;
        }
    }
}