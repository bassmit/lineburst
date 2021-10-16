using Unity.Entities;
using UnityEngine;

class BurstTestBehaviour : MonoBehaviour
{
    void Start()
    {
        var world = World.All[0];
        var s = world.CreateSystem<BurstTestSystem>();
        world.GetOrCreateSystem<SimulationSystemGroup>().AddSystemToUpdateList(s);
    }
}