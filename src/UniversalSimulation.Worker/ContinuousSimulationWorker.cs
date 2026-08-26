using Microsoft.Extensions.Hosting; using Microsoft.Extensions.Logging; using UniversalSimulation.Application;
namespace UniversalSimulation.Worker;
public sealed class ContinuousSimulationWorker(SimulationCoordinator coordinator, ILogger<ContinuousSimulationWorker> log) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct) { while (!ct.IsCancellationRequested) { try { await coordinator.Initialize(ct); break; } catch (Exception e) { log.LogWarning(e,"Database unavailable; retrying"); await Task.Delay(3000,ct); } } while(!ct.IsCancellationRequested) { try { await coordinator.RunCycle(ct); } catch(Exception e) { log.LogError(e,"Simulation cycle failed"); } await Task.Delay(1000,ct); } }
}
