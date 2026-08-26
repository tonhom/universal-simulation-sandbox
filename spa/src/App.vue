<script setup>
import { computed, defineAsyncComponent, onMounted, onUnmounted, ref, watch } from "vue";
const CosmicVisualizer = defineAsyncComponent(() =>
  import("./components/CosmicVisualizer.vue"),
);
const universes = ref([]),
  selectedId = ref(null),
  selectedWorldId = ref(null),
  selectedPersonId = ref(null),
  events = ref([]),
  open = ref(false),
  error = ref(""),
  timelineTop = ref(0),
  personTop = ref(0),
  historyWorldFilter = ref("all"),
  historyTypeFilter = ref("all"),
  historyImportanceFilter = ref("all"),
  personWorldFilter = ref("all"),
  relationshipOpen = ref(false),
  relationshipMode = ref("local"),
  relationshipWorldFilter = ref("all"),
  refinementOpen = ref(false),
  visualizerOpen = ref(false),
  refinementDraft = ref({ scale: "World", livingWorldTarget: false });
const draft = ref({
  name: "Jade Expanse",
  seed: 42,
  stars: 5,
  genesisRate: 1,
  lifeChance: 0.72,
  affinity: 0.82,
  technologyAffinity: 0.72,
  conflictRate: 0.25,
  cosmicEntityRate: 0.08,
  maxLivingWorlds: 2,
  yearsPerTick: 1,
  knowledgeRate: 1,
  relationshipRate: 1,
  craftingRate: 1,
  warRate: 1,
  worldAscensionRate: 1,
});
const selected = computed(() =>
  universes.value.find((x) => x.id === selectedId.value),
);
const worlds = computed(
  () => selected.value?.systems.flatMap((s) => s.worlds) ?? [],
);
const selectedWorld = computed(() =>
  worlds.value.find((w) => w.id === selectedWorldId.value),
);
const selectedSystem = computed(() =>
  selected.value?.systems.find((s) =>
    s.worlds.some((w) => w.id === selectedWorldId.value),
  ),
);
const selectedPerson = computed(() =>
  selected.value?.persons.find((p) => p.id === selectedPersonId.value),
);
const personWorld = computed(() =>
  worlds.value.find((w) => w.id === selectedPerson.value?.worldId),
);
const personEvents = computed(() =>
  events.value.filter((e) => e.actorId === selectedPersonId.value),
);
const worldPeople = computed(() =>
  (selected.value?.persons ?? []).filter(
    (p) => p.worldId === selectedWorldId.value,
  ),
);
const selectedWorldConnections = computed(() =>
  (selected.value?.worldConnections ?? []).filter(
    (connection) =>
      connection.worldAId === selectedWorldId.value ||
      connection.worldBId === selectedWorldId.value,
  ),
);
const selectedCapabilities = computed(() =>
  Object.entries(selectedWorld.value?.civilizationData?.capabilities ?? {}).sort(
    (a, b) => b[1] - a[1],
  ),
);
function otherWorld(connection) {
  const id =
    connection.worldAId === selectedWorldId.value
      ? connection.worldBId
      : connection.worldAId;
  return worlds.value.find((world) => world.id === id);
}
const relationshipEdges = computed(() => {
  const people = selected.value?.persons ?? [];
  const seen = new Set();
  const result = [];
  for (const source of people)
    for (const relation of source.relationships ?? []) {
      const target = people.find((p) => p.id === relation.personId);
      if (!target) continue;
      const key = [source.id, target.id].sort().join(":");
      if (seen.has(key)) continue;
      seen.add(key);
      const sourceWorld = worlds.value.find((w) => w.id === source.worldId);
      const targetWorld = worlds.value.find((w) => w.id === target.worldId);
      result.push({
        key,
        source,
        target,
        sourceWorld,
        targetWorld,
        crossWorld: source.worldId !== target.worldId,
        ...relation,
      });
    }
  return result.filter(
    (edge) =>
      (relationshipMode.value === "cross"
        ? edge.crossWorld
        : !edge.crossWorld) &&
      (relationshipWorldFilter.value === "all" ||
        edge.source.worldId === relationshipWorldFilter.value ||
        edge.target.worldId === relationshipWorldFilter.value),
  );
});
const eventTypes = computed(() =>
  [...new Set(events.value.map((e) => e.type))].sort(),
);
const eventTypeLabels = {
  "universe.genesis": "กำเนิดจักรวาล",
  "genesis.star-system": "กำเนิดระบบดาว",
  "star.ignition": "ดาวฤกษ์จุดปฏิกิริยา",
  "world.stabilized": "World เข้าสู่สภาวะเสถียร",
  "life.genesis": "กำเนิดสิ่งมีชีวิต",
  "civilization.birth": "กำเนิดอารยธรรม",
  "civilization.evolution": "วิวัฒนาการอารยธรรม",
  "technology.discovery": "ค้นพบวิทยาศาสตร์และเทคโนโลยี",
  "cultivation.discovery": "ค้นพบวิถีบำเพ็ญ",
  "knowledge.discovery": "ค้นพบองค์ความรู้",
  "knowledge.cross-world-transfer": "ถ่ายทอดความรู้ข้าม World",
  "knowledge.exchange-agreement": "ข้อตกลงแลกเปลี่ยนความรู้",
  "world-contact.detected": "ตรวจพบ World อื่น",
  "world-contact.first-contact": "ติดต่อข้าม World ครั้งแรก",
  "innovation.hybrid-breakthrough": "นวัตกรรมผสมครั้งสำคัญ",
  "person.emergence": "บุคคลสำคัญปรากฏตัว",
  "person.relationship": "ความสัมพันธ์ของตัวละคร",
  "person.cross-world-relationship": "ความสัมพันธ์ข้าม World",
  "person.combat": "การต่อสู้ของตัวละคร",
  "fortune.ancient-artifact": "พบวาสนาและ Artifact โบราณ",
  "cultivator.breakthrough": "ทะลวงระดับสำเร็จ",
  "cultivator.pill-assisted-breakthrough": "ใช้โอสถช่วยทะลวงระดับ",
  "cultivator.breakthrough-failed": "ทะลวงระดับล้มเหลว",
  "cultivator.world-bottleneck": "ติดเพดานการบำเพ็ญของ World",
  "artifact.forged": "หลอมสร้าง Artifact",
  "war.sect": "สงครามระหว่างสำนัก",
  "war.ended": "สงครามสิ้นสุด",
  "law.terrain-transformation": "ใช้กฎเปลี่ยนภูมิประเทศ",
  "world.ascension": "World ยกระดับ",
  "cosmic.refinement.preparation": "เตรียมการหลอมระดับจักรวาล",
  "cosmic.refinement.ritual": "เริ่มพิธีหลอมระดับจักรวาล",
  "cosmic.refinement.completed": "การหลอมระดับจักรวาลสำเร็จ",
  "cosmic.refinement.aftermath": "ผลกระทบหลังการหลอม",
};
const eventTypeLabel = (type) => eventTypeLabels[type] ?? type;
const importanceLabel = (importance) =>
  ({ Trace: "ร่องรอย", Notable: "ทั่วไป", Major: "สำคัญ", Historic: "ประวัติศาสตร์", Cosmic: "จักรวาล" })[importance] ?? importance;
