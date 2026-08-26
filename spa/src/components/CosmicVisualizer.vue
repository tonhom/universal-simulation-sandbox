<script setup>
import { computed, nextTick, onMounted, onUnmounted, ref, watch } from "vue";
import * as THREE from "three";
import { EffectComposer } from "three/addons/postprocessing/EffectComposer.js";
import { RenderPass } from "three/addons/postprocessing/RenderPass.js";
import { UnrealBloomPass } from "three/addons/postprocessing/UnrealBloomPass.js";

const props = defineProps({ universe: { type: Object, default: null } });
const emit = defineEmits(["close"]);

const scenarios = [
  {
    id: "genesis",
    type: "genesis",
    name: "การกำเนิดจักรวาล",
    subtitle: "จาก Origin Singularity สู่ดวงดาว โลก และเมล็ดพันธุ์ชีวิต",
    duration: 100,
    accent: "#63f5d0",
    phases: [
      [0, "ความว่างเปล่า", "ยังไม่มี Space, Time หรือสิ่งที่ผู้สังเกตการณ์เรียกว่าอนุภาค"],
      [5, "อนุภาคแรก", "ความผันผวนเล็กน้อยเริ่มหมุนวนเข้าสู่ศูนย์กลาง"],
      [14, "Origin Accretion", "Matter, Energy และความเป็นไปได้สะสมหนาแน่นขึ้นเรื่อย ๆ"],
      [23, "Big Bang", "จุดกำเนิดระเบิดออกในพริบตา พร้อมแสง ฝุ่น และสนามกฎ"],
      [34, "Space-Time Expansion", "มิติและเวลาบิดตัวออกจากศูนย์กลาง ขณะที่กฎเริ่มแยกหน้าที่"],
      [48, "Galactic Flow", "ฝุ่นออโรร่าหมุนเป็นแขนกาแล็กซีคล้ายทางช้างเผือก"],
      [61, "ดาวฤกษ์ดวงแรก", "กลุ่มก๊าซยุบตัวและจุดปฏิกิริยาตามแขนกาแล็กซี"],
      [72, "ระบบดาวก่อตัว", "ดาวฤกษ์หนึ่งดวงกับดาวเคราะห์แปดดวงก่อตัวจากจานมวลสารในแขนกาแล็กซี"],
      [80, "ดาวเคราะห์มีชีวิต", "กล้องละจากดาวฤกษ์และติดตามดาวเคราะห์ดวงที่สามพร้อมดวงจันทร์บริวาร"],
      [89, "โลกยุคพายุ", "มหาสมุทร ภูเขา แม่น้ำ เมฆ ฝน ฟ้าผ่า และภูเขาไฟกำลังปรับสมดุล"],
      [94, "Life Genesis", "พืช ต้นไม้ ใบหญ้า และสิ่งมีชีวิตแรกเริ่มแพร่กระจาย"],
      [98, "รุ่งอรุณแห่งอารยธรรม", "หลังท้องฟ้าและดวงจันทร์หมุนผ่านกาลเวลา ชุมชนมนุษย์ถือกำเนิด"],
    ],
  },
  {
    id: "geological",
    type: "collapse",
    name: "แก่นโลกและเปลือกโลกแตกสลาย",
    subtitle: "Core instability ทำให้สนามแม่เหล็กดับ แผ่นทวีปฉีก และผิวโลกหลอมเหลว",
    duration: 48,
    accent: "#4de8ff",
    phases: [
      [0, "World อยู่ในภาวะเสถียร", "แก่นโลก แมนเทิล และเปลือกดาวยังรักษาสมดุล"],
      [16, "Core Overpressure", "ความร้อนและแรงดันภายในเกินอัตราที่เปลือกดาวระบายได้"],
      [34, "รอยแยกทั่วดาว", "รอยแตกเรืองแสงเชื่อมกันผ่านแผ่นทวีปและพื้นมหาสมุทร"],
      [49, "Mantle Exposure", "เปลือกดาวแยกออกจนเห็นแก่นแมกมาภายใน"],
      [66, "Planetary Disassembly", "แรงระเบิดส่งแผ่นเปลือกดาวแต่ละชิ้นออกจากศูนย์กลาง"],
      [86, "Debris Field", "World เดิมกลายเป็นเศษดาว แร่หลอมเหลว และกลุ่มฝุ่นในวงโคจร"],
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
  return Math.min(1, Math.exp(-Math.pow((point - .235) * 31, 2)) * 1.45);
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
let formation;
let star;
let starLight;
let singularityFlash;
let auroraGroup;
let galaxyGroup;
let nebulaBackdrop;
let solarSystem;
let planetMoon;
let surfaceWorld;
let surfaceTerrain;
let surfaceWater;
let surfaceAtmosphere;
let surfaceForest;
let surfaceMountains;
let surfaceSnowCaps;
let surfaceCloudLayer;
let surfaceCivilization;
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
    varying float vHeight;
    varying vec3 vNormalW;
    varying vec3 vPoint;
    void main(){
      float slope=1.0-max(0.0,dot(vNormalW,normalize(vPoint)));
      float beach=smoothstep(-.015,.012,vHeight);
      float upland=smoothstep(.045,.12,vHeight);
      float alpine=smoothstep(.105,.17,vHeight);
      float polar=smoothstep(.68,.92,abs(vPoint.y));
      float forestNoise=sin(vPoint.x*61.0)*sin(vPoint.y*47.0)*sin(vPoint.z*53.0)*.5+.5;
      vec3 sand=vec3(.58,.49,.31);
      vec3 grass=mix(vec3(.075,.19,.09),vec3(.19,.34,.13),forestNoise)*mix(.45,1.0,uLife);
      vec3 rock=vec3(.24,.25,.24);
      vec3 snow=vec3(.82,.9,.93);
      vec3 color=mix(sand,grass,beach);
      color=mix(color,rock,clamp(upland+slope*1.8,0.0,1.0));
      color=mix(color,snow,max(alpine,polar*.78));
      float sunlight=.3+max(0.0,dot(vNormalW,normalize(vec3(-.4,.7,.6))))*.85;
      gl_FragColor=vec4(color*sunlight,uFormation);
    }
  `;
  surfaceTerrain = new THREE.Mesh(
    new THREE.SphereGeometry(radius, 160, 104),
    new THREE.ShaderMaterial({ transparent: true, uniforms: { uFormation: { value: 0 }, uLife: { value: 0 } }, vertexShader: terrainVertex, fragmentShader: terrainFragment }),
  );
  root.add(surfaceTerrain);

  surfaceWater = new THREE.Mesh(
    new THREE.SphereGeometry(radius + .012, 128, 80),
    new THREE.ShaderMaterial({
      transparent: true, depthWrite: false,
      uniforms: { uTime: { value: 0 }, uFormation: { value: 0 }, uSunDirection: { value: new THREE.Vector3(1,1,1).normalize() } },
      vertexShader: `uniform float uTime;varying vec3 vN;varying vec3 vWorld;varying float vWave;void main(){vec3 n=normalize(position);float wave=(sin(n.x*58.0+uTime*1.1)+sin(n.z*71.0-uTime*.8)+sin((n.x+n.y)*43.0+uTime*.55))*.004;vWave=wave;vec3 p=n*(5.012+wave);vN=normalize(normalMatrix*n);vec4 world=modelMatrix*vec4(p,1.0);vWorld=world.xyz;gl_Position=projectionMatrix*viewMatrix*world;}`,
      fragmentShader: `uniform float uFormation;uniform vec3 uSunDirection;varying vec3 vN;varying vec3 vWorld;varying float vWave;void main(){vec3 viewDir=normalize(cameraPosition-vWorld);float fresnel=pow(1.0-max(0.0,dot(viewDir,vN)),4.0);float diffuse=max(0.0,dot(vN,uSunDirection));vec3 halfDir=normalize(viewDir+uSunDirection);float spec=pow(max(0.0,dot(vN,halfDir)),90.0);vec3 deep=vec3(.008,.075,.16);vec3 shallow=vec3(.015,.42,.52);vec3 water=mix(deep,shallow,.2+diffuse*.35+vWave*12.0);water=mix(water,vec3(.34,.62,.78),fresnel*.52);water+=spec*.85;gl_FragColor=vec4(water,.82*uFormation);}`,
    }),
  );
  surfaceWater.renderOrder = 2;
  root.add(surfaceWater);

  surfaceAtmosphere = new THREE.Mesh(
    new THREE.SphereGeometry(radius + .34, 96, 64),
    new THREE.ShaderMaterial({
      transparent: true, side: THREE.DoubleSide, depthWrite: false,
      uniforms: { uDensity: { value: 0 } },
      vertexShader: `varying vec3 vN;varying vec3 vWorld;void main(){vN=normalize(normalMatrix*normal);vec4 w=modelMatrix*vec4(position,1.0);vWorld=w.xyz;gl_Position=projectionMatrix*viewMatrix*w;}`,
      fragmentShader: `uniform float uDensity;varying vec3 vN;varying vec3 vWorld;void main(){vec3 viewDir=normalize(cameraPosition-vWorld);float horizon=pow(1.0-abs(dot(viewDir,vN)),2.0);vec3 sky=mix(vec3(.2,.55,.9),vec3(.72,.9,1.0),horizon);gl_FragColor=vec4(sky,(.035+horizon*.22)*uDensity);}`,
    }),
  );
  root.add(surfaceAtmosphere);

  function terrainHeightAt(n) {
    return Math.sin(n.x*3.7+n.z*1.3)*Math.sin(n.y*4.9-n.x*1.7)*.13 + Math.sin(n.x*9.2+n.y*7.4+n.z*5.3)*.055 + Math.sin(n.x*21-n.z*17)*Math.sin(n.y*16)*.022 - .018;
  }
  function spherePoint(lat, lon, r) {
    return new THREE.Vector3(Math.cos(lat)*Math.cos(lon), Math.sin(lat), Math.cos(lat)*Math.sin(lon)).multiplyScalar(r);
  }
  const up = new THREE.Vector3(0,1,0);
  const matrix = new THREE.Matrix4();
  const quaternion = new THREE.Quaternion();
  const position = new THREE.Vector3();
  const scale = new THREE.Vector3();

  const mountainCount = 260;
  surfaceMountains = new THREE.InstancedMesh(new THREE.ConeGeometry(.11,.52,7),new THREE.MeshStandardMaterial({color:0x454b4c,roughness:1,flatShading:true}),mountainCount);
  surfaceSnowCaps = new THREE.InstancedMesh(new THREE.ConeGeometry(.055,.18,7),new THREE.MeshStandardMaterial({color:0xe5edf0,roughness:.92,flatShading:true}),mountainCount);
  for(let i=0;i<mountainCount;i++){
    const belt=i%3;const lon=(i/mountainCount)*Math.PI*8+(belt*.7);const lat=(belt-1)*.48+Math.sin(i*.73)*.11;
    const n=spherePoint(lat,lon,1).normalize();const h=.32+Math.random()*.7;const baseR=radius+Math.max(.04,terrainHeightAt(n));
    quaternion.setFromUnitVectors(up,n);position.copy(n).multiplyScalar(baseR+h*.47);scale.set(.7+Math.random()*.8,h/.52,.7+Math.random()*.8);matrix.compose(position,quaternion,scale);surfaceMountains.setMatrixAt(i,matrix);
    position.copy(n).multiplyScalar(baseR+h*.82);scale.set(.55+Math.random()*.32,h/.52*.44,.55+Math.random()*.32);matrix.compose(position,quaternion,scale);surfaceSnowCaps.setMatrixAt(i,matrix);
  }
  root.add(surfaceMountains,surfaceSnowCaps);

  const treeCount=2200;
  surfaceForest=new THREE.InstancedMesh(new THREE.ConeGeometry(.025,.16,6),new THREE.MeshStandardMaterial({color:0x123b24,roughness:1,flatShading:true,transparent:true,opacity:0}),treeCount);
  let placed=0,attempts=0;
  while(placed<treeCount&&attempts<treeCount*8){attempts++;const lat=(Math.random()-.5)*2.25;const lon=Math.random()*Math.PI*2;const n=spherePoint(lat,lon,1).normalize();const h=terrainHeightAt(n);if(h<.018||h>.13||Math.abs(lat)>1.05)continue;quaternion.setFromUnitVectors(up,n);position.copy(n).multiplyScalar(radius+h+.075);const s=.65+Math.random()*.95;scale.setScalar(s);matrix.compose(position,quaternion,scale);surfaceForest.setMatrixAt(placed++,matrix);}
  surfaceForest.count=placed;
  root.add(surfaceForest);

  const cloudCount=720;
  surfaceCloudLayer=new THREE.InstancedMesh(new THREE.IcosahedronGeometry(.1,1),new THREE.MeshStandardMaterial({color:0xf0f5f7,roughness:1,transparent:true,opacity:0,depthWrite:false}),cloudCount);
  for(let i=0;i<cloudCount;i++){const band=i%9;const lat=-.92+band*.23+Math.sin(i*.31)*.06;const lon=Math.random()*Math.PI*2;const n=spherePoint(lat,lon,1).normalize();position.copy(n).multiplyScalar(radius+.36+Math.random()*.16);quaternion.setFromUnitVectors(up,n);scale.set(.7+Math.random()*1.8,.35+Math.random()*.6,.65+Math.random()*1.7);matrix.compose(position,quaternion,scale);surfaceCloudLayer.setMatrixAt(i,matrix);}
  root.add(surfaceCloudLayer);

  const villageCount=180;
  surfaceCivilization=new THREE.InstancedMesh(new THREE.BoxGeometry(.045,.065,.045),new THREE.MeshStandardMaterial({color:0xb28a58,roughness:.9,transparent:true,opacity:0}),villageCount);
  for(let i=0;i<villageCount;i++){const region=Math.floor(i/30);const lat=-.55+region*.2+Math.sin(i)*.035;const lon=.3+region*.82+(i%30)*.012;const n=spherePoint(lat,lon,1).normalize();const h=Math.max(.02,terrainHeightAt(n));position.copy(n).multiplyScalar(radius+h+.035);quaternion.setFromUnitVectors(up,n);scale.setScalar(.7+Math.random()*.9);matrix.compose(position,quaternion,scale);surfaceCivilization.setMatrixAt(i,matrix);}
  root.add(surfaceCivilization);

  const riverMaterial=new THREE.MeshStandardMaterial({color:0x17647a,roughness:.2,metalness:.03,transparent:true,opacity:.9});
  for(let riverIndex=0;riverIndex<11;riverIndex++){
    const points=[];const baseLon=riverIndex*.57;for(let j=0;j<18;j++){const t=j/17;const lat=.72-t*.62+Math.sin(t*8+riverIndex)*.035;const lon=baseLon+t*.22+Math.sin(t*10+riverIndex*.8)*.025;const n=spherePoint(lat,lon,1).normalize();points.push(n.multiplyScalar(radius+.035+Math.max(0,terrainHeightAt(n))*.5));}const curve=new THREE.CatmullRomCurve3(points);root.add(new THREE.Mesh(new THREE.TubeGeometry(curve,90,.012,6,false),riverMaterial));
  }

  root.userData.radius=radius;
  root.userData.terrainHeightAt=terrainHeightAt;
  root.userData.riverMaterial=riverMaterial;
  return root;
}

function makeSolarSystem() {
  const root = new THREE.Group();
  const sun = new THREE.Mesh(new THREE.IcosahedronGeometry(.38, 3), new THREE.MeshBasicMaterial({ color: 0xffc75f }));
  sun.userData.isSun = true;
  root.add(sun);
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
    root.add(orbit);
    const body = new THREE.Group();
    const planet = new THREE.Mesh(new THREE.IcosahedronGeometry(size, 2), new THREE.MeshStandardMaterial({ color, flatShading: true, roughness: .85 }));
    body.add(planet);
    body.userData = { name, radius, orbitalSpeed: .38 / Math.sqrt(radius), phase: index * 1.51, planet };
    if (index === 2) {
      const moonPivot = new THREE.Group();
      const moon = new THREE.Mesh(new THREE.IcosahedronGeometry(.023, 1), new THREE.MeshStandardMaterial({ color: 0xc8d0d2, flatShading: true }));
      moon.position.x = .17;
      moonPivot.add(moon);
      body.add(moonPivot);
      body.userData.moonPivot = moonPivot;
      root.userData.livingPlanet = body;
    }
    if (index === 5) {
      const ring = new THREE.Mesh(new THREE.RingGeometry(size * 1.25, size * 2.05, 48), new THREE.MeshBasicMaterial({ color: 0xd6c7a0, transparent: true, opacity: .7, side: THREE.DoubleSide }));
      ring.rotation.x = Math.PI / 2.25;
      body.add(ring);
    }
    root.userData.planets.push(body);
    root.add(body);
  });
  return root;
}

function makeGalaxy() {
  const root = new THREE.Group();
  const count = 76000;
  const positions = new Float32Array(count * 3);
  const colors = new Float32Array(count * 3);
  const core = new THREE.Color(0xffe6bd);
  const armColors = [new THREE.Color(0x72bfff), new THREE.Color(0xc38cff), new THREE.Color(0x68efdc), new THREE.Color(0xffffff)];
  for (let i = 0; i < count; i++) {
    const isBulge = i < count * .18;
    const arm = i % 4;
    let radius;
    let angle;
    if (isBulge) {
      radius = Math.pow(Math.random(), 2.15) * 2.65;
      angle = Math.random() * Math.PI * 2;
    } else {
      radius = .25 + Math.pow(Math.random(), .68) * 9.5;
      const interArm = Math.random() < .16 ? (Math.random() - .5) * 1.25 : 0;
      const scatter = (Math.random() - .5) * (.34 + radius * .105) + interArm;
      angle = arm * Math.PI / 2 + radius * .69 + scatter;
    }
    positions[i * 3] = Math.cos(angle) * radius + (Math.random() - .5) * .14;
    positions[i * 3 + 1] = Math.sin(angle) * radius + (Math.random() - .5) * .14;
    positions[i * 3 + 2] = (Math.random() - .5) * (isBulge ? .95 * (1-radius/3.2) : .28 + radius * .065);
    const color = core.clone().lerp(armColors[arm], isBulge ? .12 : Math.min(1, radius / 5.2));
    const brightness = .48 + Math.random() * .52;
    colors.set([color.r * brightness, color.g * brightness, color.b * brightness], i * 3);
  }
  const geometry = new THREE.BufferGeometry();
  geometry.setAttribute("position", new THREE.BufferAttribute(positions, 3));
  geometry.setAttribute("color", new THREE.BufferAttribute(colors, 3));
  const disk = new THREE.Points(geometry, new THREE.PointsMaterial({ size: .023, vertexColors: true, transparent: true, opacity: .94, blending: THREE.AdditiveBlending, depthWrite: false, sizeAttenuation: true }));
  root.add(disk);
  root.rotation.x = .24;
  root.userData.armTarget = new THREE.Vector3(3.4, 1.1, .1);
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
        vec3 energy = mix(vec3(0.0,0.18,0.75), vec3(0.18,1.0,1.0), sin(uTime*3.0+vShade*8.0)*0.25+0.55);
        vec3 color = basalt + energy * edge * uFracture * 3.2 + energy*rim*uFracture*.42;
        gl_FragColor = vec4(color, 1.0);
      }
    `,
  });

  const shell = new THREE.IcosahedronGeometry(1.42, 2).toNonIndexed();
  const source = shell.attributes.position;
  for (let i = 0; i < source.count; i += 3) {
    const a = new THREE.Vector3().fromBufferAttribute(source, i);
    const b = new THREE.Vector3().fromBufferAttribute(source, i + 1);
    const c = new THREE.Vector3().fromBufferAttribute(source, i + 2);
    const center = a.clone().add(b).add(c).multiplyScalar(1 / 3);
    const vertices = new Float32Array([
      a.x - center.x, a.y - center.y, a.z - center.z,
      b.x - center.x, b.y - center.y, b.z - center.z,
      c.x - center.x, c.y - center.y, c.z - center.z,
    ]);
    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute("position", new THREE.BufferAttribute(vertices, 3));
    geometry.setAttribute("aBarycentric", new THREE.Float32BufferAttribute([1,0,0, 0,1,0, 0,0,1], 3));
    const shade = .18 + Math.random() * .82;
    geometry.setAttribute("aShade", new THREE.Float32BufferAttribute([shade, shade, shade], 1));
    geometry.computeVertexNormals();
    const shard = new THREE.Mesh(geometry, fragmentMaterial);
    shard.position.copy(center);
    const tangent = new THREE.Vector3(-center.y, center.x, center.z * .25).normalize();
    shard.userData.base = center.clone();
    shard.userData.velocity = center.clone().normalize().multiplyScalar(.65 + Math.random() * 1.35).addScaledVector(tangent, (Math.random() - .5) * .55);
    shard.userData.spin = new THREE.Vector3(Math.random() - .5, Math.random() - .5, Math.random() - .5).multiplyScalar(2.2);
    root.userData.fragments.push(shard);
    root.add(shard);
  }
  shell.dispose();

  magmaCore = new THREE.Mesh(
    new THREE.IcosahedronGeometry(1.16, 4),
    new THREE.ShaderMaterial({
      uniforms: { uTime: { value: 0 }, uCollapse: { value: 0 } },
      vertexShader: `varying vec3 vP; varying vec3 vN; void main(){vP=position;vN=normalize(normalMatrix*normal);gl_Position=projectionMatrix*modelViewMatrix*vec4(position,1.0);}`,
      fragmentShader: `uniform float uTime;uniform float uCollapse;varying vec3 vP;varying vec3 vN;void main(){float flow=sin(vP.x*9.0+uTime*2.0)*sin(vP.y*11.0-uTime)*sin(vP.z*8.0+uTime*.7);float rim=pow(1.0-max(0.0,dot(vN,vec3(0.0,0.0,1.0))),2.0);vec3 hot=mix(vec3(0.0,.08,.52),vec3(.05,1.,1.),flow*.5+.5);gl_FragColor=vec4(hot*(1.1+rim+uCollapse*.8),1.0);}`,
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
      fragmentShader: `uniform float uTime;uniform float uStrength;varying vec3 vN;varying vec3 vP;void main(){float rim=pow(1.0-max(0.0,dot(vN,vec3(0.0,0.0,1.0))),2.0);float plasma=sin(vP.x*13.0+uTime)*sin(vP.y*11.0-uTime*.7)*.5+.5;gl_FragColor=vec4(vec3(.0,.52,1.0)+vec3(.18,.48,.42)*plasma,rim*uStrength*(.18+plasma*.2));}`,
    }),
  );
  fractureAura.visible = false;
  root.add(fractureAura);

  const debrisCount = 1400;
  debrisField = new THREE.InstancedMesh(
    new THREE.IcosahedronGeometry(.07, 1),
    new THREE.MeshStandardMaterial({ color: 0x101923, roughness: .86, metalness: .18, emissive: 0x002c45, emissiveIntensity: .45, flatShading: true }),
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
      fragmentShader: `varying vec2 vUv;uniform float uStrength;void main(){vec2 p=(vUv-.5)*2.0;float beam=exp(-abs(p.y)*72.0)*(1.0-smoothstep(.05,1.0,abs(p.x)));float core=exp(-length(p*vec2(5.5,1.0))*12.0);vec3 color=mix(vec3(.05,.28,1.0),vec3(.55,1.0,1.0),core);gl_FragColor=vec4(color,(beam*.72+core)*uStrength);}`,
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
  for (let i = 0; i < 15000; i++) {
    const radius = 11 + Math.random() * 125;
    const theta = Math.random() * Math.PI * 2;
    const phi = Math.acos(2 * Math.random() - 1);
    starPositions[i * 3] = radius * Math.sin(phi) * Math.cos(theta);
    starPositions[i * 3 + 1] = radius * Math.cos(phi);
    starPositions[i * 3 + 2] = radius * Math.sin(phi) * Math.sin(theta);
    starSizes[i] = .6 + Math.random() * 1.5;
  }
  starGeometry.setAttribute("position", new THREE.BufferAttribute(starPositions, 3));
  starGeometry.setAttribute("aSize", new THREE.BufferAttribute(starSizes, 1));
  const starMaterial = new THREE.PointsMaterial({ color: 0xa9d9ff, size: .035, transparent: true, opacity: .72, depthWrite: false });
  const starField = new THREE.Points(starGeometry, starMaterial);
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

  formation = new THREE.Group();
  for (let i = 0; i < 5; i++) {
    const ring = new THREE.Mesh(
      new THREE.TorusGeometry(1.9 + i * .34, .008 + i * .002, 8, 128),
      new THREE.MeshBasicMaterial({ color: i % 2 ? 0xff5fa2 : 0x7dfbe0, transparent: true, opacity: .4, blending: THREE.AdditiveBlending }),
    );
    ring.rotation.x = Math.PI / 2 + (i - 2) * .18;
    ring.rotation.y = (i - 2) * .22;
    formation.add(ring);
  }
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
  [0x69f7db, 0x8f72ff, 0xff6fb3].forEach((color, index) => {
    const ribbon = new THREE.Mesh(
      new THREE.TorusKnotGeometry(1.2 + index * .35, .025 + index * .008, 220, 7, 2 + index, 3),
      new THREE.MeshBasicMaterial({ color, transparent: true, opacity: .24, wireframe: true, blending: THREE.AdditiveBlending }),
    );
    ribbon.rotation.set(index * .65, index * .8, index * .35);
    auroraGroup.add(ribbon);
  });
  scene.add(auroraGroup);

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
  fractureLight = new THREE.PointLight(0x39dfff, 0, 22, 1.4);
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
  solarSystem.visible = false;
  galaxyGroup.visible = false;
  nebulaBackdrop.visible = false;
  planetMoon.visible = false;
  surfaceWorld.visible = false;
  auroraGroup.visible = false;
  singularityFlash.visible = false;
  camera.position.x = 0;
  camera.position.y = .25;
  camera.fov = 42;
  camera.up.set(0, 1, 0);

  if (isGenesis) {
    const bangPulse = Math.exp(-Math.pow((p - .235) * 36, 2));
    singularityFlash.visible = p > .12 && p < .34;
    singularityFlash.scale.setScalar(.03 + bangPulse * 5 + smooth(p, .23, .33) * 2);
    singularityFlash.material.opacity = Math.min(1, bangPulse * 1.45) * (1 - smooth(p, .27, .34));

    auroraGroup.visible = p > .22 && p < .68;
    auroraGroup.scale.setScalar(.04 + smooth(p, .23, .55) * 2.7);
    auroraGroup.rotation.y = time * .09;
    auroraGroup.rotation.z = time * .035;
    auroraGroup.children.forEach((ribbon, index) => {
      ribbon.material.opacity = (.08 + Math.sin(time * .7 + index) * .035) * (1 - smooth(p, .55, .68));
    });

    nebulaBackdrop.visible = p >= .28 && p < .88;
    nebulaBackdrop.rotation.y = time * .0015;
    nebulaBackdrop.rotation.x = Math.sin(time * .001) * .01;
    nebulaBackdrop.children.forEach((cloud) => {
      cloud.material.opacity = cloud.userData.baseOpacity * smooth(p, .28, .42) * (1 - smooth(p, .82, .88));
    });

    galaxyGroup.visible = p >= .39 && p < .72;
    galaxyGroup.scale.setScalar(.32 + smooth(p, .39, .67) * 1.65);
    galaxyGroup.rotation.z = -.12 + time * .042;
    galaxyGroup.children.forEach((layer) => {
      layer.material.opacity = .94 * smooth(p, .39, .47) * (1 - smooth(p, .64, .72));
    });

    solarSystem.visible = p >= .64 && p < .855;
    solarSystem.scale.setScalar(.08 + smooth(p, .61, .77) * 1.08);
    const centerTransition = 1 - smooth(p, .69, .75);
    solarSystem.position.set(3.4 * centerTransition, 1.1 * centerTransition, .1 * centerTransition);
    solarSystem.rotation.x = .48;
    solarSystem.rotation.z = -.16 + time * .015;
    solarSystem.userData.planets.forEach((body, index) => {
      const orbitalClock = smooth(p, .64, .855) * 24;
      const angle = body.userData.phase + orbitalClock * body.userData.orbitalSpeed;
      body.position.set(Math.cos(angle) * body.userData.radius, 0, Math.sin(angle) * body.userData.radius);
      body.userData.planet.rotation.y = p * (42 + index * 3);
      body.userData.moonPivot && (body.userData.moonPivot.rotation.y = p * 76);
      const focusBoost = index === 2 ? 1 + smooth(p, .775, .835) * 4.5 : 1;
      const handoffFade = index === 2 ? 1 - smooth(p, .83, .852) : 1;
      body.scale.setScalar(Math.max(.02, focusBoost * handoffFade));
    });
    solarSystem.updateMatrixWorld(true);
    solarSystem.userData.livingPlanet.getWorldPosition(focusPosition);

    const planetFocus = p >= .83 && p < .895;
    const planetHandoff = smooth(p, .83, .87);
    world.visible = planetFocus;
    worldMaterial.uniforms.uDeep.value.set("#123d65");
    worldMaterial.uniforms.uBright.value.set("#72d99a");
    world.position.copy(focusPosition).multiplyScalar(1 - planetHandoff);
    world.scale.setScalar(.08 + planetHandoff * 1.2);
    world.rotation.y = p * 38;
    atmosphere.material.opacity = .08 + smooth(p, .79, .87) * .2;
    planetMoon.visible = planetFocus;
    const moonAngle = p * 34;
    planetMoon.position.copy(world.position).add(new THREE.Vector3(Math.cos(moonAngle) * 1.85, Math.sin(moonAngle * .63) * .3, Math.sin(moonAngle) * .75));
    planetMoon.scale.setScalar(.08 + planetHandoff * .92);

    surfaceWorld.visible = p >= .885;
    if (surfaceWorld.visible) {
      const surfaceEntry = smooth(p, .885, .905);
      const green = smooth(p, .93, .975);
      const city = smooth(p, .978, 1);
      const cloudCycle = .42 + Math.sin(time*.11)*.18 + Math.sin(time*.037+2.1)*.12;
      const dayCycle = time*.018 + p*5;
      const sunHeight = .25 + Math.sin(dayCycle)*.55;
      const daylightAmount = smooth(sunHeight, -.08, .48);
      const skyColor = new THREE.Color(0x24446b).lerp(new THREE.Color(0x75b9df), daylightAmount);
      renderer.setClearColor(skyColor, 1);
      scene.fog.color.copy(skyColor);
      scene.fog.density = .008 + (1-surfaceEntry)*.075;
      bloomPass.threshold = 1.05;
      bloomPass.strength = .08;
      bloomPass.radius = .15;

      surfaceTerrain.material.uniforms.uFormation.value=surfaceEntry;
      surfaceTerrain.material.uniforms.uLife.value=green;
      surfaceWater.material.uniforms.uTime.value=time;
      surfaceWater.material.uniforms.uFormation.value=surfaceEntry;
      const sunDirection=new THREE.Vector3(Math.cos(dayCycle),sunHeight,Math.sin(dayCycle)).normalize();
      surfaceWater.material.uniforms.uSunDirection.value.copy(sunDirection);
      surfaceAtmosphere.material.uniforms.uDensity.value=surfaceEntry;
      surfaceMountains.material.transparent=true;
      surfaceMountains.material.opacity=surfaceEntry;
      surfaceSnowCaps.material.transparent=true;
      surfaceSnowCaps.material.opacity=smooth(p,.91,.95);
      surfaceForest.material.opacity=green;
      surfaceCivilization.material.opacity=city;
      surfaceCloudLayer.material.opacity=surfaceEntry*Math.max(.08,cloudCycle);
      surfaceCloudLayer.rotation.y=time*.012;
      surfaceCloudLayer.rotation.z=Math.sin(time*.006)*.08;
      surfaceWorld.userData.riverMaterial.opacity=surfaceEntry*.82;
      daylight.position.copy(sunDirection).multiplyScalar(20);
      daylight.intensity=.55+daylightAmount*2.15;

      const flightClock=time*.065+Math.max(0,p-.885)*52;
      const latitude=Math.sin(flightClock*.37)*.62+Math.sin(flightClock*.13+1.7)*.2;
      const longitude=flightClock*.73+Math.sin(flightClock*.21)*.48;
      const nextClock=flightClock+.055;
      const nextLatitude=Math.sin(nextClock*.37)*.62+Math.sin(nextClock*.13+1.7)*.2;
      const nextLongitude=nextClock*.73+Math.sin(nextClock*.21)*.48;
      const normal=new THREE.Vector3(Math.cos(latitude)*Math.cos(longitude),Math.sin(latitude),Math.cos(latitude)*Math.sin(longitude)).normalize();
      const nextNormal=new THREE.Vector3(Math.cos(nextLatitude)*Math.cos(nextLongitude),Math.sin(nextLatitude),Math.cos(nextLatitude)*Math.sin(nextLongitude)).normalize();
      const terrainHeight=surfaceWorld.userData.terrainHeightAt(normal)*surfaceEntry;
      const nextHeight=surfaceWorld.userData.terrainHeightAt(nextNormal)*surfaceEntry;
      const droneAltitude=1.12+Math.sin(flightClock*.29)*.2+Math.sin(flightClock*.071)*.13;
      camera.position.copy(normal).multiplyScalar(surfaceWorld.userData.radius+terrainHeight+droneAltitude+(1-surfaceEntry)*1.5);
      const target=nextNormal.multiplyScalar(surfaceWorld.userData.radius+nextHeight+.035);
      camera.up.copy(normal);
      camera.lookAt(target);
      camera.fov=52-Math.sin(flightClock*.17)*3;
    } else {
      if (p < .48) {
        camera.position.set(0, .25, 9.6 - smooth(p, .23, .48) * 1.3);
        camera.lookAt(0, 0, 0);
      } else if (p < .69) {
        const armZoom = smooth(p, .48, .69);
        camera.position.set(armZoom * 2.75, armZoom * .82, 8.3 - armZoom * 4.15);
        camera.lookAt(3.4 * armZoom, 1.1 * armZoom, 0);
      } else if (p < .83) {
        const systemZoom = smooth(p, .69, .83);
        const widePosition = new THREE.Vector3(0, .55, 6.3);
        const nearPlanet = focusPosition.clone().add(new THREE.Vector3(0, .16, .78));
        camera.position.copy(widePosition.lerp(nearPlanet, systemZoom));
        camera.lookAt(focusPosition);
      } else {
        const handoffCamera = smooth(p, .83, .885);
        camera.position.set(
          focusPosition.x * (1 - handoffCamera),
          .28 + focusPosition.y * (1 - handoffCamera),
          .82 + handoffCamera * 2.7,
        );
        camera.lookAt(world.position);
      }
    }
    particles.material.opacity = p < .03 ? p * 4 : (p > .72 ? Math.max(.05, 1 - smooth(p, .72, .88)) : .88);
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
    formation.scale.setScalar(1 + Math.sin(time * 1.7) * .04 - p * .28);
    if (isGeological) {
      const fracture = smooth(p, .18, .55);
      const explode = smooth(p, .48, .94);
      const debrisBirth = smooth(p, .38, .58);
      world.visible = p < .48;
      fragmentGroup.visible = p >= .48;
      fragmentGroup.rotation.y = time * .035;
      fragmentMaterial.uniforms.uTime.value = time;
      fragmentMaterial.uniforms.uFracture.value = .18 + fracture * .92;
      magmaCore.visible = p >= .43 && p < .96;
      magmaCore.material.uniforms.uTime.value = time;
      magmaCore.material.uniforms.uCollapse.value = explode;
      magmaCore.scale.setScalar(Math.max(.12, 1.04 - explode * .82));
      fractureAura.visible = p >= .22 && p < .93;
      fractureAura.material.uniforms.uTime.value = time;
      fractureAura.material.uniforms.uStrength.value = fracture * (1 - smooth(p, .78, .94));
      fractureAura.scale.setScalar(1 + Math.sin(time * 1.8) * .035 + explode * .38);
      fragmentGroup.userData.fragments.forEach((shard) => {
        shard.position.copy(shard.userData.base).addScaledVector(shard.userData.velocity, Math.pow(explode, 1.35) * 3.8);
        shard.rotation.set(
          shard.userData.spin.x * explode * 2.4,
          shard.userData.spin.y * explode * 2.4,
          shard.userData.spin.z * explode * 2.4,
        );
        shard.scale.setScalar(1 - explode * .14);
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
      const beforeBang = p < .23;
      const radial = beforeBang
        ? 1.85 * (1 - smooth(p, .01, .23)) + .008
        : .012 + smooth(p, .23, .46) * 1.12;
      const galaxy = smooth(p, .36, .62);
      const angle = baseAngle + (beforeBang ? p * 13 + time * (.04 + p * .38) : galaxy * radius * .38 + time * .075);
      positions[i] = Math.cos(angle) * radius * radial + wave;
      positions[i + 1] = Math.sin(angle) * radius * radial + wave;
      positions[i + 2] = bz * radial * (1 - galaxy * .86);
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
  frame = requestAnimationFrame(animate);
});

onUnmounted(() => {
  cancelAnimationFrame(frame);
  observer?.disconnect();
  disposeObject(scene);
  composer?.dispose?.();
  renderer?.dispose();
  renderer?.domElement?.remove();
});
</script>

<template>
  <section class="cosmic-observatory" :style="{ '--cosmic-accent': scenario.accent }">
    <div ref="host" class="cosmic-canvas" aria-label="ฉากจำลองจักรวาลแบบสามมิติ" @wheel.prevent="handleWheel"></div>
    <div class="cosmic-vignette"></div>
    <div class="cosmic-bigbang-flash" :style="{ opacity: bigBangFlashOpacity }"></div>

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
