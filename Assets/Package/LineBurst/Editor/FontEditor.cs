#if UNITY_EDITOR
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace LineBurst.Authoring
{
    [CustomEditor(typeof(Font), true)]
    [CanEditMultipleObjects]
    class FontEditor : Editor
    {
        const int Width = 100;
        int _editingGlyph;
        int _editingVertex;
        Material _lineMat;
        bool _rulers = true;
        Camera _camera;

        static bool Key => Event.current.type == EventType.KeyDown;
        static bool KeyUp => Key && Event.current.keyCode == LineBurstPrefs.VertexUp;
        static bool KeyDown => Key && Event.current.keyCode == LineBurstPrefs.VertexDown;
        
        void OnSceneGUI()
        {
            if (Application.isPlaying || Selection.count > 1)
                return;

            var font = (Font) target;
            var glyphs = font.Glyphs;
            var sY = font.Size.y;
            var sX = font.Size.x;

            _camera = SceneView.lastActiveSceneView.camera;
            if (_lineMat == null)
                _lineMat = new Material(Shader.Find("Lines/Colored Blended"));
            
            _lineMat.SetPass(0);
            GL.PushMatrix();
            GL.LoadPixelMatrix();
            GL.Begin(GL.LINES);
            
            if (_rulers)
            {
                EmitLine(new float2(0, 0), new float2(sX * glyphs.Length, 0), Color.black);
                EmitLine(new float2(0, sY * font.MarginBottom), new float2(sX * glyphs.Length, sY * font.MarginBottom), Color.gray);
                EmitLine(new float2(0, sY * (1 - font.MarginTop)), new float2(sX * glyphs.Length, sY * (1 - font.MarginTop)), Color.gray);
                EmitLine(new float2(0, sY), new float2(sX * glyphs.Length, sY), Color.black);

                for (int i = 0; i <= glyphs.Length; i++)
                    EmitLine(new float2(sX * i, 0), new float2(sX * i, sY), Color.black);

                for (int i = 0; i < glyphs.Length; i++)
                {
                    EmitLine(new float2(sX * (i + font.MarginSide), 0), new float2(sX * (i + font.MarginSide), sY), Color.gray);
                    EmitLine(new float2(sX * (i + 1 - font.MarginSide), 0), new float2(sX * (i + 1 - font.MarginSide), sY), Color.gray);
                }
            }

            var editingLine = _editingVertex / 2;
            var size = font.Size * new float2(1 - 2 * font.MarginSide, 1 - font.MarginBottom - font.MarginTop);

            // Characters
            for (int i = 0; i < glyphs.Length; i++)
            {
                var offset = font.Size * new float2(i + font.MarginSide, font.MarginBottom);
            
                for (var j = 0; j < glyphs[i].Lines.Length; j++)
                {
                    var line = glyphs[i].Lines[j];
                    var color = LineBurstPrefs.GlyphColor;
                    if (i == _editingGlyph)
                        color = j == editingLine ? LineBurstPrefs.EditLineColor : LineBurstPrefs.EditColor;
                    var org = size * line.Org + offset;
                    var dest = size * line.Dest + offset;
                    EmitLine(org, dest, color);
                }
            }

            GL.End();
            GL.PopMatrix();
            
            Handles.BeginGUI();

            // Buttons
            var glyph = glyphs[_editingGlyph];
            var lines = glyph.Lines;
            var vertexCount = lines.Length * 2;
            var isOrg = _editingVertex % 2 == 0;
            
            GUILayout.Label($"Glyph: {_editingGlyph}/{glyphs.Length - 1}");
            GUILayout.Label($"Expected: \"{(char) (Font.FirstAscii + _editingGlyph)}\"");
            if (GUILayout.Button("Previous", GUILayout.Width(Width)))
                SetEditingGlyph(_editingGlyph == 0 ? glyphs.Length - 1 : _editingGlyph - 1);
            if (GUILayout.Button("Next", GUILayout.Width(Width)))
                SetEditingGlyph(_editingGlyph == glyphs.Length - 1 ? 0 : _editingGlyph + 1);

            GUILayout.Label($"Line: {editingLine}/{lines.Length - 1}");
            GUILayout.Label($"Vertex: {(isOrg ? "Origin" : "Destination")}");
            
            if (GUILayout.Button($"Previous ({LineBurstPrefs.VertexUp})", GUILayout.Width(Width)) || KeyUp)
                if (++_editingVertex >= vertexCount)
                    _editingVertex = 0;
            
            if (GUILayout.Button($"Next ({LineBurstPrefs.VertexDown})", GUILayout.Width(Width)) || KeyDown)
                if (--_editingVertex == -1)
                    _editingVertex = math.max(0, vertexCount - 1);
            
            if (GUILayout.Button("Add Line", GUILayout.Width(Width)))
            {
                Undo.RecordObject(font, "Edit glyph");
                var p = lines.Length > 0 ? lines.Last().Dest : float2.zero;
                glyph.Lines = glyph.Lines.Concat(new[] {new Glyph.Line(p, p)}).ToArray();
                _editingVertex = glyph.Lines.Length > 1 ? glyph.Lines.Length * 2 - 1 : 0;
            }

            if (GUILayout.Button("Remove Line", GUILayout.Width(Width)))
            {
                Undo.RecordObject(font, "Edit glyph");
                glyph.Lines = glyph.Lines.Take(editingLine).Concat(glyph.Lines.Skip(editingLine + 1)).ToArray();
                _editingVertex = glyph.Lines.Length == 0 ? 0 : (editingLine - 1) * 2 + 1;
            }

            if (GUILayout.Button("Push Vertex", GUILayout.Width(Width)))
            {
                if (isOrg)
                {
                    if (editingLine > 0 )
                    {
                        Undo.RecordObject(font, "Edit glyph");
                        lines[editingLine].Org = lines[editingLine - 1].Dest;
                    }
                }
                else if (editingLine < lines.Length - 1)
                {
                    Undo.RecordObject(font, "Edit glyph");
                    lines[editingLine].Dest = lines[editingLine + 1].Org;
                }
            }
            
            if (GUILayout.Button("Pull Vertex", GUILayout.Width(Width)))
            {
                if (isOrg)
                {
                    if (editingLine > 0 )
                    {
                        Undo.RecordObject(font, "Edit glyph");
                        lines[editingLine - 1].Dest = lines[editingLine].Org;
                    }
                }
                else if (editingLine < lines.Length - 1)
                {
                    Undo.RecordObject(font, "Edit glyph");
                    lines[editingLine + 1].Org = lines[editingLine].Dest;
                }
            }

            _rulers = GUILayout.Toggle(_rulers, "Rulers");

            Handles.EndGUI();

            if (editingLine >= lines.Length)
                editingLine = 0;
            
            if (editingLine < lines.Length)
            {
                var offset = font.Size * new float2(_editingGlyph + font.MarginSide, font.MarginBottom);
                
                EditorGUI.BeginChangeCheck();
                var line = lines[editingLine];
                var localPos = isOrg ? line.Org : line.Dest;
                var pos = new float3(offset + size * localPos, 0);
                float3 newPos = Handles.PositionHandle(pos, Quaternion.identity);
                
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(font, "Edit glyph");
                    var delta = (newPos - pos).xy / size;
                    if (isOrg) lines[editingLine].Org += delta;
                    else lines[editingLine].Dest += delta;
                }
            }
        }

        void EmitLine(float2 org, float2 dest, Color color)
        {
            GL.Color(color);
            EmitVertex(org);
            EmitVertex(dest);
        }

        void EmitVertex(float2 v)
        {
            var v0 = _camera.WorldToScreenPoint(new float3(v, 0));
            v0.z = 1; // why?
            GL.Vertex(v0);
        }

        void SetEditingGlyph(int index)
        {
            _editingGlyph = index;
            _editingVertex = 0;
        }
    }
}
#endif