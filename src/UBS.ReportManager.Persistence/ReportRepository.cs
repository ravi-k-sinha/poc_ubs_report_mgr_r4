namespace UBS.ReportManager.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Abstractions.Model.Domain;
    using Abstractions.Model.Exception;
    using Abstractions.Repository;
    using LendFoundry.Foundation.Date;
    using LendFoundry.Foundation.Persistence.Mongo;
    using LendFoundry.Tenant.Client;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public class ReportRepository : MongoRepository<IReport, Report>, IReportRepository
    {
        private ITenantTime TenantTime { get; }

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

        public ReportRepository(ITenantService tenantService, IMongoConfiguration mongoConfiguration,
            ITenantTime tenantTime) :
            base(tenantService, mongoConfiguration, "reports")
        {
            TenantTime = tenantTime;
            CreateIndexIfNotExists("report-name-idx",
                Builders<IReport>.IndexKeys.Ascending(r => r.TenantId).Ascending(r => r.Name), true);
            CreateIndexIfNotExists("report-code-idx",
                Builders<IReport>.IndexKeys.Ascending(r => r.TenantId).Ascending(r => r.Code), true);
        }

        public async Task<IReport> GetReport(string idOrCode, bool includeDeleted = false)
        {
            var validId = ObjectId.TryParse(idOrCode, out var sample);
            IReport report;
            
            if (validId)
            {
                report = includeDeleted
                    ? await Query.FirstOrDefaultAsync(r => r.Id == idOrCode)
                    : await Query.Where(r => r.DeletedOn.Equals(DateTimeOffset.MinValue))
                        .FirstOrDefaultAsync(r => r.Id == idOrCode);    
            }
            else
            {
                report = includeDeleted
                    ? await Query.FirstOrDefaultAsync(r => r.Code == idOrCode)
                    : await Query.Where(r => r.DeletedOn.Equals(DateTimeOffset.MinValue))
                        .FirstOrDefaultAsync(r => r.Code == idOrCode);
            }
            
            return report;
        }

        public async Task<List<IReport>> GetAllReports(bool includeDeleted = false)
        {
            var returnList = includeDeleted
                ? await Query.ToListAsync()
                : await Query.Where(r => r.DeletedOn.Equals(DateTimeOffset.MinValue)).ToListAsync();

            return returnList;
        }

        public async Task<List<IReport>> AddReports(List<IReport> newReports)
        {
            // TODO Find how we can find out whether duplicate is coming in the request, as any duplicate makes the
            // whole operation batch invalid
            
            newReports.ForEach(r =>
            {
                r.TenantId = TenantService.Current.Id;
                r.Id = ObjectId.GenerateNewId().ToString();
                r.CreatedOn = TenantTime.Now;
            });

            try
            {
                await Task.Run(() =>
                {
                    Collection.InsertMany(newReports);
                });
            }
            catch (MongoBulkWriteException mbwe)
            {
                throw new ReportStorageException(mbwe.Message);
            }

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