namespace UniversalSimulation.Domain;

public sealed class RegionState
{
    public Guid Id { get; init; } = Guid.NewGuid(); public required string Name { get; init; }
    public string Kind { get; set; } = "Continent"; public string Terrain { get; set; } = "Plains";
    public long Population { get; set; } public double QiDensity { get; set; } public double Stability { get; set; } = 1;
    public double Devastation { get; set; } public string? DominantSect { get; set; }
}
public sealed record KnowledgeState(Guid Id,string Name,string Domain,int Level,string DiscoveredBy,long DiscoveredAtTick,Guid? OriginWorldId=null,string Acquisition="Discovery");
public sealed record MaterialDeposit(Guid Id,string Name,string Grade,double Abundance,string Region,string LawAffinity);
public sealed class CivilizationState
{
    public string PrimaryPath { get; set; } = "Undeveloped";
    public string Stage { get; set; } = "Pre-Civilization";
    public int Level { get; set; }
    public Dictionary<string,double> Capabilities { get; set; } = new()
    {
        ["Energy"] = 0, ["Production"] = 0, ["Medicine"] = 0, ["Computation"] = 0,
        ["Communication"] = 0, ["Spaceflight"] = 0, ["WorldTravel"] = 0,
        ["Cultivation"] = 0, ["LawManipulation"] = 0, ["SoulTechnology"] = 0
    };
}
public sealed class WorldConnectionState
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid WorldAId { get; init; }
    public Guid WorldBId { get; init; }
    public string Status { get; set; } = "Detected";
    public string Trigger { get; init; } = "สัญญาณที่ยังไม่ทราบแหล่งกำเนิด";
    public string TravelMethod { get; set; } = "โพรบทดลอง";
    public double DistanceLightYears { get; init; }
    public double Trust { get; set; } = .25;
    public int KnowledgeExchanges { get; set; }
    public long StartedAtTick { get; init; }
}
public sealed record InnovationState(Guid Id,string Name,string Kind,string Purpose,List<string> Inputs,string DevelopedBy,long DevelopedAtTick,double CivilizationImpact,Guid? ConnectionId=null);
public sealed class ConflictState
{
    public Guid Id { get; init; }=Guid.NewGuid(); public required string Name { get; init; }
    public string Scale { get; init; }="Regional"; public required string Attacker { get; init; } public required string Defender { get; init; }
    public string Cause { get; init; }="การแข่งขันแย่งชิงทรัพยากร"; public string Status { get; set; }="Active";
    public long StartedAtTick { get; init; } public long Casualties { get; set; }
}
public sealed record PersonRelationship(Guid PersonId,string PersonName,string Type,double Strength,long StartedAtTick);
public sealed record LawMastery(string Law,int Level,double Comprehension);
