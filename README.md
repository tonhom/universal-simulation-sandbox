# Universal Simulation Sandbox

A runnable .NET 8 prototype for continuously simulating seeded universes from stellar genesis through life, xianxia civilization, important cultivators, and planet-scale cosmic refinement. PostgreSQL holds durable snapshots and a readable append-only historical event log; the Vue 3 web observatory creates and controls simulations in real time. New narrative records are persisted in Thai—including encounters, breakthroughs, failures, wars, World ascension, first contact and cosmic refinement—while stable technical event type IDs remain unchanged for filtering and debugging.

The current diversity layer includes cultivation and technological civilization paths, multi-dimensional capabilities, cross-world first contact, provenance-aware knowledge exchange and hybrid innovation alongside natural planet names, regions, demographic and samsara ledgers, sects, skills, crafting, relationships, wars, law-driven terrain change, civilization evolution, and cosmic world ascension. World tiers impose real cultivation ceilings that rise only when ascension conditions are satisfied.

The dashboard also includes a relationship observatory that separates same-world ties from cross-world ties, with direct links to each character. Personal histories record successful and failed breakthroughs, pill-assisted attempts, fortunate encounters, ancient artifact discoveries, relationships, conflicts, and other causal life events. Each world detail page lists its Heaven-favored people and links directly to their full histories.

Each generated star system now records its distance from the universe origin in light-years, while planets record orbital order and radius in AU. A system can support at most one or two living worlds; it may still contain additional barren, dead, frozen, volcanic, or refined planets. The Thai observatory supports world/type/importance history filters, a world-filtered virtualized important-person list, and a dedicated world history panel.

## Start

Requirements: Docker Desktop (or Docker Engine with Compose).

```bash
docker compose up --build
```

Open <http://localhost:8080>. Create a universe, pause/resume it, set speed from `0.01×` to `1000×`, manually advance 100 ticks, and inspect worlds, historical events, and important people. Data persists in the `universe-data` Docker volume.

Stop with `docker compose down`. Add `-v` only when you intentionally want to erase all simulated universes.

## Local development

Start PostgreSQL with `docker compose up postgres`, then run:

```bash
cd spa && npm install && npm run build && cd ..
dotnet run --project src/UniversalSimulation.Api
dotnet test
```

The default database connection is in `appsettings.json`. The app retries until PostgreSQL is ready and applies the idempotent schema migration at startup.

## API

- `GET /api/health`
- `GET /api/universes` and `GET /api/universes/{id}`
- `POST /api/universes` with `{ "name": "Jade Expanse", "parameters": { "seed": 42 } }`
- `POST /api/scenarios/cosmic-refinement` accepts `{ "scale": "Mountain|Star|World", "livingWorldTarget": false }` and creates a paused, inspectable refinement scenario
- `PATCH /api/universes/{id}/control` with `{ "status": "Paused" }` or `{ "speed": 100 }`
- `POST /api/universes/{id}/step` with `{ "ticks": 100 }`
- `DELETE /api/universes/{id}` permanently removes one simulation and its cascading event history
- `GET /api/universes/{id}/events?limit=100&importance=Major`

Parameter defaults cover genesis rate, life chance, independent cultivation and technology affinities, conflict rate, cosmic-entity rate and initial system target. See [architecture notes](docs/ARCHITECTURE.md) and [schema migration](src/UniversalSimulation.Infrastructure/Migrations/001_initial.sql).

## Current prototype boundaries

This is a core sandbox, not a finished MMO simulation. State is snapshotted as JSON for fast iteration; the normalized event ledger is the durable analytic surface. Cosmic refinement supports mountains, stars/star systems, and whole worlds (living or dead). Every operation persists its actor, purpose, ordered preparation, resulting material, aftermath, and the refiner's next action as a causal event chain.
