using UniversalSimulation.Domain;

namespace UniversalSimulation.Engine;

public sealed partial class SimulationEngine
{
    private static void EvolveSociety(UniverseState u, Random r, List<SimEvent> events)
    {
        foreach (var world in u.Systems.SelectMany(s => s.Worlds).Where(w => w.Living && !w.Refined))
        {
            EnsureCivilizationState(world);
            DiscoverKnowledge(u, world, r, events);
            EvolveCultivationAttempts(u, world, r, events);
            DiscoverFortune(u, world, r, events);
            EvolveRelationships(u, world, r, events);
            ResolvePersonalCombat(u, world, r, events);
            EvolveSectConflict(u, world, r, events);
            CraftArtifacts(u, world, r, events);
            TransformTerrainByLaw(u, world, r, events);
            EvolveWorldTier(u, world, r, events);
        }
        EvolveCrossWorldRelationships(u, r, events);
        EvolveCrossWorldCivilization(u, r, events);
    }

    private static void EvolveCultivationAttempts(UniverseState u, WorldState w, Random r, List<SimEvent> events)
    {
        var candidate = u.Persons.Where(p => p.WorldId == w.Id && p.Alive && RealmRank(p.Realm) < RealmRank(w.MaxCultivationRealm)).OrderBy(_ => r.Next()).FirstOrDefault();
        if (candidate is null || !Chance(r, .0012)) return;
        var next = candidate.Realm switch { "Qi Gathering" => "Foundation", "Foundation" => "Golden Core", "Golden Core" => "Nascent Soul", "Nascent Soul" => "Dao Lord", "Dao Lord" => "Immortal", _ => candidate.Realm };
        if (next == candidate.Realm) return;
        var pill = w.Knowledge.Any(k => k.Domain is "Medicine" or "Crafting") && Chance(r, .45);
        var baseSuccess = candidate.Realm switch { "Qi Gathering" => .62, "Foundation" => .42, "Golden Core" => .24, "Nascent Soul" => .11, "Dao Lord" => .04, _ => 0 };
        var success = Chance(r, Math.Min(.92, baseSuccess + candidate.Luck * .18 + (pill ? .22 : 0)));
        var history = candidate.CultivationHistory ?? [];
        if (!success)
        {
            history.Add(new(u.Tick, u.Years, candidate.Realm, $"ทะลวงสู่ {next} ล้มเหลว{(pill ? " แม้ใช้ Meridian Opening Pill ช่วย" : " หลังเผชิญคอขวดตามธรรมชาติ")}"));
            u.Persons[u.Persons.IndexOf(candidate)] = candidate with { CultivationHistory = history };
            events.Add(SimEvent.Create(u, "cultivator.breakthrough-failed", RealmRank(candidate.Realm) >= 4 ? EventImportance.Major : EventImportance.Notable, $"{candidate.Name} ทะลวงสู่ {next} ล้มเหลว", $"วิธีการ: {(pill ? "ใช้ Meridian Opening Pill และปิดด่านบำเพ็ญ" : "สะสมพลังตามธรรมชาติ")} ความล้มเหลวทำให้การไหลเวียน Qi บาดเจ็บ แต่ไม่ถึงแก่ชีวิต", candidate.Id, w.Id));
            return;
        }
        history.Add(new(u.Tick, u.Years, next, $"ทะลวงระดับสำเร็จ{(pill ? "โดยมี Meridian Opening Pill ช่วยประคองเส้นลมปราณ" : "จากความเข้าใจของตนเอง")}"));
        u.Persons[u.Persons.IndexOf(candidate)] = candidate with { Realm = next, CultivationHistory = history };
        events.Add(SimEvent.Create(u, pill ? "cultivator.pill-assisted-breakthrough" : "cultivator.breakthrough", RealmRank(next) >= 4 ? EventImportance.Historic : EventImportance.Major, $"{candidate.Name} ทะลวงเข้าสู่ระดับ {next}", pill ? "Meridian Opening Pill ช่วยทำให้เส้นลมปราณมั่นคงและเพิ่มโอกาสสำเร็จ" : "การทะลวงสำเร็จจากความเข้าใจที่สะสมมาโดยไม่พึ่งโอสถภายนอก", candidate.Id, w.Id));
    }

