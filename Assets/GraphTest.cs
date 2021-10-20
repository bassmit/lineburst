using Unity.Entities;
using UnityEngine;
using LineBurst;
using Unity.Collections;
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
            .WithCode(() =>
            {
                var size = fix * new float2(1, 1);
                const float border = .5f;

                var pos = new float2(0, 0);
                var graph = new Draw.Graph(pos, size, -math.PI, math.PI, 1);
                graph.Plot(new Func0(), 30, Color.red, "SIN");
                graph.Plot(new Func2(), 30, Color.green, "COS");
                var a = new NativeArray<float>(2, Allocator.Temp);
                a[0] = -math.PI / 2;
                a[1] = math.PI / 2;
                graph.Plot(new Func1(), 30, Color.blue, "TAN", a);

                pos.x += size.x + border;
                var s = new GraphSettings(pos, size * 2, -new float2(math.PI, .5f), math.PI, 1f / 3)
                {
                    MarkingInterval = 2,
                    Title = "THIS IS A TITLE",
                    HorizontalAxisName = "HORIZONTAL AXIS",
                    VerticalAxisName = "VERTICAL AXIS"
                };
                graph = new Draw.Graph(s);
                graph.Plot(new Func0(), 30, Color.red, "ALSO SIN");

            })
            .Schedule();
    }

    struct Func0 : IFunction
    {
        public float F(float x) => math.sin(x);
    }

    struct Func1 : IFunction
    {
        public float F(float x) => math.tan(x);
    }
    
    struct Func2 : IFunction
    {
        public float F(float x) => math.cos(x);
    }

}