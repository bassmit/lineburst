using LineBurst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class Main : MonoBehaviour
{
    void Update()
    {
        Draw.Sphere(new float3(-2, 0, 0), 1, Color.black, 64);
    }
}

class BurstDrawTestSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var color = Color.red; // work around burst error

        Job
            .WithBurst()
            .WithCode(() =>
            {
                Draw.Cone(new float3(), new float3(0, 1, 0), math.radians(20), color);
            })
            .Schedule();
    }
}