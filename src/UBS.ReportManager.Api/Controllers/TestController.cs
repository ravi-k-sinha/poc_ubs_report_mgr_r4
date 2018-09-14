namespace UBS.ReportManager.Api.Controllers
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Threading.Tasks;
    using LendFoundry.Foundation.Logging;
    using LendFoundry.Foundation.Services;
    using LendFoundry.Security.Tokens;
    using Microsoft.AspNetCore.Mvc;
    using MongoDB.Bson.IO;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;
    using JsonConvert = Newtonsoft.Json.JsonConvert;

    [Route("/test")]
    public class TestController  : ExtendedController
    {
        private ITokenReader TokenReader { get; }

        public TestController(ITokenReader tokenReader, ILogger logger) : base(logger)
        {
            TokenReader = tokenReader ?? throw new ArgumentNullException(nameof(tokenReader));
        }

        /// <summary>
        /// A sample endpoint that returns sample data
        /// </summary>
        [HttpGet("sample-data")]
        [Produces("application/json")]
        public async Task<IActionResult> SampleData([FromQuery] string reportParams)
        {
            TokenReader.Read(); // Try to read the token, it will raise 401, if token is not present on invalid
            Logger.Debug($"Report data received is [{reportParams}]");

            var subValue = "INVALID";
            
            if (reportParams != null)
            {
                subValue = JObject.Parse(reportParams)["sampleId"]["subId"].ToString();
            }
            
            return await ExecuteAsync(
                async () => 
                    Ok(await Task.Run(() => new {data = new
                    {
                        key1 = subValue,
                        key2 = DateTime.Now.ToString(CultureInfo.InvariantCulture)
                    }})));
        }
    }
}