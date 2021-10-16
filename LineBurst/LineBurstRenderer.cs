using System;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

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

        [SerializeField]
        GraphSettings _graphSettings;
        
        void Awake()
        {
            if (DrawInGameView && GetComponent<Camera>() == null)
                throw new Exception("LineBurstRenderer needs to be attached to the camera gameobject to draw in the game view");

            Assert.IsTrue(Managed.Instance == null);
            Managed.Instance = new Managed(MaxLines, LineMaterial, Font.Convert(), _graphSettings);
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
                Managed.Clear();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (LineMaterial == null)
                LineMaterial = Resources.Load<Material>("LineBurstLineMaterial");
            if (Font == null)
                Font = Resources.Load<Authoring.Font>("LineBurstDefaultFont");
            if (_graphSettings.AxisColor == new Color())
                _graphSettings.AxisColor = Color.black;
            if (_graphSettings.GridColor == new Color())
                _graphSettings.GridColor = new Color32(65, 65, 65, 255);
            if (_graphSettings.GridAltColor == new Color())
                _graphSettings.GridAltColor = new Color32(29, 29, 29, 255);
            if (_graphSettings.MarkingColor == new Color())
                _graphSettings.MarkingColor = Color.black;
            if (_graphSettings.MarkingScale <= 0)
                _graphSettings.MarkingScale = .3f;
        }

        void Update()
        {
            Draw.DefaulGraphSettings = _graphSettings;
        }
#endif
    }

    [Serializable]
    public struct GraphSettings
    {
        public Color AxisColor;
        public Color GridColor;
        public Color GridAltColor;
        public Color MarkingColor;
        public float MarkingScale;
    }
}