using UniversalSimulation.Domain;

namespace UniversalSimulation.Engine;

public sealed partial class SimulationEngine
{
    private static readonly (string Name, string Domain)[] CultivationKnowledge =
    [
        ("Herbal Medicine", "Medicine"), ("Spirit Iron Smelting", "Crafting"),
        ("Formation Geometry", "Formation"), ("Talisman Script", "Runes"),
        ("Soul Observation", "Samsara"), ("Qi Agriculture", "Ecology"),
        ("Artifact Forging", "Crafting"), ("Military Logistics", "Warfare")
    ];

    private static readonly (string Name, string Domain)[] TechnologyKnowledge =
    [
        ("Experimental Method", "Science"), ("Precision Metallurgy", "Production"),
        ("Steam Power", "Energy"), ("Electromagnetism", "Energy"),
        ("Digital Computation", "Computation"), ("Fusion Energy", "Energy"),
        ("Autonomous Robotics", "Automation"), ("Biotechnology", "Biotechnology"),
        ("Radio Astronomy", "Communication"), ("Orbital Engineering", "Spaceflight"),
        ("Quantum Field Theory", "Physics")
    ];

    private static void EnsureCivilizationState(WorldState world)
    {
        if (world.CivilizationData.PrimaryPath != "Undeveloped") return;
        if (world.Civilization.Contains("Technolog") || world.Civilization.Contains("Industrial") || world.Civilization.Contains("Commonwealth"))
        {
            world.CivilizationData.PrimaryPath = "Technology"; world.CivilizationData.Stage = world.Civilization; world.CivilizationData.Level = Math.Max(2, world.DevelopmentLevel);
        }
        else if (world.Civilization.Contains("Cultivation") || world.Civilization.Contains("Immortal"))
        {
            world.CivilizationData.PrimaryPath = "Cultivation"; world.CivilizationData.Stage = world.Civilization; world.CivilizationData.Level = Math.Max(2, world.DevelopmentLevel);
            RaiseCapability(world, "Cultivation", .2); RaiseCapability(world, "LawManipulation", .05);
        }
        else if (world.Civilization == "Tribal") { world.CivilizationData.PrimaryPath = "Emergent"; world.CivilizationData.Stage = "Tribal"; world.CivilizationData.Level = 1; }
    }

    private static (string, string)[] KnowledgeCandidates(WorldState world) => world.CivilizationData.PrimaryPath switch
    {
        "Technology" => TechnologyKnowledge,
        "Hybrid" => TechnologyKnowledge.Concat(CultivationKnowledge).ToArray(),
        _ => CultivationKnowledge
    };

    private static void ApplyKnowledgeCapability(WorldState world, string domain, double amount)
    {
        var capability = domain switch
        {
            "Science" or "Physics" => "Computation",
            "Production" or "Crafting" or "Automation" => "Production",
            "Energy" => "Energy",
            "Medicine" or "Biotechnology" => "Medicine",
            "Computation" => "Computation",
            "Communication" => "Communication",
            "Spaceflight" => "Spaceflight",
            "Formation" or "Runes" => "LawManipulation",
            "Samsara" => "SoulTechnology",
            _ => "Cultivation"
        };
        RaiseCapability(world, capability, amount);
        if (world.CivilizationData.PrimaryPath == "Cultivation") RaiseCapability(world, "Cultivation", amount * .35);
    }

    private static void RaiseCapability(WorldState world, string capability, double amount)
    {
        world.CivilizationData.Capabilities.TryGetValue(capability, out var current);
        world.CivilizationData.Capabilities[capability] = Math.Clamp(current + amount, 0, 1);
    }

