using System;

namespace BBB_ApplicationDashboard.Application.DTOs.Audit;

public class AuditLineChartGroupedResponse
{
    public Dictionary<string, List<AuditLineChartPoint>> Data { get; set; } = [];
}

public class AuditLineChartPoint
{
    public DateOnly Date { get; set; }
    public int Success { get; set; }
    public int Error { get; set; }
    public int Failure { get; set; }
}
