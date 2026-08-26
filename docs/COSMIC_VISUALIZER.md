# Cosmic Visualizer

หน้า **หอสังเกตการณ์ 3D** เป็นฉากจำลองเชิงภาพสำหรับ Universal Simulation Sandbox สร้างด้วย Vue, Three.js และ GPU shader โดยแยกโหลดจาก dashboard เฉพาะเมื่อผู้ใช้เปิดหน้า จึงไม่เพิ่มภาระให้หน้าหลักโดยไม่จำเป็น

## ฉากที่มีใน Prototype

1. การกำเนิดจักรวาลแบบ cinematic — ความว่างเปล่าดำสนิท, เส้นใย Chaos Energy ไหลหมุนเข้าสู่ Origin และ Big Bang แบบ white-out bloom flash เต็ม viewport ก่อน Space-Time expansion, aurora และ Galactic Flow
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

หลัง Big Bang เส้นเรขาคณิตที่แทนกฎจักรวาลจะปรากฏเพียงช่วงสั้น ๆ โดยขยายตัว เคลื่อนแยกออกจากศูนย์กลาง และค่อย ๆ จางหายจนหมดก่อนกำเนิดกาแล็กซี จากนั้น Galactic Flow จะเริ่มเป็นจุดแสงขนาดเล็กมากในอวกาศ ค่อย ๆ ขยายเข้าหากล้องจนเห็นทรงจานและความหนาของกาแล็กซีทั้งระบบ จานจะหมุนวนต่อเนื่อง ก่อนกล้องกับแนวจานเปลี่ยนเป็นมุมมองจากด้านบนเพื่อเผยแขนเกลียวเต็มรูป แล้วจึงซูมเข้าหาแขนเป้าหมายและส่งต่อเข้าสู่ฉากระบบดาว

ช่วงการบินสำรวจโลกใช้กล้องแบบ aircraft-style ที่ลดระดับลงใกล้พื้นกว่าเดิมและก้มแนวเล็งประมาณ 28 องศา กล้องยังเคลื่อนตามเส้นทางโค้งที่เปลี่ยนละติจูดต่อเนื่อง แต่จัดองค์ประกอบให้ทวีปและภูมิประเทศกินพื้นที่ส่วนใหญ่ของภาพ โดยเหลือขอบฟ้า ชั้นบรรยากาศ และท้องฟ้าไว้ด้านบน รวมถึงมีการเอียงปีกอย่างเบามากตามทางเลี้ยว

ภูเขาบนโลกใช้ทรงยอดตัดหลายระดับแทนกรวยปลายแหลม ฐานกว้างขึ้น ความสูงลดลง และผิวรอบยอดถูกบิดเป็นสันไม่สมมาตรพร้อมสุ่มทิศทาง ทำให้แนวเขาอ่านเป็นมวลธรรมชาติมากขึ้น Snow cap ใช้รูปทรงยอดตัดที่สัมพันธ์กับยอดหิน ส่วน terrain shader ขยายช่วงที่ราบและเนินสีเขียว ลดการกลายเป็นหินจาก slope และเพิ่มสีเขียวหลายเฉดให้ต้นไม้ 2,200 ต้น

เมฆบนโลกเป็นวัตถุทึบจำนวนประมาณ 225 ชิ้น แบ่งเป็นก้อนพอง ริ้วยาว และกลุ่มแตกย่อย พร้อมจัดเป็นหย่อม 38 กลุ่มและสุ่มสัดส่วน การหมุน และระดับความสูง จึงไม่คลุมโลกหนาแน่นเหมือนเมฆโปร่งแสง 720 ก้อนแบบเดิม มหาสมุทรใช้ shader ทึบโทนคราม–cobalt พร้อม terrain mask ที่ discard พิกเซลบนแผ่นดิน มีคลื่น Fresnel และ specular จากดวงอาทิตย์แต่ไม่มีการมองทะลุถึงพื้นผิวด้านล่าง

