<script setup>
import { computed, nextTick, onMounted, onUnmounted, ref, watch } from "vue";
import * as THREE from "three";
import { EffectComposer } from "three/addons/postprocessing/EffectComposer.js";
import { RenderPass } from "three/addons/postprocessing/RenderPass.js";
import { UnrealBloomPass } from "three/addons/postprocessing/UnrealBloomPass.js";

const props = defineProps({ universe: { type: Object, default: null } });
const emit = defineEmits(["close"]);
const observatory=ref(null);
const displayMode=ref(false);

const scenarios = [
  {
    id: "genesis",
    type: "genesis",
    name: "การกำเนิดจักรวาล",
    subtitle: "จาก Origin Singularity สู่ดวงดาว โลก และเมล็ดพันธุ์ชีวิต",
    duration: 135,
    accent: "#63f5d0",
    phases: [
      [0, "ความว่างเปล่า", "ยังไม่มี Space, Time หรือสิ่งที่ผู้สังเกตการณ์เรียกว่าอนุภาค"],
      [5, "เส้นใยพลังงานแรก", "Chaos Energy เริ่มไหลจากความว่างเปล่าเข้าสู่ศูนย์กลาง"],
      [14, "Origin Accretion", "เส้นใยพลังงานและความเป็นไปได้ไหลหลากหนาแน่นขึ้นเรื่อย ๆ"],
      [20, "Critical Silence", "ก้อนพลังงานแตะขีดวิกฤตและดับวูบลงสู่ความมืดสมบูรณ์ชั่วขณะ"],
      [23, "Singularity Rebirth", "หลังความมืดสนิท แกนกำเนิดกลับมาขยายตัวอย่างรวดเร็ว"],
      [25, "Big Bang", "แกนกำเนิดระเบิดเป็นแสงวาบเต็มพื้นที่ ก่อนแสงค่อย ๆ จางลง"],
      [30, "Space-Time & Law Expansion", "พื้นที่ เวลา และเส้นกฎจักรวาลพุ่งแยกจากศูนย์กลาง"],
      [43, "Galactic Seed", "จุดแสงขนาดเล็กปรากฏในห้วงอวกาศและเริ่มเผยรูปร่างทรงจาน"],
      [52, "Galactic Flow", "จานกาแล็กซีหมุนวนและเผยแขนเกลียวคล้ายทางช้างเผือก"],
      [64, "ดาวฤกษ์ดวงแรก", "กลุ่มก๊าซยุบตัวและจุดปฏิกิริยาตามแขนกาแล็กซี"],
      [70, "ระบบดาวก่อตัว", "ดาวฤกษ์หนึ่งดวงกับดาวเคราะห์แปดดวงก่อตัวจากจานมวลสารในแขนกาแล็กซี"],
      [78, "ดาวเคราะห์มีชีวิต", "มหาสมุทรและทวีปของดาวเคราะห์ดวงที่สามเริ่มเสถียร"],
      [82, "Atmosphere Genesis", "ชั้นบรรยากาศและกลุ่มเมฆค่อย ๆ ห่อหุ้มดาวก่อนกล้องเริ่มเข้าใกล้"],
      [91, "โลกยุคพายุ", "ภูเขาไฟ ลาวา พายุ ฝน และฟ้าผ่ากำลังปรับสมดุลโลกยุคแรก"],
      [96, "Life Genesis", "พืช ต้นไม้ ใบหญ้า และสิ่งมีชีวิตแรกเริ่มแพร่กระจาย"],
      [98, "Age of Giants", "สัตว์ขนาดใหญ่และไดโนเสาร์ยุคแรกเดินทางผ่านผืนทวีป"],
      [99, "รุ่งอรุณแห่งอารยธรรม", "หลังระบบนิเวศผ่านกาลเวลา ชุมชนมนุษย์ถือกำเนิด"],
    ],
  },
  {
    id: "geological",
    type: "collapse",
    name: "แก่นโลกและเปลือกโลกแตกสลาย",
    subtitle: "Core instability ฉีกเปลือกดาวบางส่วน เผยแม็กม่าและเหวี่ยงแผ่นน้ำแข็งสู่ชั้นอวกาศ",
    duration: 48,
    accent: "#ff5a2f",
    phases: [
      [0, "World อยู่ในภาวะเสถียร", "แก่นโลก แมนเทิล และเปลือกดาวยังรักษาสมดุล"],
      [16, "Core Overpressure", "ความร้อนและแรงดันภายในเกินอัตราที่เปลือกดาวระบายได้"],
      [34, "รอยแยกทั่วดาว", "รอยแตกเรืองแสงเชื่อมกันผ่านแผ่นทวีปและพื้นมหาสมุทร"],
      [49, "Mantle Exposure", "เปลือกดาวบางบริเวณเปิดออกจนเห็นแก่นแม็กมาภายใน"],
      [66, "Crust Shedding", "แผ่นเปลือกโค้ง ทวีป และมหาสมุทรที่แข็งตัวบางส่วนถูกเหวี่ยงออกจากดาว"],
      [86, "Debris Field", "World ที่เสียหายยังคงเหลือแกนและเปลือกบางส่วน โดยมีเศษหินกับน้ำแข็งโคจรรอบ ๆ"],
    ],
  },
  {
    id: "blackhole",
    type: "collapse",
    name: "ยุบตัวเป็นหลุมดำ",
    subtitle: "มวลและ Law Density เกินขีดจำกัดจน Space-Time ปิดตัว เกิดขอบฟ้าเหตุการณ์",
    duration: 52,
    accent: "#d6a8ff",
    phases: [
      [0, "World อยู่ในภาวะเสถียร", "แรงโน้มถ่วงและแรงดันภายในยังรักษาสมดุล"],
      [16, "มวลวิกฤต", "การสะสมมวล พลังงาน หรือ Law Density ทำให้สมดุลเริ่มพัง"],
      [38, "Gravitational Collapse", "พื้นผิวและชั้นกฎยุบเข้าหาศูนย์กลางอย่างหลีกเลี่ยงไม่ได้"],
      [63, "กำเนิด Accretion Disk", "สสารที่ยังไม่ตกลงไปหมุนเร็วและร้อนจนเปล่งรังสี"],
      [82, "Event Horizon", "แม้แต่แสงไม่สามารถเดินทางกลับออกจากขอบเขตนี้"],
      [96, "Black Hole เสถียร", "World เดิมเหลือเพียงมวล การหมุน ประจุ และ causal aftermath"],
    ],
  },
  {
    id: "stellar",
    type: "collapse",
    name: "ถูกดาวฤกษ์กลืนกิน",
    subtitle: "ดาวฤกษ์ขยายตัว ลมสุริยะเผาชั้นบรรยากาศ ก่อนกลืน World ทั้งใบ",
    duration: 45,
    accent: "#ffcc65",
    phases: collapsePhases("วงโคจรเสื่อม", "ชั้นบรรยากาศถูกเผา", "World ถูกกลืนโดยดาวฤกษ์"),
  },
  {
    id: "law",
    type: "collapse",
    name: "โครงข่ายกฎจักรวาลล่มสลาย",
    subtitle: "Causality, Space และ Identity สูญเสียความสอดคล้องจน Reality แตกเป็นชั้น",
    duration: 54,
    accent: "#b18cff",
    phases: collapsePhases("Law conflict สูงเกินขีดจำกัด", "Reality เกิดรอยแยก", "Identity ของ World สลาย"),
  },
  {
    id: "depletion",
    type: "collapse",
    name: "ปราณเหือดแห้งและ Realm Regression",
    subtitle: "Qi regeneration ต่ำกว่าการบริโภค เพดานบำเพ็ญลดลงและระบบนิเวศวิญญาณตาย",
    duration: 64,
    accent: "#66a7ff",
    phases: collapsePhases("Qi deficit สะสม", "Leyline และสำนักล่มสลาย", "World เข้าสู่ยุคไร้ปราณ"),
  },
  {
    id: "refinement",
    type: "collapse",
    name: "ถูกผู้ทรงอำนาจหลอมเป็นวัตถุดิบ",
    subtitle: "Formation ปิดผนึก World ก่อนสกัดแก่นกฎ Soul imprint และทรัพยากรจักรวาล",
    duration: 58,
    accent: "#ff5fa2",
    phases: collapsePhases("วางมหาค่ายกลปิดโลก", "สกัดประชากรและแก่นกฎ", "ควบแน่นเป็น World-Core Ingot"),
  },
  {
    id: "erasure",
    type: "collapse",
    name: "Conceptual Erasure",
    subtitle: "Identity, Information และ causal trace ถูกลบ—รุนแรงกว่าความตายหรือการทำลาย",
    duration: 40,
    accent: "#ff5570",
    phases: collapsePhases("Conceptual Authority ล็อกเป้าหมาย", "ประวัติศาสตร์สูญเสียความต่อเนื่อง", "World ถูกลบจาก Reality"),
  },
];

function collapsePhases(warning, rupture, end) {
  return [
    [0, "World อยู่ในภาวะเสถียร", "ระบบนิเวศ อารยธรรม และกฎท้องถิ่นยังทำงานตามปกติ"],
    [18, "ตรวจพบสัญญาณเตือน", warning],
    [42, "จุดวิกฤต", rupture],
    [68, "การล่มสลายต่อเนื่อง", "World Will ใช้ทรัพยากรสำรองและตอบโต้เพื่อรักษาตัวเอง"],
    [88, "Terminal State", end],
  ];
}

const host = ref(null);
const scenarioId = ref("genesis");
const progress = ref(0);
const playing = ref(true);
const speed = ref(1);
const userZoom = ref(1);
const webGpuAvailable = ref(false);
const rendererLabel = ref("กำลังตรวจสอบ");
const scenario = computed(() => scenarios.find((item) => item.id === scenarioId.value));
const bigBangFlashOpacity = computed(() => {
  if (scenarioId.value !== "genesis") return 0;
  const point = progress.value / 100;
  if (point < .241 || point > .292) return 0;
  return Math.min(1, Math.exp(-Math.pow((point - .246) * 56, 2)) * 1.55);
});
const cloudTransitionOpacity=computed(()=>{
  if(scenarioId.value!=="genesis")return 0;
  const p=progress.value/100;
  if(p<.892||p>.942)return 0;
  return p<.916?smooth(p,.892,.916):1-smooth(p,.916,.942);
});
const phase = computed(() => {
  const entries = scenario.value.phases;
  return [...entries].reverse().find(([threshold]) => progress.value >= threshold) ?? entries[0];
});
const elapsedYears = computed(() => {
  const seedScale = Number(props.universe?.parameters?.yearsPerTick ?? 1);
  const span = scenario.value.type === "genesis" ? 13_800_000_000 : 120_000;
  return Math.floor((progress.value / 100) * span * Math.max(1, seedScale));
});
const metrics = computed(() => {
  const p = progress.value / 100;
  if (scenario.value.type === "genesis") {
    if (p >= .885) {
      return [
        ["การก่อรูปพื้นผิว", `${(smooth(p, .885, .93) * 100).toFixed(1)}%`],
        ["ความหนาแน่นบรรยากาศ", `${(smooth(p, .885, .94) * 100).toFixed(1)}%`],
        ["การแพร่กระจายชีวมณฑล", `${(smooth(p, .93, .975) * 100).toFixed(1)}%`],
        ["การก่อรูปอารยธรรม", `${(smooth(p, .978, 1) * 100).toFixed(1)}%`],
      ];
    }
    return [
      ["ขอบเขต Space-Time", `${Math.min(100, p * 128).toFixed(1)}%`],
      ["เสถียรภาพของกฎ", `${Math.max(2, Math.min(99, (p - 0.12) * 126)).toFixed(1)}%`],
      ["ความหนาแน่นสสาร", `${Math.max(0.1, 100 * Math.exp(-p * 4)).toFixed(2)} ρ₀`],
      ["โอกาสเกิด Life", `${Math.max(0, (p - 0.82) * 550).toFixed(1)}%`],
    ];
  }
  if (scenarioId.value === "blackhole") {
    return [
      ["World Radius", `${Math.max(0.02, 1 - smooth(p, .28, .82) * .98).toFixed(3)} R₀`],
      ["Space-Time Curvature", `${Math.min(100, smooth(p, .12, .88) * 100).toFixed(1)}%`],
      ["Escape Velocity", `${Math.min(100, 12 + p * 96).toFixed(1)}% c`],
      ["Event Horizon", p >= .82 ? "ก่อตัวแล้ว" : "ยังไม่ปิดสมบูรณ์"],
    ];
  }
  if (scenarioId.value === "geological") {
    return [
      ["Crust Integrity", `${Math.max(0, 100 - smooth(p, .12, .72) * 108).toFixed(1)}%`],
      ["Mantle Exposure", `${(smooth(p, .38, .72) * 100).toFixed(1)}%`],
      ["เศษดาว", p < .48 ? "ยังเชื่อมต่อ" : "320 ชิ้นหลัก + 1,400 debris"],
      ["Debris Expansion", `${(smooth(p, .48, .94) * 100).toFixed(1)}%`],
    ];
  }
  return [
    ["World Integrity", `${Math.max(0, 100 - p * 112).toFixed(1)}%`],
    ["World Will Pressure", `${Math.min(100, 8 + p * 108).toFixed(1)}%`],
    ["Population Survival", `${Math.max(0, 100 - Math.pow(p, 2.2) * 118).toFixed(1)}%`],
    ["Causal Damage", `${Math.min(100, Math.pow(p, 1.5) * 105).toFixed(1)}%`],
  ];
});

let renderer;
let composer;
let bloomPass;
let scene;
let camera;
let world;
let worldMaterial;
let atmosphere;
let particles;
let particleBase;
let starField;
let chaosThreads;
let originEnergyCore;
let formation;
let star;
let starLight;
let singularityFlash;
let auroraGroup;
let primordialEnergyGroup;
let galaxyGroup;
let nebulaBackdrop;
let solarSystem;
let planetMoon;
let surfaceWorld;
let surfaceTerrain;
let surfaceWater;
let surfaceAtmosphere;
let surfaceSky;
let surfaceMilkyWay;
let surfaceForest;
let surfaceShrubs;
let surfaceMountains;
let surfaceSnowCaps;
let surfaceCloudLayer;
let surfaceCivilization;
let surfaceRain;
let surfaceLightningGroup;
let surfaceVolcanoGroup;
let surfaceDinosaurs;
let blackHole;
let blackHoleLens;
let spacetimeGrid;
let singularityCore;
let fragmentGroup;
let fragmentMaterial;
let magmaCore;
let debrisField;
let fractureAura;
let fractureFlare;
let fractureLight;
let rain;
let lightning;
let volcanoGlow;
let surfaceSun;
let surfaceMoon;
let daylight;
const vegetation = [];
const civilization = [];
const lifeForms = [];
const stormClouds = [];
const focusPosition = new THREE.Vector3();
const sunPosition = new THREE.Vector3();
const debrisMatrix = new THREE.Matrix4();
const debrisQuaternion = new THREE.Quaternion();
const debrisPosition = new THREE.Vector3();
const debrisScale = new THREE.Vector3();
const debrisEuler = new THREE.Euler();
let observer;
let frame;
let lastTime = performance.now();

const vertexShader = `
  uniform float uTime;
  uniform float uProgress;
  uniform float uMode;
  varying vec3 vNormalW;
  varying vec3 vPosition;
  varying float vNoise;
  float field(vec3 p) {
    return sin(p.x * 8.0 + uTime) * sin(p.y * 10.0 - uTime * .7) * sin(p.z * 7.0 + uTime * .4);
  }
  void main() {
    float n = field(position);
    float collapse = step(.5, uMode) * smoothstep(.35, 1.0, uProgress);
    vec3 displaced = position + normal * n * .055 * (1.0 + collapse * 3.4);
    vNoise = n;
    vPosition = displaced;
    vNormalW = normalize(normalMatrix * normal);
    gl_Position = projectionMatrix * modelViewMatrix * vec4(displaced, 1.0);
  }
`;

const fragmentShader = `
  uniform float uTime;
  uniform float uProgress;
  uniform float uMode;
  uniform vec3 uDeep;
  uniform vec3 uBright;
  varying vec3 vNormalW;
  varying vec3 vPosition;
  varying float vNoise;
  void main() {
    float rim = pow(1.0 - max(0.0, dot(vNormalW, vec3(0.0, 0.0, 1.0))), 2.2);
    float bands = sin(vPosition.y * 18.0 + sin(vPosition.x * 9.0) + uTime * .15);
    float cracks = smoothstep(.72, .98, abs(sin(vPosition.x * 15.0 + vPosition.y * 21.0 + vNoise * 6.0)));
    float danger = step(.5, uMode) * smoothstep(.22, .88, uProgress);
    vec3 color = mix(uDeep, uBright, smoothstep(-.8, .8, bands) * .45 + rim * .55);
    if(uMode < .5 && uProgress > .7){
      vec3 n=normalize(vPosition);
      float continent=sin(n.x*3.4+n.z*1.7)*sin(n.y*4.8-n.x*1.3)+sin(n.x*8.7+n.y*6.1+n.z*4.2)*.38;
      float land=smoothstep(-.08,.18,continent);
      float elevation=smoothstep(.25,.92,continent);
      float polar=smoothstep(.7,.93,abs(n.y));
      vec3 ocean=mix(vec3(.006,.055,.17),vec3(.015,.3,.48),rim*.45+.24);
      vec3 terrain=mix(vec3(.12,.31,.13),vec3(.31,.27,.19),elevation);
      terrain=mix(terrain,vec3(.86,.92,.94),max(polar,elevation*.72));
      color=mix(ocean,terrain,land);
      color+=vec3(.08,.3,.48)*rim*.45;
    }
    color += uBright * cracks * danger * 2.2;
    float alpha = 1.0;
    if (uMode > 6.5) alpha = 1.0 - smoothstep(.55, 1.0, uProgress) * (0.35 + cracks * .65);
    gl_FragColor = vec4(color, alpha);
  }
`;

function smooth(value, min, max) {
  return THREE.MathUtils.smoothstep(value, min, max);
}

