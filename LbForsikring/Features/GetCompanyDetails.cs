using LbForsikring.Integrations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LbForsikring.Features
{
    public static class GetCompanyDetails
    {
        public static void MapEndpoint(this WebApplication app)
        {
            app.MapGet("/company", Handle);
        }

        internal static async Task<Results<Ok<GetCompanyDetailsResponse>, ProblemHttpResult>> Handle(
            GetCompanyDetailsRequest request,
            [FromServices] ICvrService cvrService, [FromServices] IDstService dstService)
        {
            // Validate request
            if (string.IsNullOrEmpty(request.Name) && string.IsNullOrEmpty(request.Cvr))
            {
                return TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest,
                    detail: "Either Name or Cvr must be provided.");
            }
            
            var cvrData = await LookUpCompany(request, cvrService);

            //TODO: Validate cvrData

            var stats = await LookupStats(cvrData.BrancheKode, dstService);

            var response = CreateResponse(cvrData, stats);

            return TypedResults.Ok(response);
        }


        private static GetCompanyDetailsResponse CreateResponse(CvrResponse cvrData, StatsResponse stats)
        {
            //TODO: Merge to types into one
            throw new NotImplementedException();
        }

        private static async Task<StatsResponse> LookupStats(string brancheKode, IDstService dstService)
        {
            var result = await dstService.GetBrancheData(brancheKode, DateTime.Now.Year);
            return StatsResponse.FromString(result); 
        }

        private static async Task<CvrResponse> LookUpCompany(GetCompanyDetailsRequest request, ICvrService cvrService)
        {
            string result;
            
            if (request.Cvr != null)
            {
                result = await cvrService.GetByCvr(request.Cvr);
            } 
            else
            {
                result = await cvrService.GetByName(request.Name);
            }

            return CvrResponse.FromString(result);
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
            //TODO:
        }


        public class CvrResponse
        {
            public string BrancheKode { get; set; }

            public static CvrResponse FromString(string s)
            {
                //TODO: Parse string
                return new CvrResponse();
            }
        }
    }
}