    private static void DiscoverFortune(UniverseState u, WorldState w, Random r, List<SimEvent> events)
    {
        var person = u.Persons.Where(p => p.WorldId == w.Id && p.Alive && (p.Artifacts?.Count ?? 0) < 5).OrderByDescending(p => p.Luck + r.NextDouble() * .4).FirstOrDefault();
        if (person is null || !Chance(r, .00075 * (.5 + person.Luck))) return;
        var finds = new[] { ("Broken Immortal Compass", "Ancient", "Fortune", "ชี้ตำแหน่งแดนลับที่มีโครงสร้างมิติไม่เสถียร", .0002), ("Nameless Bronze Sword", "Ancient", "Sword", "เก็บเจตจำนงกระบี่ที่ยังหลับใหลอยู่", .00015), ("Samsara Bone Seal", "Forbidden", "Soul", "รักษาเศษเสี้ยวความทรงจำไว้ข้ามการเกิดใหม่", .00025), ("Worldroot Seed", "Heaven", "Life", "ฟื้นฟูเส้นชีพจรวิญญาณที่เสียหาย", .00018) };
        var find = finds[r.Next(finds.Length)]; var artifacts = person.Artifacts ?? []; if (artifacts.Any(a => a.Name == find.Item1)) return;
        var artifact = new ArtifactState(Guid.NewGuid(), find.Item1, find.Item2, find.Item3, find.Item4, find.Item5); artifacts.Add(artifact); u.Persons[u.Persons.IndexOf(person)] = person with { Artifacts = artifacts };
        var region = w.Regions.OrderBy(_ => r.Next()).FirstOrDefault();
        events.Add(SimEvent.Create(u, "fortune.ancient-artifact", EventImportance.Historic, $"{person.Name} พบมหาวาสนาและได้รับ {artifact.Name}", $"ค้นพบ Artifact ที่ {region?.Name ?? w.Name} ระดับ: {artifact.Grade} ความสามารถ: {artifact.Ability}", person.Id, region?.Id ?? w.Id));
    }

    private static void EvolveCrossWorldRelationships(UniverseState u, Random r, List<SimEvent> events)
    {
        var people = u.Persons.Where(p => p.Alive && RealmRank(p.Realm) >= 3).ToArray(); if (people.Length < 2 || !Chance(r, .001 * u.Parameters.RelationshipRate)) return;
        var a = people[r.Next(people.Length)]; var b = people.Where(p => p.WorldId != a.WorldId).OrderBy(_ => r.Next()).FirstOrDefault(); if (b is null || (a.Relationships ?? []).Any(x => x.PersonId == b.Id)) return;
        var type = new[] { "พันธมิตรข้าม World", "คู่แข่งข้าม World", "คู่บำเพ็ญ Dao", "คู่ค้าต่าง World", "ศัตรูคู่แค้น" }[r.Next(5)]; var strength = .3 + r.NextDouble() * .68;
        var ar = a.Relationships ?? []; var br = b.Relationships ?? []; ar.Add(new(b.Id, b.Name, type, strength, u.Tick)); br.Add(new(a.Id, a.Name, type, strength, u.Tick)); u.Persons[u.Persons.IndexOf(a)] = a with { Relationships = ar }; u.Persons[u.Persons.IndexOf(b)] = b with { Relationships = br };
        var aw = u.Systems.SelectMany(s => s.Worlds).First(w => w.Id == a.WorldId); var bw = u.Systems.SelectMany(s => s.Worlds).First(w => w.Id == b.WorldId);
        events.Add(SimEvent.Create(u, "person.cross-world-relationship", EventImportance.Major, $"{a.Name} แห่ง {aw.Name} และ {b.Name} แห่ง {bw.Name} กลายเป็น{type}", "ทั้งสองพบกันผ่านรอยแยก Realm งานประมูลข้าม World การจาริก หรือแดนลับร่วมกัน ความสัมพันธ์นี้เชื่อมประวัติศาสตร์ของสองโลกเข้าด้วยกัน", a.Id, aw.Id));
    }

