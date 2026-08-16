namespace LbForsikring.Integrations;

public class StatsResponse
{
    public string IndustryCode { get; set; } = string.Empty;
    public string IndustryName { get; set; } = string.Empty;
    public string Metric { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Value { get; set; }


    public static List<StatsResponse> FromString(string csv)
    {
        var lines = csv.Split(
            '\n',
            StringSplitOptions.RemoveEmptyEntries);

        return lines
            .Skip(1)
            .Select(line =>
            {
                var columns = line.Split(';');

                var industry = columns[0].Split(' ', 2);

                return new StatsResponse
                {
                    IndustryCode = industry[0],
                    IndustryName = industry[1],
                    Metric = columns[1],
                    Year = int.Parse(columns[2]),
                    Value = decimal.Parse(columns[3])
                };
            })
            .ToList();
    }
}