function makeLowPolySurface() {
  const root = new THREE.Group();
  root.visible = false;

  const terrainGeometry = new THREE.PlaneGeometry(16, 11, 34, 24);
  const positions = terrainGeometry.attributes.position;
  const terrainColors = [];
  const low = new THREE.Color("#234b38");
  const high = new THREE.Color("#769052");
  for (let i = 0; i < positions.count; i++) {
    const x = positions.getX(i);
    const y = positions.getY(i);
    const height = Math.sin(x * .8) * .16 + Math.cos(y * 1.4) * .12 + Math.sin((x + y) * 2.2) * .06;
    positions.setZ(i, height);
    const color = low.clone().lerp(high, THREE.MathUtils.clamp((height + .25) * 1.7, 0, 1));
    terrainColors.push(color.r, color.g, color.b);
  }
  terrainGeometry.setAttribute("color", new THREE.Float32BufferAttribute(terrainColors, 3));
  terrainGeometry.computeVertexNormals();
  const terrain = new THREE.Mesh(terrainGeometry, new THREE.MeshStandardMaterial({ vertexColors: true, flatShading: true, roughness: .95 }));
  terrain.rotation.x = -Math.PI / 2;
  root.add(terrain);

  const water = new THREE.Mesh(
    new THREE.PlaneGeometry(17, 12),
    new THREE.MeshStandardMaterial({ color: 0x174f73, transparent: true, opacity: .82, roughness: .25, metalness: .08 }),
  );
  water.rotation.x = -Math.PI / 2;
  water.position.y = -.16;
  root.add(water);

  const riverCurve = new THREE.CatmullRomCurve3([
    new THREE.Vector3(-7, .05, -2.2), new THREE.Vector3(-4, .07, -1.2),
    new THREE.Vector3(-1.8, .08, -.2), new THREE.Vector3(1, .07, -.8),
    new THREE.Vector3(3.5, .05, .3), new THREE.Vector3(7, .03, 1.4),
  ]);
  const river = new THREE.Mesh(new THREE.TubeGeometry(riverCurve, 70, .12, 6, false), new THREE.MeshBasicMaterial({ color: 0x4eb5db }));
  root.add(river);

  const mountainMaterial = new THREE.MeshStandardMaterial({ color: 0x5b665b, flatShading: true, roughness: 1 });
  const snowMaterial = new THREE.MeshStandardMaterial({ color: 0xd8e7e6, flatShading: true });
  [[-4.4, -1.2, 1.5], [-3.3, -.6, 1], [3.8, 1.2, 1.35], [4.9, .5, .95]].forEach(([x, z, size]) => {
    const mountain = new THREE.Mesh(new THREE.ConeGeometry(size, size * 2.1, 6), mountainMaterial);
    mountain.position.set(x, size * .72, z);
    root.add(mountain);
    const snow = new THREE.Mesh(new THREE.ConeGeometry(size * .48, size * .62, 6), snowMaterial);
    snow.position.set(x, size * 1.52, z);
    root.add(snow);
  });

  const trunkMaterial = new THREE.MeshStandardMaterial({ color: 0x513820, flatShading: true });
  const leafMaterials = [0x397b43, 0x4f913f, 0x79a44b].map((color) => new THREE.MeshStandardMaterial({ color, flatShading: true }));
  for (let i = 0; i < 74; i++) {
    const tree = new THREE.Group();
    const height = .18 + Math.random() * .26;
    const trunk = new THREE.Mesh(new THREE.CylinderGeometry(.025, .038, height, 5), trunkMaterial);
    trunk.position.y = height / 2;
    const crown = new THREE.Mesh(new THREE.ConeGeometry(.11 + Math.random() * .09, .28 + Math.random() * .22, 5), leafMaterials[i % leafMaterials.length]);
    crown.position.y = height + .13;
    tree.add(trunk, crown);
    let x = -6.8 + Math.random() * 13.6;
    let z = -4.2 + Math.random() * 8.4;
    if (Math.abs(x) < 1.3 && Math.abs(z) < 1.2) x += 2.1;
    tree.position.set(x, -.02, z);
    tree.rotation.y = Math.random() * Math.PI;
    tree.scale.setScalar(0);
    vegetation.push(tree);
    root.add(tree);
  }

  const animalMaterial = new THREE.MeshStandardMaterial({ color: 0xd4b46f, flatShading: true });
  for (let i = 0; i < 14; i++) {
    const creature = new THREE.Group();
    const body = new THREE.Mesh(new THREE.IcosahedronGeometry(.08, 0), animalMaterial);
    body.scale.set(1.5, .8, .8);
    body.position.y = .12;
    creature.add(body);
    creature.position.set(-3 + Math.random() * 6, 0, -1.5 + Math.random() * 3);
    creature.scale.setScalar(0);
    lifeForms.push(creature);
    root.add(creature);
  }

  const wallMaterial = new THREE.MeshStandardMaterial({ color: 0xc7a86c, flatShading: true });
  const roofMaterial = new THREE.MeshStandardMaterial({ color: 0x813c2f, flatShading: true });
  for (let i = 0; i < 18; i++) {
    const house = new THREE.Group();
    const size = .12 + Math.random() * .11;
    const wall = new THREE.Mesh(new THREE.BoxGeometry(size * 1.4, size, size), wallMaterial);
    wall.position.y = size / 2;
    const roof = new THREE.Mesh(new THREE.ConeGeometry(size, size * .65, 4), roofMaterial);
    roof.rotation.y = Math.PI / 4;
    roof.position.y = size * 1.25;
    house.add(wall, roof);
    house.position.set(-1.3 + Math.random() * 2.6, .02, -1 + Math.random() * 1.8);
    house.scale.setScalar(0);
    civilization.push(house);
    root.add(house);
  }

  const cloudMaterial = new THREE.MeshStandardMaterial({ color: 0xc8d4dd, transparent: true, opacity: .72, roughness: 1 });
  for (let i = 0; i < 9; i++) {
    const cloud = new THREE.Group();
    for (let j = 0; j < 5; j++) {
      const puff = new THREE.Mesh(new THREE.IcosahedronGeometry(.3 + Math.random() * .25, 1), cloudMaterial);
      puff.position.set((j - 2) * .27, Math.random() * .15, Math.random() * .18);
      puff.scale.y = .6;
      cloud.add(puff);
    }
    cloud.position.set(-6 + Math.random() * 12, 2.3 + Math.random(), -3.5 + Math.random() * 6);
    stormClouds.push(cloud);
    root.add(cloud);
  }

  const rainPositions = new Float32Array(700 * 6);
  for (let i = 0; i < rainPositions.length; i += 6) {
    const x = -7 + Math.random() * 14;
    const y = .2 + Math.random() * 4;
    const z = -4 + Math.random() * 8;
    rainPositions.set([x, y, z, x - .04, y - .28, z], i);
  }
  const rainGeometry = new THREE.BufferGeometry();
  rainGeometry.setAttribute("position", new THREE.BufferAttribute(rainPositions, 3));
  rain = new THREE.LineSegments(rainGeometry, new THREE.LineBasicMaterial({ color: 0x89c9ef, transparent: true, opacity: .45 }));
  root.add(rain);

  const lightningGeometry = new THREE.BufferGeometry().setFromPoints([
    new THREE.Vector3(2.8, 4, -.8), new THREE.Vector3(2.55, 3.2, -.7),
    new THREE.Vector3(2.8, 2.7, -.6), new THREE.Vector3(2.45, 1.8, -.55),
    new THREE.Vector3(2.6, .8, -.45),
  ]);
  lightning = new THREE.Line(lightningGeometry, new THREE.LineBasicMaterial({ color: 0xe5f8ff, transparent: true, opacity: 0 }));
  root.add(lightning);

  const volcano = new THREE.Mesh(new THREE.ConeGeometry(.75, 1.5, 7, 1, true), new THREE.MeshStandardMaterial({ color: 0x372a27, flatShading: true }));
  volcano.position.set(3.2, .62, -2.2);
  root.add(volcano);
  volcanoGlow = new THREE.Mesh(new THREE.IcosahedronGeometry(.22, 1), new THREE.MeshBasicMaterial({ color: 0xff5c28, transparent: true, opacity: 0 }));
  volcanoGlow.position.set(3.2, 1.42, -2.2);
  root.add(volcanoGlow);

  surfaceSun = new THREE.Mesh(new THREE.IcosahedronGeometry(.42, 2), new THREE.MeshBasicMaterial({ color: 0xffdb74 }));
  surfaceMoon = new THREE.Mesh(new THREE.IcosahedronGeometry(.22, 1), new THREE.MeshBasicMaterial({ color: 0xcfe2ec }));
  root.add(surfaceSun, surfaceMoon);
  return root;
}