    private static void DiscoverKnowledge(UniverseState u, WorldState w, Random r, List<SimEvent> events)
    {
        if (w.Civilization == "None" || !Chance(r, .003 * u.Parameters.KnowledgeRate)) return;
        var candidates = KnowledgeCandidates(w);
        var item = candidates.Where(x => w.Knowledge.All(k => k.Name != x.Item1)).OrderBy(_ => r.Next()).FirstOrDefault(); if (item == default) return;
        var discoverer = w.Sects.OrderBy(_ => r.Next()).FirstOrDefault()?.Name ?? "independent scholars";
        if (w.CivilizationData.PrimaryPath == "Technology") discoverer = new[] { "Royal Academy", "Open Science Assembly", "Orbital Research Union", "independent engineers" }[r.Next(4)];
        w.Knowledge.Add(new(Guid.NewGuid(), item.Item1, item.Item2, 1, discoverer, u.Tick)); w.DevelopmentLevel++; ApplyKnowledgeCapability(w, item.Item2, .08);
        events.Add(SimEvent.Create(u, "knowledge.discovery", EventImportance.Notable, $"มีการค้นพบ {item.Item1} บน {w.Name}", $"{discoverer} ขยายขอบเขตความเป็นไปได้ที่อารยธรรมของโลกรับรู้", location: w.Id));
        if (w.DevelopmentLevel >= 3 && w.Civilization == "Cultivation Sects") { w.Civilization = "Cultivation Kingdoms"; events.Add(SimEvent.Create(u, "civilization.evolution", EventImportance.Historic, $"อาณาจักรผู้บำเพ็ญรวมตัวกันทั่ว {w.Name}", "องค์ความรู้ ระบบขนส่ง และการปกครองของสำนักก่อให้เกิดรัฐผู้บำเพ็ญที่มั่นคงเป็นครั้งแรก", location: w.Id)); }
        else if (w.DevelopmentLevel >= 6 && w.WorldTier != "Mortal World" && w.Civilization == "Cultivation Kingdoms") { w.Civilization = "Immortal Dynasty"; events.Add(SimEvent.Create(u, "civilization.evolution", EventImportance.Cosmic, $"ราชวงศ์อมตะถือกำเนิดขึ้นบน {w.Name}", "โครงสร้างค่ายกล ความรู้ด้านกฎ และระดับของ World ทำให้อารยธรรมชั้นสูงครอบคลุมทั่วดาวเคราะห์", location: w.Id)); }
        else if (w.DevelopmentLevel >= 3 && w.Civilization == "Technological City-States") { w.Civilization = "Industrial Technocracy"; w.CivilizationData.Stage = "Industrial-Scientific Age"; w.CivilizationData.Level = 3; events.Add(SimEvent.Create(u, "civilization.evolution", EventImportance.Historic, $"ระบอบเทคโนแครตอุตสาหกรรมเชื่อมโยงทั่ว {w.Name}", "เครือข่ายพลังงาน การผลิต และการสื่อสารรวมเหล่านครรัฐเป็นเศรษฐกิจวิจัยระดับดาวเคราะห์", location: w.Id)); }
        else if (w.DevelopmentLevel >= 6 && w.Civilization == "Industrial Technocracy") { w.Civilization = "Interplanetary Commonwealth"; w.CivilizationData.Stage = "Spacefaring Age"; w.CivilizationData.Level = 4; RaiseCapability(w, "Spaceflight", .28); RaiseCapability(w, "Communication", .18); events.Add(SimEvent.Create(u, "civilization.evolution", EventImportance.Historic, $"{w.Name} ก้าวเข้าสู่ยุคเดินทางอวกาศ", "อุตสาหกรรมวงโคจร พลังงานฟิวชัน และระบบคำนวณอัตโนมัติเปิดทางสู่อาณานิคมทั่วระบบดาว", location: w.Id)); }
    }

