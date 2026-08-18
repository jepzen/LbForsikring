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
                [FromServices] ICvrService cvrService, 
                [FromServices] IDstService dstService,
                ILogger<GetCompanyDetails> logger)
            {
                if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(cvr))
                {
                    return TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest,
                        detail: "Either Name or Cvr must be provided.");
                }

                var request = new GetCompanyDetailsRequest { Name = name, Cvr = cvr };
                
                var cvrData = await LookUpCompany(request, cvrService, logger);

                if (cvrData.IndustryCode == string.Empty)
                {
                    return TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest,
                        detail: "Could not lookup company");
                }
                    
                var stats = await LookupStats(cvrData.IndustryCode, dstService, logger);

                var response = CreateResponse(cvrData, stats);

                return TypedResults.Ok(response);
            }


            private static async Task<List<StatsResponse>> LookupStats(string industryCode, IDstService dstService, ILogger<GetCompanyDetails> logger)
            {
                try
                {
                    var result = await dstService.GetIndustryStats(industryCode, 2024);
                    return StatsResponse.FromString(result);
                }
                catch (Exception e)
                {
                   logger.LogError(e, "Failed to lookup stats");    
                   return []; //TODO: Return some better message
                }
               
            }

            private static async Task<CvrResponse> LookUpCompany(GetCompanyDetailsRequest request, ICvrService cvrService, ILogger<GetCompanyDetails> logger)
            {
                try
                {
                    if (request.Cvr != null)
                    {
                        return await cvrService.GetByCvr(request.Cvr);
                    }

                    return await cvrService.GetByName(request.Name!);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Failed to lookup company");
                    return new CvrResponse();
                }
               
            }
            
            private static GetCompanyDetailsResponse CreateResponse(CvrResponse cvrData, List<StatsResponse> stats)
            {
                return new GetCompanyDetailsResponse
                {
                    Name = cvrData.Name,
                    IndustryCode = cvrData.IndustryCode,
                    Stats = stats
                };
            }
        }
    }
}