function makeSphericalSurface() {
  const root = new THREE.Group();
  root.visible = false;
  const radius = 5;

  const terrainVertex = `
    uniform float uFormation;
    varying float vHeight;
    varying vec3 vNormalW;
    varying vec3 vPoint;
    float terrain(vec3 n){
      float h=sin(n.x*3.7+n.z*1.3)*sin(n.y*4.9-n.x*1.7)*.13;
      h+=sin(n.x*9.2+n.y*7.4+n.z*5.3)*.055;
      h+=sin(n.x*21.0-n.z*17.0)*sin(n.y*16.0)*.022;
      return h-.018;
    }
    void main(){
      vec3 n=normalize(position);
      vHeight=terrain(n)*uFormation;
      vec3 displaced=n*(5.0+vHeight);
      vPoint=n;
      vNormalW=normalize(normalMatrix*normal);
      gl_Position=projectionMatrix*modelViewMatrix*vec4(displaced,1.0);
    }
  `;
  const terrainFragment = `
    uniform float uFormation;
    uniform float uLife;
    uniform vec3 uSunDirection;
    varying float vHeight;
    varying vec3 vNormalW;
    varying vec3 vPoint;
    void main(){
      float slope=1.0-max(0.0,dot(vNormalW,normalize(vPoint)));
      float beach=smoothstep(-.015,.012,vHeight);
      float upland=smoothstep(.09,.17,vHeight);
      float alpine=smoothstep(.16,.235,vHeight);
      float polar=smoothstep(.68,.92,abs(vPoint.y));
      float forestNoise=sin(vPoint.x*61.0)*sin(vPoint.y*47.0)*sin(vPoint.z*53.0)*.5+.5;
      vec3 sand=vec3(.58,.49,.31);
      vec3 grass=mix(vec3(.055,.26,.09),vec3(.24,.52,.15),forestNoise)*mix(.48,1.0,uLife);
      vec3 rock=vec3(.27,.28,.25);
      vec3 snow=vec3(.82,.9,.93);
      vec3 color=mix(sand,grass,beach);
      color=mix(color,rock,clamp(upland+slope*.9,0.0,1.0));
      color=mix(color,snow,max(alpine,polar*.78));
      float daylight=max(0.0,dot(vNormalW,normalize(uSunDirection)));
      float sunlight=.055+daylight*.98;
      gl_FragColor=vec4(color*sunlight,uFormation);
    }
  `;
  surfaceTerrain = new THREE.Mesh(
    new THREE.SphereGeometry(radius, 160, 104),
    new THREE.ShaderMaterial({ transparent: true, uniforms: { uFormation: { value: 0 }, uLife: { value: 0 },uSunDirection:{value:new THREE.Vector3(1,1,1).normalize()} }, vertexShader: terrainVertex, fragmentShader: terrainFragment }),
  );
  root.add(surfaceTerrain);

  surfaceWater = new THREE.Mesh(
    new THREE.SphereGeometry(radius + .012, 128, 80),
    new THREE.ShaderMaterial({
      uniforms: { uTime: { value: 0 }, uFormation: { value: 0 }, uSunDirection: { value: new THREE.Vector3(1,1,1).normalize() } },
      vertexShader: `uniform float uTime;varying vec3 vN;varying vec3 vWorld;varying vec3 vPoint;varying float vWave;void main(){vec3 n=normalize(position);float wave=(sin(n.x*58.0+uTime*1.1)+sin(n.z*71.0-uTime*.8)+sin((n.x+n.y)*43.0+uTime*.55))*.004;vWave=wave;vPoint=n;vec3 p=n*(5.012+wave);vN=normalize(normalMatrix*n);vec4 world=modelMatrix*vec4(p,1.0);vWorld=world.xyz;gl_Position=projectionMatrix*viewMatrix*world;}`,
      fragmentShader: `uniform float uFormation;uniform vec3 uSunDirection;varying vec3 vN;varying vec3 vWorld;varying vec3 vPoint;varying float vWave;float terrain(vec3 n){float h=sin(n.x*3.7+n.z*1.3)*sin(n.y*4.9-n.x*1.7)*.13;h+=sin(n.x*9.2+n.y*7.4+n.z*5.3)*.055;h+=sin(n.x*21.0-n.z*17.0)*sin(n.y*16.0)*.022;return h-.018;}void main(){if(uFormation<.05||terrain(normalize(vPoint))>.006)discard;vec3 viewDir=normalize(cameraPosition-vWorld);float fresnel=pow(1.0-max(0.0,dot(viewDir,vN)),4.0);float diffuse=max(0.0,dot(vN,uSunDirection));vec3 halfDir=normalize(viewDir+uSunDirection);float spec=pow(max(0.0,dot(vN,halfDir)),105.0);vec3 deep=vec3(.004,.035,.22);vec3 mid=vec3(.008,.14,.42);vec3 cobalt=vec3(.025,.32,.68);vec3 water=mix(deep,mid,.3+diffuse*.42+vWave*9.0);water=mix(water,cobalt,fresnel*.32);water+=vec3(.72,.9,1.0)*spec*.72;gl_FragColor=vec4(water,1.0);}`,
    }),
  );
  surfaceWater.renderOrder = 2;
  root.add(surfaceWater);

  surfaceAtmosphere = new THREE.Mesh(
    new THREE.SphereGeometry(radius + .68, 112, 72),
    new THREE.ShaderMaterial({
      transparent: true, side: THREE.DoubleSide, depthWrite: false,
      uniforms: { uDensity: { value: 0 } },
      vertexShader: `varying vec3 vN;varying vec3 vWorld;void main(){vN=normalize(normalMatrix*normal);vec4 w=modelMatrix*vec4(position,1.0);vWorld=w.xyz;gl_Position=projectionMatrix*viewMatrix*w;}`,
      fragmentShader: `uniform float uDensity;varying vec3 vN;varying vec3 vWorld;void main(){vec3 viewDir=normalize(cameraPosition-vWorld);float horizon=pow(1.0-abs(dot(viewDir,vN)),2.7);vec3 sky=mix(vec3(.035,.18,.55),vec3(.38,.72,1.0),horizon);gl_FragColor=vec4(sky,(.008+horizon*.16)*uDensity);}`,
    }),
  );
  root.add(surfaceAtmosphere);

  surfaceSky=new THREE.Mesh(
    new THREE.SphereGeometry(32,64,40),
    new THREE.ShaderMaterial({
      side:THREE.BackSide,depthWrite:false,depthTest:false,
      uniforms:{uUp:{value:new THREE.Vector3(0,1,0)},uSunDirection:{value:new THREE.Vector3(1,1,1)},uDaylight:{value:1},uStorm:{value:0}},
      vertexShader:`varying vec3 vDir;void main(){vDir=normalize(position);gl_Position=projectionMatrix*modelViewMatrix*vec4(position,1.0);}`,
      fragmentShader:`uniform vec3 uUp;uniform vec3 uSunDirection;uniform float uDaylight;uniform float uStorm;varying vec3 vDir;void main(){float altitude=dot(normalize(vDir),normalize(uUp));float horizon=1.0-smoothstep(-.16,.7,altitude);vec3 nightZenith=vec3(.001,.004,.018);vec3 nightHorizon=vec3(.008,.035,.09);vec3 dayZenith=vec3(.012,.34,.82);vec3 dayHorizon=vec3(.58,.88,1.0);vec3 night=mix(nightZenith,nightHorizon,pow(horizon,.8));vec3 day=mix(dayZenith,dayHorizon,pow(horizon,.64));float sunGlow=pow(max(0.0,dot(normalize(vDir),normalize(uSunDirection))),96.0);vec3 color=mix(night,day,smoothstep(.015,.36,uDaylight));color+=vec3(1.0,.68,.34)*sunGlow*uDaylight*.68;color=mix(color,vec3(.1,.15,.21),uStorm*.48);gl_FragColor=vec4(color,1.0);}`,
    }),
  );
  surfaceSky.renderOrder=-100;
  root.add(surfaceSky);

  const milkyCount=22000;const milkyPositions=new Float32Array(milkyCount*3);const milkyColors=new Float32Array(milkyCount*3);const milkySizes=new Float32Array(milkyCount);
  const milkyPalette=[new THREE.Color(0x91b8ff),new THREE.Color(0xd8e4ff),new THREE.Color(0xffe5c4),new THREE.Color(0xd798c9)];
  for(let i=0;i<milkyCount;i++){const isCore=i<milkyCount*.27;const longitude=isCore?.42+(Math.random()-.5)*Math.pow(Math.random(),1.7)*1.55:Math.random()*Math.PI*2;const coreEnvelope=Math.exp(-Math.pow((longitude-.42)/.62,2));const widthWave=.5+.5*Math.sin(longitude*1.65+.7)+.24*Math.sin(longitude*4.3);const bandWidth=.022+Math.pow(Math.max(.05,widthWave),1.55)*.07+coreEnvelope*.13;const gaussian=(Math.random()+Math.random()+Math.random()+Math.random()-2)*.5;const warpedCenter=Math.sin(longitude*1.7)*.025+Math.sin(longitude*3.9+1.2)*.012;const latitude=warpedCenter+gaussian*bandWidth*(isCore?1.35:1);const r=238+Math.random()*5;milkyPositions.set([Math.cos(latitude)*Math.cos(longitude)*r,Math.sin(latitude)*r,Math.cos(latitude)*Math.sin(longitude)*r],i*3);const dustLane=Math.abs(latitude-warpedCenter)<bandWidth*.16?.46:1;const c=(isCore?new THREE.Color(0xffefd1):milkyPalette[i%milkyPalette.length]).clone().multiplyScalar((isCore?.72+Math.random()*.68:.34+Math.random()*.72)*dustLane);milkyColors.set([c.r,c.g,c.b],i*3);milkySizes[i]=isCore?.65+Math.random()*2.1:.2+Math.random()*.9;}
  const milkyGeometry=new THREE.BufferGeometry();milkyGeometry.setAttribute("position",new THREE.BufferAttribute(milkyPositions,3));milkyGeometry.setAttribute("aColor",new THREE.BufferAttribute(milkyColors,3));milkyGeometry.setAttribute("aSize",new THREE.BufferAttribute(milkySizes,1));
  surfaceMilkyWay=new THREE.Points(milkyGeometry,new THREE.ShaderMaterial({transparent:true,depthWrite:false,blending:THREE.AdditiveBlending,uniforms:{uOpacity:{value:0}},vertexShader:`attribute vec3 aColor;attribute float aSize;varying vec3 vColor;void main(){vColor=aColor;vec4 mv=modelViewMatrix*vec4(position,1.0);gl_PointSize=min(4.2,(.65+aSize*1.55)*(240.0/max(1.0,-mv.z)));gl_Position=projectionMatrix*mv;}`,fragmentShader:`uniform float uOpacity;varying vec3 vColor;void main(){vec2 q=gl_PointCoord-.5;float r=length(q);if(r>.5)discard;float glow=exp(-r*7.0);gl_FragColor=vec4(vColor,glow*uOpacity);}`}));
  surfaceMilkyWay.renderOrder=-90;root.add(surfaceMilkyWay);

  surfaceMoon=new THREE.Mesh(
    new THREE.SphereGeometry(.48,96,64),
    new THREE.ShaderMaterial({
      uniforms:{uSunDirection:{value:new THREE.Vector3(1,1,1)}},
      vertexShader:`varying vec3 vN;varying vec3 vP;void main(){vN=normalize(mat3(modelMatrix)*normal);vP=normalize(position);gl_Position=projectionMatrix*modelViewMatrix*vec4(position,1.0);}`,
      fragmentShader:`uniform vec3 uSunDirection;varying vec3 vN;varying vec3 vP;float lunar(vec3 p){float a=sin(p.x*43.0+p.y*17.0)*sin(p.z*51.0-p.y*29.0);float b=sin(p.x*91.0-p.z*73.0)*sin(p.y*67.0)*.35;return a+b;}void main(){float field=lunar(vP);float maria=smoothstep(.26,.68,field);float fine=sin(vP.x*173.0+vP.z*139.0)*.035;vec3 rock=mix(vec3(.21,.22,.22),vec3(.69,.68,.64),1.0-maria*.55)+fine;float lambert=max(0.0,dot(normalize(vN),normalize(uSunDirection)));float rim=pow(1.0-max(0.0,dot(normalize(vN),vec3(0.0,0.0,1.0))),3.0);vec3 color=rock*(.012+lambert*.98)+vec3(.08,.1,.13)*rim*.08;gl_FragColor=vec4(color,1.0);}`,
    }),
  );
  root.add(surfaceMoon);

  function terrainHeightAt(n) {
    return Math.sin(n.x*3.7+n.z*1.3)*Math.sin(n.y*4.9-n.x*1.7)*.13 + Math.sin(n.x*9.2+n.y*7.4+n.z*5.3)*.055 + Math.sin(n.x*21-n.z*17)*Math.sin(n.y*16)*.022 - .018;
  }
  function spherePoint(lat, lon, r) {
    return new THREE.Vector3(Math.cos(lat)*Math.cos(lon), Math.sin(lat), Math.cos(lat)*Math.sin(lon)).multiplyScalar(r);
  }
  const up = new THREE.Vector3(0,1,0);
  const matrix = new THREE.Matrix4();
  const quaternion = new THREE.Quaternion();
  const yawQuaternion = new THREE.Quaternion();
  const position = new THREE.Vector3();
  const scale = new THREE.Vector3();

  const mountainCount = 190;
  const mountainGeometry=new THREE.CylinderGeometry(.025,.16,.34,10,5,false);
  const mountainVertices=mountainGeometry.attributes.position;
  for(let vertexIndex=0;vertexIndex<mountainVertices.count;vertexIndex++){
    const x=mountainVertices.getX(vertexIndex);const y=mountainVertices.getY(vertexIndex);const z=mountainVertices.getZ(vertexIndex);
    const angle=Math.atan2(z,x);const ridge=1+Math.sin(angle*3+y*17)*.12+Math.sin(angle*7-y*11)*.045;
    mountainVertices.setXYZ(vertexIndex,x*ridge,y+Math.sin(angle*4+y*9)*.008,z*ridge);
  }
  mountainGeometry.computeVertexNormals();
  const snowGeometry=new THREE.CylinderGeometry(.018,.082,.13,9,2,false);
  const snowVertices=snowGeometry.attributes.position;
  for(let vertexIndex=0;vertexIndex<snowVertices.count;vertexIndex++){
    const x=snowVertices.getX(vertexIndex);const y=snowVertices.getY(vertexIndex);const z=snowVertices.getZ(vertexIndex);const ridge=1+Math.sin(Math.atan2(z,x)*5+y*19)*.1;snowVertices.setXYZ(vertexIndex,x*ridge,y,z*ridge);
  }
  snowGeometry.computeVertexNormals();
  const mountainSnowFactors=new Float32Array(mountainCount);
  mountainGeometry.setAttribute("aSnow",new THREE.InstancedBufferAttribute(mountainSnowFactors,1));
  const mountainMaterial=new THREE.ShaderMaterial({
    transparent:true,
    uniforms:{uOpacity:{value:0},uSunDirection:{value:new THREE.Vector3(1,1,1).normalize()}},
    vertexShader:`attribute float aSnow;varying float vHeight;varying float vSnow;varying vec3 vNormalW;void main(){vHeight=clamp(position.y/.34+.5,0.0,1.0);vSnow=aSnow;vec4 instancePosition=instanceMatrix*vec4(position,1.0);vNormalW=normalize(mat3(modelMatrix)*mat3(instanceMatrix)*normal);gl_Position=projectionMatrix*modelViewMatrix*instancePosition;}`,
    fragmentShader:`uniform float uOpacity;uniform vec3 uSunDirection;varying float vHeight;varying float vSnow;varying vec3 vNormalW;void main(){float strata=sin(vHeight*58.0+vNormalW.x*9.0+vNormalW.z*13.0)*.5+.5;vec3 lower=mix(vec3(.24,.27,.23),vec3(.38,.40,.34),strata*.42);vec3 upper=vec3(.31,.33,.31);vec3 rock=mix(lower,upper,smoothstep(.42,.82,vHeight));float topFacing=smoothstep(.08,.58,dot(normalize(vNormalW),normalize(vec3(0.0,1.0,0.0))));float snowLine=smoothstep(.67,.88,vHeight)*vSnow;float snowMask=clamp(snowLine*(.72+topFacing*.28),0.0,1.0);vec3 snow=mix(vec3(.72,.79,.82),vec3(.96,.985,1.0),strata*.28+vHeight*.4);vec3 color=mix(rock,snow,snowMask);float daylight=max(0.0,dot(normalize(vNormalW),normalize(uSunDirection)));float lighting=.34+daylight*.78;gl_FragColor=vec4(color*lighting,uOpacity);}`
  });
  surfaceMountains = new THREE.InstancedMesh(mountainGeometry,mountainMaterial,mountainCount);
  surfaceSnowCaps = new THREE.InstancedMesh(snowGeometry,new THREE.MeshBasicMaterial({color:0xf2f7f8,transparent:true,toneMapped:false}),mountainCount);
  for(let i=0;i<mountainCount;i++){
    const belt=i%3;const lon=(i/mountainCount)*Math.PI*8+(belt*.7);const lat=(belt-1)*.48+Math.sin(i*.73)*.11;
    const n=spherePoint(lat,lon,1).normalize();const h=i<11?.25+Math.random()*.22:.065+Math.random()*.12;mountainSnowFactors[i]=i<11?1:0;const terrainH=terrainHeightAt(n);const baseR=terrainH<=.012?radius-h*1.05:radius+terrainH-Math.min(.035,h*.22);
    quaternion.setFromUnitVectors(up,n);yawQuaternion.setFromAxisAngle(up,Math.random()*Math.PI*2);quaternion.multiply(yawQuaternion);
    position.copy(n).multiplyScalar(baseR+h*.32);scale.set(.7+Math.random()*.58,h/.34,.7+Math.random()*.62);matrix.compose(position,quaternion,scale);surfaceMountains.setMatrixAt(i,matrix);
    position.copy(n).multiplyScalar(baseR+h*.7);scale.set(.64+Math.random()*.28,h/.34*.4,.64+Math.random()*.28);matrix.compose(position,quaternion,scale);surfaceSnowCaps.setMatrixAt(i,matrix);
  }
  mountainGeometry.attributes.aSnow.needsUpdate=true;
  surfaceSnowCaps.count=11;
  root.add(surfaceMountains,surfaceSnowCaps);

  function makeCrossedPlantGeometry(width,height,planes=4){const vertices=[],uvs=[];for(let plane=0;plane<planes;plane++){const a=plane*Math.PI/planes;const dx=Math.cos(a)*width*.5,dz=Math.sin(a)*width*.5;vertices.push(-dx,0,-dz,dx,0,dz,dx,height,dz,-dx,0,-dz,dx,height,dz,-dx,height,-dz);uvs.push(0,0,1,0,1,1,0,0,1,1,0,1);}const geometry=new THREE.BufferGeometry();geometry.setAttribute("position",new THREE.Float32BufferAttribute(vertices,3));geometry.setAttribute("uv",new THREE.Float32BufferAttribute(uvs,2));geometry.computeVertexNormals();return geometry;}
  function makePlantTexture(isShrub=false){const canvas=document.createElement("canvas");canvas.width=128;canvas.height=128;const ctx=canvas.getContext("2d");ctx.clearRect(0,0,128,128);if(!isShrub){ctx.fillStyle="#855b32";ctx.fillRect(58,62,12,66);}const greens=isShrub?["#2e8f45","#4fb455","#78c75f"]:["#36a852","#58c45f","#7bd36a","#2d9149"];for(let i=0;i<(isShrub?18:28);i++){ctx.beginPath();ctx.fillStyle=greens[i%greens.length];const x=64+(Math.random()-.5)*(isShrub?88:78);const y=(isShrub?66:18)+Math.random()*(isShrub?50:76);const r=(isShrub?13:11)+Math.random()*12;ctx.ellipse(x,y,r,r*(.65+Math.random()*.45),Math.random()*Math.PI,0,Math.PI*2);ctx.fill();}const texture=new THREE.CanvasTexture(canvas);texture.colorSpace=THREE.SRGBColorSpace;texture.premultiplyAlpha=true;texture.minFilter=THREE.LinearMipmapLinearFilter;texture.magFilter=THREE.LinearFilter;texture.needsUpdate=true;return texture;}
  const treeCount=2500;
  const treeTexture=makePlantTexture(false);
  surfaceForest=new THREE.InstancedMesh(makeCrossedPlantGeometry(.035,.072,4),new THREE.MeshBasicMaterial({map:treeTexture,color:0xffffff,transparent:true,alphaTest:.35,depthWrite:true,side:THREE.DoubleSide,vertexColors:false,toneMapped:false,opacity:0}),treeCount);
  let placed=0,attempts=0;
  while(placed<treeCount&&attempts<treeCount*8){attempts++;const lat=(Math.random()-.5)*2.25;const lon=Math.random()*Math.PI*2;const n=spherePoint(lat,lon,1).normalize();const h=terrainHeightAt(n);if(h<.018||h>.145||Math.abs(lat)>1.05)continue;quaternion.setFromUnitVectors(up,n);position.copy(n).multiplyScalar(radius+h+.002);const s=.55+Math.random()*.7;scale.setScalar(s);matrix.compose(position,quaternion,scale);surfaceForest.setMatrixAt(placed,matrix);placed++;}
  surfaceForest.count=placed;
  root.add(surfaceForest);

  const shrubTexture=makePlantTexture(true);const shrubCount=1800;surfaceShrubs=new THREE.InstancedMesh(makeCrossedPlantGeometry(.035,.026,4),new THREE.MeshBasicMaterial({map:shrubTexture,color:0xffffff,transparent:true,alphaTest:.35,depthWrite:true,side:THREE.DoubleSide,vertexColors:false,toneMapped:false,opacity:0}),shrubCount);let shrubPlaced=0,shrubAttempts=0;
  while(shrubPlaced<shrubCount&&shrubAttempts<shrubCount*8){shrubAttempts++;const lat=(Math.random()-.5)*2.2;const lon=Math.random()*Math.PI*2;const n=spherePoint(lat,lon,1).normalize();const h=terrainHeightAt(n);if(h<.014||h>.15)continue;quaternion.setFromUnitVectors(up,n);yawQuaternion.setFromAxisAngle(up,Math.random()*Math.PI*2);quaternion.multiply(yawQuaternion);position.copy(n).multiplyScalar(radius+h+.001);scale.setScalar(.55+Math.random()*.65);matrix.compose(position,quaternion,scale);surfaceShrubs.setMatrixAt(shrubPlaced,matrix);shrubPlaced++;}surfaceShrubs.count=shrubPlaced;root.add(surfaceShrubs);

  const cloudCount=1200;const cloudPositions=new Float32Array(cloudCount*3);const cloudNormals=new Float32Array(cloudCount*3);const cloudSizes=new Float32Array(cloudCount);
  const cloudClusters=Array.from({length:72},()=>({lat:(Math.random()-.5)*1.85,lon:Math.random()*Math.PI*2,spread:.022+Math.random()*.082}));
  for(let i=0;i<cloudCount;i++){const cluster=cloudClusters[i%cloudClusters.length];const lat=cluster.lat+(Math.random()-.5)*cluster.spread;const lon=cluster.lon+(Math.random()-.5)*cluster.spread*2.5;const n=spherePoint(lat,lon,1).normalize();const pos=n.clone().multiplyScalar(radius+.34+Math.random()*.09);cloudPositions.set([pos.x,pos.y,pos.z],i*3);cloudNormals.set([n.x,n.y,n.z],i*3);cloudSizes[i]=46+Math.random()*88;}
  const cloudGeometry=new THREE.BufferGeometry();cloudGeometry.setAttribute("position",new THREE.BufferAttribute(cloudPositions,3));cloudGeometry.setAttribute("aNormal",new THREE.BufferAttribute(cloudNormals,3));cloudGeometry.setAttribute("aSize",new THREE.BufferAttribute(cloudSizes,1));
  surfaceCloudLayer=new THREE.Points(cloudGeometry,new THREE.ShaderMaterial({
    transparent:true,depthWrite:false,
    uniforms:{uSunDirection:{value:new THREE.Vector3(1,1,1)},uOpacity:{value:0}},
    vertexShader:`attribute float aSize;attribute vec3 aNormal;varying vec3 vNormal;void main(){vNormal=normalize(normalMatrix*aNormal);vec4 mv=modelViewMatrix*vec4(position,1.0);gl_PointSize=min(42.0,aSize/max(1.0,-mv.z));gl_Position=projectionMatrix*mv;}`,
    fragmentShader:`uniform vec3 uSunDirection;uniform float uOpacity;varying vec3 vNormal;void main(){vec2 q=gl_PointCoord-.5;float r=length(q);float n1=sin(q.x*21.0+sin(q.y*13.0))*sin(q.y*17.0-q.x*7.0);float n2=sin(q.x*49.0-q.y*37.0)*.45+sin((q.x+q.y)*83.0)*.18;float edge=r+n1*.045+n2*.018;float body=smoothstep(.53,.18,edge);float vapor=smoothstep(.56,.34,edge)*(.72+n2*.18);float alpha=max(body,vapor*.72);float light=.14+max(0.0,dot(vNormal,normalize(uSunDirection)))*.86;vec3 shadow=vec3(.19,.25,.33);vec3 lit=vec3(1.0,.985,.95);vec3 color=mix(shadow,lit,light);color+=n1*.035;gl_FragColor=vec4(color,alpha*uOpacity);}`,
  }));
  surfaceCloudLayer.visible=false;
  root.add(surfaceCloudLayer);

  // Early-world weather is wrapped around the spherical surface, so the storm
  // remains visible from every latitude instead of behaving like a flat overlay.
  const rainPositions=new Float32Array(3000*6);
  for(let i=0;i<3000;i++){
    const cluster=cloudClusters[(i*7)%cloudClusters.length];const lat=cluster.lat+(Math.random()-.5)*cluster.spread*.92;const lon=cluster.lon+(Math.random()-.5)*cluster.spread*2.15;const n=spherePoint(lat,lon,1).normalize();
    const tangent=new THREE.Vector3(-n.z,0,n.x).normalize().multiplyScalar(.025+Math.random()*.035);
    const start=n.clone().multiplyScalar(radius+.27+Math.random()*.1);const end=start.clone().addScaledVector(n,-(.17+Math.random()*.22)).add(tangent);
    rainPositions.set([start.x,start.y,start.z,end.x,end.y,end.z],i*6);
  }
  const rainGeometry=new THREE.BufferGeometry();rainGeometry.setAttribute("position",new THREE.BufferAttribute(rainPositions,3));
  surfaceRain=new THREE.LineSegments(rainGeometry,new THREE.LineBasicMaterial({color:0x9fc7db,transparent:true,opacity:0,depthWrite:false}));
  root.add(surfaceRain);

  surfaceLightningGroup=new THREE.Group();
  for(let i=0;i<28;i++){
    const stormCell=new THREE.Group();const mainPoints=[];
    for(let j=0;j<13;j++)mainPoints.push(new THREE.Vector3((Math.random()-.5)*.11,-j*.052,(Math.random()-.5)*.11));
    const lightningMaterial=()=>new THREE.LineBasicMaterial({color:Math.random()>.35?0xf1f8ff:0xaedcff,transparent:true,opacity:0,blending:THREE.AdditiveBlending,depthWrite:false});
    const mainBolt=new THREE.Line(new THREE.BufferGeometry().setFromPoints(mainPoints),lightningMaterial());mainBolt.userData.strength=1;stormCell.add(mainBolt);
    const branches=3+Math.floor(Math.random()*4);
    for(let branch=0;branch<branches;branch++){const startIndex=3+Math.floor(Math.random()*7);const start=mainPoints[startIndex].clone();const branchPoints=[start];const direction=new THREE.Vector3((Math.random()-.5)*.34,-(.14+Math.random()*.28),(Math.random()-.5)*.34);for(let j=1;j<7;j++){const t=j/6;branchPoints.push(start.clone().addScaledVector(direction,t).add(new THREE.Vector3((Math.random()-.5)*.055,(Math.random()-.5)*.025,(Math.random()-.5)*.055)));}const branchBolt=new THREE.Line(new THREE.BufferGeometry().setFromPoints(branchPoints),lightningMaterial());branchBolt.userData.strength=.38+Math.random()*.34;stormCell.add(branchBolt);}
    const lat=(Math.random()-.5)*2.1;const lon=Math.random()*Math.PI*2;const n=spherePoint(lat,lon,1).normalize();stormCell.position.copy(n).multiplyScalar(radius+.68);stormCell.quaternion.setFromUnitVectors(up,n);stormCell.userData.phase=Math.random()*40;surfaceLightningGroup.add(stormCell);
  }
  root.add(surfaceLightningGroup);

  surfaceVolcanoGroup=new THREE.Group();
  const volcanoRock=new THREE.MeshBasicMaterial({color:0x66564d,toneMapped:false});
  let volcanoPlaced=0,volcanoAttempts=0;
  while(volcanoPlaced<28&&volcanoAttempts<420){
    volcanoAttempts++;
    const lat=(Math.random()-.5)*1.7;const lon=volcanoAttempts*.91+Math.random()*.45;const n=spherePoint(lat,lon,1).normalize();const terrainH=terrainHeightAt(n);
    if(terrainH<.022||terrainH>.145)continue;
    const volcano=new THREE.Group();const height=.075+Math.random()*.085;const baseRadius=.18+Math.random()*.1;const craterRadius=.045+Math.random()*.018;
    const cone=new THREE.Mesh(new THREE.CylinderGeometry(craterRadius,baseRadius,height,18,5,false),volcanoRock);cone.position.y=height*.42;volcano.add(cone);
    const crater=new THREE.Mesh(new THREE.TorusGeometry(craterRadius*.92,.009,8,28),new THREE.MeshBasicMaterial({color:0xff4b16,transparent:true,opacity:0,blending:THREE.AdditiveBlending,depthWrite:false,toneMapped:false}));crater.rotation.x=Math.PI/2;crater.position.y=height*.92;crater.userData.isLava=true;volcano.add(crater);
    for(let stream=0;stream<2;stream++){const a=stream*Math.PI+Math.random();const curve=new THREE.CatmullRomCurve3([new THREE.Vector3(Math.cos(a)*craterRadius*.7,height*.9,Math.sin(a)*craterRadius*.7),new THREE.Vector3(Math.cos(a)*baseRadius*.48,height*.42,Math.sin(a)*baseRadius*.48),new THREE.Vector3(Math.cos(a)*baseRadius*.92,-.006,Math.sin(a)*baseRadius*.92)]);const lava=new THREE.Mesh(new THREE.TubeGeometry(curve,18,.008,5,false),new THREE.MeshBasicMaterial({color:0xff3a0a,transparent:true,opacity:0,blending:THREE.AdditiveBlending,depthWrite:false,toneMapped:false}));lava.userData.isLava=true;volcano.add(lava);}
    volcano.position.copy(n).multiplyScalar(radius+terrainH-Math.min(.018,height*.14));volcano.quaternion.setFromUnitVectors(up,n);volcano.rotateY(Math.random()*Math.PI*2);volcano.userData.phase=Math.random()*12;surfaceVolcanoGroup.add(volcano);volcanoPlaced++;
  }
  root.add(surfaceVolcanoGroup);

  function makeDinosaur(index){
    const dino=new THREE.Group();const skin=new THREE.MeshStandardMaterial({color:[0x546f35,0x776037,0x4d7554][index%3],roughness:.95,flatShading:true});
    const torso=new THREE.Mesh(new THREE.SphereGeometry(.075,10,7),skin);torso.scale.set(1.75,.75,.72);torso.position.y=.09;dino.add(torso);
    const neck=new THREE.Mesh(new THREE.CylinderGeometry(.018,.035,.15,7),skin);neck.rotation.z=-.55;neck.position.set(.105,.16,0);dino.add(neck);
    const head=new THREE.Mesh(new THREE.SphereGeometry(.035,8,6),skin);head.position.set(.16,.225,0);dino.add(head);
    const tail=new THREE.Mesh(new THREE.ConeGeometry(.038,.27,7),skin);tail.rotation.z=-Math.PI/2;tail.position.set(-.2,.1,0);dino.add(tail);
    for(const x of [-.065,.07])for(const z of [-.035,.035]){const leg=new THREE.Mesh(new THREE.CylinderGeometry(.012,.016,.09,6),skin);leg.position.set(x,.035,z);dino.add(leg);}return dino;
  }
  surfaceDinosaurs=new THREE.Group();
  let dinosaurPlaced=0,dinosaurAttempts=0;
  while(dinosaurPlaced<42&&dinosaurAttempts<900){dinosaurAttempts++;const lat=(Math.random()-.5)*1.75;const lon=Math.random()*Math.PI*2;const n=spherePoint(lat,lon,1).normalize();const h=terrainHeightAt(n);if(h<.025||h>.12)continue;const dino=makeDinosaur(dinosaurPlaced);dino.position.copy(n).multiplyScalar(radius+h+.02);dino.quaternion.setFromUnitVectors(up,n);dino.rotateY(Math.random()*Math.PI*2);dino.scale.setScalar(0);dino.userData.baseScale=.24+Math.random()*.24;dino.userData.phase=Math.random()*9;surfaceDinosaurs.add(dino);dinosaurPlaced++;}
  root.add(surfaceDinosaurs);

  const villageCount=180;
  surfaceCivilization=new THREE.InstancedMesh(new THREE.BoxGeometry(.045,.065,.045),new THREE.MeshStandardMaterial({color:0xb28a58,roughness:.9,transparent:true,opacity:0}),villageCount);
  let villagePlaced=0,villageAttempts=0;while(villagePlaced<villageCount&&villageAttempts<4000){villageAttempts++;const region=Math.floor(villagePlaced/30);const lat=-.55+region*.2+Math.sin(villageAttempts)*.045;const lon=.3+region*.82+(villageAttempts%47)*.017;const n=spherePoint(lat,lon,1).normalize();const h=terrainHeightAt(n);if(h<.02||h>.135)continue;position.copy(n).multiplyScalar(radius+h+.004);quaternion.setFromUnitVectors(up,n);scale.setScalar(.55+Math.random()*.72);matrix.compose(position,quaternion,scale);surfaceCivilization.setMatrixAt(villagePlaced,matrix);villagePlaced++;}surfaceCivilization.count=villagePlaced;
  root.add(surfaceCivilization);

  root.userData.radius=radius;
  root.userData.terrainHeightAt=terrainHeightAt;
  return root;
}

