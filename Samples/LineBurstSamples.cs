using LineBurst;
using Unity.Mathematics;
using UnityEngine;

class LineBurstSamples : MonoBehaviour
{
    public float Speed;

    float3 _normal = new float3(0, 1, 0);

    void Update()
    {
        Draw.Sphere(new float3(-2, 0, 0), 1, Color.black, 64);
        Draw.Circle(new float3(3, 0, 0), .7f, _normal, Color.magenta, 64);
        Draw.Box(1, new float3(-2, 3, 0), quaternion.EulerYXZ(math.radians(new float3(20, 30, 40))), Color.green);
        Draw.Arrow(new float3(1, 2, 0), new float3(0, 2, 0), Color.white);
        Draw.Transform(new float3(3, 2.5f, 0), quaternion.RotateY(math.radians(30)));
        Draw.Cone(new float3(-.2f, -.5f, 0), 1, math.radians(20), Color.red);
        var a = Speed * Time.deltaTime;
        _normal = math.mul(quaternion.EulerYXZ(a, a, 0), _normal);
    }
}