ช่วงก่อน Big Bang เริ่มจากความมืดดำสนิทโดยยังไม่มีดาวพื้นหลังหรือกลุ่มอนุภาค เส้นใย Chaos Energy ที่มีความหนาไม่เท่ากันค่อย ๆ ปรากฏจากความว่างเปล่า กระแสแสงวิ่งไปตามเส้นโค้งและหมุนเข้าหา Origin Energy Core ขณะที่กล้องโคจรเปลี่ยนทั้งแกนราบและแกนสูงเพื่อเผยโครงสร้างสามมิติ ปลายเส้นใยหยุดอยู่นอกผิวแกน เกิดช่องว่างกักกันระหว่างเส้นใยกับก้อนพลังงาน ก้อนพลังงานด้านในค่อย ๆ เติบโตจนเกือบแตะเขตนี้ เมื่อเข้าสู่ Critical Silence ระบบใช้ collapse envelope เดียวกันลดความเร็วหมุน ความเร็วกระแสภายในเส้นใย ความสว่าง การเต้น และขนาดของแกน ทำให้เส้นใยเบาบางพร้อมกับแกนที่หดเล็กลงจนทั้งหมดหายไปพร้อมกัน หลังช่วงมืดสนิท Singularity จะถือกำเนิดซ้ำจากจุดเล็กและขยายตัวอย่างรวดเร็ว ก่อนเกิด white-out flash วัตถุ flash สามมิติจะขยายออกทางเดียวและถูกซ่อนไว้ภายในแสงวาบ จึงไม่มีลูกบอลสีขาว scale กลับลงมา ระหว่าง flash เต็มจอค่อย ๆ จาง กลุ่มสสารที่อัดแน่น ณ จุดกำเนิดจะเริ่มปรากฏและคลายตัวออกอย่างต่อเนื่อง เมื่อ flash หายหมดจึงเปิดดาว Space-Time และเส้นกฎจักรวาลให้กระจายออกเป็นฉากถัดไป

การแตกเชิงธรณีใช้ทั้งก้อนเปลือกดาวแบบ irregular polyhedron และแผ่นเปลือกโค้งคล้ายผิวส้มที่สุ่มขนาด ตำแหน่ง การหมุน และความเร็ว บางแผ่นมีชั้นน้ำแข็งสีฟ้าขาวจากมหาสมุทรที่เย็นตัวฉับพลันติดอยู่ เศษเพียงบางส่วนจะหลุดออกไป ส่วนที่เหลือยังเกาะตัวรอบแกน ทำให้ World อยู่ในสภาพเสียหายหนักแต่ไม่ได้แตกหายทั้งใบ แกนดาวยังคงอยู่ตลอดฉากโดยค่อย ๆ เย็นและหม่นลง ส่วนแสงจากรอยแยก แม็กม่า เศษร้อน และ flare ใช้โทนแดง–ส้มแทนสีฟ้า

ค่ายกลหลอมดาวเป็นโครงสร้างแบบ hierarchical formation ไม่ใช่วงแหวนทรงกลมชุดเดียว แกนหลักประกอบด้วยวงอักขระหกระนาบ แต่ละระนาบมีวงคู่ รูปหลายเหลี่ยม ดาวเรขาคณิต เส้นเชื่อมรัศมี และรูน procedural เรียงตามขอบ นอกจากนี้ยังมีค่ายกลบริวารเจ็ดกลุ่มที่โคจรคนละแกนและมีวงอักขระย่อยของตัวเอง ทุกชั้นหมุนต่างทิศ ต่างความเร็ว เต้นเป็นจังหวะ และค่อย ๆ บีบเข้าหา World ตามความคืบหน้าของการหลอม

ดาวเสาร์ในฉากระบบดาวมีวงแหวนฝุ่นเฉพาะตัว ประกอบด้วยแถบโปร่งแสงสี่ชั้น ช่องว่างระหว่างแถบคล้าย Cassini Division และอนุภาคฝุ่น 3,200 จุดที่มีสี ความสว่าง และความหนาในแนวดิ่งต่างกัน ฝุ่นหมุนช้า ๆ แยกจากการหมุนรอบตัวเองของดาว จึงไม่อ่านเป็นแผ่น RingGeometry ทึบเพียงชิ้นเดียว

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
