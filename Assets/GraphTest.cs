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
                var size = fix * new float2(1, 1);
                const float border = .2f;
                
                var pos = new float2(0, 0);
                var graph = new Draw.Graph(pos, size, -math.PI, math.PI, 1f);
                graph.Plot(new Func0(), 30, Color.red);
                graph.Plot(new Func1(), 30, Color.blue);

                pos.x += size.x + border;
                var t = new float2(math.PI, .5f);
                graph = new Draw.Graph(pos, size * 2, -t, math.PI, 1f/3,2);
                graph.Plot(new Func0(), 30, Color.red);

            })
            .Schedule();
    }

    struct Func0 : IFunction
    {
        public float F(float x) => math.sin(x);
    }

    struct Func1 : IFunction
    {
        public float F(float x) => math.cos(x);
    }
}