    private static void EvolveCrossWorldCivilization(UniverseState universe, Random random, List<SimEvent> events)
    {
        var worlds = universe.Systems.SelectMany(system => system.Worlds).Where(world => world.Living && !world.Refined && world.CivilizationData.Level >= 2).ToArray();
        if (worlds.Length < 2) return;

        foreach (var connection in universe.WorldConnections.ToArray())
        {
            var a = worlds.FirstOrDefault(world => world.Id == connection.WorldAId); var b = worlds.FirstOrDefault(world => world.Id == connection.WorldBId);
            if (a is null || b is null) continue;
            AdvanceConnection(universe, connection, a, b, random, events);
        }

        if (!Chance(random, .0015 * universe.Parameters.KnowledgeRate)) return;
        var capable = worlds.Where(CanReachAnotherWorld).OrderBy(_ => random.Next()).ToArray(); if (capable.Length == 0) return;
        var origin = capable[0]; var target = worlds.Where(world => world.Id != origin.Id && !HasConnection(universe, origin.Id, world.Id)).OrderBy(_ => random.Next()).FirstOrDefault(); if (target is null) return;
        var method = TravelMethod(universe, origin); var trigger = ContactTrigger(origin, method); var distance = WorldDistance(universe, origin.Id, target.Id);
        var newConnection = new WorldConnectionState { WorldAId = origin.Id, WorldBId = target.Id, Trigger = trigger, TravelMethod = method, DistanceLightYears = distance, StartedAtTick = universe.Tick };
        universe.WorldConnections.Add(newConnection);
        events.Add(SimEvent.Create(universe, "world-contact.detected", EventImportance.Historic, $"{origin.Name} ตรวจพบ {target.Name} ที่อยู่นอกขอบเขต World", $"สิ่งกระตุ้น: {trigger} วิธีเดินทางที่เสนอ: {method} ระยะห่างโดยประมาณ: {distance:N2} ปีแสง", location: origin.Id));
    }

    private static void AdvanceConnection(UniverseState universe, WorldConnectionState connection, WorldState a, WorldState b, Random random, List<SimEvent> events)
    {
        if (connection.Status == "Detected" && Chance(random, .006))
        {
            connection.Status = "First Contact"; connection.Trust = Math.Clamp(connection.Trust + random.NextDouble() * .18, 0, 1);
            events.Add(SimEvent.Create(universe, "world-contact.first-contact", EventImportance.Historic, $"{a.Name} และ {b.Name} ติดต่อกันเป็นครั้งแรก", $"{connection.TravelMethod} เดินทางข้ามขอบเขตสำเร็จ ความไว้ใจเริ่มต้น: {connection.Trust:P0} ทั้งสองฝ่ายยังไม่เข้าใจกฎและสถาบันของอีกฝ่ายอย่างสมบูรณ์", location: a.Id, cause: LatestCause(events, a.Id)));
            return;
        }
        if (connection.Status == "First Contact" && Chance(random, .005 * (.5 + connection.Trust)))
        {
            connection.Status = "Knowledge Exchange"; connection.Trust = Math.Clamp(connection.Trust + .12, 0, 1);
            events.Add(SimEvent.Create(universe, "knowledge.exchange-agreement", EventImportance.Historic, $"{a.Name} และ {b.Name} เปิดการแลกเปลี่ยนองค์ความรู้ข้าม World", "นักแปล นักวิชาการ ปรมาจารย์ค่ายกล และวิศวกรตกลงแลกเปลี่ยนความรู้ที่พิสูจน์ซ้ำได้ โดยยังปกปิดความลับเชิงยุทธศาสตร์", location: a.Id, cause: LatestCause(events, a.Id)));
            return;
        }
        if (connection.Status is not ("Knowledge Exchange" or "Integrated") || !Chance(random, .008 * universe.Parameters.KnowledgeRate)) return;
        if (TransferKnowledge(universe, connection, a, b, random, events)) { connection.KnowledgeExchanges++; connection.Trust = Math.Clamp(connection.Trust + .025, 0, 1); }
        TryCreateHybridInnovation(universe, connection, a, b, random, events);
    }

