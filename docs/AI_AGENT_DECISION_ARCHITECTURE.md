# AI Agent Decision Architecture — Draft 1

เอกสารนี้ต่อยอด `conceptual_draft_2`, `world_simulation_theory_draft_6` และสถานะของ Universal Simulation Sandbox PoC โดยกำหนดแนวทางให้ Agent ตัดสินใจและทำให้เหตุการณ์เกิดจาก state, perception, goal และ consequence โดยไม่ต้องเรียก LLM ใน simulation loop

## 1. ข้อสรุปการออกแบบ

แนวคิดนี้เหมาะกับ Universal Simulation Sandbox แต่ไม่ควรให้ Neural Network สร้างเหตุการณ์หรือแก้ world state โดยตรง สถาปัตยกรรมที่แนะนำคือ hybrid decision system:

```text
Canonical World State
        ↓ Observation / Information propagation
Perceived State + Beliefs + Memory
        ↓
Needs + Goals + Personality + Relationships
        ↓
Rule-generated Candidate Actions
        ↓
Shared AI Policy / Utility Scoring
        ↓
Hard Constraint & Resource Validation
        ↓
Chosen Intent / Action
        ↓
Deterministic–Probabilistic Simulation Resolution
        ↓
Consequences + Event/Cause Graph + New State
        ↓
Memory / Belief / Policy feedback
```

AI มีหน้าที่เลือก intent หรือ action จากทางเลือกที่กฎของจักรวาลอนุญาต ส่วน Simulation Engine เป็นผู้ตรวจทรัพยากร กฎ ระยะทาง พลัง ความรู้ ความเสี่ยง และตัดสินผลจริง วิธีนี้ทำให้ AI ไม่สามารถสร้างพลัง วัตถุ หรือเหตุการณ์ที่ละเมิดกฎของโลกขึ้นมาเอง

## 2. ไม่จำเป็นต้องใช้ LLM ใน runtime

การตัดสินใจที่เกิดหลายล้านครั้งเหมาะกับโมเดลขนาดเล็กและระบบเชิงตัวเลขมากกว่า LLM:

- Utility AI เป็น baseline ที่อธิบายผลได้และพัฒนาเร็ว
- Contextual bandit ใช้เรียนรู้ว่าทางเลือกใดให้ผลดีในบริบทต่างกัน
- Gradient-boosted trees หรือ small MLP ใช้ score candidate actions ด้วยต้นทุนต่ำ
- Reinforcement Learning ใช้ภายหลัง เมื่อมี environment, reward และ telemetry ที่เสถียรแล้ว
- LLM เป็น optional narrative layer สำหรับสรุปประวัติ ตั้งคำบรรยาย หรือสร้างบทสนทนาเฉพาะช่วง ไม่ควรอยู่ใน core decision loop

จุดเริ่มต้นที่เหมาะที่สุดไม่ใช่ Neural Network แต่เป็น Utility AI ที่มี feature logging ครบ เพราะข้อมูลจาก Utility AI จะกลายเป็น training dataset และ benchmark ให้โมเดลเรียนรู้ในอนาคต

## 3. Personalized AI ของ Heaven-Favored Person

ความเป็นส่วนบุคคลไม่ควรหมายถึงหนึ่ง Neural Network ต่อหนึ่งตัวละคร โดยปกติให้ทุกคนใช้ shared policy model เดียวกัน แต่ส่ง `AgentPolicyProfile` เฉพาะบุคคลเข้าไปเป็นส่วนหนึ่งของ input:

```text
Shared Policy Model
  + personality
  + values
  + cultivation path
  + risk tolerance
  + ambition
  + attachments
  + relationships
  + trauma / vows
  + beliefs and misinformation
  + memories
  + current needs and goals
  + luck / heaven favor
  + world context
  = personalized decision
```

ดังนั้นคนสองคนที่เห็นสถานการณ์เดียวกันสามารถเลือกต่างกันได้ แม้ใช้โมเดลร่วมกัน ตัวอย่างเช่น Sword Cultivator ที่ทะเยอทะยานและยึดถือศักดิ์ศรีอาจเลือกประลอง ขณะที่ Alchemist ที่ระมัดระวังและมีคนรักอยู่ในเมืองอาจเลือกเจรจาหรือถอนตัว

เฉพาะตัวละครระดับสูงมากและมีข้อมูลสะสมเพียงพอ อาจมี lightweight adapter, policy head หรือ learned latent vector ของตนเอง แต่ยังไม่ควรฝึกโมเดลเต็มก้อนแยกต่อคน

## 4. Agent State ที่ต้องเก็บ

Heaven-Favored Person ควรมี state อย่างน้อยดังนี้:

