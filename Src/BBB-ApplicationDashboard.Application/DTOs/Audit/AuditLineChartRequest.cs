using System;
using System.ComponentModel.DataAnnotations;

namespace BBB_ApplicationDashboard.Application.DTOs.Audit;

public class AuditLineChartRequest
{
    // Required: "7d", "30d", "3m"
    [Required]
    public string Preset { get; set; } = "7d";
}