function makeSolarSystem() {
  const root = new THREE.Group();
  const sun = new THREE.Mesh(new THREE.IcosahedronGeometry(.38, 3), new THREE.MeshBasicMaterial({ color: 0xffc75f }));
  sun.userData.isSun = true;
  root.add(sun);
  root.userData.sun=sun;
  const planetData = [
    ["Mercury", .66, .045, 0x8e8278], ["Venus", .9, .075, 0xc99851],
    ["Living World", 1.2, .082, 0x377fc5], ["Mars", 1.48, .06, 0xb85b3d],
    ["Jupiter", 2.02, .19, 0xc89e79], ["Saturn", 2.52, .165, 0xd9bd79],
    ["Uranus", 3.02, .12, 0x87cfdb], ["Neptune", 3.5, .115, 0x4267c7],
  ];
  root.userData.planets = [];
  planetData.forEach(([name, radius, size, color], index) => {
    const orbit = new THREE.Mesh(new THREE.RingGeometry(radius - .006, radius + .006, 96), new THREE.MeshBasicMaterial({ color: 0x527083, transparent: true, opacity: .32, side: THREE.DoubleSide }));
    orbit.rotation.x = Math.PI / 2;
    orbit.userData.isOrbit=true;
    root.add(orbit);
    const body = new THREE.Group();
    let planet;
    if(index===2){
      planet=new THREE.Mesh(new THREE.SphereGeometry(size,96,64),new THREE.ShaderMaterial({
        uniforms:{uTime:{value:0},uMaturity:{value:0}},
        vertexShader:`varying vec3 vN;varying vec3 vP;void main(){vN=normalize(normalMatrix*normal);vP=normalize(position);gl_Position=projectionMatrix*modelViewMatrix*vec4(position,1.0);}`,
        fragmentShader:`uniform float uTime;uniform float uMaturity;varying vec3 vN;varying vec3 vP;void main(){float macro=sin(vP.x*3.7+vP.z*1.4)*sin(vP.y*4.9-vP.x*1.8);float detail=sin(vP.x*11.3+vP.y*8.1+vP.z*5.7)*.34+sin(vP.x*23.0-vP.z*19.0)*sin(vP.y*17.0)*.12;float field=macro+detail;float land=smoothstep(-.05,.16,field);float mountain=smoothstep(.32,.82,field);float polar=smoothstep(.72,.94,abs(vP.y));vec3 ocean=mix(vec3(.003,.035,.18),vec3(.015,.28,.52),max(0.0,dot(vN,normalize(vec3(-.6,.45,.7))))*.55+.2);vec3 ground=mix(vec3(.16,.34,.12),vec3(.38,.29,.17),mountain);ground=mix(ground,vec3(.82,.9,.92),max(polar,mountain*.7));vec3 color=mix(ocean,ground,land*uMaturity);float light=.18+max(0.0,dot(vN,normalize(vec3(-.65,.4,.65))))*.92;gl_FragColor=vec4(color*light,1.0);}`,
      }));
    }else{
      planet=new THREE.Mesh(new THREE.SphereGeometry(size,40,28),new THREE.MeshStandardMaterial({color,roughness:.86}));
    }
    body.add(planet);
    body.userData = { name, radius, orbitalSpeed: .38 / Math.sqrt(radius), phase: index * 1.51, planet };
    if (index === 2) {
      const moonPivot = new THREE.Group();
      const moon = new THREE.Mesh(new THREE.IcosahedronGeometry(.023, 1), new THREE.MeshStandardMaterial({ color: 0xc8d0d2, flatShading: true }));
      moon.position.x = .17;
      moonPivot.add(moon);
      body.add(moonPivot);
      body.userData.moonPivot = moonPivot;
      const atmosphereShell=new THREE.Mesh(new THREE.SphereGeometry(size*1.09,72,48),new THREE.ShaderMaterial({transparent:true,side:THREE.BackSide,depthWrite:false,blending:THREE.AdditiveBlending,uniforms:{uDensity:{value:0}},vertexShader:`varying vec3 vN;void main(){vN=normalize(normalMatrix*normal);gl_Position=projectionMatrix*modelViewMatrix*vec4(position,1.0);}`,fragmentShader:`uniform float uDensity;varying vec3 vN;void main(){float rim=pow(1.0-max(0.0,dot(vN,vec3(0.0,0.0,1.0))),2.5);gl_FragColor=vec4(vec3(.08,.42,1.0)*(1.0+rim),rim*uDensity*.72);}`}));
      const cloudShell=new THREE.Mesh(new THREE.SphereGeometry(size*1.025,80,52),new THREE.ShaderMaterial({transparent:true,depthWrite:false,uniforms:{uTime:{value:0},uDensity:{value:0}},vertexShader:`varying vec3 vP;varying vec3 vN;void main(){vP=normalize(position);vN=normalize(normalMatrix*normal);gl_Position=projectionMatrix*modelViewMatrix*vec4(position,1.0);}`,fragmentShader:`uniform float uTime;uniform float uDensity;varying vec3 vP;varying vec3 vN;void main(){float cloud=sin(vP.x*19.0+uTime*.03)*sin(vP.y*23.0-uTime*.02)*sin(vP.z*17.0+uTime*.015);cloud+=sin(vP.x*41.0-vP.z*37.0)*.34;float mask=smoothstep(.38,.72,cloud);float light=.45+max(0.0,dot(vN,normalize(vec3(-.6,.45,.7))))*.55;gl_FragColor=vec4(vec3(.92,.96,1.0)*light,mask*uDensity*.82);}`}));
      body.add(atmosphereShell,cloudShell);
      body.userData.atmosphereShell=atmosphereShell;
      body.userData.cloudShell=cloudShell;
      root.userData.livingPlanet = body;
    }
    if (index === 5) {
      const ringSystem = new THREE.Group();
      const bands = [
        [1.28, 1.47, 0xb9a47c, .42],
        [1.5, 1.72, 0xe0c998, .56],
        [1.79, 2.02, 0xc6ad7d, .48],
        [2.08, 2.28, 0x8f806b, .3],
      ];
      bands.forEach(([inner, outer, bandColor, opacity]) => {
        ringSystem.add(new THREE.Mesh(
          new THREE.RingGeometry(size * inner, size * outer, 128),
          new THREE.MeshBasicMaterial({ color: bandColor, transparent: true, opacity, side: THREE.DoubleSide, depthWrite: false }),
        ));
      });

      const dustCount = 3200;
      const dustPositions = new Float32Array(dustCount * 3);
      const dustColors = new Float32Array(dustCount * 3);
      const dustPalette = [new THREE.Color(0xe5d3aa),new THREE.Color(0xb7a17b),new THREE.Color(0x827565)];
      for (let dustIndex = 0; dustIndex < dustCount; dustIndex++) {
        let normalizedRadius = 1.27 + Math.random() * 1.02;
        if (normalizedRadius > 1.72 && normalizedRadius < 1.79) normalizedRadius += .075;
        if (normalizedRadius > 2.02 && normalizedRadius < 2.08) normalizedRadius += .065;
        const angle = Math.random() * Math.PI * 2;
        const radius = size * normalizedRadius;
        dustPositions.set([
          Math.cos(angle) * radius,
          Math.sin(angle) * radius,
          (Math.random() - .5) * size * .045,
        ], dustIndex * 3);
        const dustColor = dustPalette[dustIndex % dustPalette.length].clone().multiplyScalar(.62 + Math.random() * .38);
        dustColors.set([dustColor.r,dustColor.g,dustColor.b],dustIndex*3);
      }
      const dustGeometry = new THREE.BufferGeometry();
      dustGeometry.setAttribute("position",new THREE.BufferAttribute(dustPositions,3));
      dustGeometry.setAttribute("color",new THREE.BufferAttribute(dustColors,3));
      const dust = new THREE.Points(dustGeometry,new THREE.PointsMaterial({
        size:.006,
        vertexColors:true,
        transparent:true,
        opacity:.9,
        depthWrite:false,
        sizeAttenuation:true,
      }));
      ringSystem.add(dust);
      ringSystem.rotation.x = Math.PI / 2.25;
      ringSystem.userData.dust = dust;
      body.userData.saturnRings = ringSystem;
      body.add(ringSystem);
    }
    root.userData.planets.push(body);
    root.add(body);
  });
  return root;
}

function makeGalaxy() {
  const root = new THREE.Group();
  const count = 132000;
  const positions = new Float32Array(count * 3);
  const colors = new Float32Array(count * 3);
  const core = new THREE.Color(0xffe6bd);
  const armColors = [new THREE.Color(0x72bfff), new THREE.Color(0xc38cff), new THREE.Color(0x68efdc), new THREE.Color(0xffffff)];
  for (let i = 0; i < count; i++) {
    const isBulge = i < count * .28;
    const arm = i % 4;
    let radius;
    let angle;
    if (isBulge) {
      radius = Math.pow(Math.random(), 2.35) * 3.15;
      angle = Math.random() * Math.PI * 2;
    } else {
      radius = .18 + Math.pow(Math.random(), .72) * 9.7;
      const interArm = Math.random() < .24 ? (Math.random() - .5) * 1.4 : 0;
      const scatter = (Math.random() - .5) * (.48 + radius * .12) + interArm;
      angle = arm * Math.PI / 2 + radius * .54 + scatter;
    }
    const barStretch=isBulge?1.0+Math.max(0,1-radius/3.2)*.72:1;
    positions[i * 3] = Math.cos(angle) * radius*barStretch + (Math.random() - .5) * .18;
    positions[i * 3 + 1] = (Math.sin(angle) * radius + (Math.random() - .5) * .18)*.82;
    positions[i * 3 + 2] = (Math.random() - .5) * (isBulge ? 1.25 * Math.max(.18,1-radius/3.6) : .34 + radius * .072);
    const color = core.clone().lerp(armColors[arm], isBulge ? .12 : Math.min(1, radius / 5.2));
    const brightness = .48 + Math.random() * .52;
    colors.set([color.r * brightness, color.g * brightness, color.b * brightness], i * 3);
  }
  const geometry = new THREE.BufferGeometry();
  geometry.setAttribute("position", new THREE.BufferAttribute(positions, 3));
  geometry.setAttribute("color", new THREE.BufferAttribute(colors, 3));
  const disk = new THREE.Points(geometry, new THREE.PointsMaterial({ size: .018, vertexColors: true, transparent: true, opacity: .96, blending: THREE.AdditiveBlending, depthWrite: false, sizeAttenuation: true }));
  root.add(disk);
  root.rotation.x = .78;
  root.userData.armTarget = new THREE.Vector3(3.4, 1.1, .1);
  return root;
}

function makeChaosThreads() {
  const root = new THREE.Group();
  root.visible = false;
  const palette = [0x67f7ff, 0x785cff, 0xd44cff, 0x43a8ff, 0xff4eaa];

  for (let threadIndex = 0; threadIndex < 34; threadIndex++) {
    const phase = Math.random() * Math.PI * 2;
    const turns = 2.4 + Math.random() * 2.8;
    const outerRadius = 5.8 + Math.random() * 3.8;
    const points = [];
    for (let step = 0; step < 18; step++) {
      const t = step / 17;
      const inward = Math.pow(1 - t, 1.18);
      const radius = 2.45 + outerRadius * inward;
      const angle = phase + t * Math.PI * 2 * turns + Math.sin(t * 9 + phase) * .18;
      const turbulence = Math.sin(t * 15 + phase * 1.7) * (.18 + inward * .42);
      points.push(new THREE.Vector3(
        Math.cos(angle) * radius,
        Math.sin(angle) * radius,
        (Math.sin(angle * .63 + phase) * 1.25 + turbulence) * inward,
      ));
    }

    const curve = new THREE.CatmullRomCurve3(points);
    const geometry = new THREE.TubeGeometry(curve, 150, .008 + Math.random() * .018, 5, false);
    const material = new THREE.ShaderMaterial({
      transparent: true,
      blending: THREE.AdditiveBlending,
      depthWrite: false,
      uniforms: {
        uTime: { value: 0 },
        uIntensity: { value: 0 },
        uOffset: { value: Math.random() },
        uColor: { value: new THREE.Color(palette[threadIndex % palette.length]) },
      },
      vertexShader: `varying vec2 vUv;void main(){vUv=uv;gl_Position=projectionMatrix*modelViewMatrix*vec4(position,1.0);}`,
      fragmentShader: `
        uniform float uTime;
        uniform float uIntensity;
        uniform float uOffset;
        uniform vec3 uColor;
        varying vec2 vUv;
        void main(){
          float stream=fract(vUv.x*2.7-uTime*.72-uOffset);
          float pulse=pow(max(0.0,sin(stream*6.2831853)),10.0);
          float filament=.16+pulse*1.65;
          float endFade=smoothstep(0.0,.035,vUv.x)*(1.0-smoothstep(.94,1.0,vUv.x));
          vec3 color=mix(uColor,vec3(1.0),pulse*.58);
          gl_FragColor=vec4(color,clamp(uIntensity*filament*endFade,0.0,1.0));
        }
      `,
    });
    const thread = new THREE.Mesh(geometry, material);
    thread.rotation.set((Math.random()-.5)*.8,(Math.random()-.5)*.8,(Math.random()-.5)*.25);
    thread.userData.material = material;
    root.add(thread);
  }
  return root;
}

