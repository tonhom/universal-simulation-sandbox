# Cosmic Visualizer

หน้า **หอสังเกตการณ์ 3D** เป็นฉากจำลองเชิงภาพสำหรับ Universal Simulation Sandbox สร้างด้วย Vue, Three.js และ GPU shader โดยแยกโหลดจาก dashboard เฉพาะเมื่อผู้ใช้เปิดหน้า จึงไม่เพิ่มภาระให้หน้าหลักโดยไม่จำเป็น

## ฉากที่มีใน Prototype

1. การกำเนิดจักรวาลแบบ cinematic — อนุภาคแรกหมุนเข้าสู่ศูนย์กลาง, Origin accretion และ Big Bang แบบ white-out bloom flash เต็ม viewport ก่อน Space-Time expansion, aurora และ Galactic Flow
2. Galactic zoom — กล้องติดตามแขนกาแล็กซีไปยังกลุ่มก๊าซที่ก่อดาว ก่อนเปลี่ยนสเกลเข้าสู่ระบบดาวซึ่งจำลองโครงสร้างใกล้เคียงระบบสุริยะ: ดาวฤกษ์หนึ่งดวง ดาวเคราะห์แปดดวง ดวงจันทร์ของดาวเคราะห์มีชีวิต และวงแหวนของดาวเคราะห์ชั้นนอก
3. การก่อรูปดาวเคราะห์มีชีวิต — กล้องละจากดาวฤกษ์แล้วติดตามดาวเคราะห์ดวงที่สามโดยตรง เห็น atmosphere, ocean และ continent texture ก่อนเข้าสู่โหมดสำรวจพื้นผิวทรงกลม
4. แก่นโลกและเปลือกโลกแตกสลาย — ดาวหลักความละเอียดสูง, รอยแตก procedural เรืองแสง, แก่นพลังงาน, เปลือกดาว 320 ชิ้น และ instanced orbital debris 1,400 ชิ้นซึ่งมีขนาด วงโคจร ทิศทาง และการหมุนต่างกัน
5. World ยุบตัวเป็นหลุมดำ — gravitational collapse, conceptual singularity, curved Space-Time grid, turbulent accretion disk, Doppler beaming, gravitational lens arcs, photon rings, event horizon และ polar jets
6. World ถูกดาวฤกษ์กลืนกิน
7. โครงข่ายกฎจักรวาลล่มสลาย
8. Qi depletion และ Realm Regression
9. World ถูกผู้ทรงอำนาจหลอมเป็นวัตถุดิบจักรวาล
10. Conceptual Erasure ซึ่งลบ Identity, Information และ causal trace

### Spherical World Survey

ฉากพื้นผิวไม่ได้ใช้แผนที่แบนหรือกล้องแพนขึ้นลงอีกต่อไป แต่ใช้ World ทรงกลมที่ประกอบด้วย:

- high-resolution procedural terrain แยกชายหาด ที่ราบ ป่า หิน ภูเขาและขั้วโลกหิมะจากความสูงและละติจูด
- water sphere แยกจาก terrain ใช้คลื่นขนาดเล็ก Fresnel, specular reflection และสี deep/shallow water โดยปิด bloom ในโหมดพื้นผิว
- แม่น้ำโค้งตามผิวดาว ใช้วัสดุน้ำจริงร่วมกับ roughness/specular แทน emissive หรือ glow
- atmosphere shell สีฟ้าและ horizon scattering
- ทิวเขา 260 จุดพร้อม snow caps, ป่าสนประมาณ 2,200 ต้น และเมฆประมาณ 720 ก้อนแบบ GPU instancing
- ชุมชนหลายภูมิภาครวม 180 สิ่งปลูกสร้าง ซึ่งค่อย ๆ ปรากฏตาม civilization phase

