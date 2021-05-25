using Unity.Entities;
using UnityEngine;

class BurstTestBehaviour : MonoBehaviour
{
    void Start()
    {
        var em = World.All[0].EntityManager;
        em.CreateEntity(typeof(BurstTestComponent));
    }
}