function makeRefinementFormation() {
  const root = new THREE.Group();
  root.userData.layers = [];
  root.userData.orbiters = [];
  root.userData.runes = [];
  root.userData.materials = [];
  const palette = [0xff5fa2, 0xffc857, 0x75f5e8, 0xb78cff, 0xff7657, 0x82a7ff];

  const makeMaterial = (color, opacity = .62) => {
    const material = new THREE.LineBasicMaterial({
      color,
      transparent: true,
      opacity,
      blending: THREE.AdditiveBlending,
      depthWrite: false,
    });
    material.userData.baseOpacity = opacity;
    root.userData.materials.push(material);
    return material;
  };

  const makeLoop = (radius, sides, material, starStep = 1) => {
    const points = [];
    for (let i = 0; i <= sides; i++) {
      const index = (i * starStep) % sides;
      const angle = index / sides * Math.PI * 2;
      points.push(new THREE.Vector3(Math.cos(angle) * radius, Math.sin(angle) * radius, 0));
    }
    return new THREE.Line(new THREE.BufferGeometry().setFromPoints(points), material);
  };

  const makeRune = (material, size, variant) => {
    const group = new THREE.Group();
    const strokes = [
      [[-.34,-.48],[0,.5],[.34,-.48],[-.2,-.08],[.24,-.08]],
      [[-.42,.42],[.36,.18],[-.28,-.02],[.38,-.42],[0,.48],[0,-.5]],
      [[-.4,-.35],[-.08,.48],[.34,.26],[-.25,.02],[.35,-.42]],
      [[-.38,.46],[.32,.46],[-.18,.04],[.34,-.04],[-.34,-.46]],
      [[0,.5],[-.36,.08],[0,-.5],[.36,.08],[-.26,.08],[.26,.08]],
      [[-.4,.34],[.08,.5],[.38,.02],[-.04,-.46],[-.34,-.12],[.28,-.12]],
    ][variant % 6];
    const points = strokes.map(([x,y]) => new THREE.Vector3(x*size,y*size,0));
    group.add(new THREE.Line(new THREE.BufferGeometry().setFromPoints(points), material));
    const cross = new THREE.BufferGeometry().setFromPoints([
      new THREE.Vector3(-size*.22,0,0),new THREE.Vector3(size*.22,0,0),
      new THREE.Vector3(0,-size*.22,0),new THREE.Vector3(0,size*.22,0),
    ]);
    group.add(new THREE.LineSegments(cross,material));
    root.userData.runes.push(group);
    return group;
  };

  for (let layerIndex = 0; layerIndex < 6; layerIndex++) {
    const layer = new THREE.Group();
    const color = palette[layerIndex % palette.length];
    const material = makeMaterial(color, .46 + layerIndex * .035);
    const radius = 1.72 + layerIndex * .31;
    const torusMaterial = new THREE.MeshBasicMaterial({
      color,
      transparent: true,
      opacity: .36,
      blending: THREE.AdditiveBlending,
      depthWrite: false,
    });
    torusMaterial.userData.baseOpacity = .36;
    root.userData.materials.push(torusMaterial);
    layer.add(new THREE.Mesh(new THREE.TorusGeometry(radius, .009 + layerIndex*.0015, 6, 192), torusMaterial));
    layer.add(new THREE.Mesh(new THREE.TorusGeometry(radius-.12, .0045, 5, 160), torusMaterial));
    const sides = 6 + layerIndex * 2;
    layer.add(makeLoop(radius * .91, sides, material));
    if (layerIndex > 1) layer.add(makeLoop(radius * .72, sides % 2 ? sides : sides-1, material, 2));

    const spokePoints = [];
    const runeCount = 8 + layerIndex * 2;
    for (let runeIndex = 0; runeIndex < runeCount; runeIndex++) {
      const angle = runeIndex / runeCount * Math.PI * 2;
      spokePoints.push(
        new THREE.Vector3(Math.cos(angle)*radius*.45,Math.sin(angle)*radius*.45,0),
        new THREE.Vector3(Math.cos(angle)*radius*.84,Math.sin(angle)*radius*.84,0),
      );
      const rune = makeRune(material, .18 + layerIndex*.012, runeIndex + layerIndex);
      rune.position.set(Math.cos(angle)*radius,Math.sin(angle)*radius,0);
      rune.rotation.z = angle - Math.PI/2;
      layer.add(rune);
    }
    layer.add(new THREE.LineSegments(new THREE.BufferGeometry().setFromPoints(spokePoints), material));
    layer.rotation.set((layerIndex-2.5)*.23,layerIndex%2 ? .34 : -.28,layerIndex*.19);
    layer.userData.baseRotation = layer.rotation.clone();
    layer.userData.spin = new THREE.Vector3((layerIndex%2 ? 1 : -1)*(.035+layerIndex*.006),(.018+layerIndex*.004)*(layerIndex%2 ? -1 : 1),(.07+layerIndex*.011)*(layerIndex%2 ? 1 : -1));
    layer.userData.phase = layerIndex*.8;
    root.userData.layers.push(layer);
    root.add(layer);
  }

  for (let clusterIndex = 0; clusterIndex < 7; clusterIndex++) {
    const pivot = new THREE.Group();
    const cluster = new THREE.Group();
    const material = makeMaterial(palette[(clusterIndex+2)%palette.length], .5);
    const orbitRadius = 2.55 + (clusterIndex%3)*.42;
    const angle = clusterIndex/7*Math.PI*2;
    cluster.position.set(Math.cos(angle)*orbitRadius,Math.sin(angle)*orbitRadius,(clusterIndex%2-.5)*1.25);
    cluster.scale.setScalar(.42 + (clusterIndex%3)*.08);
    cluster.add(makeLoop(1,5+clusterIndex%4,material));
    cluster.add(makeLoop(.68,7,material,2));
    for(let runeIndex=0;runeIndex<6;runeIndex++){
      const runeAngle=runeIndex/6*Math.PI*2;
      const rune=makeRune(material,.24,runeIndex+clusterIndex);
      rune.position.set(Math.cos(runeAngle)*1.12,Math.sin(runeAngle)*1.12,0);
      rune.rotation.z=runeAngle;
      cluster.add(rune);
    }
    pivot.rotation.set(clusterIndex*.31,clusterIndex*.17,clusterIndex*.53);
    pivot.userData.baseRotation = pivot.rotation.clone();
    pivot.userData.spin=(clusterIndex%2 ? -1 : 1)*(.08+clusterIndex*.009);
    pivot.add(cluster);
    root.userData.orbiters.push(pivot);
    root.add(pivot);
  }
  return root;
}

function makeNebulaBackdrop() {
  const root = new THREE.Group();
  const colors = [0x2548a8, 0x582985, 0x176d72, 0x79294e, 0x315577];
  const worldUp = new THREE.Vector3(0, 1, 0);

  for (let cloudIndex = 0; cloudIndex < 10; cloudIndex++) {
    const count = 2200;
    const positions = new Float32Array(count * 3);
    const azimuth = Math.random() * Math.PI * 2;
    const elevation = (Math.random() - .5) * 1.15;
    const direction = new THREE.Vector3(
      Math.cos(elevation) * Math.cos(azimuth),
      Math.sin(elevation),
      Math.cos(elevation) * Math.sin(azimuth),
    ).normalize();
    const center = direction.clone().multiplyScalar(44 + Math.random() * 24);
    const tangent = new THREE.Vector3().crossVectors(direction, worldUp);
    if (tangent.lengthSq() < .01) tangent.set(1, 0, 0);
    tangent.normalize();
    const bitangent = new THREE.Vector3().crossVectors(direction, tangent).normalize();
    const width = 5 + Math.random() * 8;
    const height = 2.5 + Math.random() * 5;

    for (let i = 0; i < count; i++) {
      const radial = Math.pow(Math.random(), 1.8);
      const angle = Math.random() * Math.PI * 2;
      const point = center.clone()
        .addScaledVector(tangent, Math.cos(angle) * radial * width)
        .addScaledVector(bitangent, Math.sin(angle) * radial * height)
        .addScaledVector(direction, (Math.random() - .5) * 2.5);
      positions.set([point.x, point.y, point.z], i * 3);
    }

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute("position", new THREE.BufferAttribute(positions, 3));
    const baseOpacity = .055 + Math.random() * .035;
    const cloud = new THREE.Points(geometry, new THREE.PointsMaterial({
      color: colors[cloudIndex % colors.length],
      size: .16 + Math.random() * .12,
      transparent: true,
      opacity: baseOpacity,
      blending: THREE.AdditiveBlending,
      depthWrite: false,
      depthTest: true,
      sizeAttenuation: true,
    }));
    cloud.userData.baseOpacity = baseOpacity;
    root.add(cloud);
  }

  return root;
}

function makePlanetFragments() {
  const root = new THREE.Group();
  root.visible = false;
  root.userData.fragments = [];
  fragmentMaterial = new THREE.ShaderMaterial({
    side: THREE.DoubleSide,
    uniforms: { uTime: { value: 0 }, uFracture: { value: 0 } },
    vertexShader: `
      attribute vec3 aBarycentric;
      attribute float aShade;
      varying vec3 vBarycentric;
      varying float vShade;
      varying vec3 vNormal;
      void main(){
        vBarycentric = aBarycentric;
        vShade = aShade;
        vNormal = normalize(normalMatrix * normal);
        gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);
      }
    `,
    fragmentShader: `
      uniform float uTime;
      uniform float uFracture;
      varying vec3 vBarycentric;
      varying float vShade;
      varying vec3 vNormal;
      void main(){
        float nearestEdge = min(vBarycentric.x, min(vBarycentric.y, vBarycentric.z));
        float edge = 1.0 - smoothstep(0.015, 0.105, nearestEdge);
        float light = 0.34 + max(0.0, dot(vNormal, normalize(vec3(-0.5, 0.8, 0.7)))) * 0.66;
        float grain=sin(vNormal.x*31.0+vNormal.y*19.0+vNormal.z*27.0+vShade*13.0)*.08;
        vec3 basalt = mix(vec3(0.055,0.065,0.075), vec3(0.19,0.16,0.13), clamp(vShade+grain,0.0,1.0)) * light;
        float rim=pow(1.0-max(0.0,vNormal.z),2.4);
        vec3 energy = mix(vec3(0.48,0.012,0.0), vec3(1.0,0.48,0.025), sin(uTime*3.0+vShade*8.0)*0.25+0.55);
        vec3 color = basalt + energy * edge * uFracture * 3.2 + energy*rim*uFracture*.42;
        gl_FragColor = vec4(color, 1.0);
      }
    `,
  });

  const iceMaterial = new THREE.MeshStandardMaterial({
    color: 0xbfeaff,
    emissive: 0x17394c,
    emissiveIntensity: .22,
    transparent: true,
    opacity: .72,
    roughness: .38,
    metalness: .04,
    side: THREE.DoubleSide,
    flatShading: true,
  });

  for (let i = 0; i < 112; i++) {
    const isPlate = i < 36;
    let direction;
    let geometry;
    let center;
    if (isPlate) {
      const phiLength = .24 + Math.random() * .34;
      const thetaLength = .18 + Math.random() * .28;
      const thetaStart = .18 + Math.random() * Math.max(.12, Math.PI - thetaLength - .36);
      const phiStart = Math.random() * Math.PI * 2;
      geometry = new THREE.SphereGeometry(1.43, 6, 4, phiStart, phiLength, thetaStart, thetaLength).toNonIndexed();
      center = new THREE.Vector3();
      const plateVertices = geometry.attributes.position;
      for (let vertexIndex = 0; vertexIndex < plateVertices.count; vertexIndex++) {
        center.x += plateVertices.getX(vertexIndex);
        center.y += plateVertices.getY(vertexIndex);
        center.z += plateVertices.getZ(vertexIndex);
      }
      center.multiplyScalar(1 / plateVertices.count);
      direction = center.clone().normalize();
      geometry.translate(-center.x, -center.y, -center.z);
    } else {
      direction = new THREE.Vector3().randomDirection();
      const size = .105 + Math.pow(Math.random(), 1.7) * .29;
      geometry = new THREE.IcosahedronGeometry(size, Math.random() < .18 ? 1 : 0).toNonIndexed();
      center = direction.clone().multiplyScalar(.72 + Math.random() * .7);
    }
    const vertices = geometry.attributes.position;
    if (!isPlate) {
      const stretch = new THREE.Vector3(.62 + Math.random() * 1.25, .55 + Math.random() * 1.4, .58 + Math.random() * 1.2);
      for (let vertexIndex = 0; vertexIndex < vertices.count; vertexIndex++) {
        const x = vertices.getX(vertexIndex);
        const y = vertices.getY(vertexIndex);
        const z = vertices.getZ(vertexIndex);
        const distortion = .78 + Math.sin(x * 47 + y * 31 + z * 23 + i) * .2;
        vertices.setXYZ(vertexIndex, x * stretch.x * distortion, y * stretch.y * distortion, z * stretch.z * distortion);
      }
    }
    const barycentric = new Float32Array(vertices.count * 3);
    for (let vertexIndex = 0; vertexIndex < vertices.count; vertexIndex += 3) {
      barycentric.set([1,0,0, 0,1,0, 0,0,1], vertexIndex * 3);
    }
    geometry.setAttribute("aBarycentric", new THREE.BufferAttribute(barycentric, 3));
    const shade = .18 + Math.random() * .82;
    geometry.setAttribute("aShade", new THREE.Float32BufferAttribute(new Array(vertices.count).fill(shade), 1));
    geometry.computeVertexNormals();
    const shard = new THREE.Mesh(geometry, fragmentMaterial);
    shard.position.copy(center);
    if (!isPlate) shard.rotation.set(Math.random()*Math.PI,Math.random()*Math.PI,Math.random()*Math.PI);
    if (isPlate && Math.random() < .58) {
      const iceLayer = new THREE.Mesh(geometry.clone(), iceMaterial);
      iceLayer.scale.setScalar(1.018);
      iceLayer.position.copy(direction).multiplyScalar(.012);
      iceLayer.userData.isIceLayer = true;
      shard.add(iceLayer);
    }
    const tangent = new THREE.Vector3(-center.y, center.x, center.z * .25).normalize();
    const detached = Math.random() < (isPlate ? .72 : .58);
    shard.userData.base = center.clone();
    shard.userData.detached = detached;
    shard.userData.isPlate = isPlate;
    shard.userData.velocity = direction.multiplyScalar(detached ? .65 + Math.random() * 1.35 : .025 + Math.random() * .055).addScaledVector(tangent, (Math.random() - .5) * (detached ? .72 : .08));
    shard.userData.spin = new THREE.Vector3(Math.random() - .5, Math.random() - .5, Math.random() - .5).multiplyScalar(2.2);
    root.userData.fragments.push(shard);
    root.add(shard);
  }

  magmaCore = new THREE.Mesh(
    new THREE.IcosahedronGeometry(1.16, 4),
    new THREE.ShaderMaterial({
      uniforms: { uTime: { value: 0 }, uCollapse: { value: 0 } },
      vertexShader: `varying vec3 vP; varying vec3 vN; void main(){vP=position;vN=normalize(normalMatrix*normal);gl_Position=projectionMatrix*modelViewMatrix*vec4(position,1.0);}`,
      fragmentShader: `uniform float uTime;uniform float uCollapse;varying vec3 vP;varying vec3 vN;void main(){float flow=sin(vP.x*9.0+uTime*2.0)*sin(vP.y*11.0-uTime)*sin(vP.z*8.0+uTime*.7);float rim=pow(1.0-max(0.0,dot(vN,vec3(0.0,0.0,1.0))),2.0);vec3 hot=mix(vec3(.42,.008,.0),vec3(1.,.32,.015),flow*.5+.5);float cooling=smoothstep(.18,1.0,uCollapse);vec3 cooled=mix(hot,vec3(.055,.018,.012),cooling*.82);float brightness=mix(1.35+rim,.28+rim*.14,cooling);gl_FragColor=vec4(cooled*brightness,1.0);}`,
    }),
  );
  magmaCore.visible = false;
  root.add(magmaCore);

  fractureAura = new THREE.Mesh(
    new THREE.IcosahedronGeometry(1.62, 4),
    new THREE.ShaderMaterial({
      transparent: true, side: THREE.BackSide, blending: THREE.AdditiveBlending, depthWrite: false,
      uniforms: { uTime: { value: 0 }, uStrength: { value: 0 } },
      vertexShader: `varying vec3 vN;varying vec3 vP;void main(){vN=normalize(normalMatrix*normal);vP=position;gl_Position=projectionMatrix*modelViewMatrix*vec4(position,1.0);}`,
      fragmentShader: `uniform float uTime;uniform float uStrength;varying vec3 vN;varying vec3 vP;void main(){float rim=pow(1.0-max(0.0,dot(vN,vec3(0.0,0.0,1.0))),2.0);float plasma=sin(vP.x*13.0+uTime)*sin(vP.y*11.0-uTime*.7)*.5+.5;gl_FragColor=vec4(vec3(.58,.018,.0)+vec3(.42,.19,.015)*plasma,rim*uStrength*(.18+plasma*.2));}`,
    }),
  );
  fractureAura.visible = false;
  root.add(fractureAura);

  const debrisCount = 1400;
  debrisField = new THREE.InstancedMesh(
    new THREE.IcosahedronGeometry(.07, 1),
    new THREE.MeshStandardMaterial({ color: 0x1b100d, roughness: .86, metalness: .18, emissive: 0x5c0900, emissiveIntensity: .48, flatShading: true }),
    debrisCount,
  );
  debrisField.instanceMatrix.setUsage(THREE.DynamicDrawUsage);
  debrisField.userData.orbits = [];
  for (let i = 0; i < debrisCount; i++) {
    const radius = 1.5 + Math.pow(Math.random(), .72) * 4.9;
    const data = {
      radius,
      angle: Math.random() * Math.PI * 2,
      height: (Math.random() - .5) * (1.1 + radius * .32),
      speed: (.18 + Math.random() * .62) / Math.sqrt(radius),
      size: .16 + Math.pow(Math.random(), 2.4) * 2.9,
      tilt: (Math.random() - .5) * .65,
      spin: new THREE.Vector3(Math.random(), Math.random(), Math.random()).multiplyScalar(5),
    };
    debrisField.userData.orbits.push(data);
    debrisPosition.set(Math.cos(data.angle) * radius, data.height, Math.sin(data.angle) * radius);
    debrisScale.setScalar(data.size);
    debrisMatrix.compose(debrisPosition, debrisQuaternion, debrisScale);
    debrisField.setMatrixAt(i, debrisMatrix);
  }
  debrisField.visible = false;
  root.add(debrisField);
  return root;
}

function makeFractureFlare() {
  const flare = new THREE.Mesh(
    new THREE.PlaneGeometry(11, 1.5),
    new THREE.ShaderMaterial({
      transparent: true, blending: THREE.AdditiveBlending, depthWrite: false, depthTest: false,
      uniforms: { uStrength: { value: 0 } },
      vertexShader: `varying vec2 vUv;void main(){vUv=uv;gl_Position=projectionMatrix*modelViewMatrix*vec4(position,1.0);}`,
      fragmentShader: `varying vec2 vUv;uniform float uStrength;void main(){vec2 p=(vUv-.5)*2.0;float beam=exp(-abs(p.y)*72.0)*(1.0-smoothstep(.05,1.0,abs(p.x)));float core=exp(-length(p*vec2(5.5,1.0))*12.0);vec3 color=mix(vec3(.72,.025,.0),vec3(1.0,.58,.08),core);gl_FragColor=vec4(color,(beam*.72+core)*uStrength);}`,
    }),
  );
  flare.position.set(-3.5, 1.05, .2);
  flare.renderOrder = 20;
  flare.visible = false;
  return flare;
}

