namespace LbForsikring.Integrations
{
    public class DstService : IDstService
    {
        public void GetBrancheData()
        {
            throw new NotImplementedException();
        }
    }

    public interface IDstService
    {
        public void GetBrancheData();
    }
}
