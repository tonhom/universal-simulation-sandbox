using UniversalSimulation.Domain;
using UniversalSimulation.Engine;
using Xunit;

public class SimulationEngineTests
{
    [Fact] public void Same_seed_and_state_are_deterministic()
    {
        var a = new UniverseState { Name = "A", Parameters = new(123, InitialStarSystems: 3) };
        var b = new UniverseState { Name = "B", Parameters = new(123, InitialStarSystems: 3) };
        var engine = new SimulationEngine();
        engine.Advance(a, 500); engine.Advance(b, 500);
        Assert.Equal(a.Systems.Select(x => (x.Name, x.Stage, x.Worlds.Count)), b.Systems.Select(x => (x.Name, x.Stage, x.Worlds.Count)));
    }
    [Fact] public void Speed_changes_elapsed_simulation_time()
    {
        var u = new UniverseState { Speed = 10 }; new SimulationEngine().Advance(u, 3); Assert.Equal(30, u.Years);
    }
    [Fact] public void Higher_realms_are_progressively_harder_to_break_through()
    {
        var realms = new[] { "Qi Gathering", "Foundation", "Golden Core", "Nascent Soul", "Dao Lord" };
        var chances = realms.Select(r => SimulationEngine.BreakthroughChance(r, .5)).ToArray();
        Assert.True(chances.Zip(chances.Skip(1), (lower, higher) => lower > higher).All(x => x));
    }
    [Fact] public void Living_world_records_death_and_samsara_ledgers()
    {
        var u = new UniverseState { Parameters = new(7, InitialStarSystems: 0) };
        var system = new StarSystemState { Name = "Test Star", Stage = "Main Sequence" };
        var world = new WorldState { Name = "Test World", Stage = "Living", Living = true, Civilization = "Tribal", Population = 1_000_000 };
        system.Worlds.Add(world); u.Systems.Add(system); new SimulationEngine().Advance(u, 100);
        Assert.True(world.DeathsTotal > 0); Assert.True(world.BirthsTotal > 0); Assert.True(world.SoulsInSamsara >= 0); Assert.True(world.ReincarnatedTotal > 0);
    }
    [Fact] public void Generated_worlds_have_natural_names_designations_orbits_and_regions()
    {
        var u = new UniverseState { Parameters = new(91, InitialStarSystems: 1) }; new SimulationEngine().Advance(u, 1000);
        var worlds = Assert.Single(u.Systems).Worlds; Assert.NotEmpty(worlds);
        Assert.All(worlds, w => { Assert.StartsWith("USS-", w.Designation); Assert.True(w.Orbit > 0); Assert.DoesNotContain("Celestial-", w.Name); Assert.NotEmpty(w.Regions); });
    }
    [Fact] public void World_ascension_raises_the_cultivation_ceiling_and_emits_cosmic_history()
    {
        var u = new UniverseState { Parameters = new(33, InitialStarSystems: 0) };
        var system = new StarSystemState { Name = "Threshold Star", Stage = "Main Sequence" }; var world = new WorldState { Name = "Threshold Realm", Stage = "Living", Living = true, Civilization = "Cultivation Sects", Population = 1_000_000 };
        world.Knowledge.AddRange([new(Guid.NewGuid(),"Qi Agriculture","Ecology",1,"Test",1),new(Guid.NewGuid(),"Formation Geometry","Formation",1,"Test",2),new(Guid.NewGuid(),"Artifact Forging","Crafting",1,"Test",3)]); system.Worlds.Add(world); u.Systems.Add(system); u.Persons.Add(new(Guid.NewGuid(),"Golden Ancestor","Golden Core",world.Id,0));
        var events = new SimulationEngine().Advance(u, 10_000);
        Assert.NotEqual("Golden Core", world.MaxCultivationRealm); Assert.Contains(events, e => e.Type == "world.ascension" && e.Importance == EventImportance.Cosmic);
    }
    [Fact] public void Star_system_never_exceeds_two_living_worlds_and_records_distance()
    {
        var u = new UniverseState { Parameters = new(77, LifeChance: 1, InitialStarSystems: 1, MaxLivingWorldsPerSystem: 2) };
        new SimulationEngine().Advance(u, 10_000); var system = Assert.Single(u.Systems);
        Assert.InRange(system.LivingWorldCapacity, 1, 2); Assert.InRange(system.Worlds.Count(w => w.Living && !w.Refined), 0, system.LivingWorldCapacity); Assert.True(system.DistanceFromOriginLightYears > 0);
        Assert.All(system.Worlds, w => Assert.True(w.OrbitalRadiusAu > 0));
    }
    [Fact] public void Important_people_can_form_cross_world_relationships()
    {
        var u = new UniverseState { Parameters = new(12, InitialStarSystems: 0, RelationshipRate: 3) }; var system = new StarSystemState { Name = "Bridge Star" };
        var a = new WorldState { Name = "East Realm", Living = true, Stage = "Living" }; var b = new WorldState { Name = "West Realm", Living = true, Stage = "Living" }; system.Worlds.AddRange([a, b]); u.Systems.Add(system);
        u.Persons.Add(new(Guid.NewGuid(), "East Sage", "Golden Core", a.Id, 0)); u.Persons.Add(new(Guid.NewGuid(), "West Sage", "Golden Core", b.Id, 0)); new SimulationEngine().Advance(u, 10_000);
        Assert.Contains(u.Persons.SelectMany(p => p.Relationships ?? []), r => r.PersonId == u.Persons[1].Id || r.PersonId == u.Persons[0].Id);
    }
    [Fact] public void Cultivation_history_includes_failed_or_pill_assisted_attempts()
    {
        var u = new UniverseState { Parameters = new(19, InitialStarSystems: 0) }; var system = new StarSystemState { Name = "Pill Star" }; var world = new WorldState { Name = "Alchemy Realm", Living = true, Stage = "Living", MaxCultivationRealm = "Immortal" };
        world.Knowledge.Add(new(Guid.NewGuid(), "Herbal Medicine", "Medicine", 1, "Healer", 0)); system.Worlds.Add(world); u.Systems.Add(system); var person = new ImportantPerson(Guid.NewGuid(), "Pill Seeker", "Qi Gathering", world.Id, 0, Luck: .5); u.Persons.Add(person);
        var events = new SimulationEngine().Advance(u, 10_000); Assert.Contains(events, e => e.Type is "cultivator.breakthrough-failed" or "cultivator.pill-assisted-breakthrough");
    }
    [Fact] public void Cosmic_refinement_records_actor_preparation_purpose_aftermath_and_next_action()
    {
        var u = new UniverseState { Parameters = new(41, InitialStarSystems: 0, CosmicEntityRate: 100) };
        var system = new StarSystemState { Name = "Crucible Star", Stage = "Dying Giant", QiDensity = 1 };
        var world = new WorldState { Name = "Offering Realm", Living = true, Stage = "Living", Population = 10_000, QiDensity = 1 };
        world.Regions.Add(new() { Name = "Offering Mountain", Kind = "Sacred Mountain", Population = 300, QiDensity = 2 }); system.Worlds.Add(world); u.Systems.Add(system);
        var refiner = new ImportantPerson(Guid.NewGuid(), "Test Refiner", "Immortal", world.Id, 0); u.Persons.Add(refiner);
        var events = new SimulationEngine().Advance(u, 100);
        Assert.NotEmpty(u.Refinements); var record = u.Refinements[0];
        Assert.Equal(refiner.Id, record.ActorId); Assert.NotEmpty(record.PreparationSteps); Assert.False(string.IsNullOrWhiteSpace(record.Purpose)); Assert.False(string.IsNullOrWhiteSpace(record.Aftermath)); Assert.False(string.IsNullOrWhiteSpace(record.NextAction));
        Assert.Contains(events, e => e.Type == "cosmic.refinement.preparation"); Assert.Contains(events, e => e.Type == "cosmic.refinement.completed");
    }
    [Fact] public void Tribal_world_can_evolve_into_a_technological_civilization()
    {
        var u = new UniverseState { Parameters = new(73, CultivationAffinity: 0, InitialStarSystems: 0) };
        var system = new StarSystemState { Name = "Reason Star", Stage = "Main Sequence" };
        var world = new WorldState { Name = "Logos", Living = true, Stage = "Living", Civilization = "Tribal", Population = 100_000 };
        system.Worlds.Add(world); u.Systems.Add(system); var events = new SimulationEngine().Advance(u, 10_000);
        Assert.Contains(world.CivilizationData.PrimaryPath, new[] { "Technology", "Hybrid" }); Assert.Contains(events, e => e.Type == "technology.discovery"); Assert.Contains(world.Knowledge, knowledge => knowledge.Domain is "Science" or "Energy" or "Computation");
    }
    [Fact] public void Capable_worlds_make_contact_exchange_knowledge_and_can_create_hybrid_innovation()
    {
        var u = new UniverseState { Parameters = new(117, InitialStarSystems: 0, KnowledgeRate: 3) };
        var systemA = new StarSystemState { Name = "Machine Star", DistanceFromOriginLightYears = 10 }; var systemB = new StarSystemState { Name = "Dao Star", DistanceFromOriginLightYears = 42 };
        var tech = new WorldState { Name = "Machina", Living = true, Stage = "Living", Civilization = "Interplanetary Commonwealth", Population = 1_000_000, DevelopmentLevel = 7, CivilizationData = new() { PrimaryPath = "Technology", Stage = "Spacefaring Age", Level = 4 } };
        tech.CivilizationData.Capabilities["Spaceflight"] = .8; tech.Knowledge.Add(new(Guid.NewGuid(), "Digital Computation", "Computation", 2, "Machine Academy", 0)); tech.Knowledge.Add(new(Guid.NewGuid(), "Fusion Energy", "Energy", 2, "Machine Academy", 0));
        var dao = new WorldState { Name = "Dao Haven", Living = true, Stage = "Living", Civilization = "Cultivation Kingdoms", Population = 1_000_000, DevelopmentLevel = 7, CivilizationData = new() { PrimaryPath = "Cultivation", Stage = "Sect Kingdom Age", Level = 4 } };
        dao.CivilizationData.Capabilities["LawManipulation"] = .7; dao.Knowledge.Add(new(Guid.NewGuid(), "Formation Geometry", "Formation", 2, "Heaven Array Sect", 0)); dao.Knowledge.Add(new(Guid.NewGuid(), "Herbal Medicine", "Medicine", 2, "Jade Physician Hall", 0));
        systemA.Worlds.Add(tech); systemB.Worlds.Add(dao); u.Systems.AddRange([systemA, systemB]); var events = new SimulationEngine().Advance(u, 10_000);
        Assert.NotEmpty(u.WorldConnections); Assert.Contains(events, e => e.Type == "world-contact.first-contact"); Assert.Contains(events, e => e.Type == "knowledge.cross-world-transfer"); Assert.True(tech.Innovations.Count + dao.Innovations.Count > 0); Assert.Contains(events, e => e.Type == "innovation.hybrid-breakthrough");
    }
    [Fact] public void New_event_narratives_are_persisted_in_Thai_while_type_ids_remain_stable()
    {
        var u = new UniverseState { Parameters = new(209, InitialStarSystems: 1) }; var events = new SimulationEngine().Advance(u, 1_000);
        var genesis = Assert.Single(events, e => e.Type == "genesis.star-system");
        Assert.Contains(genesis.Summary, character => character is >= '\u0E00' and <= '\u0E7F'); Assert.Equal("genesis.star-system", genesis.Type);
    }
}
