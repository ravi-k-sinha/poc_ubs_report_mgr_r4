namespace UBS.ReportManager.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using LendFoundry.Foundation.Logging;
    using LendFoundry.Foundation.Services;
    using LendFoundry.Security.Tokens;
    using Microsoft.AspNetCore.Mvc;

    [Route("/sample")]
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
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> SampleData()
        {
            TokenReader.Read(); // Try to read the token, it will raise 401, if token is not present on invalid
            return await ExecuteAsync(
                async () => 
                    Ok(await Task.Run(() => new {data = new
                    {
                        key1 = "Value1",
                        key2 = DateTime.Now
                    }})));
        }
    }
}