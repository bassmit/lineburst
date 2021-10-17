using LineBurst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

class LineBurstSamples : MonoBehaviour
{
    public float Speed;

    float3 _normal = new float3(0, 1, 0);

    void Update()
    {
        Draw.Box(1, new float3(-2, 3, 0), quaternion.EulerYXZ(math.radians(new float3(20, 30, 40))), Color.green);
        Draw.Arrow(new float3(1, 2, 0), new float3(0, 1.7f, 0), Color.white);
        Draw.Transform(new float3(3, 2.5f, 0), quaternion.RotateY(math.radians(30)));
        Draw.Sphere(new float3(-2, .8f, 0), .85f, Color.black, 64);
        Draw.Cone(new float3(.2f, .1f, 0), 1, math.radians(20), Color.red);
        Draw.Circle(new float3(3.2f, .8f, 0), .7f, _normal, Color.yellow, 64);
        Draw.Text("++LINEBURST++", float4x4.TRS(new float3(-3.6f, -.4f, 0), quaternion.identity, new float3(1.6f)), Color.magenta);
        var a = Speed * Time.deltaTime;
        _normal = math.mul(quaternion.EulerYXZ(a, a, 0), _normal);
        
        
        var size = new float2(3);
        const float border = .5f;
                
        var pos = new float2(5, 0);
        var graph = new Draw.Graph(pos, size, -math.PI, math.PI, 1);
        // Use IFunction for Burstable API
        graph.Plot(math.sin, 30, Color.red, "SIN");
        graph.Plot(math.cos, 30, Color.green, "COS");
        var explicitSamples = new NativeArray<float>(2, Allocator.Temp);
        explicitSamples[0] = -math.PI / 2;
        explicitSamples[1] = math.PI / 2;
        graph.Plot(math.tan, 30, Color.blue, "TAN", explicitSamples);

        pos.x += size.x + border;
        var s = new GraphSettings(pos, size * 2, -new float2(math.PI, .5f), math.PI, 1f / 3)
        {
            MarkingInterval = 2,
            Title = "THIS IS A TITLE",
            HorizontalAxisName = "HORIZONTAL AXIS",
            VerticalAxisName = "VERTICAL AXIS"
        };
        graph = new Draw.Graph(s);
        graph.Plot(math.sin, 30, Color.red, "ALSO SIN");
    }
}