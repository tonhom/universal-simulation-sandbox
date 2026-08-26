# Architecture

The prototype deploys as one process but is divided into independent projects with one-way dependencies:

`Domain ← Engine ← Application ← Worker/API` and `Application ← Infrastructure ← API`.

- **Domain** owns universal state, events and causal identifiers.
- **Engine** owns deterministic evolution rules and has no web/database dependency.
- **Application** coordinates use cases and defines persistence ports.
- **Infrastructure** implements PostgreSQL persistence and embedded migrations.
- **Worker** advances continuous runtime cycles.
- **API** is the composition root and HTTP surface.
- **Web** is an independent Vue/Vite application under `spa/`.

## Causal loop

`state → conditions → decision/chance → action → event → persisted history → new state`

Every event records its simulation tick, actor/location and optional `caused_by_event_id`. This is the first edge of the universal event/cause graph; future versions can add observations and beliefs so agents decide from perceived rather than omniscient state.

New event summaries, details, cultivation milestone causes, encounters and refinement narratives are persisted in Thai. Stable technical event type IDs remain language-neutral for API filtering and analytics. Existing historical rows are preserved verbatim; the Vue observatory maps their type and importance labels to Thai without rewriting causal history.

The engine advances in deterministic ticks. Randomness is derived from universe seed and tick, making experiments reproducible. Wall-clock speed only determines how many ticks are evaluated per runner cycle. A lock protects each in-memory universe while PostgreSQL stores the latest JSON state and append-only event ledger transactionally.

## Evolution pipeline

1. Universe and star-system genesis
2. Planet stabilization and life genesis
3. Tribal civilization
4. Discovery of qi and cultivation sects
5. Named cultivators and realm breakthroughs
6. Immortal-scale refinement of mountains, stars/star systems, or dead/living worlds into cosmic materials
7. Independent technological civilizations, cross-world first contact, knowledge exchange and hybrid innovation

The diversity layer adds natural/canonical naming, orbital identity, regions, demographic and samsara accounting, knowledge discovery, materials, crafting, relationships, personal combat, sect wars, law-driven terrain transformation, and world-tier ascension. World tiers enforce cultivation ceilings; a breakthrough cannot exceed what local reality can sustain until a cosmic world-ascension event raises that ceiling.

The refinement path deliberately uses the same running universe rather than a hard-coded cinematic. It appears only when an Immortal exists. Every operation records the actor, target, intent, ordered preparation, material result, aftermath, and next action; history is emitted as a cause-linked preparation to completion to aftermath chain. Living-world consumption additionally preserves its karmic and samsara implications.

## Production direction

For higher scale, partition universes into workers, use optimistic versioning or an event stream, move snapshots to a separate table, and replace tick-wide scans with a scheduled-action queue. Agent knowledge, needs, goals and utility scoring belong in the domain layer; the event schema already supports causal links.

The proposed successor to the current condition-and-probability rules is documented in `AI_AGENT_DECISION_ARCHITECTURE.md`. It uses rule-generated candidate actions, a shared low-cost policy conditioned by each agent's personalized profile, and authoritative simulation resolution. Heaven-Favored people receive the highest decision LOD; an LLM is not required in the runtime loop.

`WORLD_WILL_ARCHITECTURE.md` defines World Will as a three-layer controller: immutable World Law, homeostatic pressure management, and budget-constrained strategic intervention. It also defines Heaven Favor as an expansion of opportunity rather than guaranteed success.

`CROSS_WORLD_CIVILIZATION_ARCHITECTURE.md` documents the implemented cultivation/technology capability model and the detection → contact → exchange → hybrid integration lifecycle.
