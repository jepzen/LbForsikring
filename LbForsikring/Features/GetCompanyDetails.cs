using LbForsikring.Integrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LbForsikring.Features
{
    public static class GetCompanyDetails
    {

        public static void MapEndpoint(this WebApplication app)
        {
            app.MapGet("/comapny", Handle);
        }
        

        public static async Task<Results<Ok<GetCompanyDetailsResponse>, ProblemHttpResult>> Handle(GetCoapnyDetailsRequest request, 
            [FromServices]ICvrService cvrService, [FromServices]IDstService dstService)
        {
            // Validate request
            if (string.IsNullOrEmpty(request.Name) && string.IsNullOrEmpty(request.Cvr))
            {
                return TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Either Name or Cvr must be provided.");
            }


            var cvrData = await LookUpCompany(request, cvrService);
            
            //TODO: Validate cvrData

            var stats = await LookupStats(cvrData);


            var response = CreateResponse(cvrData, stats);
           
            return TypedResults.Ok(response);
        }

        
        private static GetCompanyDetailsResponse CreateResponse(CvrResponse cvrData, StatsResponse stats)
        {
            throw new NotImplementedException();
        }

        private static async Task<StatsResponse> LookupStats(CvrResponse cvrData)
        {
            throw new NotImplementedException();
        }

        private static async Task<CvrResponse> LookUpCompany(GetCoapnyDetailsRequest request, ICvrService cvrService)
        {
            if (request.Cvr != null) {
                cvrService.GetByCvr(request.Cvr);
            }
            
            cvrService.GetByName(request.Name);

            return new CvrResponse();
        }


    internal class StatsResponse
    {
    }

    public record GetCoapnyDetailsRequest
    {
        public string? Name { get; set; }
        public string? Cvr { get; set; }
    }

    public record GetCompanyDetailsResponse
    {
        //TODO:
    }


    public record CvrResponse
    {
        public string BrancheKode { get; set; }
    }
} 
}
