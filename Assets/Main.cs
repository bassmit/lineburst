using LineBurst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class Main : MonoBehaviour
{
    public float Speed;

    float3 _normal = new float3(0, 1, 0);

    void Update()
    {
        Draw.Sphere(new float3(-2, 0, 0), 1, Color.black, 64);
        Draw.Circle(new float3(3, 0, 0), .7f, _normal, Color.magenta, 64);
        var a = Speed * Time.deltaTime;
        _normal = math.mul(quaternion.EulerYXZ(a, a, 0), _normal);
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
                Draw.Cone(0, 1, math.radians(20), color);
            })
            .Schedule();
    }
}