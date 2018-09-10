namespace UBS.ReportManager.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Abstractions.Model.Domain;
    using Abstractions.Model.Exception;
    using Abstractions.Service;
    using Microsoft.AspNetCore.Mvc;
    using LendFoundry.Foundation.Logging;
    using LendFoundry.Foundation.Services;

    [Route("/reports")]
    public class ReportController : ExtendedController
    {
        private IReportService ReportService { get; }
        
        // TODO Integrate AuditTrail library
        // TODO Integrate with Category
        // TODO Integrate with Tags library
        public ReportController(IReportService reportService, ILogger logger) : base(logger)
        {
            ReportService = reportService ?? throw new ArgumentException(nameof(reportService));
        }

        /// <summary>
        /// Returns all reports in the system that are active (not deleted). Optionally query parameter 'includeDeleted'
        /// can be specified to true if deleted reports need to be included
        /// </summary>
        /// <param name="includeDeleted">if 'true' then all report records will be returned</param>
        /// <returns>A list of reports</returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetAllReports([FromQuery] bool includeDeleted = false)
        {
            return await ExecuteAsync(
                async () => 
                    Ok(await ReportService.GetAllReports(includeDeleted).ConfigureAwait(false))).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Returns information on a report identified by given <code>id</code>
        /// </summary>
        /// <param name="id">Id of an existing report</param>
        /// <returns>Report Information, an instance of <code>Report</code></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReport([FromRoute] string id)
        {
            Logger.Info($"Received a request with id={id}");
            return await Task.Run(() =>
            {
                ReportService.GetReport(id);
                return Ok("Not Yet Fully Implemented");
            });
        }
        
        /// <summary>
        /// Adds one or more report
        /// </summary>
        /// <param name="newReports">New reports to be added</param>
        /// <returns>???</returns>
        [HttpPost]
        public async Task<IActionResult> AddReports([FromBody] List<Report> newReports)
        {
            return await Task.Run(() =>
            {
                ReportService.AddReports(newReports);
                return Ok("Not Yet Fully Implemented");
            });
        }
        
        /// <summary>
        /// Updates one or more reports
        /// </summary>
        /// <param name="updatedReports">Updated information on one or more reports</param>
        /// <returns>???</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateReport([FromBody] List<Report> updatedReports)
        {
            return await Task.Run(() => Ok("Not Yet Implemented"));
        }
        
        /// <summary>
        /// Soft-Deletes an existing report identified by the given code. If the report is already deleted,
        /// then no action is taken
        /// </summary>
        /// <param name="id">Identifier of the report to be deleted</param>
        /// <returns>Confirmation of delete operation</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteReport([FromRoute] string id)
        {
            return await ExecuteAsync(
                async () =>
                {
                    try
                    {
                        await ReportService.DeleteReport(id).ConfigureAwait(false);
                        return NoContent();
                    }
                    catch (ReportNotFoundException rnfe)
                    {
                        throw new NotFoundException(rnfe.Message);
                    }
                }
                    
            );
        }

        /// <summary>
        /// Generates a report file using the template specified by the report identified with <code>id</code>
        /// </summary>
        /// <param name="id">Identifier of the report for which report file needs to be generated</param>
        /// <returns>Generated report</returns>
        [HttpGet("{id}/generated")]
        // TODO Target format can be specified as a query parameter (PDF, HTML, etc.)
        public async Task<IActionResult> GenerateReport([FromRoute] string id)
        {
            return await Task.Run(() => Ok("Not Yet Implemented"));
        }
    }
}