    private static void EvolveRelationships(UniverseState u, WorldState w, Random r, List<SimEvent> events)
    {
        var people = u.Persons.Where(p => p.WorldId == w.Id && p.Alive).ToArray(); if (people.Length < 2 || !Chance(r, .0025 * u.Parameters.RelationshipRate)) return;
        var a = people[r.Next(people.Length)]; var b = people.Where(x => x.Id != a.Id).OrderBy(_ => r.Next()).First();
        if ((a.Relationships ?? []).Any(x => x.PersonId == b.Id)) return;
        var type = new[] { "สหายร่วมสาบาน", "คู่แข่ง", "คู่บำเพ็ญ Dao", "ศิษย์อาจารย์", "ศัตรูคู่แค้น" }[r.Next(5)]; var strength = .35 + r.NextDouble() * .6;
        var ar = a.Relationships ?? []; var br = b.Relationships ?? []; ar.Add(new(b.Id, b.Name, type, strength, u.Tick)); br.Add(new(a.Id, a.Name, type, strength, u.Tick));
        u.Persons[u.Persons.IndexOf(a)] = a with { Relationships = ar }; u.Persons[u.Persons.IndexOf(b)] = b with { Relationships = br };
        var summary = type == "คู่บำเพ็ญ Dao" ? $"{a.Name} และ {b.Name} ผูกพันหัวใจและวิถี Dao ร่วมกัน" : $"{a.Name} และ {b.Name} กลายเป็น{type}";
        events.Add(SimEvent.Create(u, "person.relationship", type is "คู่บำเพ็ญ Dao" or "ศัตรูคู่แค้น" ? EventImportance.Major : EventImportance.Notable, summary, $"ความสัมพันธ์เริ่มต้นขึ้นบน {w.Name} ด้วยความผูกพันระดับ {strength:P0}", a.Id, w.Id));
    }

    private static void ResolvePersonalCombat(UniverseState u, WorldState w, Random r, List<SimEvent> events)
    {
        var people = u.Persons.Where(p => p.WorldId == w.Id && p.Alive).ToArray(); if (people.Length < 2 || !Chance(r, .003 * u.Parameters.ConflictRate)) return;
        var a = people[r.Next(people.Length)]; var b = people.Where(x => x.Id != a.Id).OrderBy(_ => r.Next()).First(); var high = RealmRank(a.Realm) >= 4 || RealmRank(b.Realm) >= 4;
        var winner = (RealmRank(a.Realm) + a.Luck + (a.Skills?.Count ?? 0) * .08 + r.NextDouble()) >= (RealmRank(b.Realm) + b.Luck + (b.Skills?.Count ?? 0) * .08 + r.NextDouble()) ? a : b; var loser = winner.Id == a.Id ? b : a; var lethal = Chance(r, high ? .12 : .035);
        if (lethal) u.Persons[u.Persons.IndexOf(loser)] = loser with { Alive = false };
        var cause = (a.Relationships ?? []).FirstOrDefault(x => x.PersonId == b.Id)?.Type ?? "การแย่งชิงวาสนาและโอกาส";
        events.Add(SimEvent.Create(u, "person.combat", lethal || high ? EventImportance.Historic : EventImportance.Major, $"{winner.Name} เอาชนะ {loser.Name}", $"สาเหตุ: {cause} ระดับการต่อสู้: {(high ? "สั่นสะเทือนกฎของพื้นที่" : "การประลองเฉพาะถิ่น")} ผลลัพธ์: {(lethal ? "เสียชีวิตและเข้าสู่เส้นทาง Samsara" : "พ่ายแพ้แต่ไม่เสียชีวิต")}", winner.Id, w.Id));
        if (lethal) { w.DeathsTotal++; w.SoulsInSamsara++; }
    }

