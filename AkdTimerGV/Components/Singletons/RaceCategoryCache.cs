using AkdTimerGV.Components.Models;
using System.Collections.Immutable;
using System.Text.Json;

/// <summary>
/// Singleton that holds all the RaceCategories from the RaceCategory json.
/// Initialized once as a Immutable Dictionary, 
/// </summary>
public sealed class RaceCategoryCache {
    public static readonly ImmutableDictionary<string, RaceCategory> Categories = JsonSerializer.Deserialize<List<RaceCategory>>(File.ReadAllText("wwwroot/RaceCategory.json")).ToImmutableDictionary(e => e.Name);
}