กล้องทำงานเหมือนโดรนที่บินเหนือผิวดาวไม่รู้จบ เส้นทางเกิดจากการเปลี่ยน latitude, longitude, altitude และ heading หลายความถี่พร้อมกัน จึงค่อย ๆ ผ่านมหาสมุทร เกาะ ชายฝั่ง ทิวเขา ป่าสน เขตหิมะ แม่น้ำและชุมชนโดยไม่วนตามเส้นศูนย์สูตรตรง ๆ เมฆหมุนด้วยความเร็วต่างจากพื้นโลกและ opacity เปลี่ยนตามเวลาเพื่อให้กลุ่มเมฆมาและหายไป

Terrain formation, atmosphere density, ocean visibility, forest growth, snow coverage, cloud density และ settlement emergence ผูกกับ Genesis timeline โดยตรง หลัง timeline สิ้นสุด กล้องและสภาพแวดล้อมแบบ ambient ยังคงเคลื่อนไหวเพื่อให้สำรวจโลกต่อได้

### Planet Fracture Cinematography

ฉากดาวแตกใช้กล้องโคจรรอบ World พร้อมเปลี่ยน focal length และระยะกล้องตามความรุนแรง: เริ่มจาก establishing orbit, ซูมเข้าเห็นรอยแตกและพื้นผิว, เคลื่อนผ่าน debris หลายระยะ แล้วถอยออกเพื่อเห็น debris field ทั้งหมด แสงประกอบด้วย cool cyan rim light, procedural plasma aura, HDR bloom และ anamorphic flare แนวนอน

Camera shake ทำงานเฉพาะช่วงแรงดันและการแตกตัว แล้วลดลงจนเป็นศูนย์ก่อน terminal phase ช่วงสุดท้ายกล้อง เศษเปลือกดาว และ orbital debris จะหมุนวนอย่างต่อเนื่องโดยไม่มีอาการสั่น แม้ timeline หยุดที่ 100%

ภาพอ้างอิงภายนอกใช้สำหรับอ่านองค์ประกอบ สี แสง ระยะชัดลึก และจังหวะกล้องเท่านั้น ระบบไม่ฝังรูปอ้างอิง ไม่อ่านข้อความ/ลายน้ำ และไม่ใช้รูปนั้นเป็น texture วัสดุทั้งหมดสร้างจาก geometry, shader และแสงภายใน Three.js

ทุกฉากรองรับ play/pause, restart, timeline scrubbing และความเร็วตั้งแต่ 0.25× ถึง 16× พร้อม phase marker และค่าตรวจวัดตามเวลา

ผู้ใช้สามารถหมุนล้อเมาส์เหนือ canvas เพื่อ zoom in/out ด้วยการปรับ field of view ช่วง zoom ถูกจำกัดเพื่อไม่ให้กล้องทะลุ geometry และมีปุ่มแสดงระดับ zoom สำหรับรีเซ็ตกลับ 1× เมื่อเปลี่ยนฉากระบบจะรีเซ็ต zoom ให้อัตโนมัติ

Galactic Flow ใช้เวลานานขึ้นใน timeline และประกอบด้วยอนุภาคของจานกาแล็กซีประมาณ 76,000 จุดกับดาวพื้นหลังประมาณ 15,000 จุด โครงสร้างมี central bulge หนา, inter-arm stars และแขนเกลียวหลักสี่แขนที่กระจายตัวกว้างขึ้นเพื่อให้ใกล้ลักษณะของ Milky Way มากกว่าจานเส้นบาง ส่วนเนบิวลาประมาณ 22,000 จุดถูกแยกเป็น deep-space backdrop คนละชั้นกับกาแล็กซี อยู่ไกลออกไปและเคลื่อนช้ามาก จึงไม่หมุน ขยาย หรือซูมตามจานกาแล็กซี

