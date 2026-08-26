using UniversalSimulation.Domain;

namespace UniversalSimulation.Engine;

public sealed partial class SimulationEngine
{
    public IReadOnlyList<SimEvent> Advance(UniverseState u, int steps = 1)
    {
        var events = new List<SimEvent>(); var random = new Random(HashCode.Combine(u.Parameters.Seed, u.Tick));
        for (var i = 0; i < Math.Clamp(steps, 1, 10_000); i++) { u.Tick++; u.Years += Math.Max(.01, u.Speed * u.Parameters.YearsPerTick); Genesis(u, random, events); EvolveWorlds(u, random, events); EvolveCultivators(u, random, events); EvolveSociety(u, random, events); CosmicRefinement(u, random, events); }
        return events;
    }
    private static void Genesis(UniverseState u, Random r, List<SimEvent> e) { if (u.Systems.Count >= u.Parameters.InitialStarSystems || !Chance(r, .05 * u.Parameters.GenesisRate)) return; var catalog = u.Systems.Count + 1; var s = new StarSystemState { Name = GenerateStarName(r, catalog), QiDensity = .2 + r.NextDouble() * .8, DistanceFromOriginLightYears = Math.Round(2 + r.NextDouble() * 1800, 2), LivingWorldCapacity = r.Next(1, Math.Clamp(u.Parameters.MaxLivingWorldsPerSystem, 1, 2) + 1) }; for (var i = 0; i < r.Next(2, 9); i++) { var w = new WorldState { Name = GenerateWorldName(r), Designation = $"USS-{u.Parameters.Seed:X}-{catalog:000}-{i + 1:00}", Orbit = i + 1, OrbitalRadiusAu = Math.Round(.25 * Math.Pow(1.65, i) + r.NextDouble() * .2, 2), QiDensity = s.QiDensity * (.5 + r.NextDouble()), Habitability = r.NextDouble(), Biome = new[] { "Oceanic", "Continental", "Desert", "Frozen", "Volcanic" }[r.Next(5)], SpiritualVeins = r.Next(0, 12) }; SeedGeography(w, r); s.Worlds.Add(w); } u.Systems.Add(s); e.Add(SimEvent.Create(u, "genesis.star-system", EventImportance.Major, $"{s.Name} ควบแน่นขึ้นจากทะเลปฐมกาล", $"ดาวเคราะห์ {s.Worlds.Count} ดวงก่อตัวขึ้น รองรับ World ที่มีชีวิตได้สูงสุด {s.LivingWorldCapacity} ดวง และอยู่ห่างจุดกำเนิด {s.DistanceFromOriginLightYears:N2} ปีแสง", location: s.Id)); }
    private static void EvolveWorlds(UniverseState u, Random r, List<SimEvent> e)
    {
        foreach (var s in u.Systems.Where(x => !x.Refined))
        {
            if (s.Stage == "Protostar" && Chance(r, .025)) { s.Stage = "Main Sequence"; e.Add(SimEvent.Create(u, "star.ignition", EventImportance.Major, $"{s.Name} จุดปฏิกิริยาและกลายเป็นดาวฤกษ์อย่างสมบูรณ์", location: s.Id)); }
            foreach (var w in s.Worlds.Where(x => !x.Refined))
            {
                if (w.Stage == "Molten" && Chance(r, .035)) { w.Stage = "Stable"; e.Add(SimEvent.Create(u, "world.stabilized", EventImportance.Notable, $"{w.Name} เย็นตัวและเข้าสู่สภาวะเสถียร", location: w.Id)); }
                else if (w.Stage == "Stable" && s.Worlds.Count(x => x.Living && !x.Refined) < Math.Clamp(s.LivingWorldCapacity, 1, 2) && Chance(r, .02 * u.Parameters.LifeChance * Math.Max(.15, w.Habitability))) { w.Stage = "Living"; w.Living = true; w.Population = 1_000; w.BirthsTotal = 1_000; e.Add(SimEvent.Create(u, "life.genesis", EventImportance.Historic, $"สิ่งมีชีวิตถือกำเนิดขึ้นบน {w.Name}", "ระบบชีวภาพชุดแรกเริ่มปรับตัวกับสภาพแวดล้อมของดาวเคราะห์", location: w.Id)); }
                else if (w.Stage == "Living" && w.Civilization == "None" && Chance(r, .018)) { w.Civilization = "Tribal"; w.CivilizationData = new() { PrimaryPath = "Emergent", Stage = "Tribal", Level = 1 }; w.CivilizationData.Capabilities["Production"] = .05; w.Population = 100_000; w.BirthsTotal = Math.Max(w.BirthsTotal, 100_000); e.Add(SimEvent.Create(u, "civilization.birth", EventImportance.Historic, $"ชนเผ่ารุ่นแรกถือกำเนิดขึ้นบน {w.Name}", "ภาษา เครื่องมือ และความร่วมมือเริ่มก่อรูปเป็นอารยธรรม", location: w.Id)); }
                else if (w.Civilization == "Tribal")
                {
                    var cultivationChance = .012 * u.Parameters.CultivationAffinity; var technologyChance = .011 * u.Parameters.TechnologyAffinity;
                    if (Chance(r, cultivationChance)) { w.Civilization = "Cultivation Sects"; w.CivilizationData = new() { PrimaryPath = "Cultivation", Stage = "Sect Age", Level = 2 }; w.CivilizationData.Capabilities["Cultivation"] = .22; w.CivilizationData.Capabilities["LawManipulation"] = .05; w.Population *= 10; w.BirthsTotal = Math.Max(w.BirthsTotal, w.Population); FoundFirstSects(w, r); e.Add(SimEvent.Create(u, "cultivation.discovery", EventImportance.Cosmic, $"มีการค้นพบวิถี Dao บน {w.Name}", $"สำนักผู้ก่อตั้ง {w.Sects.Count} แห่งเข้าครอบครองเส้นชีพจรวิญญาณและเริ่มถ่ายทอดวิชาบำเพ็ญ", location: w.Id)); }
                    else if (Chance(r, technologyChance)) { w.Civilization = "Technological City-States"; w.CivilizationData = new() { PrimaryPath = "Technology", Stage = "Scientific Awakening", Level = 2 }; w.CivilizationData.Capabilities["Energy"] = .14; w.CivilizationData.Capabilities["Production"] = .18; w.CivilizationData.Capabilities["Communication"] = .08; w.Knowledge.Add(new(Guid.NewGuid(), "Experimental Method", "Science", 1, "นักปรัชญาธรรมชาติ", u.Tick)); w.Population *= 8; w.BirthsTotal = Math.Max(w.BirthsTotal, w.Population); e.Add(SimEvent.Create(u, "technology.discovery", EventImportance.Historic, $"การค้นคว้าทางวิทยาศาสตร์เปลี่ยนแปลง {w.Name}", "การทดลองที่ทำซ้ำได้ การผลิตเชิงกล และสถาบันความรู้สาธารณะก่อให้เกิดนครรัฐเทคโนโลยีรุ่นแรก", location: w.Id)); }
                }
                if (w.Population > 0) AdvancePopulationCycle(w, r);
                foreach (var sect in w.Sects) { sect.Members = (int)Math.Min(2_000_000, sect.Members * (1 + r.NextDouble() * .002)); sect.Influence = Math.Min(100, sect.Influence + (Chance(r, .002) ? 1 : 0)); }
            }
        }
    }
    private static void EvolveCultivators(UniverseState u, Random r, List<SimEvent> e)
    { foreach (var w in u.Systems.SelectMany(s => s.Worlds).Where(w => w.Civilization.Contains("Cultivation") || w.Civilization == "Immortal Dynasty").Where(w => !w.Refined)) { if (u.Persons.Count(p => p.WorldId == w.Id && p.Alive) < 5 && Chance(r, .01)) { var lineage = w.Sects.Count > 0 ? w.Sects[r.Next(w.Sects.Count)].Name : "ผู้พเนจรไร้สังกัด"; var history = new List<CultivationMilestone> { new(u.Tick, u.Years, "Qi Gathering", "รากวิญญาณตื่นขึ้นและเริ่มรับรู้ Qi") }; var luck = .35 + r.NextDouble() * .64; var favor = FavorReason(r, w, luck); var artifacts = InitialArtifacts(r, luck); var p = new ImportantPerson(Guid.NewGuid(), Name(r), "Qi Gathering", w.Id, u.Tick, true, lineage, history, luck, favor, artifacts, [], [], InitialSkills(w, lineage)); u.Persons.Add(p); e.Add(SimEvent.Create(u, "person.emergence", EventImportance.Major, $"{p.Name} ผู้บำเพ็ญที่ได้รับความโปรดปรานจากสวรรค์ปรากฏตัว", $"{favor}; โชคชะตา: {luck:P0}; สายสืบทอด: {lineage}; ทักษะ: {string.Join(", ", p.Skills ?? [])}", actor: p.Id, location: w.Id)); } foreach (var p in u.Persons.Where(p => p.WorldId == w.Id && p.Alive).ToArray()) { var artifactBonus = (p.Artifacts ?? []).Sum(a => a.BreakthroughBonus); var next = NextRealm(p.Realm); if (RealmRank(next) > RealmRank(w.MaxCultivationRealm)) { if (Chance(r, .00015)) e.Add(SimEvent.Create(u, "cultivator.world-bottleneck", EventImportance.Major, $"{p.Name} สัมผัสเพดานการบำเพ็ญของ {w.Name}", $"World นี้รองรับได้สูงสุดเพียง {w.MaxCultivationRealm} การก้าวต่อไปต้องรอให้ World ยกระดับเสียก่อน", p.Id, w.Id)); continue; } if (Chance(r, BreakthroughChance(p.Realm, p.Luck, artifactBonus)) && next != p.Realm) { var history = p.CultivationHistory ?? []; history.Add(new(u.Tick, u.Years, next, $"ฝ่าคอขวดของ {p.Realm} ด้วยโชคชะตา {p.Luck:P0} และแรงสนับสนุนจาก Artifact {artifactBonus:P1}")); u.Persons[u.Persons.IndexOf(p)] = p with { Realm = next, CultivationHistory = history }; var sect = w.Sects.FirstOrDefault(s => s.Name == p.Lineage); if (sect is not null && RealmRank(next) > RealmRank(sect.HighestRealm)) sect.HighestRealm = next; e.Add(SimEvent.Create(u, "cultivator.breakthrough", EventImportance.Major, $"{p.Name} ทะลวงเข้าสู่ระดับ {next}", $"ระดับเดิม: {p.Realm}; สายสืบทอด: {p.Lineage}; โอกาสสำเร็จต่อ tick: {BreakthroughChance(p.Realm, p.Luck, artifactBonus):P3}", actor: p.Id, location: w.Id)); } } } }
    private static void CosmicRefinement(UniverseState u, Random r, List<SimEvent> e)
    {
        var immortals = u.Persons.Where(p => p.Realm == "Immortal" && p.Alive).ToArray();
        if (immortals.Length == 0 || !Chance(r, .003 * u.Parameters.CosmicEntityRate)) return;
        var worlds = u.Systems.SelectMany(s => s.Worlds).Where(w => !w.Refined).ToArray();
        if (worlds.Length == 0) return;
        var actor = immortals[r.Next(immortals.Length)];
        var scaleRoll = r.NextDouble();
        var scale = scaleRoll < .45 ? "Mountain" : scaleRoll < .82 ? "World" : "Star";
        Guid targetId; string targetName; string nature; string purpose; List<string> preparation; CosmicMaterial material; string result; string aftermath; string nextAction;

        if (scale == "Mountain" && worlds.Where(w => w.Regions.Any(region => region.Devastation < .8)).ToArray() is { Length: > 0 } mountainWorlds)
        {
            var world = mountainWorlds[r.Next(mountainWorlds.Length)]; var availableRegions = world.Regions.Where(region => region.Devastation < .8).ToArray(); var region = availableRegions[r.Next(availableRegions.Length)];
            targetId = region.Id; targetName = region.Name; nature = $"{region.Kind} บน {world.Name}"; purpose = "สร้างสมบัติ Dao สำหรับตรึงภูมิประเทศ";
            preparation = ["อ่านโครงสร้างเส้นชีพจรปฐพีในพื้นที่", "อพยพชุมชนออกนอกขอบเขตค่ายกล", "ฝังธงค่ายกลตามจุดตัดเส้นพลังหลัก", "ชุบเตาหลอมด้วยเปลวเพลิงกฎปฐพี"];
            material = new CosmicMaterial("Earthheart Law Ingot", "Dao", Math.Max(1, region.QiDensity * 10_000), region.Name, false);
            region.Population = 0; region.Terrain = "Refinement Crater"; region.Devastation = Math.Max(region.Devastation, .8); region.Stability *= .45;
            result = "มวลภูเขาและรูปแบบกฎปฐพีควบแน่นเป็นแท่งวัตถุดิบที่เสถียร"; aftermath = "แอ่งแก้วและแม่น้ำ Qi ที่เปลี่ยนทิศเข้ามาแทนภูมิประเทศเดิม"; nextAction = "หลอมแท่งวัตถุดิบเป็นสมอตรึงกฎ เพื่อรักษาทวีปที่กำลังสูญเสียเสถียรภาพ";
        }
        else if (scale == "Star" && u.Systems.Where(s => !s.Refined).ToArray() is { Length: > 0 } systems)
        {
            var system = systems[r.Next(systems.Length)]; targetId = system.Id; targetName = system.Name; nature = $"ระบบดาวฤกษ์ในสภาวะ {system.Stage}"; purpose = "สร้างแก่นดาวฤกษ์สำหรับ Immortal Domain";
            preparation = ["วัดวัฏจักรฟิวชันและ Qi ของดาวฤกษ์", "เปิดเส้นทางอพยพออกนอก heliopause", "ตรึงโซ่ห้วงว่างสิบสองเส้น", "คุ้มครองกระแสกรรมของระบบดาว"];
            material = new CosmicMaterial("Stellar Dao Core", "Primordial", Math.Max(1, system.QiDensity * 1e12), system.Name, system.Worlds.Any(w => w.Living));
            system.Refined = true; foreach (var world in system.Worlds) { world.Refined = true; if (world.Living) { world.DeathsTotal += world.Population; world.Population = 0; world.Living = false; } }
            result = "ดาวฤกษ์ศูนย์กลางและสสารในวงโคจรกลายเป็น Dao Core ที่หล่อเลี้ยงตนเองได้"; aftermath = "แอ่งห้วงว่างเย็นเยียบเข้ามาแทนระบบดาว ผู้รอดชีวิตต้องพึ่งเส้นทางอพยพ"; nextAction = "ติดตั้งแก่นพลังใน Immortal Domain และชุบหลอมต่อภายในพายุห้วงว่าง";
        }
        else
        {
            scale = "World"; var world = worlds[r.Next(worlds.Length)]; targetId = world.Id; targetName = world.Name; nature = world.Living ? "World ที่ยังมีชีวิต" : "World ตายหรือรกร้าง"; purpose = world.Living ? "เก็บรักษาวัฏจักรกรรมสมบูรณ์ไว้ภายใน World Pearl" : "ซ่อมแซม Artifact อมตะโบราณ";
            preparation = world.Living ? ["ตรวจสอบบัญชีกรรมของ World", "เจรจากับสำนักผู้ปกครอง", "เตรียมนาวาวิญญาณและประตูอพยพ", "ปิดผนึกขอบเขตดาวเคราะห์"] : ["ทำแผนที่แก่นดาวเคราะห์ที่แตกร้าว", "ชำระวิญญาณเร่ร่อน", "ติดตั้งสมอยึดแรงโน้มถ่วง", "ปลุกเปลวเพลิงดาวดับ"];
            material = new CosmicMaterial(world.Living ? "Karmic World Pearl" : "Stellar Earth Essence", world.Living ? "Forbidden" : "Immortal", Math.Max(1, world.QiDensity * 1_000_000), world.Name, world.Living);
            world.Refined = true; if (world.Living) { world.DeathsTotal += world.Population; world.Population = 0; world.Living = false; }
            result = material.FromLivingWorld ? "กฎของ World และวัฏจักรกรรมควบแน่นเป็นไข่มุกที่ดำรงตนเองได้" : "แก่นดาวเคราะห์ให้แก่นปฐพีดาราที่ผ่านการชำระแล้ว"; aftermath = material.FromLivingWorld ? "ทวีปต่าง ๆ หายไป ขณะที่วิญญาณซึ่งได้รับการช่วยเหลือเข้าสู่วัฏจักรภายในไข่มุก" : "วงโคจรเดิมเหลือเพียงวงแหวนสสารเฉื่อย"; nextAction = material.FromLivingWorld ? "ปลุกอารยธรรมที่ช่วยเหลือไว้ภายใน World Pearl" : "ใช้แก่นพลังฟื้นฟูอาวุธอมตะโบราณ";
        }

        var record = new CosmicRefinementRecord(Guid.NewGuid(), scale, actor.Id, actor.Name, targetId, targetName, nature, purpose, preparation, material, result, aftermath, nextAction, u.Tick, u.Years);
        u.Refinements.Add(record);
        var prepared = SimEvent.Create(u, "cosmic.refinement.preparation", EventImportance.Historic, $"{actor.Name} เตรียมการหลอม {targetName}", string.Join(" → ", preparation), actor.Id, targetId);
        e.Add(prepared);
        var completed = SimEvent.Create(u, "cosmic.refinement.completed", EventImportance.Cosmic, $"{actor.Name} หลอม {targetName} เป็น {material.Name}", $"จุดประสงค์: {purpose} ผลลัพธ์: {result}", actor.Id, targetId, prepared.Id);
        e.Add(completed);
        e.Add(SimEvent.Create(u, "cosmic.refinement.aftermath", EventImportance.Historic, $"ผลกระทบหลังการหลอม {targetName}", $"{aftermath} ขั้นต่อไป: {nextAction}", actor.Id, targetId, completed.Id));
    }
    private static bool Chance(Random r, double p) => r.NextDouble() < p;
    private static string Name(Random r) => new[] { "Li Silent-River", "Mei Star-Seeker", "Jian Voidstep", "Xue Moonblade", "Tao Ashborn" }[r.Next(5)] + $" {r.Next(10, 99)}";
    private static string NextRealm(string x) => x switch { "Qi Gathering" => "Foundation", "Foundation" => "Golden Core", "Golden Core" => "Nascent Soul", "Nascent Soul" => "Dao Lord", "Dao Lord" => "Immortal", _ => x };
    private static void FoundFirstSects(WorldState w, Random r) { var roots = new[] { "Jade", "Heaven", "Crimson", "Quiet", "Azure", "Iron", "Moon", "Void", "Verdant", "Thunder" }; var forms = new[] { "River Sect", "Pavilion", "Furnace Hall", "Sword Valley", "Cloud Monastery", "Star Palace", "Mountain School" }; var paths = new[] { "Orthodox", "Sword Dao", "Alchemy", "Body Cultivation", "Formation", "Soul Dao", "Talisman" }; var count = r.Next(2, 5); for (var i = 0; i < count; i++) { string name; do { name = $"{roots[r.Next(roots.Length)]} {forms[r.Next(forms.Length)]} of {w.Name}"; } while (w.Sects.Any(s => s.Name == name)); w.Sects.Add(new() { Name = name, Path = paths[r.Next(paths.Length)], Leader = Name(r), Members = r.Next(300, 5000), Influence = r.Next(15, 55) }); } }
    public static double BreakthroughChance(string realm, double luck, double artifactBonus = 0)
    {
        var baseChance = realm switch { "Qi Gathering" => .008, "Foundation" => .0035, "Golden Core" => .0014, "Nascent Soul" => .00045, "Dao Lord" => .00008, _ => 0 };
        return Math.Min(baseChance * 2, baseChance * (.65 + Math.Clamp(luck, 0, 1) * .7) + Math.Clamp(artifactBonus, 0, .002));
    }
    private static string FavorReason(Random r, WorldState w, double luck) => new[] { $"ถือกำเนิดใต้ดวงดาวชะตาที่บรรจบเหนือ {w.Name}", $"ครอบครองรากวิญญาณหายากที่สอดคล้องกับเส้นชีพจรของ World ทั้ง {w.SpiritualVeins} เส้น", "รอดชีวิตจากภัยพิบัติที่ควรตัดเส้นชะตาของมนุษย์", "มีกรรมที่ยังไม่คลี่คลายร่วมกับ Immortal โบราณ", luck > .8 ? "World Will เลือกวิญญาณนี้เป็นผู้สมัครแห่งชะตา" : "คำสัตย์ของบรรพชนดึงดูดความสนใจจากสวรรค์" }[r.Next(5)];
    private static List<ArtifactState> InitialArtifacts(Random r, double luck) { var result = new List<ArtifactState>(); if (r.NextDouble() > luck * .65) return result; var items = new[] { ("Cloudstep Jade", "Earth", "Movement", "ช่วยหลบหนีจากการไล่ล่าที่อาจถึงชีวิตได้หนึ่งครั้ง", .00005), ("Nine-Petal Furnace", "Heaven", "Alchemy", "เพิ่มความบริสุทธิ์ของโอสถ", .00015), ("Broken Dao Mirror", "Ancient", "Insight", "เปิดเผยข้อบกพร่องในวิชาบำเพ็ญ", .0003) }; var x = items[r.Next(items.Length)]; result.Add(new(Guid.NewGuid(), x.Item1, x.Item2, x.Item3, x.Item4, x.Item5)); return result; }
    private static void AdvancePopulationCycle(WorldState w, Random r)
    {
        var births = Math.Max(1, (long)(w.Population * (.0015 + r.NextDouble() * .0025)));
        var mortality = w.Civilization == "Cultivation Sects" ? .00045 : .0009;
        var deaths = Math.Min(w.Population, Math.Max(0, (long)(w.Population * (mortality + r.NextDouble() * .0005))));
        var enterSamsara = (long)(deaths * (.72 + r.NextDouble() * .2)); var wandering = deaths - enterSamsara;
        w.SoulsInSamsara += enterSamsara; w.WanderingDead += wandering; w.DeathsTotal += deaths; w.BirthsTotal += births;
        var reincarnated = Math.Min(w.SoulsInSamsara, (long)(births * (.2 + r.NextDouble() * .55)));
        w.SoulsInSamsara -= reincarnated; w.ReincarnatedTotal += reincarnated;
        w.Population = (int)Math.Clamp((long)w.Population + births - deaths, 0, int.MaxValue);
        if (w.Regions.Count > 0) { var totalWeight = w.Regions.Sum(x => Math.Max(.05, 1 - x.Devastation)); foreach (var region in w.Regions) region.Population = (long)(w.Population * (Math.Max(.05, 1 - region.Devastation) / totalWeight)); }
    }
    private static string GenerateStarName(Random r, int catalog) => new[] { "Aurelia", "Vesper", "Tianlu", "Naraka", "Eidolon", "Shenhai", "Orison", "Caelum" }[r.Next(8)] + $" {catalog}";
    private static string GenerateWorldName(Random r) => new[] { "Azure Hollow", "Canglan Realm", "Emberwake", "Moonfall", "Verdant Crown", "Nine Rivers", "Frostveil", "Sunken Jade", "Ashen Meridian", "Starbloom" }[r.Next(10)] + $" {r.Next(10, 99)}";
    private static void SeedGeography(WorldState w, Random r) { var prefixes = new[] { "Jade", "Storm", "Silent", "Crimson", "Silver", "Ancient", "Azure", "Golden" }; var forms = new[] { "Reach", "Continent", "Wilds", "Basin", "Archipelago", "Steppe" }; var terrains = new[] { "Mountain Range", "Riverlands", "Ancient Forest", "Spirit Desert", "Volcanic Basin", "Coastal Plains" }; for (var i = 0; i < r.Next(2, 6); i++) { var name = $"{prefixes[r.Next(prefixes.Length)]} {forms[r.Next(forms.Length)]}"; w.Regions.Add(new() { Name = name, Kind = forms[r.Next(forms.Length)], Terrain = terrains[r.Next(terrains.Length)], QiDensity = w.QiDensity * (.6 + r.NextDouble() * .8), Stability = .65 + r.NextDouble() * .35 }); } var materials = new[] { ("Spirit Iron", "Metal"), ("Moon Jade", "Yin"), ("Sunfire Crystal", "Fire"), ("Void Sand", "Space") }; foreach (var m in materials.OrderBy(_ => r.Next()).Take(r.Next(1, 4))) w.Materials.Add(new(Guid.NewGuid(), m.Item1, r.NextDouble() > .85 ? "Heaven" : "Earth", .1 + r.NextDouble() * .8, w.Regions[r.Next(w.Regions.Count)].Name, m.Item2)); }
    private static List<string> InitialSkills(WorldState w, string lineage) { var path = w.Sects.FirstOrDefault(s => s.Name == lineage)?.Path ?? "Wandering"; return path switch { "Sword Dao" => ["Sword Intent", "Cloudstep"], "Alchemy" => ["Spirit Herb Appraisal", "Pill Refining"], "Formation" => ["Formation Geometry", "Qi Threading"], "Body Cultivation" => ["Iron Skin", "Blood Furnace"], "Soul Dao" => ["Soul Sense", "Dream Walking"], _ => ["Qi Circulation", "Meditation"] }; }
}