function makeBlackHole() {
  const root = new THREE.Group();

  spacetimeGrid = new THREE.Mesh(
    new THREE.PlaneGeometry(10, 10, 52, 52),
    new THREE.ShaderMaterial({
      wireframe: true, transparent: true, depthWrite: false,
      uniforms: { uStrength: { value: 0 }, uTime: { value: 0 } },
      vertexShader: `uniform float uStrength;uniform float uTime;varying float vR;void main(){vec3 p=position;vR=length(p.xy);p.z-=uStrength*2.2/(vR*.85+.22);p.z+=sin(vR*7.0-uTime)*.025*uStrength;gl_Position=projectionMatrix*modelViewMatrix*vec4(p,1.0);}`,
      fragmentShader: `uniform float uStrength;varying float vR;void main(){float fade=(1.0-smoothstep(2.4,5.8,vR))*uStrength;gl_FragColor=vec4(.28,.38,.72,fade*.34);}`,
    }),
  );
  spacetimeGrid.rotation.x = -1.08;
  spacetimeGrid.position.set(0, -.85, -.75);
  root.add(spacetimeGrid);

  blackHoleLens = new THREE.Mesh(
    new THREE.PlaneGeometry(6.8, 6.8),
    new THREE.ShaderMaterial({
      transparent: true, depthWrite: false, depthTest: false,
      uniforms: { uTime: { value: 0 }, uProgress: { value: 0 } },
      vertexShader: `varying vec2 vUv;void main(){vUv=uv;gl_Position=projectionMatrix*modelViewMatrix*vec4(position,1.0);}`,
      fragmentShader: `
        uniform float uTime; uniform float uProgress; varying vec2 vUv;
        float hash(vec2 p){return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453);}
        float noise(vec2 p){vec2 i=floor(p),f=fract(p);f=f*f*(3.0-2.0*f);return mix(mix(hash(i),hash(i+vec2(1.0,0.0)),f.x),mix(hash(i+vec2(0.0,1.0)),hash(i+vec2(1.0,1.0)),f.x),f.y);}
        void main(){
          vec2 uv=(vUv-.5)*2.0; uv.x*=1.0;
          float r=length(uv); float a=atan(uv.y,uv.x);
          float spin=uTime*1.7; float turbulence=noise(vec2(a*5.0+spin,r*22.0-uTime*2.0));
          float radialMask=smoothstep(.18,.3,r)*(1.0-smoothstep(.72,1.18,r));
          float thinDisk=exp(-abs(uv.y)*95.0/(1.0+r*4.0))*radialMask;
          float warpedY=abs(uv.y)-(.11+.055*cos(a*2.0));
          float lensArc=exp(-abs(warpedY)*85.0)*smoothstep(.2,.29,r)*(1.0-smoothstep(.48,.72,r));
          float photon=exp(-abs(r-.205)*180.0);
          float secondary=exp(-abs(r-.245)*95.0)*.35;
          float bands=pow(.18+.82*turbulence,2.2);
          float doppler=.28+1.55*smoothstep(-.9,.85,cos(a-.18));
          vec3 ember=mix(vec3(1.0,.08,.005),vec3(1.0,.92,.62),smoothstep(.2,.9,bands));
          vec3 diskColor=ember*(thinDisk*bands*doppler+lensArc*(.7+bands)*doppler);
          vec3 ringColor=vec3(1.0,.88,.62)*(photon*2.8+secondary);
          float glow=exp(-r*3.8)*smoothstep(.19,.29,r)*.2;
          vec3 color=diskColor+ringColor+vec3(.35,.22,.7)*glow;
          float alpha=clamp(thinDisk+lensArc+photon+secondary+glow,0.0,1.0)*smoothstep(.18,.72,uProgress);
          if(r<.19){color=vec3(0.0);alpha=smoothstep(.18,.55,uProgress);}
          gl_FragColor=vec4(color,alpha);
        }
      `,
    }),
  );
  blackHoleLens.position.z = .14;
  blackHoleLens.renderOrder = 10;
  root.add(blackHoleLens);

  const horizon = new THREE.Mesh(new THREE.SphereGeometry(.64, 64, 64), new THREE.MeshBasicMaterial({ color: 0x000000 }));
  horizon.renderOrder = 11;
  root.add(horizon);
  const photonShell = new THREE.Mesh(new THREE.TorusGeometry(.72, .022, 12, 192), new THREE.MeshBasicMaterial({ color: 0xffefd0, transparent: true, opacity: .95, blending: THREE.AdditiveBlending }));
  photonShell.renderOrder = 12;
  root.add(photonShell);

  for (let i = 0; i < 17; i++) {
    const ring = new THREE.Mesh(new THREE.TorusGeometry(.82 + i * .095, .012 + i * .003, 7, 180), new THREE.MeshBasicMaterial({ color: i < 4 ? 0xfff4d0 : i < 11 ? 0xff8b32 : 0x9b4cff, transparent: true, opacity: .72 - i * .027, blending: THREE.AdditiveBlending, depthWrite: false }));
    ring.rotation.x = 1.18;
    ring.scale.y = .32;
    ring.userData.spin = .35 + i * .055;
    root.add(ring);
  }

  singularityCore = new THREE.Mesh(new THREE.IcosahedronGeometry(.075, 2), new THREE.MeshBasicMaterial({ color: 0xe5c1ff, transparent: true, opacity: 0, blending: THREE.AdditiveBlending }));
  root.add(singularityCore);
  const jetMaterial = new THREE.MeshBasicMaterial({ color: 0x95cfff, transparent: true, opacity: .18, blending: THREE.AdditiveBlending, depthWrite: false });
  const jetA = new THREE.Mesh(new THREE.ConeGeometry(.08, 6, 20, 1, true), jetMaterial);
  const jetB = jetA.clone();
  jetA.position.y = 3.25; jetB.position.y = -3.25; jetB.rotation.z = Math.PI;
  root.add(jetA, jetB);
  return root;
}

function makePrimordialEnergy() {
  const root=new THREE.Group();
  const palettes=[[0xff5a28,0xffc35a],[0x38a8ff,0x86e8ff],[0x9de8ff,0xdffaff],[0xb17945,0xf0c878],[0xa34cff,0xff63c8]];
  root.userData.clouds=[];root.userData.bolts=[];
  palettes.forEach((pair,layer)=>{
    const count=760;const positions=new Float32Array(count*3);const sizes=new Float32Array(count);const phases=new Float32Array(count);const colors=new Float32Array(count*3);const ca=new THREE.Color(pair[0]),cb=new THREE.Color(pair[1]);
    const branch=layer%3;const angle=branch*Math.PI*2/3+.35;const direction=new THREE.Vector3(Math.cos(angle),Math.sin(angle),(branch-1)*.28).normalize();const side=new THREE.Vector3(-direction.y,direction.x,.1).normalize();
    for(let i=0;i<count;i++){const t=Math.pow(Math.random(),.72);const curl=Math.sin(t*13+layer)*(.08+t*.6);const spread=(Math.random()-.5)*(.12+t*.82);const p=direction.clone().multiplyScalar(.15+t*5.4).addScaledVector(side,curl+spread);p.z+=(Math.random()-.5)*(.15+t*.7);positions.set([p.x,p.y,p.z],i*3);sizes[i]=5+Math.random()*18;phases[i]=Math.random()*6.283;const c=ca.clone().lerp(cb,Math.random());colors.set([c.r,c.g,c.b],i*3);}
    const geometry=new THREE.BufferGeometry();geometry.setAttribute("position",new THREE.BufferAttribute(positions,3));geometry.setAttribute("aSize",new THREE.BufferAttribute(sizes,1));geometry.setAttribute("aPhase",new THREE.BufferAttribute(phases,1));geometry.setAttribute("aColor",new THREE.BufferAttribute(colors,3));
    const material=new THREE.ShaderMaterial({transparent:true,depthWrite:false,blending:THREE.AdditiveBlending,uniforms:{uTime:{value:0},uOpacity:{value:0},uExpansion:{value:0}},vertexShader:`attribute float aSize;attribute float aPhase;attribute vec3 aColor;uniform float uTime;uniform float uExpansion;varying vec3 vColor;varying float vPhase;void main(){vColor=aColor;vPhase=aPhase;vec3 p=position*(.06+uExpansion);p+=normalize(position)*sin(uTime*1.8+aPhase)*.035*uExpansion;vec4 mv=modelViewMatrix*vec4(p,1.0);gl_PointSize=min(28.0,aSize*(280.0/max(1.0,-mv.z)));gl_Position=projectionMatrix*mv;}`,fragmentShader:`uniform float uTime;uniform float uOpacity;varying vec3 vColor;varying float vPhase;void main(){vec2 q=gl_PointCoord-.5;float r=length(q);if(r>.5)discard;float mist=smoothstep(.5,.08,r)*(.58+.42*sin(uTime*1.4+vPhase));gl_FragColor=vec4(vColor,mist*uOpacity*.42);}`});
    const cloud=new THREE.Points(geometry,material);cloud.userData.layer=layer;root.userData.clouds.push(cloud);root.add(cloud);
  });
  for(let i=0;i<14;i++){const branch=i%3;const angle=branch*Math.PI*2/3+.35;const points=[];for(let j=0;j<10;j++){const t=j/9;points.push(new THREE.Vector3(Math.cos(angle)*t*4.8+Math.sin(t*17+i)*.16*t,Math.sin(angle)*t*4.8+Math.sin(t*11+i*.7)*.13*t,(branch-1)*t+Math.sin(t*19+i)*.12));}const bolt=new THREE.Line(new THREE.BufferGeometry().setFromPoints(points),new THREE.LineBasicMaterial({color:i%2?0xaeeaff:0xffd69a,transparent:true,opacity:0,blending:THREE.AdditiveBlending,depthWrite:false}));bolt.userData.phase=Math.random()*8;root.userData.bolts.push(bolt);root.add(bolt);}
  root.visible=false;return root;
}

function createScene() {
  scene = new THREE.Scene();
  scene.fog = new THREE.FogExp2(0x02060d, 0.028);
  camera = new THREE.PerspectiveCamera(42, 1, 0.1, 600);
  camera.position.set(0, 0.25, 7.4);

  renderer = new THREE.WebGLRenderer({ antialias: true, alpha: false, powerPreference: "high-performance" });
  renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
  renderer.setClearColor(0x02060d, 1);
  renderer.outputColorSpace = THREE.SRGBColorSpace;
  renderer.toneMapping = THREE.ACESFilmicToneMapping;
  renderer.toneMappingExposure = 1.08;
  host.value.appendChild(renderer.domElement);
  rendererLabel.value = renderer.capabilities.isWebGL2 ? "WebGL 2 · GPU shader" : "WebGL · GPU shader";
  composer = new EffectComposer(renderer);
  composer.addPass(new RenderPass(scene, camera));
  bloomPass = new UnrealBloomPass(new THREE.Vector2(1, 1), 1.05, .7, .18);
  composer.addPass(bloomPass);

  const worldGeometry = new THREE.IcosahedronGeometry(1.42, 5);
  worldMaterial = new THREE.ShaderMaterial({
    vertexShader,
    fragmentShader,
    transparent: true,
    uniforms: {
      uTime: { value: 0 }, uProgress: { value: 0 }, uMode: { value: 0 },
      uDeep: { value: new THREE.Color("#071f3d") }, uBright: { value: new THREE.Color("#63f5d0") },
    },
  });
  world = new THREE.Mesh(worldGeometry, worldMaterial);
  scene.add(world);

  atmosphere = new THREE.Mesh(
    new THREE.SphereGeometry(1.55, 64, 64),
    new THREE.MeshBasicMaterial({ color: 0x63f5d0, transparent: true, opacity: .1, side: THREE.BackSide, blending: THREE.AdditiveBlending }),
  );
  world.add(atmosphere);

  const starGeometry = new THREE.BufferGeometry();
  const starPositions = new Float32Array(15000 * 3);
  const starSizes = new Float32Array(15000);
  const starColors = new Float32Array(15000 * 3);
  const starPhases = new Float32Array(15000);
  const stellarPalette=[new THREE.Color(0x9bbcff),new THREE.Color(0xcad8ff),new THREE.Color(0xffffff),new THREE.Color(0xfff0c7),new THREE.Color(0xffc28b),new THREE.Color(0xff8b72)];
  for (let i = 0; i < 15000; i++) {
    const radius = 11 + Math.random() * 125;
    const theta = Math.random() * Math.PI * 2;
    const phi = Math.acos(2 * Math.random() - 1);
    starPositions[i * 3] = radius * Math.sin(phi) * Math.cos(theta);
    starPositions[i * 3 + 1] = radius * Math.cos(phi);
    starPositions[i * 3 + 2] = radius * Math.sin(phi) * Math.sin(theta);
    starSizes[i] = .6 + Math.random() * 1.5;
    const stellarColor=stellarPalette[Math.floor(Math.pow(Math.random(),1.7)*stellarPalette.length)].clone().multiplyScalar(.72+Math.random()*.38);
    starColors.set([stellarColor.r,stellarColor.g,stellarColor.b],i*3);starPhases[i]=Math.random()*Math.PI*2;
  }
  starGeometry.setAttribute("position", new THREE.BufferAttribute(starPositions, 3));
  starGeometry.setAttribute("aSize", new THREE.BufferAttribute(starSizes, 1));
  starGeometry.setAttribute("aColor",new THREE.BufferAttribute(starColors,3));starGeometry.setAttribute("aPhase",new THREE.BufferAttribute(starPhases,1));
  const starMaterial = new THREE.ShaderMaterial({transparent:true,depthWrite:false,blending:THREE.AdditiveBlending,uniforms:{uTime:{value:0},uOpacity:{value:0}},vertexShader:`attribute float aSize;attribute float aPhase;attribute vec3 aColor;varying vec3 vColor;varying float vPhase;varying float vLarge;void main(){vColor=aColor;vPhase=aPhase;vLarge=step(1.82,aSize);vec4 mv=modelViewMatrix*vec4(position,1.0);gl_PointSize=min(8.0,(1.1+aSize*1.45)*(260.0/max(1.0,-mv.z)));gl_Position=projectionMatrix*mv;}`,fragmentShader:`uniform float uTime;uniform float uOpacity;varying vec3 vColor;varying float vPhase;varying float vLarge;void main(){vec2 p=gl_PointCoord-.5;float r=length(p);if(r>.5)discard;float core=1.0-smoothstep(.03,.3,r);float cross=exp(-abs(p.x)*46.0)*(1.0-smoothstep(.04,.48,abs(p.y)))+exp(-abs(p.y)*46.0)*(1.0-smoothstep(.04,.48,abs(p.x)));float diagonal=(exp(-abs(p.x-p.y)*56.0)+exp(-abs(p.x+p.y)*56.0))*(1.0-smoothstep(.04,.43,r))*.34;float twinkle=.68+.32*sin(uTime*(1.1+fract(vPhase)*2.4)+vPhase);float alpha=(core+vLarge*(cross+diagonal)*.72)*twinkle*uOpacity;if(alpha<.02)discard;gl_FragColor=vec4(vColor*(1.0+vLarge*.45),alpha);}`});
  starField = new THREE.Points(starGeometry, starMaterial);
  scene.add(starField);

  const particleGeometry = new THREE.BufferGeometry();
  const count = 2400;
  particleBase = new Float32Array(count * 3);
  const colors = new Float32Array(count * 3);
  for (let i = 0; i < count; i++) {
    const r = .15 + Math.pow(Math.random(), .46) * 5.2;
    const theta = Math.random() * Math.PI * 2;
    const z = (Math.random() - .5) * 2.2;
    particleBase[i * 3] = Math.cos(theta) * r;
    particleBase[i * 3 + 1] = Math.sin(theta) * r;
    particleBase[i * 3 + 2] = z;
    const c = new THREE.Color().setHSL(.48 + Math.random() * .15, .75, .55 + Math.random() * .3);
    colors.set([c.r, c.g, c.b], i * 3);
  }
  particleGeometry.setAttribute("position", new THREE.BufferAttribute(particleBase.slice(), 3));
  particleGeometry.setAttribute("color", new THREE.BufferAttribute(colors, 3));
  particles = new THREE.Points(particleGeometry, new THREE.PointsMaterial({ size: .045, vertexColors: true, transparent: true, opacity: .84, blending: THREE.AdditiveBlending, depthWrite: false }));
  scene.add(particles);

  chaosThreads = makeChaosThreads();
  scene.add(chaosThreads);

  originEnergyCore = new THREE.Mesh(
    new THREE.IcosahedronGeometry(1, 5),
    new THREE.ShaderMaterial({
      transparent: true,
      blending: THREE.AdditiveBlending,
      depthWrite: false,
      uniforms: { uTime: { value: 0 }, uIntensity: { value: 0 } },
      vertexShader: `
        uniform float uTime;
        varying vec3 vP;
        varying vec3 vN;
        void main(){
          float turbulence=sin(position.x*9.0+uTime*2.1)*sin(position.y*11.0-uTime*1.7)*sin(position.z*8.0+uTime*.9);
          vec3 displaced=position+normal*turbulence*.075;
          vP=displaced;
          vN=normalize(normalMatrix*normal);
          gl_Position=projectionMatrix*modelViewMatrix*vec4(displaced,1.0);
        }
      `,
      fragmentShader: `
        uniform float uTime;
        uniform float uIntensity;
        varying vec3 vP;
        varying vec3 vN;
        void main(){
          float flow=sin(vP.x*13.0+uTime*2.4)*sin(vP.y*10.0-uTime*1.3)*sin(vP.z*15.0+uTime);
          float rim=pow(1.0-max(0.0,dot(vN,vec3(0.0,0.0,1.0))),2.2);
          float pulse=.72+sin(uTime*4.3+flow*2.0)*.28;
          vec3 color=mix(vec3(.12,.025,.48),vec3(.24,1.0,.92),flow*.5+.5);
          color=mix(color,vec3(1.0,.38,.9),rim*.48);
          gl_FragColor=vec4(color*(.75+pulse+rim),uIntensity*(.66+rim*.34));
        }
      `,
    }),
  );
  const originHalo = new THREE.Mesh(
    new THREE.SphereGeometry(1.18,48,48),
    new THREE.MeshBasicMaterial({ color:0x705cff,transparent:true,opacity:.12,side:THREE.BackSide,blending:THREE.AdditiveBlending,depthWrite:false }),
  );
  originEnergyCore.add(originHalo);
  originEnergyCore.visible = false;
  scene.add(originEnergyCore);

  formation = makeRefinementFormation();
  scene.add(formation);

  star = new THREE.Mesh(
    new THREE.SphereGeometry(1, 48, 48),
    new THREE.MeshBasicMaterial({ color: 0xffa43b, transparent: true, opacity: .9 }),
  );
  star.position.set(-7, 1.2, -2);
  scene.add(star);
  starLight = new THREE.PointLight(0xff9b52, 3, 40);
  starLight.position.copy(star.position);
  scene.add(starLight);

  singularityFlash = new THREE.Mesh(
    new THREE.IcosahedronGeometry(1, 4),
    new THREE.MeshBasicMaterial({ color: 0xffffff, transparent: true, opacity: 0, blending: THREE.AdditiveBlending, depthWrite: false }),
  );
  scene.add(singularityFlash);

  auroraGroup = new THREE.Group();
  [0x69f7db,0x8f72ff,0xff6fb3].forEach((color,index)=>{
    const ribbon=new THREE.Mesh(new THREE.TorusKnotGeometry(1.2+index*.35,.025+index*.008,220,7,2+index,3),new THREE.MeshBasicMaterial({color,transparent:true,opacity:.24,wireframe:true,blending:THREE.AdditiveBlending,depthWrite:false}));
    ribbon.rotation.set(index*.65,index*.8,index*.35);ribbon.userData.expansionDirection=new THREE.Vector3(Math.cos(index*Math.PI*2/3+.35),Math.sin(index*Math.PI*2/3+.35),(index-1)*.38).normalize();auroraGroup.add(ribbon);
  });
  scene.add(auroraGroup);

  primordialEnergyGroup=makePrimordialEnergy();
  scene.add(primordialEnergyGroup);

  galaxyGroup = makeGalaxy();
  galaxyGroup.visible = false;
  scene.add(galaxyGroup);

  nebulaBackdrop = makeNebulaBackdrop();
  nebulaBackdrop.visible = false;
  scene.add(nebulaBackdrop);

  solarSystem = makeSolarSystem();
  solarSystem.visible = false;
  scene.add(solarSystem);

  surfaceWorld = makeSphericalSurface();
  scene.add(surfaceWorld);

  blackHole = makeBlackHole();
  blackHole.visible = false;
  scene.add(blackHole);

  fragmentGroup = makePlanetFragments();
  scene.add(fragmentGroup);

  fractureFlare = makeFractureFlare();
  scene.add(fractureFlare);
  fractureLight = new THREE.PointLight(0xff3b16, 0, 22, 1.4);
  fractureLight.position.set(-3.2, 1.1, 2.2);
  scene.add(fractureLight);

  planetMoon = new THREE.Mesh(new THREE.IcosahedronGeometry(.2, 2), new THREE.MeshStandardMaterial({ color: 0xbfc9ce, flatShading: true, roughness: 1 }));
  planetMoon.visible = false;
  scene.add(planetMoon);

  scene.add(new THREE.HemisphereLight(0x9bcfff, 0x26351f, 1.25));
  daylight = new THREE.DirectionalLight(0xffe2b8, 2.2);
  daylight.position.set(-4, 7, 4);
  scene.add(daylight);

  observer = new ResizeObserver(resize);
  observer.observe(host.value);
  resize();
}