    private static bool TransferKnowledge(UniverseState universe, WorldConnectionState connection, WorldState a, WorldState b, Random random, List<SimEvent> events)
    {
        var directions = random.Next(2) == 0 ? new[] { (Source: a, Target: b), (Source: b, Target: a) } : new[] { (Source: b, Target: a), (Source: a, Target: b) };
        foreach (var direction in directions)
        {
            var knowledge = direction.Source.Knowledge.Where(item => direction.Target.Knowledge.All(existing => existing.Name != item.Name)).OrderBy(_ => random.Next()).FirstOrDefault();
            if (knowledge is null) continue;
            direction.Target.Knowledge.Add(new(Guid.NewGuid(), knowledge.Name, knowledge.Domain, Math.Max(1, knowledge.Level - 1), $"แลกเปลี่ยนกับ {direction.Source.Name}", universe.Tick, direction.Source.Id, "ถ่ายทอดข้าม World"));
            direction.Target.DevelopmentLevel++; ApplyKnowledgeCapability(direction.Target, knowledge.Domain, .055);
            events.Add(SimEvent.Create(universe, "knowledge.cross-world-transfer", EventImportance.Major, $"{direction.Target.Name} เรียนรู้ {knowledge.Name} จาก {direction.Source.Name}", $"องค์ความรู้ถูกถ่ายทอดผ่าน {connection.TravelMethod} และเริ่มนำมาใช้ในท้องถิ่นที่ระดับ {Math.Max(1, knowledge.Level - 1)} ขณะนี้สามารถประยุกต์ร่วมกับความรู้เดิมของผู้รับได้", location: direction.Target.Id, cause: LatestCause(events, direction.Source.Id)));
            return true;
        }
        return false;
    }

    private static void TryCreateHybridInnovation(UniverseState universe, WorldConnectionState connection, WorldState a, WorldState b, Random random, List<SimEvent> events)
    {
        if (!Chance(random, .16 * universe.Parameters.KnowledgeRate)) return;
        var domains = a.Knowledge.Concat(b.Knowledge).Select(item => item.Domain).ToHashSet();
        var recipes = new[]
        {
            new InnovationRecipe("Programmable Formation Network", "Tech-Cultivation", "ควบคุมและประสานค่ายกลในระดับดาวเคราะห์", new[] { "Computation", "Formation" }, "Communication"),
            new InnovationRecipe("Planetary Qi Power Grid", "Energy Infrastructure", "กระจายพลังงานไฟฟ้าจากดาวฤกษ์และ Qi จากเส้นชีพจรผ่านโครงข่ายเดียว", new[] { "Energy", "Formation" }, "Energy"),
            new InnovationRecipe("Automated Pill Foundry", "Medicine Production", "ผลิตโอสถบำเพ็ญมาตรฐานในปริมาณมาก", new[] { "Automation", "Medicine" }, "Production"),
            new InnovationRecipe("Artificial Meridian Scanner", "Bio-Spiritual Medicine", "สร้างแผนที่เส้นลมปราณโดยไม่ต้องมีประสาทสัมผัส Qi แต่กำเนิด", new[] { "Biotechnology", "Medicine" }, "Medicine"),
            new InnovationRecipe("Resonant World Gate", "World Travel", "ลดพลังงานและความเสียหายต่อขอบเขตจากการเดินทางข้าม World ซ้ำ ๆ", new[] { "Spaceflight", "Formation" }, "WorldTravel"),
            new InnovationRecipe("Soul–Machine Consciousness Bridge", "Soul Computation", "เชื่อมการรับรู้ที่เข้ารหัสกับรูปแบบวิญญาณที่ตรวจวัดได้", new[] { "Computation", "Samsara" }, "SoulTechnology")
        };
        var recipe = recipes.Where(candidate => candidate.RequiredDomains.All(domains.Contains) && a.Innovations.All(item => item.Name != candidate.Name) && b.Innovations.All(item => item.Name != candidate.Name)).OrderBy(_ => random.Next()).FirstOrDefault();
        if (recipe is null) return;
        var developer = a.Innovations.Count <= b.Innovations.Count ? a : b; var partner = developer.Id == a.Id ? b : a;
        var innovation = new InnovationState(Guid.NewGuid(), recipe.Name, recipe.Kind, recipe.Purpose, recipe.RequiredDomains.ToList(), $"สถาบันวิจัยร่วมของ {developer.Name} และ {partner.Name}", universe.Tick, .16, connection.Id);
        developer.Innovations.Add(innovation); developer.DevelopmentLevel += 2; developer.CivilizationData.PrimaryPath = "Hybrid"; developer.CivilizationData.Stage = "Cross-World Synthesis"; developer.CivilizationData.Level = Math.Max(5, developer.CivilizationData.Level + 1); developer.Civilization = "Hybrid World Civilization"; RaiseCapability(developer, recipe.Capability, .22); RaiseCapability(developer, "WorldTravel", .06);
        connection.Status = "Integrated"; connection.Trust = Math.Clamp(connection.Trust + .1, 0, 1);
        events.Add(SimEvent.Create(universe, "innovation.hybrid-breakthrough", EventImportance.Cosmic, $"{developer.Name} สร้าง {recipe.Name} สำเร็จ", $"ประยุกต์ {string.Join(" + ", recipe.RequiredDomains)} จาก {developer.Name} และ {partner.Name} จุดประสงค์: {recipe.Purpose} ระดับอารยธรรมและความสามารถด้าน {recipe.Capability} เพิ่มสูงขึ้น", location: developer.Id, cause: LatestCause(events, developer.Id)));
    }

