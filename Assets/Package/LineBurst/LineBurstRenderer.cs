using System;
using LineBurst.Authoring;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using Font = LineBurst.Authoring.Font;

namespace LineBurst
{
    /// <summary>
    /// Attach to any gameobject to render in the scene view, or the camera to also render in the game view
    /// </summary>
    public class LineBurstRenderer : MonoBehaviour
    {
        [Min(1000)]
        public int MaxLines = 250 * 1000;
        public bool DrawInGameView;
        public bool AutoClear = true;
        public static JobHandle Handle;
        bool _clear;
        public Material LineMaterial;
        
        public Authoring.Font Font;

        void Awake()
        {
            if (DrawInGameView && GetComponent<Camera>() == null)
                throw new Exception("LineBurstRenderer needs to be attached to the camera gameobject to draw in the game view");

            Assert.IsTrue(Managed.Instance == null);
            Managed.Instance = new Managed(MaxLines, LineMaterial, Font.Convert());
            RenderPipelineManager.endFrameRendering += (arg1, arg2) => GameViewRender();
        }

        void OnPostRender()
        {
            GameViewRender();
        }

        void GameViewRender()
        {
            if (Application.isPlaying && DrawInGameView)
            {
                Render();

                if (!Application.isEditor || _clear)
                {
                    if (AutoClear)
                        Clear();
                    _clear = false;
                }
                else
                {
                    _clear = true;
                }
            }
        }

        void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Render();

                if (!DrawInGameView || _clear)
                {
                    if (AutoClear)
                        Clear();
                    _clear = false;
                }
                else
                {
                    _clear = true;
                }
            }
        }

        void OnDestroy()
        {
            Managed.Instance?.Dispose();
            Managed.Instance = null;
        }

        public static void Render()
        {
            Handle.Complete();
            Managed.Instance.CopyFromCpuToGpu();
            Managed.Instance.Render();
        }

        public static void Clear()
        {
            if (Managed.Instance != null)
                Managed.Instance.Clear();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (LineMaterial == null)
                LineMaterial = Resources.Load<Material>("LineBurstLineMaterial");
        }
#endif
    }
}