function resize() {
  if (!renderer || !host.value) return;
  const width = host.value.clientWidth;
  const height = host.value.clientHeight;
  renderer.setSize(width, height, false);
  composer?.setSize(width, height);
  camera.aspect = width / Math.max(1, height);
  camera.updateProjectionMatrix();
}

function modeIndex() {
  return scenarios.findIndex((item) => item.id === scenarioId.value);
}

function applyScenario(time) {
  const p = progress.value / 100;
  const mode = modeIndex();
  const isGenesis = scenario.value.type === "genesis";
  const isBlackHole = scenarioId.value === "blackhole";
  const isGeological = scenarioId.value === "geological";
  const accent = new THREE.Color(scenario.value.accent);
  renderer.setClearColor(0x02060d, 1);
  scene.fog.color.set(0x02060d);
  scene.fog.density = .028;
  daylight.intensity = 2.2;
  bloomPass.strength = isBlackHole ? 1.28 : .88;
  bloomPass.radius = isBlackHole ? .76 : .56;
  bloomPass.threshold = .18;
  fractureFlare.visible = false;
  fractureLight.intensity = 0;
  worldMaterial.uniforms.uTime.value = time;
  worldMaterial.uniforms.uProgress.value = p;
  worldMaterial.uniforms.uMode.value = mode;
  worldMaterial.uniforms.uBright.value.copy(accent);
  worldMaterial.uniforms.uDeep.value.set(isGenesis ? "#061a31" : "#170b19");
  atmosphere.material.color.copy(accent);
  atmosphere.material.opacity = isGenesis ? .05 + p * .15 : Math.max(0, .18 - p * .16);

  world.rotation.y = time * .065;
  world.rotation.x = Math.sin(time * .12) * .08;
  world.position.set(0, 0, 0);
  particles.rotation.z = 0;
  formation.rotation.z = time * .08;
  formation.rotation.y = time * .04;
  formation.visible = scenarioId.value === "refinement";
  star.visible = scenarioId.value === "stellar";
  starLight.visible = star.visible;
  blackHole.visible = isBlackHole;
  fragmentGroup.visible = false;
  magmaCore.visible = false;
  chaosThreads.visible = false;
  originEnergyCore.visible = false;
  starField.visible = true;
  starField.material.uniforms.uTime.value=time;
  starField.material.uniforms.uOpacity.value=.72;
  particles.visible = true;
  solarSystem.visible = false;
  galaxyGroup.visible = false;
  nebulaBackdrop.visible = false;
  planetMoon.visible = false;
  surfaceWorld.visible = false;
  auroraGroup.visible = false;
  primordialEnergyGroup.visible=false;
  singularityFlash.visible = false;
  camera.position.x = 0;
  camera.position.y = .25;
  camera.fov = 42;
  camera.up.set(0, 1, 0);

  if (isGenesis) {
    if (p < .292) {
      renderer.setClearColor(0x000000, 1);
      scene.fog.color.set(0x000000);
      scene.fog.density = .045;
    }
    starField.visible = p >= .69;
    starField.material.uniforms.uTime.value=time;
    starField.material.uniforms.uOpacity.value=starField.visible?smooth(p,.69,.735)*.78:0;
    particles.visible = p >= .278;
    const energyCollapse = 1 - smooth(p, .18, .216);
    const buildupProgress = Math.min(p, .16);
    const slowdownProgress = THREE.MathUtils.clamp((Math.min(p,.216)-.16)/.056,0,1);
    const energyMotionPhase = buildupProgress*44+buildupProgress*buildupProgress*120+(slowdownProgress-slowdownProgress*slowdownProgress*.5)*4.5;
    chaosThreads.visible = p > .005 && p < .217;
    const threadIntensity = smooth(p, .012, .12) * energyCollapse;
    chaosThreads.rotation.z = energyMotionPhase;
    chaosThreads.rotation.y = Math.sin(energyMotionPhase*.38) * .18;
    chaosThreads.scale.setScalar(1.1 - smooth(p, .025, .23) * .38);
    chaosThreads.children.forEach((thread, index) => {
      thread.userData.material.uniforms.uTime.value = energyMotionPhase * (1.15 + index % 5 * .035);
      thread.userData.material.uniforms.uIntensity.value = threadIntensity * (.58 + (index % 7) * .065);
    });
    const originBuild = smooth(p,.055,.178);
    const rebirthExpansion=smooth(p,.226,.241);
    const rebirthFade=1-smooth(p,.241,.248);
    const isOriginalCore=p>.045&&p<.217;
    const isRebirthCore=p>.226&&p<.248;
    originEnergyCore.visible=isOriginalCore||isRebirthCore;
    if(isOriginalCore){
      const collapseScale=Math.pow(energyCollapse,.72);
      originEnergyCore.scale.setScalar((.08+originBuild*1.58+Math.sin(energyMotionPhase*2.2)*.03*originBuild)*collapseScale);
      originEnergyCore.rotation.set(energyMotionPhase*.14,energyMotionPhase*.21,energyMotionPhase*.09);
      originEnergyCore.material.uniforms.uTime.value=energyMotionPhase;
      originEnergyCore.material.uniforms.uIntensity.value=originBuild*energyCollapse;
      originEnergyCore.children[0].material.opacity=originBuild*energyCollapse*.22;
    }else if(isRebirthCore){
      const rebirthPhase=smooth(p,.226,.246)*7.5;
      originEnergyCore.scale.setScalar(.006+rebirthExpansion*2.32);
      originEnergyCore.rotation.set(rebirthPhase*.17,rebirthPhase*.28,rebirthPhase*.11);
      originEnergyCore.material.uniforms.uTime.value=rebirthPhase;
      originEnergyCore.material.uniforms.uIntensity.value=rebirthExpansion*rebirthFade;
      originEnergyCore.children[0].material.opacity=rebirthExpansion*rebirthFade*.3;
    }

    const flashBurst = smooth(p,.241,.249);
    singularityFlash.visible = p >= .241 && p < .258;
    singularityFlash.scale.setScalar(.02+flashBurst*13.5);
    singularityFlash.material.opacity = smooth(p,.241,.244)*(1-smooth(p,.247,.258));

    primordialEnergyGroup.visible=p>.255&&p<.435;
    const primordialExpansion=smooth(p,.255,.37);
    const primordialFade=1-smooth(p,.365,.435);
    primordialEnergyGroup.rotation.z=time*.025;
    primordialEnergyGroup.userData.clouds.forEach((cloud,index)=>{cloud.material.uniforms.uTime.value=time*(.72+index*.08);cloud.material.uniforms.uExpansion.value=primordialExpansion*(.82+index*.07);cloud.material.uniforms.uOpacity.value=smooth(p,.255+index*.004,.29)*primordialFade;cloud.rotation.x=Math.sin(time*.08+index)*.08;});
    primordialEnergyGroup.userData.bolts.forEach((bolt,index)=>{const pulse=Math.pow(Math.max(0,Math.sin(time*(3.4+index%3)+bolt.userData.phase)),24);bolt.material.opacity=primordialFade*smooth(p,.265,.305)*pulse*.72;});

    auroraGroup.visible = p > .292 && p < .42;
    const lawExpansion = smooth(p, .292, .405);
    const lawFade = 1 - smooth(p, .36, .42);
    auroraGroup.scale.setScalar(.035+lawExpansion*3.65);
    auroraGroup.rotation.y=time*.12;
    auroraGroup.rotation.z=time*.055;
    auroraGroup.children.forEach((ribbon, index) => {
      ribbon.position.copy(ribbon.userData.expansionDirection).multiplyScalar(lawExpansion*(1.15+index*.62));
      ribbon.material.opacity=(.13+Math.sin(time*1.1+index)*.045)*smooth(p,.292,.32)*lawFade;
    });

    nebulaBackdrop.visible = p >= .69;
    nebulaBackdrop.rotation.y = time * .0015;
    nebulaBackdrop.rotation.x = Math.sin(time * .001) * .01;
    nebulaBackdrop.children.forEach((cloud) => {
      cloud.material.opacity = cloud.userData.baseOpacity * smooth(p, .69, .74) * (p<.91?(1-smooth(p,.88,.91)):0);
    });

    galaxyGroup.visible = p >= .425 && p < .755;
    const galaxyReveal = smooth(p, .425, .46);
    const galaxyOverviewZoom = smooth(p, .425, .56);
    const galaxyTopDown = smooth(p, .5, .6);
    const galaxyArmApproach = smooth(p, .6, .72);
    galaxyGroup.scale.setScalar(.015 + galaxyOverviewZoom * 1.62 + galaxyArmApproach * .28);
    galaxyGroup.rotation.x = THREE.MathUtils.lerp(.78, .035, galaxyTopDown);
    galaxyGroup.rotation.y = THREE.MathUtils.lerp(-.16, 0, galaxyTopDown);
    galaxyGroup.rotation.z = -.12 + time * .048;
    galaxyGroup.children.forEach((layer) => {
      layer.rotation.z = time * .014;
      layer.material.opacity = .94 * galaxyReveal * (1 - smooth(p, .705, .755));
    });

    solarSystem.visible = p >= .7 && p < .918;
    solarSystem.scale.setScalar(.03 + smooth(p, .7, .79) * 1.13);
    const centerTransition = 1 - smooth(p, .72, .77);
    solarSystem.position.set(3.4 * centerTransition, 1.1 * centerTransition, .1 * centerTransition);
    solarSystem.rotation.x = .48;
    solarSystem.rotation.z = -.16 + time * .015;
    solarSystem.children.forEach((child)=>{if(child.userData.isOrbit)child.material.opacity=.32*(1-smooth(p,.875,.905));});
    solarSystem.userData.planets.forEach((body, index) => {
      const orbitalClock = smooth(p, .7, .905) * 31;
      const angle = body.userData.phase + orbitalClock * body.userData.orbitalSpeed;
      body.position.set(Math.cos(angle) * body.userData.radius, 0, Math.sin(angle) * body.userData.radius);
      body.userData.planet.rotation.y = p * (42 + index * 3);
      body.userData.moonPivot && (body.userData.moonPivot.rotation.y = p * 76);
      if (body.userData.saturnRings) body.userData.saturnRings.userData.dust.rotation.z = time * .045;
      if(index===2){
        const atmosphereBirth=smooth(p,.79,.86);
        body.userData.planet.material.uniforms.uTime.value=time;
        body.userData.planet.material.uniforms.uMaturity.value=smooth(p,.755,.82);
        body.userData.atmosphereShell.material.uniforms.uDensity.value=atmosphereBirth;
        body.userData.cloudShell.material.uniforms.uTime.value=time;
        body.userData.cloudShell.material.uniforms.uDensity.value=smooth(p,.815,.875)*.78;
        body.userData.cloudShell.rotation.y=time*.025;
      }
      body.scale.setScalar(1);
    });
    solarSystem.updateMatrixWorld(true);
    solarSystem.userData.livingPlanet.getWorldPosition(focusPosition);
    solarSystem.userData.sun.getWorldPosition(sunPosition);

    world.visible = false;
    worldMaterial.uniforms.uDeep.value.set("#123d65");
    worldMaterial.uniforms.uBright.value.set("#72d99a");
    planetMoon.visible = false;

    surfaceWorld.visible = p >= .91;
    if (surfaceWorld.visible) {
      const surfaceEntry = smooth(p, .91, .928);
      const stormRise=smooth(p,.912,.925);
      const storm=stormRise*(1-smooth(p,.968,.982));
      const green = smooth(p, .956, .978);
      const dinosaurAge=smooth(p,.979,.989)*(1-smooth(p,.993,.999));
      const city = smooth(p, .993, 1);
      const cloudCycle = .42 + Math.sin(time*.11)*.18 + Math.sin(time*.037+2.1)*.12;
      const dayCycle = time*.018 + p*5;
      const sunHeight = .25 + Math.sin(dayCycle)*.55;
      const daylightAmount = smooth(sunHeight, -.08, .48);
      const skyColor = new THREE.Color(0x01040d).lerp(new THREE.Color(0x66b8ed),Math.pow(daylightAmount,.72)).lerp(new THREE.Color(0x202a33),storm*.66);
      renderer.setClearColor(skyColor, 1);
      scene.fog.color.copy(skyColor);
      scene.fog.density = .008 + (1-surfaceEntry)*.075;
      bloomPass.threshold = 1.05;
      bloomPass.strength = .08+storm*.22;
      bloomPass.radius = .15;

      surfaceTerrain.material.uniforms.uFormation.value=surfaceEntry;
      surfaceTerrain.material.uniforms.uLife.value=green;
      surfaceWater.material.uniforms.uTime.value=time;
      surfaceWater.material.uniforms.uFormation.value=surfaceEntry;
      const sunDirection=new THREE.Vector3(Math.cos(dayCycle),sunHeight,Math.sin(dayCycle)).normalize();
      surfaceTerrain.material.uniforms.uSunDirection.value.copy(sunDirection);
      surfaceWater.material.uniforms.uSunDirection.value.copy(sunDirection);
      surfaceMountains.material.uniforms.uSunDirection.value.copy(sunDirection);
      surfaceAtmosphere.visible=false;
      surfaceMountains.material.uniforms.uOpacity.value=surfaceEntry;
      surfaceSnowCaps.material.transparent=true;
      surfaceSnowCaps.material.opacity=smooth(p,.91,.95);
      surfaceForest.material.opacity=green;
      surfaceShrubs.material.opacity=green;
      surfaceCivilization.material.opacity=city;
      surfaceRain.material.opacity=storm*(.58+Math.sin(time*4.1)*.16);
      surfaceLightningGroup.children.forEach((stormCell,index)=>{const pulse=Math.pow(Math.max(0,Math.sin(time*(4.8+index%3)+stormCell.userData.phase)),18);stormCell.children.forEach((bolt,branchIndex)=>{bolt.material.opacity=storm*pulse*bolt.userData.strength*(1-branchIndex*.035);});});
      surfaceVolcanoGroup.children.forEach((volcano)=>{volcano.children.forEach((part)=>{if(part.userData.isLava)part.material.opacity=storm*(.35+.65*Math.max(0,Math.sin(time*2.4+volcano.userData.phase)));});});
      surfaceDinosaurs.children.forEach((dino)=>{const s=dino.userData.baseScale*dinosaurAge;dino.scale.setScalar(s);dino.rotation.y+=.0015*Math.sin(time+dino.userData.phase);});
      surfaceCloudLayer.visible=surfaceEntry>.16;
      surfaceCloudLayer.material.uniforms.uSunDirection.value.copy(sunDirection);
      surfaceCloudLayer.material.uniforms.uOpacity.value=surfaceEntry*(.82+storm*.16)*(.78+cloudCycle*.16);
      surfaceCloudLayer.rotation.y=time*(.014+storm*.07);
      surfaceCloudLayer.rotation.z=Math.sin(time*.006)*.08;
      surfaceRain.rotation.copy(surfaceCloudLayer.rotation);
      starField.material.uniforms.uOpacity.value=surfaceEntry*(1-smooth(daylightAmount,.08,.32))*.82;
      const nightVisibility=surfaceEntry*(1-smooth(daylightAmount,.02,.24));
      nebulaBackdrop.children.forEach((cloud)=>{cloud.material.opacity=cloud.userData.baseOpacity*nightVisibility*.52;});
      surfaceMilkyWay.position.copy(camera.position);
      surfaceMilkyWay.rotation.set(.62,time*.002,.24);
      surfaceMilkyWay.material.uniforms.uOpacity.value=nightVisibility*.78;
      const moonOrbit=time*.037+p*3.1;
      surfaceMoon.position.set(Math.cos(moonOrbit)*36,Math.sin(moonOrbit*.83)*18,Math.sin(moonOrbit)*36);
      surfaceMoon.visible=nightVisibility>.08;
      surfaceMoon.material.uniforms.uSunDirection.value.copy(sunDirection);
      surfaceMoon.rotation.y=time*.015;
      daylight.position.copy(sunDirection).multiplyScalar(20);
      daylight.intensity=.55+daylightAmount*2.15;

      const flightClock=time*.065+Math.max(0,p-.91)*52;
      const latitude=Math.sin(flightClock*.37)*.62+Math.sin(flightClock*.13+1.7)*.2;
      const longitude=flightClock*.73+Math.sin(flightClock*.21)*.48;
      const horizonClock=flightClock+.34;
      const horizonLatitude=Math.sin(horizonClock*.37)*.62+Math.sin(horizonClock*.13+1.7)*.2;
      const horizonLongitude=horizonClock*.73+Math.sin(horizonClock*.21)*.48;
      const normal=new THREE.Vector3(Math.cos(latitude)*Math.cos(longitude),Math.sin(latitude),Math.cos(latitude)*Math.sin(longitude)).normalize();
      const horizonNormal=new THREE.Vector3(Math.cos(horizonLatitude)*Math.cos(horizonLongitude),Math.sin(horizonLatitude),Math.cos(horizonLatitude)*Math.sin(horizonLongitude)).normalize();
      const terrainHeight=surfaceWorld.userData.terrainHeightAt(normal)*surfaceEntry;
      const droneAltitude=.8+Math.sin(flightClock*.29)*.13+Math.sin(flightClock*.071)*.09;
      camera.position.copy(normal).multiplyScalar(surfaceWorld.userData.radius+terrainHeight+droneAltitude+(1-surfaceEntry)*1.5);
      surfaceSky.position.copy(camera.position);
      surfaceSky.material.uniforms.uUp.value.copy(normal);
      surfaceSky.material.uniforms.uSunDirection.value.copy(sunDirection);
      surfaceSky.material.uniforms.uDaylight.value=daylightAmount;
      surfaceSky.material.uniforms.uStorm.value=storm;
      const forward=horizonNormal.addScaledVector(normal,-horizonNormal.dot(normal)).normalize();
      const right=new THREE.Vector3().crossVectors(forward,normal).normalize();
      const target=camera.position.clone().addScaledVector(forward,3.6).addScaledVector(normal,-1.9);
      camera.up.copy(normal).addScaledVector(right,Math.sin(flightClock*.19)*.055).normalize();
      camera.lookAt(target);
      camera.fov=54-Math.sin(flightClock*.17)*2;
    } else {
      if (p < .218) {
        const originOrbit=smooth(p,.018,.205)*Math.PI*2.35;
        const orbitRadius=8.4-smooth(p,.04,.19)*1.35;
        camera.position.set(
          Math.sin(originOrbit)*orbitRadius*.46,
          Math.sin(originOrbit*.63+.4)*1.65,
          Math.cos(originOrbit)*orbitRadius*.13+orbitRadius,
        );
        camera.up.set(0,1,0);
        camera.lookAt(0,0,0);
      } else if (p < .5) {
        camera.position.set(0, .25, 9.6 - smooth(p, .23, .5) * 1.3);
        camera.lookAt(0, 0, 0);
      } else if (p < .6) {
        const overheadMove = smooth(p, .5, .6);
        camera.position.set(
          Math.sin(overheadMove * Math.PI) * .28,
          THREE.MathUtils.lerp(.25, 0, overheadMove),
          THREE.MathUtils.lerp(8.3, 7.7, overheadMove),
        );
        camera.lookAt(0, 0, 0);
      } else if (p < .72) {
        const armZoom = smooth(p, .6, .72);
        camera.position.set(armZoom * 2.75, armZoom * .82, 7.7 - armZoom * 3.55);
        camera.lookAt(3.4 * armZoom, 1.1 * armZoom, 0);
      } else if (p < .855) {
        const systemZoom = smooth(p, .72, .855);
        const widePosition = new THREE.Vector3(2.75, .82, 4.15);
        const radial=focusPosition.clone().sub(sunPosition).normalize();
        const orbitalNormal=new THREE.Vector3(0,1,0).applyQuaternion(solarSystem.quaternion).normalize();
        const tangent=new THREE.Vector3().crossVectors(orbitalNormal,radial).normalize();
        if(tangent.dot(widePosition.clone().sub(focusPosition))<0)tangent.negate();
        const nearPlanet=focusPosition.clone().addScaledVector(tangent,1.38).addScaledVector(radial,.34).addScaledVector(orbitalNormal,.38);
        const control=sunPosition.clone().addScaledVector(tangent,3.15).addScaledVector(orbitalNormal,2.25).addScaledVector(radial,.7);
        const inverse=1-systemZoom;
        camera.position.copy(widePosition).multiplyScalar(inverse*inverse).addScaledVector(control,2*inverse*systemZoom).addScaledVector(nearPlanet,systemZoom*systemZoom);
        camera.lookAt(focusPosition);
      } else {
        const earthApproach = smooth(p, .855, .91);
        const radial=focusPosition.clone().sub(sunPosition).normalize();
        const orbitalNormal=new THREE.Vector3(0,1,0).applyQuaternion(solarSystem.quaternion).normalize();
        const tangent=new THREE.Vector3().crossVectors(orbitalNormal,radial).normalize();
        const reference=new THREE.Vector3(2.75,.82,4.15).sub(focusPosition);if(tangent.dot(reference)<0)tangent.negate();
        const approachStart=focusPosition.clone().addScaledVector(tangent,1.38).addScaledVector(radial,.34).addScaledVector(orbitalNormal,.38);
        const approachEnd=focusPosition.clone().addScaledVector(tangent,.52).addScaledVector(radial,.18).addScaledVector(orbitalNormal,.26);
        camera.position.copy(approachStart.lerp(approachEnd,earthApproach));
        camera.lookAt(focusPosition);
      }
    }
    const originMatterReveal=smooth(p,.278,.315);
    const genesisParticleOpacity=p < .03 ? p * 4 : (p > .72 ? Math.max(.05, 1 - smooth(p, .72, .88)) : .88);
    particles.material.opacity = genesisParticleOpacity*originMatterReveal;
  } else {
    world.visible = (p < .995 || scenarioId.value !== "erasure") && !(isBlackHole && p > .79);
    particles.scale.setScalar(1 + p * (scenarioId.value === "geological" ? 1.8 : .72));
    particles.material.opacity = .16 + p * .82;
    camera.position.z = 7.1 - Math.sin(p * Math.PI) * .7;
    let scale = 1;
    if (scenarioId.value === "geological") scale = 1 + Math.max(0, p - .55) * .85;
    if (isBlackHole) scale = 1 - smooth(p, .28, .82) * .94;
    if (scenarioId.value === "law") scale = 1 + Math.sin(time * 14) * p * .045;
    if (scenarioId.value === "depletion") scale = 1 - p * .12;
    if (scenarioId.value === "refinement") scale = 1 - Math.pow(p, 2) * .76;
    if (scenarioId.value === "erasure") scale = 1 - Math.max(0, p - .72) * 2.8;
    world.scale.setScalar(Math.max(.03, scale));
    if (star.visible) {
      star.position.x = -7 + p * 4.5;
      star.scale.setScalar(1 + p * 2.7);
    }
    if (formation.visible) {
      const seal = smooth(p, .04, .3);
      const compression = smooth(p, .46, .96);
      formation.scale.setScalar((1.42 - seal * .38 - compression * .22) * (1 + Math.sin(time * 1.7) * .025));
      formation.userData.layers.forEach((layer, index) => {
        const spin = layer.userData.spin;
        const base = layer.userData.baseRotation;
        layer.rotation.set(
          base.x + time * spin.x,
          base.y + time * spin.y,
          base.z + time * spin.z * (1 + compression * 2.4),
        );
        layer.scale.setScalar(.86 + seal * .14 + Math.sin(time * 1.4 + layer.userData.phase) * .018);
      });
      formation.userData.orbiters.forEach((pivot, index) => {
        const base = pivot.userData.baseRotation;
        pivot.rotation.set(
          base.x,
          base.y + Math.sin(time * .22 + index) * .12,
          base.z + time * pivot.userData.spin * (1 + compression * 1.8),
        );
      });
      formation.userData.runes.forEach((rune, index) => {
        const pulse = .82 + Math.sin(time * 3.1 + index * .67) * .18;
        rune.scale.setScalar(pulse);
      });
      formation.userData.materials.forEach((material) => {
        material.opacity = material.userData.baseOpacity * (.18 + seal * .82) * (1 - smooth(p, .94, 1) * .62);
      });
    }
    if (isGeological) {
      const fracture = smooth(p, .18, .55);
      const explode = smooth(p, .48, .94);
      const debrisBirth = smooth(p, .38, .58);
      world.visible = p < .48;
      fragmentGroup.visible = p >= .48;
      fragmentGroup.rotation.y = time * .035;
      fragmentMaterial.uniforms.uTime.value = time;
      fragmentMaterial.uniforms.uFracture.value = .18 + fracture * .92;
      magmaCore.visible = p >= .43;
      magmaCore.material.uniforms.uTime.value = time;
      magmaCore.material.uniforms.uCollapse.value = explode;
      magmaCore.scale.setScalar(1.04 - explode * .18);
      fractureAura.visible = p >= .22 && p < .93;
      fractureAura.material.uniforms.uTime.value = time;
      fractureAura.material.uniforms.uStrength.value = fracture * (1 - smooth(p, .78, .94));
      fractureAura.scale.setScalar(1 + Math.sin(time * 1.8) * .035 + explode * .38);
      fragmentGroup.userData.fragments.forEach((shard) => {
        const travel = shard.userData.detached ? 3.8 : .32;
        const tumble = shard.userData.detached ? 2.4 : .22;
        shard.position.copy(shard.userData.base).addScaledVector(shard.userData.velocity, Math.pow(explode, 1.35) * travel);
        shard.rotation.set(
          shard.userData.spin.x * explode * tumble,
          shard.userData.spin.y * explode * tumble,
          shard.userData.spin.z * explode * tumble,
        );
        shard.scale.setScalar(1 - explode * (shard.userData.detached ? .14 : .025));
      });
      debrisField.visible = debrisBirth > .02;
      debrisField.userData.orbits.forEach((data, index) => {
        const radius = data.radius * (.34 + explode * .86);
        const angle = data.angle + p * data.speed * 18 + time * data.speed * .13;
        debrisPosition.set(
          Math.cos(angle) * radius,
          data.height * (.4 + explode) + Math.sin(angle * 1.7 + data.tilt) * .22,
          Math.sin(angle) * radius,
        );
        debrisEuler.set(data.spin.x * p, data.spin.y * p, data.spin.z * p);
        debrisQuaternion.setFromEuler(debrisEuler);
        debrisScale.setScalar(data.size * debrisBirth * (.72 + explode * .46));
        debrisMatrix.compose(debrisPosition, debrisQuaternion, debrisScale);
        debrisField.setMatrixAt(index, debrisMatrix);
      });
      debrisField.instanceMatrix.needsUpdate = true;

      fractureFlare.visible = p >= .16 && p < .97;
      fractureFlare.material.uniforms.uStrength.value = fracture * (1 - smooth(p, .88, .98));
      fractureLight.intensity = fracture * (5.5 + Math.sin(time * 4.2) * .7) * (1 - smooth(p, .87, 1));
      bloomPass.strength = 1.25 + fracture * .82;
      bloomPass.radius = .72;

      const cameraOrbit = .22 + smooth(p, .05, .92) * Math.PI * 1.18;
      const cameraDistance = 7.7 - smooth(p, .08, .62) * 3.5 + smooth(p, .73, 1) * 2.4;
      camera.position.set(
        Math.sin(cameraOrbit) * cameraDistance,
        .55 + Math.sin(cameraOrbit * .72) * 1.05,
        Math.cos(cameraOrbit) * cameraDistance,
      );
      const cameraShake = fracture * (1 - smooth(p, .7, .86));
      camera.position.x += Math.sin(time * 19) * cameraShake * .015;
      camera.position.y += Math.cos(time * 17) * cameraShake * .01;
      camera.fov = 42 - smooth(p, .18, .68) * 8 + smooth(p, .8, 1) * 5;
    }
    if (isBlackHole) {
      blackHole.scale.setScalar(.02 + smooth(p, .28, .88) * 1.28);
      blackHole.rotation.z = -.08;
      blackHoleLens.material.uniforms.uTime.value = time;
      blackHoleLens.material.uniforms.uProgress.value = p;
      spacetimeGrid.material.uniforms.uTime.value = time;
      spacetimeGrid.material.uniforms.uStrength.value = smooth(p, .14, .84);
      singularityCore.material.opacity = smooth(p, .3, .48) * (1 - smooth(p, .58, .76));
      singularityCore.scale.setScalar(.3 + Math.sin(time * 8) * .08 + smooth(p, .3, .62) * .8);
      blackHole.children.forEach((object, index) => {
        if (object.geometry?.type === "TorusGeometry" && Number.isFinite(object.userData.spin)) object.rotation.z = time * object.userData.spin + index * .08;
      });
      const orbitView = smooth(p, .42, .76);
      camera.position.x = Math.sin(orbitView * Math.PI) * 1.05;
      camera.position.y = .25 + Math.sin(orbitView * Math.PI * .7) * .42;
      camera.position.z = 7.4 - smooth(p, .2, .78) * 1.45;
    }
    camera.lookAt(0, 0, 0);
    if (isGeological) fractureFlare.quaternion.copy(camera.quaternion);
  }

  camera.fov = THREE.MathUtils.clamp(camera.fov * userZoom.value, 18, 78);
  camera.updateProjectionMatrix();

  const positions = particles.geometry.attributes.position.array;
  for (let i = 0; i < positions.length; i += 3) {
    const bx = particleBase[i];
    const by = particleBase[i + 1];
    const bz = particleBase[i + 2];
    const radius = Math.sqrt(bx * bx + by * by);
    const baseAngle = Math.atan2(by, bx);
    const wave = Math.sin(time * 1.6 + i * .017) * .018 * (1 + p * 2);
    if (isGenesis) {
      const beforeBang = p < .246;
      const radial = beforeBang ? 1.85*(1-smooth(p,.01,.23))+.008 : .008+smooth(p,.278,.46)*1.12;
      const galaxy = smooth(p, .36, .62);
      const angle = baseAngle + (beforeBang ? p * 13 + time * (.04 + p * .38) : galaxy * radius * .38 + time * .075);
      const diskX=Math.cos(angle)*radius*radial+wave;const diskY=Math.sin(angle)*radius*radial+wave;const diskZ=bz*radial*(1-galaxy*.86);
      if(!beforeBang&&p<.44){
        const branch=(i/3)%3;const branchAngle=branch*Math.PI*2/3+.35;const dx=Math.cos(branchAngle),dy=Math.sin(branchAngle),dz=(branch-1)*.24;const length=radius*radial;const curl=Math.sin(radius*2.7+time*.8+branch)*(.06+length*.055);const lawX=dx*length-bz*.12+curl*(-dy);const lawY=dy*length+bx*.055+curl*dx;const lawZ=dz*length+by*.12;const lawToGalaxy=smooth(p,.395,.44);positions[i]=THREE.MathUtils.lerp(lawX,diskX,lawToGalaxy);positions[i+1]=THREE.MathUtils.lerp(lawY,diskY,lawToGalaxy);positions[i+2]=THREE.MathUtils.lerp(lawZ,diskZ,lawToGalaxy);
      }else{positions[i]=diskX;positions[i+1]=diskY;positions[i+2]=diskZ;}
    } else if (isBlackHole) {
      const radial = 1 - smooth(p, .22, .94) * .91;
      const angle = baseAngle + smooth(p, .2, 1) * 10 + time * .32;
      positions[i] = Math.cos(angle) * radius * radial;
      positions[i + 1] = Math.sin(angle) * radius * radial * .34;
      positions[i + 2] = bz * radial;
    } else {
      const radial = 1 + Math.max(0, p - .35) * (mode === 1 ? 2.3 : .75);
      positions[i] = bx * radial + wave;
      positions[i + 1] = by * radial + wave * .7;
      positions[i + 2] = bz * radial;
    }
  }
  particles.geometry.attributes.position.needsUpdate = true;
}