ระหว่าง Galactic Flow จานกาแล็กซีหมุนวนต่อเนื่อง โดยเริ่มจากมุมเฉียงที่มองเห็นความหนาของจาน จากนั้นกล้องและแนวจานจะค่อย ๆ เปลี่ยนเป็นมุมมองจากด้านบนเพื่อเผยโครงสร้างแขนเกลียวเต็มรูป ก่อนซูมเข้าไปยังแขนเป้าหมายและส่งต่ออย่างต่อเนื่องเข้าสู่ฉากระบบดาว

ช่วงการบินสำรวจโลกใช้กล้องแบบ aircraft-style แทนการก้มตรงลงพื้น กล้องเคลื่อนตามเส้นทางโค้งที่เปลี่ยนละติจูดอย่างต่อเนื่อง แต่เล็งไปไกลตามแนวสัมผัสของทรงกลมพร้อมก้มลงเล็กน้อย จึงเห็นทั้งภูมิประเทศด้านล่าง เส้นขอบฟ้า ชั้นบรรยากาศ และท้องฟ้าในเฟรมเดียวกัน รวมถึงมีการเอียงปีกอย่างเบามากตามทางเลี้ยว

ฉากหลุมดำเป็นการสร้างภาพเชิงศิลป์ที่อิงปรากฏการณ์สัมพัทธภาพทั่วไป ไม่ใช่ numerical general-relativity solver ตัว singularity จริงไม่สามารถมองเห็นจากภายนอก event horizon ได้ ดังนั้นจุดสว่างของ singularity จะแสดงเฉพาะช่วง conceptual collapse ก่อนขอบฟ้าเหตุการณ์ปิดสมบูรณ์ หลังจากนั้นผู้สังเกตการณ์จะเห็นเพียงเงาหลุมดำ photon ring และสสารใน accretion disk

## Rendering Architecture

```text
Vue UI / Timeline
        ↓
Scenario State (type, progress, speed, phase)
        ↓
Visual Mapping Layer
        ↓
Three.js Scene Graph
├─ World shader mesh
├─ Atmosphere
├─ Cosmic particle field
├─ Refinement formation rings
├─ Stellar hazard
└─ Camera / GPU renderer
```

Prototype ใช้ WebGL2 เป็น renderer หลักเพื่อความเข้ากันได้กับ browser ปัจจุบัน และตรวจสอบ `navigator.gpu` เพื่อแสดงว่าเครื่องรองรับ WebGPU หรือไม่ State ของฉากและ timeline ไม่ผูกกับ renderer จึงสามารถเพิ่ม WebGPU renderer ภายหลังได้

## ความสัมพันธ์กับ Simulation Core

ปัจจุบันฉากเป็น deterministic visual scenario: timeline เป็นตัวขับ shader และ particle state ขณะที่ชื่อจักรวาลและ `yearsPerTick` อ่านจาก universe ที่เลือกอยู่

ขั้นต่อไปสามารถเพิ่ม Visual Event Adapter เพื่อแปลง event จาก Simulation Core เป็น visual cue:

```text
Simulation Event
├─ Type / Importance
├─ Causes / Conditions
├─ Location / Scope
├─ Law Expressions
└─ Consequences
        ↓
Visual Event Adapter
        ↓
Camera cue + shader state + particles + timeline marker
```

ตัวอย่าง mapping:

| Simulation event | Visual response |
|---|---|
| `universe.genesis` | เริ่ม Genesis timeline |
| `world.ascension` | เพิ่ม atmosphere, Law density และ energy pulse |
| `world.collapse.started` | เลือก collapse mechanism จาก event payload |
| `law.network.rupture` | เปิดรอยแยกและ distortion ของ World mesh |
| `world.refinement.ritual` | แสดง formation rings และ extraction particles |
| `world.erased` | ลด Identity coherence และ dissolve mesh |

## ไฟล์สำคัญ

- `spa/src/components/CosmicVisualizer.vue` — scene, shader, animation และ timeline state
- `spa/src/cosmic-visualizer.css` — full-screen observatory UI และ responsive layout
- `spa/src/App.vue` — ปุ่มเปิดหน้าและการส่ง universe ที่เลือก
