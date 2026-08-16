using LbForsikring.Integrations;

namespace LbForsikring.Features;

public record GetCompanyDetailsResponse
{
    public required string Name { get; set; }

    public required string IndustryCode { get; set; }
    public List<StatsResponse> Stats { get; set; } = [];
}