function animate(now) {
  const delta = Math.min(.05, (now - lastTime) / 1000);
  lastTime = now;
  if (playing.value) {
    progress.value = Math.min(100, progress.value + (delta / scenario.value.duration) * 100 * speed.value);
    if (progress.value >= 100) playing.value = false;
  }
  applyScenario(now / 1000);
  composer.render();
  frame = requestAnimationFrame(animate);
}

function chooseScenario(id) {
  scenarioId.value = id;
  progress.value = 0;
  playing.value = true;
  userZoom.value = 1;
}

function handleWheel(event) {
  userZoom.value = THREE.MathUtils.clamp(userZoom.value * Math.exp(event.deltaY * .0011), .45, 1.7);
}

function restart() {
  progress.value = 0;
  playing.value = true;
}

async function enterDisplayMode(){
  displayMode.value=true;
  try{await observatory.value?.requestFullscreen?.();}catch{/* CSS fallback remains active when native fullscreen is unavailable. */}
}

function syncFullscreen(){
  if(!document.fullscreenElement)displayMode.value=false;
}
function handleDisplayKey(event){if(event.key==="Escape"&&!document.fullscreenElement)displayMode.value=false;}

function disposeObject(object) {
  object.traverse?.((child) => {
    child.geometry?.dispose?.();
    if (Array.isArray(child.material)) child.material.forEach((material) => material.dispose());
    else child.material?.dispose?.();
  });
}

watch(scenarioId, () => nextTick(() => applyScenario(performance.now() / 1000)));

onMounted(() => {
  webGpuAvailable.value = "gpu" in navigator;
  createScene();
  document.addEventListener("fullscreenchange",syncFullscreen);
  document.addEventListener("keydown",handleDisplayKey);
  frame = requestAnimationFrame(animate);
});

onUnmounted(() => {
  cancelAnimationFrame(frame);
  observer?.disconnect();
  disposeObject(scene);
  composer?.dispose?.();
  renderer?.dispose();
  renderer?.domElement?.remove();
  document.removeEventListener("fullscreenchange",syncFullscreen);
  document.removeEventListener("keydown",handleDisplayKey);
});
</script>

<template>
  <section ref="observatory" class="cosmic-observatory" :class="{ 'display-mode': displayMode }" :style="{ '--cosmic-accent': scenario.accent }">
    <div ref="host" class="cosmic-canvas" aria-label="ฉากจำลองจักรวาลแบบสามมิติ" @wheel.prevent="handleWheel"></div>
    <div class="cosmic-vignette"></div>
    <div class="cosmic-bigbang-flash" :style="{ opacity: bigBangFlashOpacity }"></div>
    <div class="cosmic-cloud-transition" :style="{ opacity: cloudTransitionOpacity }"></div>

    <header class="cosmic-header">
      <div>
        <small>UNIVERSAL LAW OBSERVATORY · {{ universe?.name ?? "SANDBOX" }}</small>
        <h1>{{ scenario.name }}</h1>
        <p>{{ scenario.subtitle }}</p>
      </div>
      <div class="cosmic-header-actions">
        <span class="renderer-chip"><i></i>{{ rendererLabel }}</span>
        <span class="renderer-chip" :class="{ ready: webGpuAvailable }">WebGPU {{ webGpuAvailable ? "พร้อมใช้" : "ไม่พร้อม" }}</span>
        <button type="button" class="renderer-chip zoom-chip" @click="userZoom = 1">ล้อเมาส์ · ZOOM {{ (1 / userZoom).toFixed(2) }}×</button>
        <button type="button" class="renderer-chip display-chip" @click="enterDisplayMode">เต็มจอไร้ส่วนควบคุม</button>
        <button type="button" class="cosmic-close" @click="emit('close')" aria-label="ปิดฉากจำลอง">ปิด ×</button>
      </div>
    </header>

    <nav class="scenario-rail" aria-label="เลือกฉากจำลอง">
      <button
        v-for="item in scenarios"
        :key="item.id"
        type="button"
        :class="{ active: scenarioId === item.id }"
        @click="chooseScenario(item.id)"
      >
        <span>{{ item.type === "genesis" ? "GENESIS" : "COLLAPSE" }}</span>
        <b>{{ item.name }}</b>
      </button>
    </nav>

    <aside class="cosmic-readout">
      <small>สถานะปัจจุบัน · {{ progress.toFixed(1) }}%</small>
      <h2>{{ phase[1] }}</h2>
      <p>{{ phase[2] }}</p>
      <dl>
        <div v-for="metric in metrics" :key="metric[0]"><dt>{{ metric[0] }}</dt><dd>{{ metric[1] }}</dd></div>
      </dl>
    </aside>

    <footer class="cosmic-timeline">
      <div class="timeline-status">
        <button type="button" class="play-control" @click="playing = !playing">{{ playing ? "❚❚" : "▶" }}</button>
        <button type="button" class="restart-control" @click="restart">↺ เริ่มใหม่</button>
        <div><small>เวลาจำลอง</small><strong>{{ elapsedYears.toLocaleString() }} ปี</strong></div>
        <label>ความเร็ว
          <select v-model.number="speed">
            <option :value="0.25">0.25×</option><option :value="0.5">0.5×</option>
            <option :value="1">1×</option><option :value="2">2×</option>
            <option :value="4">4×</option><option :value="8">8×</option><option :value="16">16×</option>
          </select>
        </label>
      </div>
      <div class="scrubber-wrap">
        <input v-model.number="progress" type="range" min="0" max="100" step="0.05" aria-label="ไทม์ไลน์ฉากจำลอง" />
        <div class="phase-marks">
          <button
            v-for="entry in scenario.phases"
            :key="entry[0]"
            type="button"
            :class="{ passed: progress >= entry[0] }"
            :style="{ left: `${entry[0]}%` }"
            @click="progress = entry[0]"
          ><i></i><span>{{ entry[1] }}</span></button>
        </div>
      </div>
    </footer>
  </section>
</template>
