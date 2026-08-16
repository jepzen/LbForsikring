using LbForsikring.Integrations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LbForsikring.Features
{
    public static class GetCompanyDetails
    {
        public static void MapGetCompanyDetailsEndpoint(this WebApplication app)
        {
            app.MapGet("/company", Handle);
        }

        internal static async Task<Results<Ok<GetCompanyDetailsResponse>, ProblemHttpResult>> Handle(
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


        private static GetCompanyDetailsResponse CreateResponse(CvrResponse cvrData, StatsResponse stats)
        {
            return new GetCompanyDetailsResponse()
            {
                Name = cvrData.Name,
                IndustryCode = cvrData.IndustryCode
                //TODO: map more
            };
        }

        private static async Task<StatsResponse> LookupStats(string brancheKode, IDstService dstService)
        {
            return new StatsResponse();
            var result = await dstService.GetBrancheData(brancheKode, DateTime.Now.Year);
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


        internal class StatsResponse
        {
            public static StatsResponse FromString(string s)
            {
                //TODO: Parse string
                return new StatsResponse();
            }
        }

        public record GetCompanyDetailsRequest
        {
            public string? Name { get; set; }
            public string? Cvr { get; set; }
        }

        public record GetCompanyDetailsResponse
        {
            public required string Name { get; set; }

            public required string IndustryCode { get; set; }
            
        }
    }
}