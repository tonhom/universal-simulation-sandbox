using System.Text.Json;

namespace UniversalSimulation.Domain;

public enum SimulationStatus { Running, Paused }
public enum EventImportance { Trace, Notable, Major, Historic, Cosmic }
public sealed record UniverseParameters(int Seed = 42, double GenesisRate = 1, double LifeChance = .72, double CultivationAffinity = .82, double ConflictRate = .25, double CosmicEntityRate = .08, int InitialStarSystems = 5,
    int MaxLivingWorldsPerSystem = 2, double YearsPerTick = 1, double KnowledgeRate = 1, double RelationshipRate = 1, double CraftingRate = 1, double WarRate = 1, double WorldAscensionRate = 1, double TechnologyAffinity = .72);
public sealed class UniverseState
{
    public Guid Id { get; init; } = Guid.NewGuid(); public string Name { get; init; } = "Unnamed Universe"; public UniverseParameters Parameters { get; init; } = new();
    public SimulationStatus Status { get; set; } = SimulationStatus.Running; public double Speed { get; set; } = 1; public long Tick { get; set; }
    public double Years { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow; public List<StarSystemState> Systems { get; } = []; public List<ImportantPerson> Persons { get; } = [];
    public List<CosmicRefinementRecord> Refinements { get; } = [];
    public List<WorldConnectionState> WorldConnections { get; } = [];
}
public sealed class StarSystemState { public Guid Id { get; init; } = Guid.NewGuid(); public required string Name { get; init; } public string Stage { get; set; } = "Protostar"; public double QiDensity { get; set; } public double DistanceFromOriginLightYears { get; init; } public int LivingWorldCapacity { get; init; } = 1; public bool Refined { get; set; } public List<WorldState> Worlds { get; } = []; }
public sealed class WorldState
{
    public Guid Id { get; init; } = Guid.NewGuid(); public required string Name { get; init; }
    public string Designation { get; init; } = "Uncatalogued"; public int Orbit { get; init; }
    public double OrbitalRadiusAu { get; init; }
    public string Stage { get; set; } = "Molten"; public string Biome { get; set; } = "Barren";
    public string WorldTier { get; set; } = "Mortal World"; public string MaxCultivationRealm { get; set; } = "Golden Core"; public int DevelopmentLevel { get; set; }
    public bool Living { get; set; }
    public string Civilization { get; set; } = "None"; public int Population { get; set; }
    public long DeathsTotal { get; set; }
    public long WanderingDead { get; set; }
    public long SoulsInSamsara { get; set; }
    public long ReincarnatedTotal { get; set; }
    public long BirthsTotal { get; set; }
    public double QiDensity { get; set; }
    public double Habitability { get; set; }
    public int SpiritualVeins { get; set; }
    public bool Refined { get; set; }
    public CivilizationState CivilizationData { get; set; } = new();
    public List<SectState> Sects { get; } = [];
    public List<RegionState> Regions { get; } = []; public List<KnowledgeState> Knowledge { get; } = [];
    public List<MaterialDeposit> Materials { get; } = []; public List<ConflictState> Conflicts { get; } = [];
    public List<InnovationState> Innovations { get; } = [];
}
public sealed class SectState
{
    public Guid Id { get; init; } = Guid.NewGuid(); public required string Name { get; init; }
    public string Path { get; init; } = "Orthodox"; public string Leader { get; set; } = "Unknown";
    public int Members { get; set; }
    public int Influence { get; set; }
    public string HighestRealm { get; set; } = "Foundation";
}
public sealed record CultivationMilestone(long Tick, double Year, string Realm, string Cause);
public sealed record ArtifactState(Guid Id, string Name, string Grade, string Kind, string Ability, double BreakthroughBonus);
public sealed record ImportantPerson(Guid Id, string Name, string Realm, Guid WorldId, long BornAtTick, bool Alive = true,
    string Lineage = "Unaffiliated Wanderer", List<CultivationMilestone>? CultivationHistory = null,
    double Luck = .5, string HeavenFavorReason = "ชะตายังไม่อาจอ่านได้อย่างชัดเจน", List<ArtifactState>? Artifacts = null,
    List<PersonRelationship>? Relationships = null, List<LawMastery>? Laws = null, List<string>? Skills = null);
public sealed record SimEvent(Guid Id, Guid UniverseId, long Tick, double Year, string Type, EventImportance Importance, string Summary, string Details, Guid? ActorId, Guid? LocationId, Guid? CausedByEventId, DateTimeOffset RecordedAt)
{
    public static SimEvent Create(UniverseState u, string type, EventImportance importance, string summary, string details = "", Guid? actor = null, Guid? location = null, Guid? cause = null) => new(Guid.NewGuid(), u.Id, u.Tick, u.Years, type, importance, summary, details, actor, location, cause, DateTimeOffset.UtcNow);
}
public sealed record CosmicMaterial(string Name, string Grade, double Mass, string Origin, bool FromLivingWorld);
public sealed record CosmicRefinementRecord(
    Guid Id,
    string Scale,
    Guid ActorId,
    string ActorName,
    Guid TargetId,
    string TargetName,
    string TargetNature,
    string Purpose,
    List<string> PreparationSteps,
    CosmicMaterial Material,
    string Result,
    string Aftermath,
    string NextAction,
    long CompletedAtTick,
    double CompletedAtYear);
public static class Snapshot { public static string Serialize(UniverseState u) => JsonSerializer.Serialize(u); public static UniverseState Deserialize(string json) => JsonSerializer.Deserialize<UniverseState>(json)!; }