    private static bool CanReachAnotherWorld(WorldState world) => Capability(world, "WorldTravel") >= .2 || Capability(world, "Spaceflight") >= .35 || Capability(world, "LawManipulation") >= .28;
    private static double Capability(WorldState world, string name) => world.CivilizationData.Capabilities.TryGetValue(name, out var value) ? value : 0;
    private static bool HasConnection(UniverseState universe, Guid a, Guid b) => universe.WorldConnections.Any(connection => connection.WorldAId == a && connection.WorldBId == b || connection.WorldAId == b && connection.WorldBId == a);
    private static string TravelMethod(UniverseState universe, WorldState world)
    {
        if (Capability(world, "WorldTravel") >= .2) return "ประตู World แบบสั่นพ้อง";
        if (Capability(world, "Spaceflight") >= .35) return "ยานวิจัย Fold Drive";
        var traveler = universe.Persons.FirstOrDefault(person => person.WorldId == world.Id && person.Alive && RealmRank(person.Realm) >= 5);
        return traveler is null ? "โพรบ Realm ที่นำทางด้วยกฎ" : $"เส้นทางห้วงว่างที่เปิดโดย {traveler.Name}";
    }
    private static string ContactTrigger(WorldState world, string method) => world.CivilizationData.PrimaryPath switch
    {
        "Technology" => $"ดาราศาสตร์วิทยุและเซนเซอร์ควอนตัมแยกสัญญาณที่ไม่เกิดขึ้นตามธรรมชาติได้ จึงเริ่มใช้ {method}",
        "Cultivation" => $"ปรมาจารย์ค่ายกลตรวจพบการสั่นพ้องของกฎต่างแดน และใช้ {method} ติดตามร่องรอยกรรม",
        _ => $"หอสังเกตการณ์แบบผสมจับคู่ข้อมูลดาวฤกษ์กับการสั่นของขอบเขต World จึงเตรียม {method}"
    };
    private static double WorldDistance(UniverseState universe, Guid a, Guid b)
    {
        var sa = universe.Systems.First(system => system.Worlds.Any(world => world.Id == a)); var sb = universe.Systems.First(system => system.Worlds.Any(world => world.Id == b));
        return sa.Id == sb.Id ? .001 : Math.Max(.1, Math.Abs(sa.DistanceFromOriginLightYears - sb.DistanceFromOriginLightYears));
    }

    private sealed record InnovationRecipe(string Name, string Kind, string Purpose, string[] RequiredDomains, string Capability);
}
