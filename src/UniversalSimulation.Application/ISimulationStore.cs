using UniversalSimulation.Domain;
namespace UniversalSimulation.Application;
public interface ISimulationStore { Task Initialize(CancellationToken ct); Task<IReadOnlyList<UniverseState>> LoadAll(CancellationToken ct); Task Save(UniverseState universe, IReadOnlyList<SimEvent> events, CancellationToken ct); Task<IReadOnlyList<SimEvent>> Events(Guid universeId, int limit, EventImportance? minimum, CancellationToken ct); Task Delete(Guid universeId, CancellationToken ct); }
