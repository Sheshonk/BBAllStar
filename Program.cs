using BBAllStar.Models.Output;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

string leagueName = "forged-in-blood---season-v-dancing-with-vampires";
string divisionName = "15965";

//general setup
var c = new RestClient("https://tourplay.net/api");

//get list of divisions and teams
var req = new RestRequest($"tournament/{leagueName}/clasifications?type=COACH");
var rep = c.ExecuteGet(req);
var body = JObject.Parse(rep.Content);
var season = body.Children().ToList().Where(_ => ((JProperty)_).Name == divisionName).First();
var divisions = new List<Division>();
foreach (var d in season.Children()["classifications"].Children())
{
    foreach (var t in d.Children().Children())
    {
        var division = divisions.Where(_ => _.Id == (int.Parse(((JProperty)d).Name))).FirstOrDefault();
        if (division == null)
        {
            divisions.Add(new Division()
            {
                Id = (int)t["group"]["id"],
                Name = (string)t["group"]["name"],
            });

            division = divisions[divisions.Count() - 1];
        }

        division.Teams.Add(new Team()
        {
            Id = (int)t["roster"]["id"],
            Name = (string)t["roster"]["teamName"],
            Race = (string)t["roster"]["teamRace"],
            Username = (string)t["inscription"]["player"]["userNameToShow"],
        });
    }
}

//get list of players for each team
foreach (var d in divisions)
{
    foreach (var t in d.Teams)
    {
        req = new RestRequest($"rosters/{t.Id}");
        rep = c.ExecuteGet(req);
        body = JObject.Parse(rep.Content);

        foreach (var p in body["lineUps"].Children())
        {
            var skills = new List<string>();
            foreach (var s in p["skills"].Children())
            {
                skills.Add((string)s["skillMaster"]["name"]);
            }

            skills = ParseStatUps(p, "ag", skills);
            skills = ParseStatUps(p, "av", skills);
            skills = ParseStatUps(p, "ma", skills);
            skills = ParseStatUps(p, "st", skills);
            skills = ParseStatUps(p, "pa", skills);
            
            t.Players.Add(new Player()
            {
                Id = (int)p["id"],
                Name = (string)p["name"],
                Number = (int)p["number"],
                Position = (string)p["position"],
                SkillUps = skills,
                TeamValue = (int)p["cost"],
                TotalStarPlayerPoints = (int)p["totalStarPlayerPoints"],
            });
        }
    }
}

//calculate all stars
foreach (var d in divisions)
{
    foreach (var t in d.Teams)
    {
        foreach (var p in t.Players.OrderByDescending(_ => _.TotalStarPlayerPoints).ThenByDescending(_ => _.TeamValue).ToList())
        {
            if (t.AllStars.Count >= 2 && t.AllStars[t.AllStars.Count - 1].TotalStarPlayerPoints != p.TotalStarPlayerPoints)
                break;

            t.AllStars.Add(p);

            if (t.AllStars.Count <= 2)
                d.TotalTeamValue += p.TeamValue;
        }
    }
}

//output
foreach (var d in divisions.OrderBy(_ => _.Name).ToList())
{
    Console.WriteLine($"{d.Name} - TV: {(d.TotalTeamValue/1000)}K");

    foreach (var t in d.Teams.OrderBy(_ => _.Username).ToList())
    {
        Console.WriteLine($"\t{t.Username}'s \"{t.Name}\" {t.Race} Team");

        foreach (var p in t.AllStars)
        {
            Console.WriteLine($"\t\t{p.TotalStarPlayerPoints} SPP #{p.Number} {p.Position} \"{p.Name}\" at ${p.TeamValue / 1000}K with skills: {String.Join(", ", p.SkillUps.OrderBy(_ => _).ToArray())}");
        }
    }
}

Console.WriteLine($"TV Diff = {(divisions[0].TotalTeamValue - divisions[1].TotalTeamValue)/1000}K");

Console.WriteLine("fin");



List<string> ParseStatUps(JToken jToken, string stat, List<string> skillUps)
{
    int diff = (int)jToken[stat] - (int)jToken["lineUpMaster"][stat];
    if (diff > 0)
        skillUps.Add($"{stat.ToUpper()}+{diff}");
    else if (diff < 0)
        skillUps.Add($"{stat.ToUpper()}-{diff}");

    return skillUps;
}