| กลุ่มข้อมูล | ตัวอย่าง |
|---|---|
| Identity | อายุ สายเลือด สำนัก cultivation path ฉายา |
| Personality | ambition, empathy, greed, loyalty, patience, curiosity |
| Values | ครอบครัว สำนัก Dao อำนาจ ความรู้ การอยู่รอด |
| Needs | ความปลอดภัย ทรัพยากร ยา breakthrough ความสัมพันธ์ |
| Goals | เป้าหมายระยะสั้น กลาง ยาว และ priority |
| Beliefs | สิ่งที่เชื่อเกี่ยวกับบุคคล สำนัก สมบัติ และโลก |
| Knowledge | ข้อมูลที่รู้ แหล่งข่าว confidence และเวลาได้รับข้อมูล |
| Memory | เหตุการณ์สำคัญ ผู้เกี่ยวข้อง emotional weight และ causal link |
| Relationships | trust, affection, fear, debt, rivalry, obligation |
| Capability | realm, skills, laws, artifacts, health, resources |
| Policy Profile | risk tolerance, time preference, exploration, moral boundaries |
| Fate Context | luck, heaven favor, karma, prophecy และ tribulation pressure |

Belief ต้องแยกจาก canonical truth เสมอ Agent ตัดสินใจจากสิ่งที่รับรู้ ไม่ใช่ข้อมูลจริงทั้งหมด

## 5. Candidate Action และ Decision Score

กฎของ subsystem เป็นผู้สร้าง candidate actions เช่น `Negotiate`, `Trade`, `Investigate`, `Cultivate`, `UsePill`, `ExploreRuin`, `ProtectPerson`, `ChallengeRival`, `StartWar`, `Flee`, `RefineMountain` หรือ `AttemptWorldAscension`

โมเดลให้คะแนนแต่ละทางเลือกโดยประมาณ:

```text
Score(action) =
    GoalAlignment
  + ExpectedReward
  + PersonalityFit
  + RelationshipImpact
  + KnowledgeValue
  + FateOpportunity
  - ExpectedRisk
  - ResourceCost
  - MoralViolation
  - UncertaintyPenalty
  - WorldLawResistance
```

ก่อน execute ต้องผ่าน hard constraints เช่น มีความรู้หรือไม่ ระดับพลังพอหรือไม่ มีวัตถุดิบจริงหรือไม่ เดินทางถึงได้หรือไม่ World cultivation ceiling อนุญาตหรือไม่ และ action ละเมิด invariant ของ simulation หรือไม่

## 6. เหตุการณ์ต้อง emerge จาก Action Resolution

AI ไม่ควร output ว่า “เกิดสงครามใหญ่” โดยตรง แต่ควรเลือกการกระทำระดับเจตนา:

```text
Need strategic metal
→ believes neighboring sect has ore
→ evaluates trade / theft / diplomacy / invasion
→ chooses coercive negotiation
→ negotiation fails
→ mobilizes disciples
→ border encounter escalates
→ sect war emerges
```

Event Graph บันทึกแต่ละขั้นด้วย `actor`, `target`, `location`, `observation`, `decision`, `action`, `outcome` และ `caused_by_event_id` ทำให้หน้าเว็บตอบได้ทั้ง “เกิดอะไรขึ้น” และ “ทำไมจึงเกิด”

## 7. Heaven-Favored Person และผลกระทบต่อ World

Heaven-Favored Person ต้องใช้ Agent LOD สูงกว่าประชากรทั่วไป เพราะการตัดสินใจของบุคคลเหล่านี้สร้าง ripple effect ต่อโลก:

```text
Personal decision
→ relationship / sect / region consequence
→ economic, ecological or political pressure
→ civilization response
→ World Will observation
→ favor, resistance, tribulation or world ascension pressure
→ new goals and decisions
```

ควรมี `WorldImpactVector` สำหรับ action สำคัญ เช่น population, stability, qi ecology, knowledge, karma, law cohesion, sect balance และ ascension pressure เพื่อให้ World runtime ประเมินผลสะสมได้ ไม่ต้องรอ event แบบ hard-coded เพียงครั้งเดียว

Heaven favor ไม่ควรบังคับให้ตัวละครชนะ แต่ปรับ possibility space เช่น พบโอกาสมากขึ้น รอดจากผลลัพธ์ร้ายแรงบางส่วน ได้ข้อมูลก่อนผู้อื่น หรือพบผู้ช่วย ขณะเดียวกัน karma และ World Will สามารถเพิ่มภัยพิบัติ คู่แข่ง หรือ tribulation เพื่อรักษาสมดุล

## 8. Simulation Level of Detail

ไม่ควรรัน personalized AI เต็มรูปแบบกับสิ่งมีชีวิตทุกคน:

| LOD | สิ่งที่จำลอง | วิธีตัดสินใจ |
|---|---|---|
| LOD 0 | ประชากรจำนวนมาก | statistical transition / aggregate rates |
| LOD 1 | กลุ่ม อาชีพ หน่วยทหาร | cohort utility model |
| LOD 2 | named agents และผู้นำ | shared policy + personalized profile + memory |
| LOD 3 | Heaven-Favored / world-changing entities | full candidate evaluation, planning, counterfactual rollout |

Agent สามารถเลื่อนหรือลด LOD ได้โดยรักษา identity, relationships, possessions, key memories และ causal history ไว้

