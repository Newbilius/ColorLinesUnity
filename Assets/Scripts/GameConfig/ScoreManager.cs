using System.Linq;

//todo можно сделать синглтоном, сохраняя промежуточные результаты и работая целиком через этот объект
public static class ScoreManager
{
    public static int GetMinimumValue()
    {
        var scoreResults = GetScores();

        return scoreResults.Data
            .OrderBy(x => x.Value)
            .First()
            .Value;
    }

    public static string GetNamesListForUI()
    {
        var scoreResults = GetScores();
        int counter = 0;
        return string.Join("\n", scoreResults.Data.Select(x =>
        {
            counter++;
            return (counter < 10 ? " " : "") + counter + ". " + x.Name;
        }).ToArray());
    }

    public static string GetValuesListForUI()
    {
        var scoreResults = GetScores();
        return string.Join("\n", scoreResults.Data.Select(x => x.Value.ToString()).ToArray());
    }

    public static Score GetHighScore()
    {
        return GetScores().Data.OrderByDescending(x => x.Value).First();
    }

    public static Scores GetScores()
    {
        var results = Config.GetRawScores();
        if (!results.Data.Any())
        {
            results.Data.Add(new Score
            {
                Name = "Король",
                Value = 100
            });
        }

        return AddAdditionalScores(results);
    }

    private static Scores AddAdditionalScores(Scores results)
    {
        while (results.Data.Count < 10)
            results.Data.Add(new Score
            {
                Name = "-----",
                Value = 0
            });
        return results;
    }
}
