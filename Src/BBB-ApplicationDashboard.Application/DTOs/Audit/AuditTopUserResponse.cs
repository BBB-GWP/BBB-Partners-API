using System;

namespace BBB_ApplicationDashboard.Application.DTOs.Audit;

public class AuditTopUserResponse
{
    public string User { get; set; } = "";
    public int Success { get; set; }
    public int Error { get; set; }
    public int Failure { get; set; }
    public int Total => Success + Error + Failure;
}
