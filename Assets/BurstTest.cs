using UnityEngine;
using LineBurst;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;

class BurstTest : MonoBehaviour
{
    float3 _normal = new float3(0, 1, 0);

    void Update()
    {
        var dt = Time.deltaTime;
        _normal = math.mul(quaternion.EulerYXZ(dt, dt, 0), _normal);
        new Job
        {
            DeltaTime = dt,
            Normal = _normal
        }.Schedule();
    }

    [BurstCompile]
    struct Job : IJob
    {
        public float DeltaTime;
        public float3 Normal;
        
        public void Execute()
        {
            Normal = math.mul(quaternion.EulerYXZ(DeltaTime, DeltaTime, 0), Normal);
            Draw.Box(1, new float3(-2, 3, 0), quaternion.EulerYXZ(math.radians(new float3(20, 30, 40))), Color.green);
            Draw.Arrow(new float3(1, 2, 0), new float3(0, 1.7f, 0), Color.white);
            Draw.Transform(new float3(3, 2.5f, 0), quaternion.RotateY(math.radians(30)));
            Draw.Sphere(new float3(-2, .8f, 0), .85f, Color.black, 64);
            Draw.Cone(new float3(.2f, .1f, 0), 1, math.radians(20), Color.red);
            Draw.Circle(new float3(3.2f, .8f, 0), .7f, Normal, Color.yellow, 64);
            Draw.Text("++LINEBURST++", float4x4.TRS(new float3(-3.6f, -.4f, 0), quaternion.identity, new float3(1.6f)), Color.magenta);
        }
    }
}