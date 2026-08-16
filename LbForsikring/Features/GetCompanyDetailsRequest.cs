namespace LbForsikring.Features;

public record GetCompanyDetailsRequest
{
    public string? Name { get; set; }
    public string? Cvr { get; set; }
}