## 9. Learning และ Training Pipeline

ควรแยก training ออกจาก production simulation:

1. รัน Utility AI และบันทึก feature, candidates, scores, choice และ outcome
2. สร้าง replay dataset จากหลาย world seeds
3. ฝึก model offline เพื่อ predict utility หรือ long-term value
4. เปรียบเทียบกับ baseline ผ่าน deterministic replay
5. ตรวจ invariant, diversity, stability และ exploit behavior
6. version model และ policy schema
7. deploy แบบ shadow mode ก่อนให้โมเดลควบคุมจริง

ทุก decision ต้องเก็บ `model_version`, `policy_profile_version`, seed และ candidate set เพื่อ replay และ debug ได้ ผลลัพธ์ที่ stochastic ใช้ RNG stream ของ universe/agent/action เพื่อให้การทดลองทำซ้ำได้

## 10. Planning สำหรับตัวละครระดับสูง

LOD 3 ไม่จำเป็นต้องใช้ generative AI สามารถใช้ bounded planning:

- สร้าง candidate plans จาก goal templates
- จำลองผลลัพธ์ล่วงหน้า 3–10 ขั้นใน sandbox copy ของ state
- ใช้ Monte Carlo rollouts ประเมินความเสี่ยงและผลระยะยาว
- จำกัด compute budget ต่อ tick และ cache plan เดิมจนมีเหตุการณ์สำคัญเปลี่ยนบริบท
- interrupt plan เมื่อเกิดภัย ความสัมพันธ์เปลี่ยน ทรัพยากรหาย หรือ World Will แทรกแซง

ตัวอย่างแผน refinement:

```text
Goal: repair primordial sword
→ research required material
→ identify dead-world candidate
→ negotiate access / defeat guardian
→ gather formation anchors
→ purify wandering souls
→ refine world core
→ repair sword
→ react to karmic and faction consequences
```

## 11. ขอบเขตอำนาจของ AI

เพื่อรักษาความถูกต้องของ simulation:

- AI เลือกเฉพาะ action schema ที่ engine ประกาศไว้
- AI ไม่มีสิทธิ์แก้ canonical state โดยตรง
- Engine ตรวจ prerequisites, ownership, accounting, conservation และ world laws
- Outcome resolver เป็นผู้คำนวณความสำเร็จ ความล้มเหลว และ externalities
- Event writer บันทึกเหตุผล คะแนนทางเลือก และ causal edges
- ระบบ fallback กลับ Utility AI ได้เมื่อ model หาย ล้มเหลว หรือให้ค่าผิดปกติ

## 12. Roadmap จาก PoC ปัจจุบัน

| ระยะ | สิ่งที่เพิ่ม |
|---|---|
| A — Explainable baseline | AgentProfile, Need, Goal, Belief, Observation, CandidateAction และ Utility scorer |
| B — Personal history | episodic memory, relationship dimensions, vows, trauma และ decision audit log |
| C — World coupling | WorldImpactVector, faction reaction และ World Will policy |
| D — Learned scorer | ฝึก small model จาก replay data และรัน shadow comparison |
| E — Bounded planning | counterfactual rollout สำหรับ Heaven-Favored และ cosmic entities |
| F — Adaptive policy | contextual bandit/RL พร้อม model versioning และ safety gates |

PoC ปัจจุบันอยู่ก่อนระยะ A: มี state, parameter, seeded probability, prerequisite rules, action-like systems และ causal event IDs แล้ว แต่ decision ส่วนใหญ่ยังเป็น hard-coded condition ตามด้วย probability roll จึงควรสร้าง explainable Utility AI ก่อน Neural Network

## 13. Decision Record ที่แนะนำ

```text
AgentDecision
- id / universe_id / agent_id / tick
- perceived_state_hash
- active_needs[]
- active_goals[]
- candidate_actions[]
- utility_components per action
- rejected_by_constraint[]
- selected_action
- confidence / exploration_probability
- model_version / policy_profile_version
- resulting_event_id
- realized_reward / delayed_outcomes[]
```

ข้อมูลนี้ทำให้ dashboard แสดงได้ว่า “เขารู้อะไร เชื่ออะไร มีทางเลือกใด และเหตุใดจึงตัดสินใจเช่นนั้น” พร้อมใช้เป็นข้อมูลฝึกโมเดลในอนาคต

## 14. หลักการสุดท้าย

```text
Neural Network does not invent reality.
It chooses intent under uncertainty.
The simulation laws resolve reality.
The causal graph preserves why it happened.
```

สถาปัตยกรรมนี้ลดค่าใช้จ่ายจาก LLM, รองรับ agent จำนวนมาก, ทำให้ Heaven-Favored Person มีพฤติกรรมเฉพาะตัว และยังคง debug/replay เหตุและผลของจักรวาลได้

การขับเคลื่อน World Will ซึ่งเป็นทั้ง law authority, homeostatic controller และ strategic world-level agent แยกรายละเอียดไว้ใน `WORLD_WILL_ARCHITECTURE.md`
