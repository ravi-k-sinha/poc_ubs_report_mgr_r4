namespace UBS.ReportManager.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Abstractions.Model.Domain;
    using Abstractions.Repository;
    using LendFoundry.Foundation.Persistence.Mongo;
    using LendFoundry.Tenant.Client;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public class ReportRepository : MongoRepository<IReport, Report>, IReportRepository
    {
        
        static ReportRepository()
        {
            BsonClassMap.RegisterClassMap<Report>(map =>
            {
                map.AutoMap();

                var type = typeof(Report);
                map.SetDiscriminator($"{type.FullName}, {type.GetTypeInfo().Assembly.GetName().Name}");
                map.SetIsRootClass(true);

                map.MapMember(m => m.CreatedOn).SetSerializer(new DateTimeOffsetSerializer(BsonType.Document));
                map.MapMember(m => m.UpdatedOn).SetSerializer(new DateTimeOffsetSerializer(BsonType.Document));
                map.MapMember(m => m.DeletedOn).SetSerializer(new DateTimeOffsetSerializer(BsonType.Document));
            });
        }

        public ReportRepository(ITenantService tenantService, IMongoConfiguration mongoConfiguration) : 
            base(tenantService, mongoConfiguration, "reports")
        {
            CreateIndexIfNotExists("report-template-code-idx", 
                Builders<IReport>.IndexKeys.Ascending(r => r.TenantId).Ascending(r => r.TemplateCode), true);
        }
        
        public async Task<IReport> GetReport(string id)
        {
            // TODO Maybe use a parameter that specifies whether deleted entry needs to be included
            return await Query.Where(r => r.DeletedOn.Equals(DateTimeOffset.MinValue))
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<IReport>> GetAllReports()
        {
            var reports = await Collection.FindAsync(c => c.DeletedOn.Equals(DateTimeOffset.MinValue));
            return reports.ToList();
        }

        public async Task<List<IReport>> AddReports(List<IReport> newReports)
        {
            // TODO Find how we can find out whether duplicate is coming in the request
            newReports.ForEach(r =>
            {
                r.TenantId = TenantService.Current.Id;
                r.Id = ObjectId.GenerateNewId().ToString();
            });

            await Task.Run(() => { Collection.InsertManyAsync(newReports); });

            return newReports;
        }

        public Task<List<IReport>> UpdateReports(List<IReport> newReports)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<IReport>> DeleteReport(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}