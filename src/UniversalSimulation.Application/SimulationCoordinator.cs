using System.Collections.Concurrent;
using UniversalSimulation.Domain;
using UniversalSimulation.Engine;
namespace UniversalSimulation.Application;
public sealed class SimulationCoordinator(SimulationEngine engine, ISimulationStore store)
{
    private readonly ConcurrentDictionary<Guid, UniverseState> universes = new(); public IEnumerable<UniverseState> Universes => universes.Values.OrderBy(x => x.CreatedAt); public UniverseState? Find(Guid id) => universes.GetValueOrDefault(id);
    private readonly ConcurrentDictionary<Guid,SemaphoreSlim> gates=new(); private SemaphoreSlim Gate(Guid id)=>gates.GetOrAdd(id,_=>new(1,1));
    public async Task Initialize(CancellationToken ct) { await store.Initialize(ct); foreach (var u in await store.LoadAll(ct)) { universes[u.Id] = u; Gate(u.Id); } }
    public async Task<UniverseState> Create(string name, UniverseParameters p, CancellationToken ct) { var u = new UniverseState { Name = string.IsNullOrWhiteSpace(name) ? "จักรวาลไร้นาม" : name.Trim(), Parameters = p }; universes[u.Id] = u; Gate(u.Id); await store.Save(u, [SimEvent.Create(u, "universe.genesis", EventImportance.Cosmic, $"{u.Name} ถือกำเนิดขึ้นจากห้วงปฐมโกลาหล", $"World Seed: {p.Seed}")], ct); return u; }
    public async Task<UniverseState> CreateCosmicRefinementScenario(string? requestedScale, bool livingWorldTarget, CancellationToken ct)
    {
        var scale = requestedScale?.Trim().ToLowerInvariant() switch { "mountain" => "Mountain", "star" => "Star", _ => "World" };
        var u = new UniverseState { Name = $"Crucible: {scale} Refinement", Parameters = new(Seed: 9001, CosmicEntityRate: 1, InitialStarSystems: 1), Speed = 10, Status = SimulationStatus.Paused };
        var s = new StarSystemState { Name = "Twin Fate System", Stage = "Dying Giant", QiDensity = .95, DistanceFromOriginLightYears = 18_400, LivingWorldCapacity = 1 };
        var dead = new WorldState { Name = "Cinder Tomb", Designation="CRU-9001-01", Orbit=1, OrbitalRadiusAu=.62, Stage = "Dead", Biome="Ash Wastes", Habitability=.02, QiDensity = .88, SpiritualVeins=3, DeathsTotal=4_200_000_000, WanderingDead=380_000_000 };
        var living = new WorldState { Name = "Azure Mercy", Designation="CRU-9001-02", Orbit=2, OrbitalRadiusAu=1.14, Stage = "Living", WorldTier="Ascendant World", MaxCultivationRealm="Immortal", Biome="Continental", Habitability=.91, Living = true, Civilization = "Cultivation Sects", Population = 80_000_000, BirthsTotal=640_000_000, DeathsTotal=560_000_000, WanderingDead=12_000_000, SoulsInSamsara=48_000_000, ReincarnatedTotal=390_000_000, QiDensity = 1.2, SpiritualVeins=27 };
        living.CivilizationData = new() { PrimaryPath = "Cultivation", Stage = "Ascendant Sect Age", Level = 5 }; living.CivilizationData.Capabilities["Cultivation"] = .88; living.CivilizationData.Capabilities["LawManipulation"] = .72; living.CivilizationData.Capabilities["WorldTravel"] = .24;
        var mountain = new RegionState { Name = "Mount Ninefold Furnace", Kind = "Sacred Mountain", Terrain = "Volcanic Peaks", Population = 12_000, QiDensity = 3.8, Stability = .94, DominantSect = "Azure Mercy Palace" };
        living.Regions.Add(mountain); living.Sects.Add(new(){Name="Azure Mercy Palace",Path="Orthodox",Leader="Saintess Lian",Members=120_000,Influence=92,HighestRealm="Dao Lord"}); s.Worlds.AddRange([dead, living]); u.Systems.Add(s);
        var history=new List<CultivationMilestone>{new(0,0,"Qi Gathering","ปลุกรากวิญญาณเพลิงปฐมกาล"),new(120,1200,"Foundation","สืบทอด Solar Furnace Sutra"),new(900,9000,"Golden Core","หลอมแก่นสุริยันเก้าลวดลาย"),new(5000,50000,"Nascent Soul","รอดชีวิตจากทัณฑ์สวรรค์เพลิงม่วง"),new(30000,300000,"Dao Lord","บรรลุความเข้าใจ Dao แห่งการหลอม"),new(100000,1000000,"Immortal","ก้าวข้ามขอบเขตของ World")};
        var artifacts=new List<ArtifactState>{new(Guid.NewGuid(),"Ten Thousand Sun Crucible","Primordial","Refinement","สามารถหลอมภูเขา ดาวฤกษ์ และ World ทั้งดวงให้เป็นวัตถุดิบอมตะ",.001)};
        var actor = new ImportantPerson(Guid.NewGuid(), "Venerable Furnace Beyond Heaven", "Immortal", living.Id, 0, true, "Solar Furnace Lineage", history, .97, "ถือกำเนิดจากเปลวเพลิงแรกของจักรวาลและได้รับการยอมรับจาก Fire Dao", artifacts);
        u.Persons.Add(actor);

        Guid targetId; string targetName; string nature; string purpose; string materialName; string grade; double mass; List<string> preparation; string result; string aftermath; string nextAction;
        if (scale == "Mountain")
        {
            targetId = mountain.Id; targetName = mountain.Name; nature = "ภูเขาศักดิ์สิทธิ์ที่มีเขตสำนักและประชากรอาศัยอยู่"; purpose = "สร้าง Ninefold Mountain Seal เพื่อค้ำจุนทวีปที่กำลังพังทลาย"; materialName = "Ninefold Earthheart Ingot"; grade = "Dao"; mass = 84_000;
            preparation = ["สำรวจเส้นชีพจรปฐพีทั้งเก้าของภูเขาเป็นเวลา 81 วัน", "เจรจากับ Azure Mercy Palace และอพยพผู้อยู่อาศัย 12,000 คน", "วางธงค่ายกล 3,600 ต้นรอบเชิงเขา", "เติมผลึกกฎปฐพีให้ Ten Thousand Sun Crucible"];
            mountain.Terrain = "Ninefold Glass Crater"; mountain.Population = 0; mountain.Devastation = .86; mountain.Stability = .38;
            result = "ภูเขาศักดิ์สิทธิ์ยุบตัวเป็นแท่งวัตถุดิบอัดแน่นด้วยกฎ โดยไม่ทำลายทวีปโดยรอบ"; aftermath = "ตำแหน่งเดิมกลายเป็นแอ่งแก้ว และกระแส Qi เปลี่ยนทางเข้าสู่หุบเขาใหม่สามแห่ง"; nextAction = "ผู้หลอมจะสร้าง Ninefold Mountain Seal และฝังไว้ใต้แนวรอยเลื่อนของทวีป";
        }
        else if (scale == "Star")
        {
            targetId = s.Id; targetName = s.Name; nature = "ดาวฤกษ์ศูนย์กลางใกล้ดับและระบบวงโคจรโดยรอบ"; purpose = "สร้าง Solar Furnace Core สำหรับป้อมปราการอมตะเคลื่อนที่"; materialName = "Dying-Sun Dao Core"; grade = "Primordial"; mass = 9.7e11;
            preparation = ["คำนวณวัฏจักรฟิวชันสุดท้าย 12,000 รอบของดาวฤกษ์", "เปิดประตูอพยพจากวงโคจรดาวเคราะห์ทั้งสอง", "ตรึงโซ่ห้วงว่างสิบสองเส้นไว้นอก heliopause", "คุ้มครองกระแส Samsara ภายในมิติชั้นในของเตาหลอม"];
            s.Refined = true; foreach (var world in s.Worlds) { world.Refined = true; if (world.Living) { world.DeathsTotal += world.Population; world.Population = 0; world.Living = false; } }
            result = "ดาวฤกษ์ใกล้ดับและสสารในวงโคจรควบแน่นเป็น Dying-Sun Dao Core ที่เสถียร"; aftermath = "ระบบดาวกลายเป็นแอ่งห้วงว่างเย็นเยียบ ส่วนผู้รอดชีวิตอาศัยอยู่ในมิติชั้นในของเตาหลอม"; nextAction = "ผู้หลอมจะติดตั้งแก่นพลังในป้อมปราการเคลื่อนที่ แล้วค้นหาพายุห้วงว่างเพื่อชุบหลอมต่อ";
        }
        else
        {
            var target = livingWorldTarget ? living : dead; targetId = target.Id; targetName = target.Name; nature = livingWorldTarget ? "Ascendant World ที่ยังมีชีวิต" : "World ตายที่ยังมีแก่นปฐพีและพลังหยินตกค้าง"; purpose = livingWorldTarget ? "สร้าง Karmic World Pearl ที่รองรับวัฏจักร Samsara สมบูรณ์" : "สกัด Stellar Earth Essence เพื่อซ่อมอาวุธอมตะที่แตกหัก"; materialName = livingWorldTarget ? "Karmic World Pearl" : "Stellar Earth Essence"; grade = livingWorldTarget ? "Forbidden" : "Immortal"; mass = Math.Max(1, target.QiDensity * 1_000_000);
            preparation = livingWorldTarget ? ["ตรวจสอบบัญชีกรรมของโลกเป็นเวลาสามศตวรรษ", "เจรจากับเจ็ดสำนักใหญ่ให้ยอมรับการอพยพ", "สร้างนาวาวิญญาณสำหรับประชาชนและกระแส Samsara", "ปิดผนึกขอบเขตดาวเคราะห์ด้วย Thirty-Six Heaven Formation"] : ["ทำแผนที่แก่นดาวที่แตกร้าวของโลกตาย", "ชำระวิญญาณเร่ร่อน 380 ล้านดวงผ่านค่ายกล Samsara", "ติดตั้งสมอยึดแรงโน้มถ่วงแปดจุดตามแนวศูนย์สูตร", "ปลุกเตาหลอมด้วยเปลวเพลิงดาวดับ"];
            target.Refined = true; if (target.Living) { target.DeathsTotal += target.Population; target.Population = 0; target.Living = false; }
            result = livingWorldTarget ? "ขอบเขตโลก กฎ และวัฏจักรกรรมควบแน่นเป็น World Pearl ที่ดำรงตนเองได้" : "แก่นดาวเคราะห์ตายให้แก่นปฐพีบริสุทธิ์หลังขจัดความอาฆาตธาตุหยิน"; aftermath = livingWorldTarget ? "ผู้อยู่อาศัยส่วนใหญ่ถูกย้ายสู่นาวาวิญญาณ เมืองและทวีปที่ถูกทิ้งไว้สิ้นสภาพทางกายภาพ" : "วงโคจรเดิมเหลือเพียงวงฝุ่นเฉื่อยบาง ๆ และเส้นทางบริสุทธิ์เข้าสู่ Samsara"; nextAction = livingWorldTarget ? "ผู้หลอมจะเพาะ World Pearl ภายในป้อมปราการและปลุกอารยธรรมที่ช่วยเหลือไว้" : "ผู้หลอมจะใช้แก่นพลังซ่อมคมที่แตกหักของ Heaven-Cleaving Blade";
        }
        var material = new CosmicMaterial(materialName, grade, mass, targetName, scale == "World" && livingWorldTarget);
        var record = new CosmicRefinementRecord(Guid.NewGuid(), scale, actor.Id, actor.Name, targetId, targetName, nature, purpose, preparation, material, result, aftermath, nextAction, u.Tick, u.Years);
        u.Refinements.Add(record); universes[u.Id] = u;
        var prepared = SimEvent.Create(u, "cosmic.refinement.preparation", EventImportance.Historic, $"{actor.Name} เตรียมการหลอม {targetName}", string.Join(" → ", preparation), actor.Id, targetId);
        var begun = SimEvent.Create(u, "cosmic.refinement.ritual", EventImportance.Cosmic, $"พิธีหลอม {targetName} ระดับ {scale} เริ่มต้นขึ้น", $"จุดประสงค์: {purpose}", actor.Id, targetId, prepared.Id);
        var completed = SimEvent.Create(u, "cosmic.refinement.completed", EventImportance.Cosmic, $"{actor.Name} หลอม {targetName} เป็น {materialName}", $"{result} ระดับ: {grade}; มวล: {mass:N1}", actor.Id, targetId, begun.Id);
        var after = SimEvent.Create(u, "cosmic.refinement.aftermath", EventImportance.Historic, $"ผลกระทบหลังการหลอม {targetName}", $"{aftermath} ขั้นต่อไป: {nextAction}", actor.Id, targetId, completed.Id);
        await store.Save(u, [prepared, begun, completed, after], ct); return u;
    }
    public async Task<bool> Control(Guid id, SimulationStatus? status, double? speed, CancellationToken ct) { if (!universes.TryGetValue(id, out var u)) return false; lock (u) { if (status is not null) u.Status = status.Value; if (speed is not null) u.Speed = Math.Clamp(speed.Value, .01, 1000); } await store.Save(u, [], ct); return true; }
    public async Task<bool> Step(Guid id, int ticks, CancellationToken ct) { var gate=Gate(id); await gate.WaitAsync(ct); try { if (!universes.TryGetValue(id, out var u)) return false; var e=engine.Advance(u,ticks); await store.Save(u,e,ct); return true; } finally { gate.Release(); } }
    public async Task<bool> Delete(Guid id,CancellationToken ct) { var gate=Gate(id); await gate.WaitAsync(ct); try { if(!universes.TryRemove(id,out _)) return false; await store.Delete(id,ct); return true; } finally { gate.Release(); } }
    public async Task RunCycle(CancellationToken ct) { foreach (var u in universes.Values.Where(x => x.Status == SimulationStatus.Running)) await Step(u.Id, Math.Clamp((int)Math.Ceiling(u.Speed), 1, 100), ct); }
}