    private static void EvolveSectConflict(UniverseState u, WorldState w, Random r, List<SimEvent> events)
    {
        var active = w.Conflicts.FirstOrDefault(c => c.Status == "Active");
        if (active is not null && Chance(r, .015)) { active.Status = "Ended"; events.Add(SimEvent.Create(u, "war.ended", EventImportance.Historic, $"{active.Name} สิ้นสุดลง", $"ยอดผู้เสียชีวิตและบาดเจ็บที่บันทึกได้: {active.Casualties:N0} สาเหตุของสงคราม: {active.Cause}", location: w.Id)); return; }
        if (active is not null) { var losses = (long)(w.Population * r.NextDouble() * .00015); active.Casualties += losses; w.Population = (int)Math.Max(0, w.Population - losses); w.DeathsTotal += losses; w.SoulsInSamsara += (long)(losses * .8); w.WanderingDead += (long)(losses * .2); return; }
        if (w.Sects.Count < 2 || !Chance(r, .0018 * u.Parameters.ConflictRate * u.Parameters.WarRate)) return;
        var sides = w.Sects.OrderBy(_ => r.Next()).Take(2).ToArray(); var material = w.Materials.OrderByDescending(m => m.Abundance).FirstOrDefault(); var cause = material is null ? "ความขัดแย้งด้านคำสอนและวิถี Dao" : $"การแย่งชิงแหล่งแร่ {material.Name}";
        var conflict = new ConflictState { Name = $"สงครามระหว่าง {sides[0].Name} กับ {sides[1].Name}", Scale = "สงครามสำนัก", Attacker = sides[0].Name, Defender = sides[1].Name, Cause = cause, StartedAtTick = u.Tick }; w.Conflicts.Add(conflict);
        events.Add(SimEvent.Create(u, "war.sect", EventImportance.Historic, conflict.Name + " เริ่มต้นขึ้น", $"ชนวนเหตุ: {cause} ทั้งสองฝ่ายระดมผู้บำเพ็ญและกองทัพมนุษย์เข้าสู่แนวรบ", location: w.Id, cause: LatestCause(events, w.Id)));
    }

    private static void CraftArtifacts(UniverseState u, WorldState w, Random r, List<SimEvent> events)
    {
        if (!w.Knowledge.Any(k => k.Domain == "Crafting") || !Chance(r, .0018 * u.Parameters.CraftingRate)) return; var crafter = u.Persons.Where(p => p.WorldId == w.Id && p.Alive).OrderByDescending(p => p.Luck).FirstOrDefault(); if (crafter is null) return;
        var material = w.Materials.OrderByDescending(m => m.Abundance).FirstOrDefault(); if (material is null) return; var artifacts = crafter.Artifacts ?? [];
        var name = $"{new[] { "Heaven-Piercing", "Quiet Moon", "River-Sundering", "Starforged" }[r.Next(4)]} {new[] { "Sword", "Cauldron", "Seal", "Banner" }[r.Next(4)]}";
        var artifact = new ArtifactState(Guid.NewGuid(), name, material.Grade, "Crafted", $"นำพลังของ {material.LawAffinity} Law ผ่านวัตถุดิบ {material.Name}", material.Grade == "Heaven" ? .00025 : .00008); artifacts.Add(artifact); u.Persons[u.Persons.IndexOf(crafter)] = crafter with { Artifacts = artifacts };
        events.Add(SimEvent.Create(u, "artifact.forged", EventImportance.Major, $"{crafter.Name} หลอมสร้าง {name}", $"วัตถุดิบ: {material.Name}; องค์ความรู้: Artifact Forging; กฎที่สอดคล้อง: {material.LawAffinity}", crafter.Id, w.Id, cause: LatestCause(events, w.Id)));
    }

