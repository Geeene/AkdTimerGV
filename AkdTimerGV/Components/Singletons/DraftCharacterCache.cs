using AkdTimerGV.Components.Draft;
using AkdTimerGV.Components.Resources;
using System.Collections.Immutable;
using System.Text.Json;

// Singleton to hold all the open Lobbies
public sealed class DraftCharacterCache {
    public static readonly ImmutableDictionary<string, DraftGrouping> DraftGroupings = JsonSerializer.Deserialize<List<DraftGrouping>>(File.ReadAllText("wwwroot/DraftCharacters.json")).ToImmutableDictionary(e => e.ShortName);
    private static ImmutableList<DraftGrouping> OrderedGroupings;

    public static ImmutableList<DraftGrouping> GetGroupingsOrdered() {
        if (OrderedGroupings == null) { 
            OrderedGroupings = DraftGroupings.Values.OrderBy(dg => dg.Row).ThenBy(dg => dg.Column).ToImmutableList();
        
        }
        return OrderedGroupings;
    }
}