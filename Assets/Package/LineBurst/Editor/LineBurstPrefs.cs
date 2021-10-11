#if UNITY_EDITOR
using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LineBurst
{
    class LineBurstPrefs
    {
        const string PrefName = "LineBurstPreferences";
        public static Color GlyphColor;
        public static Color EditColor;
        public static Color EditLineColor;
        public static KeyCode VertexUp;
        public static KeyCode VertexDown;
        static bool _loaded;
        static StringBuilder _sb;

        [PreferenceItem("LineBurst")]
        static void LineBurstPreferences()
        {
            Init();

            VertexUp = (KeyCode) EditorGUILayout.EnumPopup("Vertex Up", VertexUp);
            VertexDown = (KeyCode) EditorGUILayout.EnumPopup("Vertex Down", VertexDown);
            GlyphColor = EditorGUILayout.ColorField("Glyph Color", GlyphColor);
            EditColor = EditorGUILayout.ColorField("Edit Color", EditColor);
            EditLineColor = EditorGUILayout.ColorField("Edit Line Color", EditLineColor);

            if (GUI.changed)
                Store();
        }

        static LineBurstPrefs()
        {
            Init();
        }

        static void Init()
        {
            if (!_loaded)
            {
                _loaded = true;
                _sb = new StringBuilder();

                if (EditorPrefs.HasKey(PrefName))
                {
                    try
                    {
                        var data = EditorPrefs.GetString(PrefName).Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
                        GlyphColor = GetColor(data[0]);
                        EditColor = GetColor(data[1]);
                        VertexUp = GetKeyCode(data[3]);
                        VertexDown = GetKeyCode(data[4]);
                        EditLineColor = GetColor(data[5]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed to load lineburst preferences: {e}");
                        SetDefaults();
                        Store();
                    }
                }
                else
                {
                    SetDefaults();
                }
            }
        }


        static void SetDefaults()
        {
            GlyphColor = Color.blue;
            EditColor = Color.red;
            VertexUp = KeyCode.M;
            VertexDown = KeyCode.N;
            EditLineColor = Color.magenta;
        }

        static void Store()
        {
            _sb.Clear();
            Append(GlyphColor, _sb);
            Append(EditColor, _sb);
            Append(VertexUp, _sb);
            Append(VertexDown, _sb);
            Append(EditLineColor, _sb);
            EditorPrefs.SetString(PrefName, _sb.ToString());
        }

        static void Append(KeyCode kc, StringBuilder sb)
        {
            sb.Append($"{kc.ToString()};");
        }

        static KeyCode GetKeyCode(string s)
        {
            return (KeyCode) Enum.Parse(typeof(KeyCode), s);
        }

        static void Append(Color color, StringBuilder sb)
        {
            sb.Append($"{Convert.ToString(color.r)},{Convert.ToString(color.g)},{Convert.ToString(color.b)},{Convert.ToString(color.a)};");
        }

        static Color GetColor(string s)
        {
            var d = s.Split(',');
            return new Color(Convert.ToSingle(d[0]), Convert.ToSingle(d[1]), Convert.ToSingle(d[2]), Convert.ToSingle(d[3]));
        }
    }
}
#endif