    private static void TransformTerrainByLaw(UniverseState u, WorldState w, Random r, List<SimEvent> events)
    {
        var actor = u.Persons.Where(p => p.WorldId == w.Id && p.Alive && RealmRank(p.Realm) >= 4).OrderBy(_ => r.Next()).FirstOrDefault(); if (actor is null || w.Regions.Count == 0 || !Chance(r, .0007)) return;
        var region = w.Regions[r.Next(w.Regions.Count)]; var law = (actor.Laws ?? []).FirstOrDefault()?.Law ?? new[] { "Sword", "Fire", "Water", "Space", "Life" }[r.Next(5)]; var before = region.Terrain;
        region.Terrain = law switch { "Fire" => "Glassfire Wastes", "Water" => "Boundless Inland Sea", "Sword" => "Ten Thousand Blade Ravines", "Space" => "Fractured Spatial Basin", "Life" => "Immortal Verdant Forest", _ => "Law-Scarred Expanse" }; region.Devastation = Math.Clamp(region.Devastation + (law == "Life" ? -.15 : .2), 0, 1); region.Stability = Math.Clamp(region.Stability - .12, 0, 1);
        var laws = actor.Laws ?? []; if (laws.All(x => x.Law != law)) laws.Add(new(law, 1, .2 + r.NextDouble() * .4)); u.Persons[u.Persons.IndexOf(actor)] = actor with { Laws = laws };
        events.Add(SimEvent.Create(u, "law.terrain-transformation", actor.Realm == "Immortal" ? EventImportance.Cosmic : EventImportance.Historic, $"{actor.Name} ใช้กฎเปลี่ยนภูมิประเทศของ {region.Name}", $"{before} เปลี่ยนเป็น {region.Terrain} ด้วย {law} Law เสถียรภาพของภูมิภาคลดลงเหลือ {region.Stability:P0}", actor.Id, region.Id));
    }

    private static void EvolveWorldTier(UniverseState u, WorldState w, Random r, List<SimEvent> events)
    {
        if (w.WorldTier == "Mortal World" && (w.Civilization.Contains("Cultivation") || w.Civilization == "Immortal Dynasty") && w.Knowledge.Count >= 3 && u.Persons.Any(p => p.WorldId == w.Id && p.Realm == "Golden Core" && p.Alive) && Chance(r, .001 * u.Parameters.WorldAscensionRate)) { var old = w.MaxCultivationRealm; w.WorldTier = "Spirit World"; w.MaxCultivationRealm = "Dao Lord"; w.QiDensity *= 1.25; w.SpiritualVeins += 5; events.Add(SimEvent.Create(u, "world.ascension", EventImportance.Cosmic, $"ฟ้าดินแปรเปลี่ยนครั้งใหญ่ — {w.Name} ยกระดับเป็น Spirit World", $"องค์ความรู้ที่สะสม แรงกดดันจาก Golden Core และ Qi ของโลกข้ามเกณฑ์ Ascension เพดานการบำเพ็ญเพิ่มจาก {old} เป็น {w.MaxCultivationRealm} พร้อมกำเนิดเส้นชีพจรวิญญาณใหญ่ห้าเส้น", location: w.Id, cause: LatestCause(events, w.Id))); }
        else if (w.WorldTier == "Spirit World" && u.Persons.Any(p => p.WorldId == w.Id && p.Realm == "Dao Lord" && p.Alive) && w.Knowledge.Count >= 6 && Chance(r, .00025 * u.Parameters.WorldAscensionRate)) { var old = w.MaxCultivationRealm; w.WorldTier = "Ascendant World"; w.MaxCultivationRealm = "Immortal"; w.QiDensity *= 1.5; w.SpiritualVeins += 12; events.Add(SimEvent.Create(u, "world.ascension", EventImportance.Cosmic, $"ขอบเขต World ยกระดับ — {w.Name} ก้าวเข้าสู่ Ascendant Tier", $"Dao Lord ตรึงกฎระดับสูงไว้ ขณะที่อารยธรรมพัฒนาองค์ความรู้ครบหกแขนง เพดานการบำเพ็ญเพิ่มจาก {old} เป็น {w.MaxCultivationRealm} เส้นชีพจรระดับ Heaven สิบสองเส้นปรากฏขึ้นและทุกภูมิภาครับรู้ถึงการเปลี่ยนแปลง", location: w.Id, cause: LatestCause(events, w.Id))); }
    }
    private static int RealmRank(string realm) => realm switch { "Qi Gathering" => 1, "Foundation" => 2, "Golden Core" => 3, "Nascent Soul" => 4, "Dao Lord" => 5, "Immortal" => 6, _ => 0 };
    private static Guid? LatestCause(List<SimEvent> events, Guid location) => events.LastOrDefault(e => e.LocationId == location)?.Id;
}
