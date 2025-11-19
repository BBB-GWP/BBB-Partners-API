using BBB_ApplicationDashboard.Application.DTOs.Audit;
using BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;
using BBB_ApplicationDashboard.Application.Interfaces;
using BBB_ApplicationDashboard.Domain.Entities;
using BBB_ApplicationDashboard.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace BBB_ApplicationDashboard.Infrastructure.Services.Audit
{
    public class AuditService(ApplicationDbContext context) : IAuditService
    {
        public async Task LogActivityEvent(ActivityEvent activityEvent)
        {
            await context.ActivityEvents.AddAsync(activityEvent);
            await context.SaveChangesAsync();
        }

        public async Task<ActivityEvent?> GetActivityEventById(Guid id)
        {
            return await context.ActivityEvents.FirstOrDefaultAsync(ae => ae.Id == id);
        }

        public async Task DeleteActivityEvent(Guid id)
        {
            var activityEvent = await context.ActivityEvents.FirstOrDefaultAsync(ae => ae.Id == id);
            if (activityEvent != null)
            {
                context.ActivityEvents.Remove(activityEvent);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAllActivityEvents()
        {
            var allEvents = context.ActivityEvents;
            context.ActivityEvents.RemoveRange(allEvents);
            await context.SaveChangesAsync();
        }

        public async Task<PaginatedResponse<SimpleAuditResponse>> GetAllFilteredActivityEvents(
            AuditPaginationRequest request
        )
        {
            var query = context.ActivityEvents.AsQueryable();

            // Apply requests dynamically
            if (!string.IsNullOrEmpty(request.User))
                query = query.Where(ae => ae.User == request.User);

            if (!string.IsNullOrEmpty(request.Action))
                query = query.Where(ae => ae.Action == request.Action);

            if (!string.IsNullOrEmpty(request.Entity))
                query = query.Where(ae => ae.Entity == request.Entity);

            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(ae => ae.Status == request.Status);

            if (request.FromDate.HasValue)
                query = query.Where(ae => ae.Timestamp >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(ae => ae.Timestamp <= request.ToDate.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim();

                query = query.Where(a => (a.EntityIdentifier ?? string.Empty).Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(request.UserVersion))
                query = query.Where(ae => ae.UserVersion == request.UserVersion);

            var count = await query.CountAsync();
            int pageIndex = request.PageNumber - 1;
            int pageSize = Math.Max(1, Math.Min(100, request.PageSize));

            var audits = await query
                .OrderByDescending(ae => ae.Timestamp)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(ae => new SimpleAuditResponse
                {
                    Id = ae.Id,
                    User = ae.User,
                    Action = ae.Action,
                    Timestamp = ae.Timestamp,
                    Entity = ae.Entity,
                    EntityIdentifier = ae.EntityIdentifier,
                    Status = ae.Status,
                    UserVersion = ae.UserVersion,
                })
                .ToListAsync();
            return new PaginatedResponse<SimpleAuditResponse>(pageIndex, pageSize, count, audits);
        }

        public async Task<List<string>> GetActions()
        {
            var actions = await context
                .ActivityEvents.Select(ae => ae.Action)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();
            return actions;
        }

        public async Task<List<string>> GetUsers()
        {
            var users = await context
                .ActivityEvents.Select(ae => ae.User)
                .Distinct()
                .OrderBy(u => u)
                .ToListAsync();
            return users;
        }

        public async Task<List<string>> GetEntities()
        {
            var entities = await context
                .ActivityEvents.Select(ae => ae.Entity)
                .Distinct()
                .OrderBy(e => e)
                .ToListAsync();
            return entities;
        }

        public async Task<List<string?>> GetStatuses()
        {
            var statuses = await context
                .ActivityEvents.Select(ae => ae.Status)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();
            return statuses;
        }

        public async Task<List<string?>> GetUserVersions()
        {
            var userVersions = await context
                .ActivityEvents.Select(ae => ae.UserVersion)
                .Distinct()
                .OrderByDescending(v => v)
                .ToListAsync();
            return userVersions;
        }

        public async Task<AuditLineChartGroupedResponse> GetActivityLineChartDataGrouped(
            AuditLineChartRequest request
        )
        {
            // -------------------------------------------------
            // 1) Determine date range based on preset
            // -------------------------------------------------
            var today = DateTime.Now.Date;
            DateOnly toDate = DateOnly.FromDateTime(today);

            DateOnly fromDate = request.Preset switch
            {
                "7d" => toDate.AddDays(-7),
                "14d" => toDate.AddDays(-14),
                "30d" => toDate.AddDays(-30),
                _ => throw new ArgumentException("Invalid preset. Allowed: 7d, 14d, 30d"),
            };

            // -------------------------------------------------
            // 2) Convert DateOnly → DateTimeOffset (UTC REQUIRED)
            // -------------------------------------------------
            var fromUtc = new DateTimeOffset(fromDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            var toUtc = new DateTimeOffset(toDate.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);

            // -------------------------------------------------
            // 3) Excluded entities
            // -------------------------------------------------
            var ignoredEntities = new HashSet<string> { "USER", "SYSTEM" };

            // -------------------------------------------------
            // 4) Optimized DB query (UTC filters → INDEX FRIENDLY)
            // -------------------------------------------------
            var sw = System.Diagnostics.Stopwatch.StartNew();

            var filtered = await context
                .ActivityEvents.Where(ae =>
                    !ignoredEntities.Contains(ae.Entity)
                    && ae.Timestamp >= fromUtc
                    && ae.Timestamp <= toUtc
                )
                .Select(ae => new
                {
                    ae.Entity,
                    Date = DateOnly.FromDateTime(ae.Timestamp.LocalDateTime), // convert AFTER query
                    ae.Status,
                })
                .ToListAsync();

            sw.Stop();
            Console.WriteLine($"⏱ Query Execution Time: {sw.ElapsedMilliseconds} ms");

            // -------------------------------------------------
            // 5) Group the results by Entity + Date
            // -------------------------------------------------
            var grouped = filtered
                .GroupBy(x => new { x.Entity, x.Date })
                .Select(g => new
                {
                    g.Key.Entity,
                    g.Key.Date,
                    Success = g.Count(e => e.Status == "SUCCESS"),
                    Error = g.Count(e => e.Status == "ERROR"),
                    Failure = g.Count(e => e.Status == "FAILURE"),
                })
                .ToList();

            // -------------------------------------------------
            // 6) Prepare final response
            // -------------------------------------------------
            var response = new AuditLineChartGroupedResponse();
            var entityTypes = grouped.Select(g => g.Entity).Distinct().ToList();

            // -------------------------------------------------
            // 7) Build timeline for each entity
            // -------------------------------------------------
            foreach (var entity in entityTypes)
            {
                var entityRows = grouped.Where(g => g.Entity == entity).ToList();
                var lookup = entityRows.ToDictionary(e => e.Date);

                var list = new List<AuditLineChartPoint>();

                for (var d = fromDate; d <= toDate; d = d.AddDays(1))
                {
                    if (lookup.TryGetValue(d, out var hit))
                    {
                        list.Add(
                            new AuditLineChartPoint
                            {
                                Date = d,
                                Success = hit.Success,
                                Error = hit.Error,
                                Failure = hit.Failure,
                            }
                        );
                    }
                    else
                    {
                        list.Add(
                            new AuditLineChartPoint
                            {
                                Date = d,
                                Success = 0,
                                Error = 0,
                                Failure = 0,
                            }
                        );
                    }
                }

                response.Data[entity] = list;
            }

            // -------------------------------------------------
            // 8) Build "ALL" timeline by summing all entities
            // -------------------------------------------------
            var allList = new List<AuditLineChartPoint>();

            for (var d = fromDate; d <= toDate; d = d.AddDays(1))
            {
                int sumSuccess = 0,
                    sumError = 0,
                    sumFailure = 0;

                foreach (var entity in entityTypes)
                {
                    var point = response.Data[entity].First(p => p.Date == d);

                    sumSuccess += point.Success;
                    sumError += point.Error;
                    sumFailure += point.Failure;
                }

                allList.Add(
                    new AuditLineChartPoint
                    {
                        Date = d,
                        Success = sumSuccess,
                        Error = sumError,
                        Failure = sumFailure,
                    }
                );
            }

            response.Data["ALL"] = allList;

            return response;
        }

        public async Task<
            Dictionary<string, List<AuditTopUserResponse>>
        > GetTopUsersPerEntityWithStatus()
        {
            var ignoredEntities = new HashSet<string> { "USER", "SYSTEM" };

            var data = await context
                .ActivityEvents.Where(ae => !ignoredEntities.Contains(ae.Entity))
                .GroupBy(ae => new { ae.Entity, ae.User })
                .Select(g => new
                {
                    g.Key.Entity,
                    g.Key.User,
                    Success = g.Count(e => e.Status == "SUCCESS"),
                    Error = g.Count(e => e.Status == "ERROR"),
                    Failure = g.Count(e => e.Status == "FAILURE"),
                })
                .ToListAsync();

            var result = data.GroupBy(x => x.Entity)
                .ToDictionary(
                    entityGroup => entityGroup.Key,
                    entityGroup =>
                        entityGroup
                            .Select(x => new AuditTopUserResponse
                            {
                                User = x.User,
                                Success = x.Success,
                                Error = x.Error,
                                Failure = x.Failure,
                            })
                            .OrderByDescending(x => x.Total)
                            .Take(3)
                            .ToList()
                );

            return result;
        }
    }
}
