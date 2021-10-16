using Unity.Entities;
using UnityEngine;
using LineBurst;
using Unity.Mathematics;

class GraphTest : MonoBehaviour
{
    void Start()
    {
        var world = World.All[0];
        var s = world.CreateSystem<GraphTestSystem>();
        world.GetOrCreateSystem<SimulationSystemGroup>().AddSystemToUpdateList(s);
    }
}

[DisableAutoCreation]
class GraphTestSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var fix = 1;
        
        Job
            .WithBurst()
            .WithCode(() =>
            {
                var pos = new float2(0, 0);
                var size = new float2(1, 1);
                var graph = new Draw.Graph(pos, size, -math.PI, math.PI, 1f);
                graph.Plot(new Func0(), fix * 30, Color.red);
            })
            .Schedule();
    }

    struct Func0 : IFunction
    {
        public float F(float x) => math.sin(x);
    }
}