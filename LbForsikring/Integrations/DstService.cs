namespace LbForsikring.Integrations
{
    public class DstService : IDstService
    {
        public async Task<string> GetBrancheData(string brancheKode, int year)
        {
            throw new NotImplementedException();
        }
    }

    public interface IDstService
    {
        public Task<string> GetBrancheData(string brancheKode, int year);
    }
}