const connectionStatusLabel = (status) =>
  ({ Detected: "ตรวจพบแล้ว", "First Contact": "ติดต่อครั้งแรก", "Knowledge Exchange": "กำลังแลกเปลี่ยนความรู้", Integrated: "บูรณาการร่วมกัน" })[status] ?? status;
function eventBelongsToWorld(event, worldId) {
  if (worldId === "all") return true;
  const world = worlds.value.find((w) => w.id === worldId);
  if (!world) return false;
  const locations = new Set([world.id, ...world.regions.map((r) => r.id)]);
  return (
    locations.has(event.locationId) ||
    selected.value?.persons.some(
      (p) => p.id === event.actorId && p.worldId === worldId,
    )
  );
}
const filteredEvents = computed(() =>
  events.value.filter(
    (e) =>
      eventBelongsToWorld(e, historyWorldFilter.value) &&
      (historyTypeFilter.value === "all" ||
        e.type === historyTypeFilter.value) &&
      (historyImportanceFilter.value === "all" ||
        e.importance === historyImportanceFilter.value),
  ),
);
const worldEvents = computed(() =>
  selectedWorld.value
    ? events.value.filter((e) => eventBelongsToWorld(e, selectedWorld.value.id))
    : [],
);
const rowHeight = 108,
  overscan = 4;
