using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace LineBurst
{
    class Managed : IDisposable
    {
        readonly Material _lineMaterial;
        int _vertsTodraw;

        ComputeBuffer _vertexBuffer;
        // ComputeBuffer _colorBuffer;

        static Managed _instance;
        // static readonly int ColorBuffer = Shader.PropertyToID("colorBuffer");
        static readonly int VertexBuffer = Shader.PropertyToID("LineBurstVertex");

        internal static Managed Instance;
        bool _warned;

        internal Managed(int maxLines, Material lineMaterial, BlobAssetReference<Font> font, GraphSettings graphSettings)
        {
            if (lineMaterial == null)
                throw new Exception("Line burst line material not assigned");
            Unmanaged.Instance.Data.Initialize(maxLines, font, graphSettings);
            _lineMaterial = lineMaterial;
#if !UNITY_DOTSRUNTIME
            AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
#endif
        }
        static void OnDomainUnload(object sender, EventArgs e)
        {
            _instance?.Dispose();
        }

        internal void CopyFromCpuToGpu()
        {
            // Recreate compute buffer if needed.
            // if (_colorBuffer == null || _colorBuffer.count != Unmanaged.Instance.Data.ColorData.Length)
            // {
            //     if (_colorBuffer != null)
            //     {
            //         _colorBuffer.Release();
            //         _colorBuffer = null;
            //     }
            //
            //     _colorBuffer = new ComputeBuffer(Unmanaged.Instance.Data.ColorData.Length, UnsafeUtility.SizeOf<float4>());
            //     _lineMaterial.SetBuffer(ColorBuffer, _colorBuffer);
            //     _colorBuffer.SetData(Unmanaged.Instance.Data.ColorData.ToNativeArray());
            // }

            if (_vertexBuffer == null || _vertexBuffer.count != Unmanaged.Instance.Data.LineBuffer.Length)
            {
                if (_vertexBuffer != null)
                {
                    _vertexBuffer.Release();
                    _vertexBuffer = null;
                }

                _vertexBuffer = new ComputeBuffer(Unmanaged.Instance.Data.LineBuffer.Length, UnsafeUtility.SizeOf<float4>());
                _lineMaterial.SetBuffer(VertexBuffer, _vertexBuffer);
            }

            _vertsTodraw = Unmanaged.Instance.Data.LineBufferAllocations.Filled * 2;
            if (!_warned && _vertsTodraw == _vertexBuffer.count)
            {
                _warned = true;
                Debug.Log($"### Warning - Maximum number of lines reached, additional lines will not be drawn");
            }

            _vertexBuffer.SetData(Unmanaged.Instance.Data.LineBuffer.ToNativeArray(), 0, 0, _vertsTodraw);
        }

        internal static void Clear()
        {
            Unmanaged.Instance.Data.Clear();
        }

        internal void Render()
        {
            _lineMaterial.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Lines, _vertsTodraw);
        }

/*
        internal void Render(HDCamera hdCamera, CommandBuffer cmd)
        {
            if (hdCamera.camera.cameraType != CameraType.Game)
                return;
            cmd.DrawProcedural(Matrix4x4.identity, resources.textMaterial, 0, MeshTopology.Triangles, NumTextBoxesToDraw * 6, 1);
            cmd.DrawProcedural(Matrix4x4.identity, resources.graphMaterial, 0, MeshTopology.Triangles, NumGraphsToDraw * 6, 1);
        }

        internal void Render3D(HDCamera hdCamera, CommandBuffer cmd)
        {
            cmd.DrawProcedural(Matrix4x4.identity, _lineMaterial, 0, MeshTopology.Lines, NumLinesToDraw, 1);
        }
*/

        public void Dispose()
        {
            _vertexBuffer?.Dispose();
            _vertexBuffer = null;
            // _colorBuffer?.Dispose();
            // _colorBuffer = null;

            Unmanaged.Instance.Data.Dispose();
            if (_instance == this)
                _instance = null;
        }
    }
}