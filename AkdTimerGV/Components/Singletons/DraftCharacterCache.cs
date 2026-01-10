using AkdTimerGV.Components.Draft;
using AkdTimerGV.Components.Resources;
using System.Collections.Immutable;
using System.Text.Json;

/// <summary>
/// Singleton that holds all the Character from the DraftCharacters json.
/// Initialized once as a Immutable Dictionary, 
/// </summary>
public sealed class DraftCharacterCache {
    public static readonly ImmutableDictionary<string, DraftGrouping> DraftGroupings = JsonSerializer.Deserialize<List<DraftGrouping>>(File.ReadAllText("wwwroot/DraftCharacters.json")).ToImmutableDictionary(e => e.InternalName);
}