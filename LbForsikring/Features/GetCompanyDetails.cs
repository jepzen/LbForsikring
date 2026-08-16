using LbForsikring.Integrations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LbForsikring.Features
{
    public class GetCompanyDetails
    {
        public class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapGet("/company", Handle);

            }
            
            public static async Task<Results<Ok<GetCompanyDetailsResponse>, ProblemHttpResult>> Handle(
                [FromQuery(Name = "name")] string? name,
                [FromQuery(Name = "cvr")] string? cvr,
                [FromServices] ICvrService cvrService, [FromServices] IDstService dstService)
            {
                // Validate request
                if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(cvr))
                {
                    return TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest,
                        detail: "Either Name or Cvr must be provided.");
                }

                var request = new GetCompanyDetailsRequest { Name = name, Cvr = cvr };
                var cvrData = await LookUpCompany(request, cvrService);

                //TODO: Validate cvrData

                var stats = await LookupStats(cvrData.IndustryCode, dstService);

                var response = CreateResponse(cvrData, stats);

                return TypedResults.Ok(response);
            }


            private static async Task<List<StatsResponse>> LookupStats(string industryCode, IDstService dstService)
            {
                var result = await dstService.GetIndustryStats(industryCode, 2024);
                return StatsResponse.FromString(result);
            }

            private static async Task<CvrResponse> LookUpCompany(GetCompanyDetailsRequest request, ICvrService cvrService)
            {
                if (request.Cvr != null)
                {
                    return await cvrService.GetByCvr(request.Cvr);
                }

                return await cvrService.GetByName(request.Name!);
            }
            
            private static GetCompanyDetailsResponse CreateResponse(CvrResponse cvrData, List<StatsResponse> stats)
            {
                return new GetCompanyDetailsResponse()
                {
                    Name = cvrData.Name,
                    IndustryCode = cvrData.IndustryCode,
                    Stats = stats
                };
            }
        }
    }
}