const timelineStart = computed(() =>
  Math.max(0, Math.floor(timelineTop.value / rowHeight) - overscan),
);
const visibleEvents = computed(() =>
  filteredEvents.value.slice(
    timelineStart.value,
    timelineStart.value + Math.ceil(600 / rowHeight) + overscan * 2,
  ),
);
const filteredPeople = computed(() =>
  (selected.value?.persons ?? []).filter(
    (p) =>
      personWorldFilter.value === "all" ||
      p.worldId === personWorldFilter.value,
  ),
);
const personRowHeight = 62;
const personStart = computed(() =>
  Math.max(0, Math.floor(personTop.value / personRowHeight) - 3),
);
const visiblePeople = computed(() =>
  filteredPeople.value.slice(personStart.value, personStart.value + 10),
);
const api = async (url, options = {}) => {
  const r = await fetch(url, {
    headers: { "content-type": "application/json" },
    ...options,
  });
  if (!r.ok && r.status !== 204) throw Error(await r.text());
  return r.status === 204 ? null : r.json();
};
async function refresh() {
  try {
    universes.value = await api("/api/universes");
    if (selectedId.value && !selected.value) selectedId.value = null;
    if (selectedId.value)
      events.value = await api(
        `/api/universes/${selectedId.value}/events?limit=1000&importance=Notable`,
      );
    error.value = "";
  } catch (e) {
    error.value = "Cannot reach the observatory API: " + e.message;
  }
}
async function create() {
  const d = draft.value,
    u = await api("/api/universes", {
      method: "POST",
      body: JSON.stringify({
        name: d.name,
        parameters: {
          seed: d.seed,
          initialStarSystems: d.stars,
          genesisRate: d.genesisRate,
          lifeChance: d.lifeChance,
          cultivationAffinity: d.affinity,
          technologyAffinity: d.technologyAffinity,
          conflictRate: d.conflictRate,
          cosmicEntityRate: d.cosmicEntityRate,
          maxLivingWorldsPerSystem: d.maxLivingWorlds,
          yearsPerTick: d.yearsPerTick,
          knowledgeRate: d.knowledgeRate,
          relationshipRate: d.relationshipRate,
          craftingRate: d.craftingRate,
          warRate: d.warRate,
          worldAscensionRate: d.worldAscensionRate,
        },
      }),
    });
  selectedId.value = u.id;
  open.value = false;
  await refresh();
}
async function createRefinement() {
  const u = await api("/api/scenarios/cosmic-refinement", {
    method: "POST",
    body: JSON.stringify(refinementDraft.value),
  });
  selectedId.value = u.id;
  refinementOpen.value = false;
  await refresh();
}
async function control(body) {
  await api(`/api/universes/${selectedId.value}/control`, {
    method: "PATCH",
    body: JSON.stringify(body),
  });
  await refresh();
}
async function step() {
  await api(`/api/universes/${selectedId.value}/step`, {
    method: "POST",
    body: JSON.stringify({ ticks: 100 }),
  });
  await refresh();
}
async function removeSimulation() {
  if (
    !selected.value ||
    !window.confirm(
      `ลบจักรวาล "${selected.value.name}" และประวัติศาสตร์ทั้งหมดอย่างถาวรหรือไม่?`,
    )
  )
    return;
  await api(`/api/universes/${selectedId.value}`, { method: "DELETE" });
  selectedId.value = null;
  events.value = [];
  await refresh();
}
function changeSpeed(e) {
  control({ speed: 10 ** Number(e.target.value) });
}
function openWorld(id) {
  selectedPersonId.value = null;
  selectedWorldId.value = id;
}
function openPerson(id) {
  selectedWorldId.value = null;
  selectedPersonId.value = id;
  relationshipOpen.value = false;
}
watch(selectedId, () => {
  selectedWorldId.value = null;
  selectedPersonId.value = null;
  timelineTop.value = 0;
  personTop.value = 0;
  historyWorldFilter.value = "all";
  historyTypeFilter.value = "all";
  historyImportanceFilter.value = "all";
  personWorldFilter.value = "all";
  refresh();
});
watch([historyWorldFilter, historyTypeFilter, historyImportanceFilter], () => {
  timelineTop.value = 0;
});
watch(personWorldFilter, () => {
  personTop.value = 0;
});
let timer;
onMounted(() => {
  refresh();
  timer = setInterval(refresh, 3000);
});
onUnmounted(() => clearInterval(timer));
</script>
<template>
  <header>
    <div>
      <small>หอสังเกตการณ์ / ศูนย์ควบคุม</small>
      <h1>Universal Simulation Sandbox</h1>
    </div>
    <div class="controls">
      <button class="ghost" @click="visualizerOpen = true">
        หอสังเกตการณ์ 3D
      </button>
      <button v-if="selected" class="ghost" @click="relationshipOpen = true">
        เครือข่ายความสัมพันธ์
      </button>
      <button class="ghost" @click="refinementOpen = true">
        ตัวอย่าง Cosmic Refinement</button
      ><button @click="open = true">+ สร้างจักรวาล</button>
    </div>
  </header>
  <div v-if="error" class="error">{{ error }}</div>
  <main>
    <aside>
      <h2>จักรวาล</h2>
      <button
        v-for="u in universes"
        :key="u.id"
        class="universe"
        :class="{ active: u.id === selectedId }"
        @click="selectedId = u.id"
      >
        <b>{{ u.name }}</b
        ><span
          >{{ u.status === "Running" ? "กำลังดำเนิน" : "หยุดชั่วคราว" }} · tick
          {{ u.tick.toLocaleString() }}</span
        >
      </button>
    </aside>
    <section v-if="selected">
      <div class="hero">
        <div>
          <small>จักรวาล · SEED {{ selected.parameters.seed }}</small>
          <h1>{{ selected.name }}</h1>
        </div>
        <div class="controls">
          <button
            @click="
              control({
                status: selected.status === 'Running' ? 'Paused' : 'Running',
              })
            "
          >
            {{
              selected.status === "Running" ? "หยุดชั่วคราว" : "ดำเนินต่อ"
            }}</button
          ><input
            type="range"
            min="-2"
            max="3"
            step=".1"
            :value="Math.log10(selected.speed)"
            @change="changeSpeed"
          /><b>{{ selected.speed.toFixed(2) }}×</b
          ><button class="ghost" @click="step">+100 ticks</button
          ><button class="danger" @click="removeSimulation">ลบจักรวาล</button>
        </div>
      </div>
      <div class="stats overview-stats">
        <article>
          <small>ปีจำลอง</small
          ><strong>{{ Math.floor(selected.years).toLocaleString() }}</strong>
        </article>
        <article>
          <small>ระบบดาว</small><strong>{{ selected.systems.length }}</strong>
        </article>
        <article>
          <small>โลก / ดาวเคราะห์ทั้งหมด</small
          ><strong>{{ worlds.length }}</strong>
        </article>
        <article>
          <small>โลกที่มีชีวิต</small
          ><strong>{{
            worlds.filter((x) => x.living && !x.refined).length
          }}</strong>
        </article>
        <article>
          <small>โลกร้าง / โลกตาย / ถูกหลอม</small
          ><strong>{{
            worlds.filter((x) => !x.living || x.refined).length
          }}</strong>
        </article>
        <article>
          <small>บุคคลสำคัญ</small
          ><strong>{{ selected.persons.length }}</strong>
        </article>
      </div>
      <details class="config">
        <summary>
          พารามิเตอร์จักรวาล
          <span>Seed {{ selected.parameters.seed }}</span>
        </summary>
        <div class="config-grid">
          <div>
            <small>WORLD SEED</small><b>{{ selected.parameters.seed }}</b>
          </div>
          <div>
            <small>ความเร็ว RUNTIME</small
            ><b>{{ selected.speed.toFixed(2) }}×</b>
          </div>
          <div>
            <small>จำนวนระบบดาวเป้าหมาย</small
            ><b>{{ selected.parameters.initialStarSystems }}</b>
          </div>
          <div>
            <small>อัตราการกำเนิด</small
            ><b>{{ selected.parameters.genesisRate.toFixed(2) }}×</b>
          </div>
          <div>
            <small>โอกาสกำเนิดชีวิต</small
            ><b>{{ Math.round(selected.parameters.lifeChance * 100) }}%</b>
          </div>
          <div>
            <small>ความสอดคล้องกับการบำเพ็ญ</small
            ><b
              >{{
                Math.round(selected.parameters.cultivationAffinity * 100)
              }}%</b
            >
          </div>
          <div>
            <small>ความโน้มเอียงทางเทคโนโลยี</small
            ><b
              >{{
                Math.round((selected.parameters.technologyAffinity ?? 0.72) * 100)
              }}%</b
            >
          </div>
          <div>
            <small>อัตราความขัดแย้ง</small
            ><b>{{ Math.round(selected.parameters.conflictRate * 100) }}%</b>
          </div>
          <div>
            <small>อัตราตัวตนระดับจักรวาล</small
            ><b
              >{{ Math.round(selected.parameters.cosmicEntityRate * 100) }}%</b
            >
          </div>
          <div>
            <small>โลกมีชีวิตสูงสุด / ระบบ</small
            ><b>{{ selected.parameters.maxLivingWorldsPerSystem }}</b>
          </div>
          <div>
            <small>ปีต่อ TICK</small
            ><b>{{ selected.parameters.yearsPerTick }}</b>
          </div>
          <div>
            <small>ตัวคูณความรู้</small
            ><b>{{ selected.parameters.knowledgeRate }}×</b>
          </div>
          <div>
            <small>ตัวคูณความสัมพันธ์</small
            ><b>{{ selected.parameters.relationshipRate }}×</b>
          </div>
          <div>
            <small>ตัวคูณการคราฟต์</small
            ><b>{{ selected.parameters.craftingRate }}×</b>
          </div>
          <div>
            <small>ตัวคูณสงคราม</small><b>{{ selected.parameters.warRate }}×</b>
          </div>
          <div>
            <small>ตัวคูณยกระดับโลก</small
            ><b>{{ selected.parameters.worldAscensionRate }}×</b>
          </div>
        </div>
      </details>
      <section v-if="selected.refinements?.length" class="refinement-ledger">
        <div class="section-title">
          <div>
            <small>COSMIC REFINEMENT LEDGER</small>
            <h2>บันทึกกระบวนการหลอม</h2>
          </div>
          <span>{{ selected.refinements.length }} ครั้ง</span>
        </div>
        <article
          v-for="refinement in selected.refinements"
          :key="refinement.id"
          class="refinement-card"
        >
          <div class="refinement-title">
            <span class="scale-badge">ระดับ {{ refinement.scale }}</span>
            <div>
              <h3>{{ refinement.targetName }} → {{ refinement.material.name }}</h3>
              <p>
                ผู้หลอม <b>{{ refinement.actorName }}</b> · ปี
                {{ Math.floor(refinement.completedAtYear).toLocaleString() }}
              </p>
            </div>
          </div>
          <div class="refinement-summary">
            <div><small>เป้าหมาย</small><b>{{ refinement.targetNature }}</b></div>
            <div><small>หลอมไปเพื่อ</small><b>{{ refinement.purpose }}</b></div>
            <div>
              <small>วัตถุดิบที่ได้</small
              ><b>{{ refinement.material.name }} · {{ refinement.material.grade }}</b>
            </div>
          </div>
          <div class="refinement-flow">
            <div class="flow-step preparation">
              <strong>1 · การเตรียมตัว</strong>
              <ol>
                <li v-for="step in refinement.preparationSteps" :key="step">
                  {{ step }}
                </li>
              </ol>
            </div>
            <div class="flow-step"><strong>2 · ผลการหลอม</strong><p>{{ refinement.result }}</p></div>
            <div class="flow-step"><strong>3 · ผลกระทบหลังหลอม</strong><p>{{ refinement.aftermath }}</p></div>
            <div class="flow-step next"><strong>4 · สิ่งที่จะทำต่อ</strong><p>{{ refinement.nextAction }}</p></div>
          </div>
        </article>
      </section>
      <div class="grid">
        <div>
          <div class="section-title">
            <h2>บันทึกประวัติศาสตร์</h2>
            <small>{{ filteredEvents.length }} เหตุการณ์</small>
          </div>
          <div class="filter-bar">
            <label
              >โลก<select v-model="historyWorldFilter">
                <option value="all">ทุกโลก</option>
                <option v-for="w in worlds" :key="w.id" :value="w.id">
                  {{ w.name }}
                </option>
              </select></label
            >
            <label
              >ประเภท<select v-model="historyTypeFilter">
                <option value="all">ทุกประเภท</option>
                <option v-for="type in eventTypes" :key="type" :value="type">
                  {{ eventTypeLabel(type) }}
                </option>
              </select></label
            >
            <label
              >ความสำคัญ<select v-model="historyImportanceFilter">
                <option value="all">ทุกระดับ</option>
                <option value="Notable">ทั่วไป</option>
                <option value="Major">สำคัญ</option>
                <option value="Historic">ประวัติศาสตร์</option>
                <option value="Cosmic">จักรวาล</option>
              </select></label
            >
          </div>
          <div class="event-legend">
            <span class="notable">ทั่วไป</span><span class="major">สำคัญ</span
            ><span class="historic">ประวัติศาสตร์</span
            ><span class="cosmic">จักรวาล</span>
          </div>
          <div class="timeline" @scroll="timelineTop = $event.target.scrollTop">
            <div
              class="timeline-space"
              :style="{ height: filteredEvents.length * rowHeight + 'px' }"
            >
              <div
                class="timeline-window"
                :style="{
                  transform: `translateY(${timelineStart * rowHeight}px)`,
                }"
              >
                <div
                  v-for="e in visibleEvents"
                  :key="e.id"
                  class="event"
                  :class="`importance-${e.importance.toLowerCase()}`"
                >
                  <div class="event-meta">
                    <span class="importance-badge">{{ importanceLabel(e.importance) }}</span
                    ><small>ปี {{ Math.floor(e.year) }}</small>
                  </div>
                  <small>{{ eventTypeLabel(e.type) }}</small>
                  <b>{{ e.summary }}</b>
                  <p>{{ e.details }}</p>
                </div>
              </div>
            </div>
            <p v-if="!filteredEvents.length">ไม่พบเหตุการณ์ตามตัวกรอง</p>
          </div>
        </div>
        <div>
          <div class="section-title">
            <h2>บุคคลสำคัญ</h2>
            <small>{{ filteredPeople.length }} คน</small>
          </div>
          <div class="filter-bar">
            <label
              >กรองตามโลก<select v-model="personWorldFilter">
                <option value="all">ทุกโลก</option>
                <option v-for="w in worlds" :key="w.id" :value="w.id">
                  {{ w.name }}
                </option>
              </select></label
            >
          </div>
          <div
            class="person-virtual"
            @scroll="personTop = $event.target.scrollTop"
          >
            <div
              class="person-space"
              :style="{
                height: filteredPeople.length * personRowHeight + 'px',
              }"
            >
              <div
                class="person-window"
                :style="{
                  transform: `translateY(${personStart * personRowHeight}px)`,
                }"
              >
                <button
                  v-for="p in visiblePeople"
                  :key="p.id"
                  class="row link-row person-row"
                  @click="openPerson(p.id)"
                >
                  <b>{{ p.name }}</b
                  ><br /><small
                    >{{ p.realm }} ·
                    {{ worlds.find((w) => w.id === p.worldId)?.name }} ·
                    {{ p.lineage }}</small
                  >
                </button>
              </div>
            </div>
          </div>
          <p v-if="!filteredPeople.length">ยังไม่มีบุคคลสำคัญในโลกที่เลือก</p>
          <div class="section-title">
            <h2>ระบบดาวและโลก</h2>
            <small
              >{{ selected.systems.length }} ระบบ ·
              {{ worlds.length }} โลก</small
            >
          </div>
          <article
            v-for="system in selected.systems"
            :key="system.id"
            class="system-group"
          >
            <div class="system-head">
              <div>
                <small>ระบบดาว</small><b>{{ system.name }}</b>
              </div>
              <span>{{ system.worlds.length }} ดาวเคราะห์</span>
            </div>
            <div class="system-meta">
              {{ system.stage }} · Qi ดาวฤกษ์
              {{ system.qiDensity.toFixed(2) }} · ห่างจุดกำเนิด
              {{ (system.distanceFromOriginLightYears || 0).toLocaleString() }}
              ปีแสง · รองรับโลกมีชีวิต {{ system.livingWorldCapacity || 1 }}
            </div>
            <button
              v-for="w in system.worlds"
              :key="w.id"
              class="row link-row planet-row"
              @click="openWorld(w.id)"
            >
              <span
                class="planet-dot"
                :class="{
                  living: w.living && !w.refined,
                  dead: !w.living,
                  refined: w.refined,
                }"
              ></span
              ><span
                ><b>{{ w.name }}</b> <em>{{ w.designation }}</em
                ><br /><small
                  >วงโคจร {{ w.orbit || "?" }} ·
                  {{ (w.orbitalRadiusAu || 0).toFixed(2) }} AU ·
                  {{ w.worldTier }} ·
                  {{
                    w.living && !w.refined
                      ? "โลกที่มีชีวิต"
                      : w.refined
                        ? "โลกที่ถูกหลอม"
                        : "โลกร้าง / โลกตาย"
                  }}
                  · Qi {{ w.qiDensity.toFixed(2) }}</small
                ></span
              >
            </button>
          </article>
          <p v-if="!selected.systems.length">ยังไม่มีระบบดาวก่อตัว</p>
        </div>
      </div>
      <div v-if="relationshipOpen" class="detail relationship-page">
        <div class="detail-head">
          <div>
            <small>PERSON RELATIONSHIP NETWORK</small>
            <h1>เครือข่ายความสัมพันธ์</h1>
          </div>
          <button class="ghost" @click="relationshipOpen = false">ปิด</button>
        </div>
        <div class="relationship-controls">
          <button
            :class="{ active: relationshipMode === 'local' }"
            @click="relationshipMode = 'local'"
          >
            ภายในโลกเดียวกัน</button
          ><button
            :class="{ active: relationshipMode === 'cross' }"
            @click="relationshipMode = 'cross'"
          >
            ความสัมพันธ์ข้ามโลก</button
          ><label
            >โลก<select v-model="relationshipWorldFilter">
              <option value="all">ทุกโลก</option>
              <option v-for="w in worlds" :key="w.id" :value="w.id">
                {{ w.name }}
              </option>
            </select></label
          >
        </div>
        <div class="relationship-list">
          <article
            v-for="edge in relationshipEdges"
            :key="edge.key"
            class="relationship-card"
            :class="{ cross: edge.crossWorld }"
          >
            <button @click="openPerson(edge.source.id)">
              <b>{{ edge.source.name }}</b
              ><small
                >{{ edge.sourceWorld?.name }} · {{ edge.source.realm }}</small
              >
            </button>
            <div>
              <strong>{{ edge.type }}</strong
              ><span>ความผูกพัน {{ Math.round(edge.strength * 100) }}%</span
              ><i></i>
            </div>
            <button @click="openPerson(edge.target.id)">
              <b>{{ edge.target.name }}</b
              ><small
                >{{ edge.targetWorld?.name }} · {{ edge.target.realm }}</small
              >
            </button>
          </article>
          <p v-if="!relationshipEdges.length">
            ยังไม่มีความสัมพันธ์ในหมวดและโลกที่เลือก
          </p>
        </div>
      </div>
      <div v-if="selectedWorld" class="detail">
        <div class="detail-head">
          <div>
            <small
              >{{ selectedSystem?.name }} / {{ selectedWorld.designation }} /
              ORBIT {{ selectedWorld.orbit || "?" }}</small
            >
            <h1>{{ selectedWorld.name }}</h1>
          </div>
          <button class="ghost" @click="selectedWorldId = null">ปิด</button>
        </div>
        <div class="stats">
          <article>
            <small>ประชากรมีชีวิต</small
            ><strong>{{ selectedWorld.population.toLocaleString() }}</strong>
          </article>
          <article>
            <small>ผู้เสียชีวิตสะสม</small
            ><strong>{{
              (selectedWorld.deathsTotal || 0).toLocaleString()
            }}</strong>
          </article>
          <article>
            <small>ระดับโลก</small
            ><strong>{{ selectedWorld.worldTier }}</strong>
          </article>
          <article>
            <small>เพดานการบำเพ็ญ</small
            ><strong>{{ selectedWorld.maxCultivationRealm }}</strong>
          </article>
        </div>
        <article class="samsara">
          <div>
            <small>สถานะชีวมณฑล</small
            ><b>{{
              selectedWorld.living && !selectedWorld.refined
                ? "Living"
                : selectedWorld.refined
                  ? "Refined / extinguished"
                  : "Dead or barren"
            }}</b>
          </div>
          <div>
            <small>วิญญาณใน Samsara</small
            ><b>{{ (selectedWorld.soulsInSamsara || 0).toLocaleString() }}</b>
          </div>
          <div>
            <small>กลับชาติมาเกิดสะสม</small
            ><b>{{
              (selectedWorld.reincarnatedTotal || 0).toLocaleString()
            }}</b>
          </div>
          <div>
            <small>วิญญาณเร่ร่อน</small
            ><b>{{ (selectedWorld.wanderingDead || 0).toLocaleString() }}</b>
          </div>
          <div>
            <small>การเกิดสะสม</small
            ><b>{{ (selectedWorld.birthsTotal || 0).toLocaleString() }}</b>
          </div>
          <p>
            Death sends most souls into samsara; a fraction remain as wandering
            dead. Reincarnated souls return through new births.
          </p>
        </article>
        <article class="dossier world-favored">
          <div class="section-title">
            <h2>ผู้ได้รับความโปรดปรานจากสวรรค์</h2>
            <small>{{ worldPeople.length }} คน</small>
          </div>
          <div class="world-favored-list">
            <button
              v-for="p in worldPeople"
              :key="p.id"
              class="link-row row"
              @click="openPerson(p.id)"
            >
              <b>{{ p.name }}</b
              ><span
                >{{ p.realm }} · โชคชะตา
                {{ Math.round((p.luck ?? 0.5) * 100) }}%</span
              ><small>{{ p.heavenFavorReason }}</small>
            </button>
            <p v-if="!worldPeople.length">
              โลกนี้ยังไม่มี Heaven-favored person ที่ถูกบันทึก
            </p>
          </div>
        </article>
        <div class="detail-grid">
          <article class="dossier">
            <h2>สถานะโลก</h2>
            <dl>
              <dt>รหัสดาราศาสตร์</dt>
              <dd>{{ selectedWorld.designation }}</dd>
              <dt>ลำดับวงโคจร</dt>
              <dd>
                {{ selectedWorld.orbit || "ไม่ทราบ" }} ·
                {{ (selectedWorld.orbitalRadiusAu || 0).toFixed(2) }} AU
              </dd>
              <dt>สภาวะดาวเคราะห์</dt>
              <dd>{{ selectedWorld.stage }}</dd>
              <dt>อารยธรรม</dt>
              <dd>{{ selectedWorld.civilization }}</dd>
              <dt>เส้นทางหลัก</dt>
              <dd>{{ selectedWorld.civilizationData?.primaryPath || "ยังไม่พัฒนา" }}</dd>
              <dt>ยุคอารยธรรม</dt>
              <dd>{{ selectedWorld.civilizationData?.stage || "ไม่ทราบ" }}</dd>
              <dt>ระดับการพัฒนา</dt>
              <dd>
                Level {{ selectedWorld.civilizationData?.level || selectedWorld.developmentLevel || 0 }}
                · Research {{ selectedWorld.developmentLevel || 0 }}
              </dd>
              <dt>ชีวนิเวศหลัก</dt>
              <dd>{{ selectedWorld.biome }}</dd>
              <dt>ความเหมาะสมต่อชีวิต</dt>
              <dd>{{ Math.round(selectedWorld.habitability * 100) }}%</dd>
              <dt>ความหนาแน่น Qi</dt>
              <dd>{{ selectedWorld.qiDensity.toFixed(3) }}</dd>
              <dt>เส้นชีพจรวิญญาณ</dt>
              <dd>{{ selectedWorld.spiritualVeins }}</dd>
              <dt>ดาวฤกษ์ศูนย์กลาง</dt>
              <dd>{{ selectedSystem?.name }} — {{ selectedSystem?.stage }}</dd>
            </dl>
          </article>
          <article class="dossier">
            <div class="section-title">
              <h2>สำนักบำเพ็ญ</h2>
              <small>{{ selectedWorld.sects?.length || 0 }} สำนัก</small>
            </div>
            <div
              v-for="sect in selectedWorld.sects"
              :key="sect.id"
              class="sect"
            >
              <div>
                <b>{{ sect.name }}</b
                ><small>{{ sect.path }}</small>
              </div>
              <span>ศิษย์ {{ sect.members.toLocaleString() }} คน</span
              ><span>ผู้นำ: {{ sect.leader }}</span
              ><span
                >{{ sect.highestRealm }} · อิทธิพล {{ sect.influence }}</span
              >
            </div>
            <p v-if="!selectedWorld.sects?.length">
              ยังไม่มีสำนักบำเพ็ญก่อตั้ง
            </p>
          </article>
        </div>
        <div class="world-systems">
          <article class="dossier capability-panel">
            <div class="section-title">
              <h2>ขีดความสามารถอารยธรรม</h2>
              <small>{{ selectedWorld.civilizationData?.primaryPath }}</small>
            </div>
            <div
              v-for="[name, value] in selectedCapabilities"
              :key="name"
              class="capability-row"
            >
              <span>{{ name }}</span>
              <div><i :style="{ width: Math.round(value * 100) + '%' }"></i></div>
              <b>{{ Math.round(value * 100) }}%</b>
            </div>
          </article>
          <article class="dossier world-connections">
            <div class="section-title">
              <h2>การเชื่อมต่อข้าม World</h2>
              <small>{{ selectedWorldConnections.length }} เส้นทาง</small>
            </div>
            <div
              v-for="connection in selectedWorldConnections"
              :key="connection.id"
              class="connection-card"
            >
              <b>{{ otherWorld(connection)?.name || "Unknown World" }}</b>
              <span>{{ connectionStatusLabel(connection.status) }} · {{ connection.travelMethod }}</span>
              <small>
                {{ connection.distanceLightYears.toFixed(3) }} ปีแสง · ความไว้ใจ
                {{ Math.round(connection.trust * 100) }}% · แลกเปลี่ยนความรู้
                {{ connection.knowledgeExchanges }} ครั้ง
              </small>
              <p>Trigger: {{ connection.trigger }}</p>
              <button
                v-if="otherWorld(connection)"
                class="ghost"
                @click="openWorld(otherWorld(connection).id)"
              >ดู World ปลายทาง</button>
            </div>
            <p v-if="!selectedWorldConnections.length">
              ยังไม่ตรวจพบหรือสร้างเส้นทางไปยัง World อื่น
            </p>
          </article>
          <article class="dossier innovation-panel">
            <div class="section-title">
              <h2>นวัตกรรมและการประยุกต์ร่วม</h2>
              <small>{{ selectedWorld.innovations?.length || 0 }}</small>
            </div>
            <div
              v-for="innovation in selectedWorld.innovations || []"
              :key="innovation.id"
              class="innovation-card"
            >
              <b>{{ innovation.name }}</b>
              <span>{{ innovation.kind }} · {{ innovation.developedBy }}</span>
              <p>{{ innovation.purpose }}</p>
              <small>ความรู้ที่ใช้: {{ innovation.inputs.join(" + ") }}</small>
            </div>
            <p v-if="!selectedWorld.innovations?.length">
              ยังไม่มีการประยุกต์องค์ความรู้ข้ามศาสตร์
            </p>
          </article>
          <article class="dossier">
            <div class="section-title">
              <h2>ภูมิภาคและทวีป</h2>
              <small>{{ selectedWorld.regions?.length || 0 }}</small>
            </div>
            <div
              v-for="region in selectedWorld.regions || []"
              :key="region.id"
              class="region"
            >
              <b>{{ region.name }}</b
              ><span>{{ region.kind }} · {{ region.terrain }}</span
              ><small
                >ประชากร {{ region.population.toLocaleString() }} · Qi
                {{ region.qiDensity.toFixed(2) }} · เสถียรภาพ
                {{ Math.round(region.stability * 100) }}% · ความเสียหาย
                {{ Math.round(region.devastation * 100) }}%</small
              >
            </div>
          </article>
          <article class="dossier">
            <h2>ความรู้และวัตถุดิบ</h2>
            <div
              v-for="k in selectedWorld.knowledge || []"
              :key="k.id"
              class="compact"
            >
              <b>{{ k.name }}</b
              ><span
                >{{ k.domain }} Lv.{{ k.level }} · {{ k.discoveredBy }} ·
                {{ k.acquisition || "Discovery" }}</span
              >
            </div>
            <div
              v-for="m in selectedWorld.materials || []"
              :key="m.id"
              class="compact material"
            >
              <b>{{ m.name }}</b
              ><span
                >{{ m.grade }} · {{ m.lawAffinity }} Law · {{ m.region }} ·
                abundance {{ Math.round(m.abundance * 100) }}%</span
              >
            </div>
          </article>
          <article class="dossier">
            <h2>สงครามและความขัดแย้ง</h2>
            <div
              v-for="c in selectedWorld.conflicts || []"
              :key="c.id"
              class="conflict"
            >
              <b>{{ c.name }}</b
              ><span :class="{ active: c.status === 'Active' }">{{
                c.status
              }}</span>
              <p>{{ c.attacker }} vs {{ c.defender }} · {{ c.cause }}</p>
              <small
                >{{ c.scale }} · casualties
                {{ c.casualties.toLocaleString() }}</small
              >
            </div>
            <p v-if="!selectedWorld.conflicts?.length">
              ยังไม่มีสงครามที่ถูกบันทึก
            </p>
          </article>
        </div>
        <article class="dossier world-history">
          <div class="section-title">
            <h2>ประวัติศาสตร์ของ {{ selectedWorld.name }}</h2>
            <small>{{ worldEvents.length }} เหตุการณ์</small>
          </div>
          <div class="world-history-list">
            <div
              v-for="e in worldEvents"
              :key="e.id"
              class="personal-event"
              :class="`importance-${e.importance.toLowerCase()}`"
            >
              <div class="event-meta">
                <span class="importance-badge">{{ importanceLabel(e.importance) }}</span
                ><small>ปี {{ Math.floor(e.year).toLocaleString() }}</small>
              </div>
              <small>{{ eventTypeLabel(e.type) }}</small>
              <b>{{ e.summary }}</b>
              <p>{{ e.details }}</p>
            </div>
            <p v-if="!worldEvents.length">
              ยังไม่มีเหตุการณ์ที่เชื่อมโยงกับโลกหรือภูมิภาคนี้
            </p>
          </div>
        </article>
      </div>
      <div v-if="selectedPerson" class="detail">
        <div class="detail-head">
          <div>
            <small>บุคคลสำคัญ / บันทึกการบำเพ็ญ</small>
            <h1>{{ selectedPerson.name }}</h1>
          </div>
          <button class="ghost" @click="selectedPersonId = null">ปิด</button>
        </div>
        <div class="stats">
          <article>
            <small>Realm ปัจจุบัน</small
            ><strong>{{ selectedPerson.realm }}</strong>
          </article>
          <article>
            <small>โชคชะตา</small
            ><strong
              >{{ Math.round((selectedPerson.luck ?? 0.5) * 100) }}%</strong
            >
          </article>
          <article>
            <small>โลกกำเนิด</small
            ><strong>{{ personWorld?.name || "ไม่ทราบ" }}</strong>
          </article>
          <article>
            <small>สถานะ</small
            ><strong>{{
              selectedPerson.alive ? "มีชีวิต" : "เสียชีวิต"
            }}</strong>
          </article>
        </div>
        <article class="favor">
          <small>เหตุใดสวรรค์จึงโปรดปราน</small
          ><b>{{ selectedPerson.heavenFavorReason }}</b
          ><span>สายสืบทอด: {{ selectedPerson.lineage }}</span>
        </article>
        <div class="detail-grid">
          <article class="dossier">
            <h2>สายสืบทอดการบำเพ็ญ</h2>
            <div class="lineage">
              <div
                v-for="m in selectedPerson.cultivationHistory || []"
                :key="m.tick"
              >
                <i></i>
                <div>
                  <small>ปี {{ Math.floor(m.year).toLocaleString() }}</small
                  ><b>{{ m.realm }}</b>
                  <p>{{ m.cause }}</p>
                </div>
              </div>
            </div>
            <p v-if="!selectedPerson.cultivationHistory?.length">
              Earlier lineage records are unknown.
            </p>
            <h2>สมบัติและ Artifact</h2>
            <div
              v-for="a in selectedPerson.artifacts || []"
              :key="a.id"
              class="artifact"
            >
              <b>{{ a.name }}</b
              ><small>{{ a.grade }} · {{ a.kind }}</small>
              <p>{{ a.ability }}</p>
              <span
                >โบนัสการทะลวงระดับ: +{{
                  (a.breakthroughBonus * 100).toFixed(3)
                }}%</span
              >
            </div>
            <p v-if="!selectedPerson.artifacts?.length">
              ยังไม่มีสมบัติผูกชะตาที่ถูกบันทึก
            </p>
            <h2>ความสัมพันธ์</h2>
            <div
              v-for="r in selectedPerson.relationships || []"
              :key="r.personId"
              class="compact"
            >
              <b>{{ r.personName }}</b
              ><span
                >{{ r.type }} · strength
                {{ Math.round(r.strength * 100) }}%</span
              >
            </div>
            <h2>ความรู้และทักษะ</h2>
            <div
              v-for="skill in selectedPerson.skills || []"
              :key="skill"
              class="skill-chip"
            >
              {{ skill }}
            </div>
            <p v-if="!selectedPerson.skills?.length">
              ยังไม่มีวิชาที่มีชื่อถูกบันทึก
            </p>
            <h2>ความเข้าใจ Law</h2>
            <div
              v-for="law in selectedPerson.laws || []"
              :key="law.law"
              class="compact"
            >
              <b>{{ law.law }} Law</b
              ><span
                >Level {{ law.level }} · comprehension
                {{ Math.round(law.comprehension * 100) }}%</span
              >
            </div>
          </article>
          <article class="dossier">
            <h2>ประวัติส่วนบุคคล</h2>
            <div
              v-for="e in personEvents"
              :key="e.id"
              class="personal-event"
              :class="`importance-${e.importance.toLowerCase()}`"
            >
              <div class="event-meta">
                <span class="importance-badge">{{ importanceLabel(e.importance) }}</span
                ><small>ปี {{ Math.floor(e.year).toLocaleString() }}</small>
              </div>
              <small>{{ eventTypeLabel(e.type) }}</small>
              <b>{{ e.summary }}</b>
              <p>{{ e.details }}</p>
            </div>
            <p v-if="!personEvents.length">
              ยังไม่มีเหตุการณ์ส่วนบุคคลในช่วงประวัติศาสตร์ที่โหลดอยู่
            </p>
          </article>
        </div>
      </div>
    </section>
    <section v-else class="empty">
      สร้างจักรวาลเพื่อเริ่มสังเกตเหตุและผล
    </section>
  </main>
  <dialog :open="refinementOpen" class="refinement-dialog">
    <form @submit.prevent="createRefinement">
      <small>COSMIC REFINEMENT SCENARIO</small>
      <h2>สร้างตัวอย่างกระบวนการหลอม</h2>
      <p>
        ระบบจะสร้างผู้หลอม เป้าหมาย การเตรียมพิธี ผลผลิต ผลกระทบ
        และแผนการหลังหลอมเป็นประวัติศาสตร์ที่เชื่อมเหตุและผล
      </p>
      <label
        >ระดับการหลอม<select v-model="refinementDraft.scale">
          <option value="Mountain">หลอมภูเขา</option>
          <option value="Star">หลอมดาวฤกษ์และระบบดาว</option>
          <option value="World">หลอม World / ดาวเคราะห์ทั้งดวง</option>
        </select></label
      >
      <label v-if="refinementDraft.scale === 'World'" class="check-label">
        <input v-model="refinementDraft.livingWorldTarget" type="checkbox" />
        ใช้โลกที่ยังมีชีวิตเป็นเป้าหมาย
      </label>
      <div class="scenario-warning" v-if="refinementDraft.scale === 'Mountain'">
        กระทบภูมิประเทศ เส้นชีพจรปราณ และประชากรเฉพาะภูมิภาค
      </div>
      <div class="scenario-warning" v-else-if="refinementDraft.scale === 'Star'">
        ทำลายดาวฤกษ์ วงโคจร และ World ทั้งระบบ เป็นเหตุการณ์ระดับสูงสุด
      </div>
      <div class="scenario-warning" v-else>
        หลอมกฎ ขอบเขตโลก และแก่นดาวเคราะห์; โลกมีชีวิตจะเกี่ยวข้องกับกรรมและ Samsara
      </div>
      <menu>
        <button type="button" class="ghost" @click="refinementOpen = false">
          ยกเลิก</button
        ><button>สร้างและเปิดดู</button>
      </menu>
    </form>
  </dialog>
  <dialog :open="open">
    <form @submit.prevent="create">
      <h2>กำหนดพารามิเตอร์จักรวาล</h2>
      <div class="creation-grid">
        <label>ชื่อจักรวาล<input v-model="draft.name" required /></label
        ><label
          >World seed<input v-model.number="draft.seed" type="number" /></label
        ><label
          >จำนวนระบบดาว<input
            v-model.number="draft.stars"
            type="number"
            min="1"
            max="100" /></label
        ><label
          >โลกมีชีวิตสูงสุดต่อระบบ<select
            v-model.number="draft.maxLivingWorlds"
          >
            <option :value="1">1 โลก</option>
            <option :value="2">2 โลก</option>
          </select></label
        ><label
          >ปีต่อ tick<input
            v-model.number="draft.yearsPerTick"
            type="number"
            min="0.01"
            max="1000"
            step="0.01" /></label
        ><label
          >ความสอดคล้องกับการบำเพ็ญ<input
            v-model.number="draft.affinity"
            type="range"
            min="0"
            max="1"
            step=".01"
          /><output>{{ draft.affinity }}</output></label
        >
        <label
          >ความโน้มเอียงทางเทคโนโลยี<input
            v-model.number="draft.technologyAffinity"
            type="range"
            min="0"
            max="1"
            step=".01"
          /><output>{{ draft.technologyAffinity }}</output></label
        >
        <label
          >อัตราการกำเนิด<input
            v-model.number="draft.genesisRate"
            type="range"
            min="0.1"
            max="3"
            step="0.1"
          /><output>{{ draft.genesisRate }}×</output></label
        >
        <label
          >โอกาสกำเนิดชีวิต<input
            v-model.number="draft.lifeChance"
            type="range"
            min="0"
            max="1"
            step="0.01"
          /><output>{{ Math.round(draft.lifeChance * 100) }}%</output></label
        >
        <label
          >อัตราความขัดแย้ง<input
            v-model.number="draft.conflictRate"
            type="range"
            min="0"
            max="2"
            step="0.05"
          /><output>{{ draft.conflictRate }}×</output></label
        >
        <label
          >อัตราตัวตนระดับจักรวาล<input
            v-model.number="draft.cosmicEntityRate"
            type="range"
            min="0"
            max="2"
            step="0.05"
          /><output>{{ draft.cosmicEntityRate }}×</output></label
        >
        <label
          >อัตราการค้นพบความรู้<input
            v-model.number="draft.knowledgeRate"
            type="range"
            min="0.1"
            max="3"
            step="0.1"
          /><output>{{ draft.knowledgeRate }}×</output></label
        >
        <label
          >อัตราความสัมพันธ์<input
            v-model.number="draft.relationshipRate"
            type="range"
            min="0.1"
            max="3"
            step="0.1"
          /><output>{{ draft.relationshipRate }}×</output></label
        >
        <label
          >อัตราการคราฟต์<input
            v-model.number="draft.craftingRate"
            type="range"
            min="0.1"
            max="3"
            step="0.1"
          /><output>{{ draft.craftingRate }}×</output></label
        >
        <label
          >อัตราสงคราม<input
            v-model.number="draft.warRate"
            type="range"
            min="0.1"
            max="3"
            step="0.1"
          /><output>{{ draft.warRate }}×</output></label
        >
        <label
          >อัตราการยกระดับโลก<input
            v-model.number="draft.worldAscensionRate"
            type="range"
            min="0.1"
            max="3"
            step="0.1"
          /><output>{{ draft.worldAscensionRate }}×</output></label
        >
      </div>
      <menu>
        <button type="button" class="ghost" @click="open = false">ยกเลิก</button
        ><button>สร้างจักรวาล</button>
      </menu>
    </form>
  </dialog>
  <CosmicVisualizer
    v-if="visualizerOpen"
    :universe="selected"
    @close="visualizerOpen = false"
  />
</template>
