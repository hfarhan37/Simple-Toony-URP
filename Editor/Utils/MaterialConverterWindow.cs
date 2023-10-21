using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SimpleToonyURP.Editor.Utils
{
    public class MaterialConverterWindow : EditorWindow
    {
        struct MaterialData
        {
            public bool excludeConversion;
            public Material material;
        }

        private MaterialData[] allMaterials;
        private MaterialData[] selectedSourceMaterials;
        private string[] allShaderNames;
        private string[] projectMaterialShaderNames;

        private int sourceShaderIndex;
        private int sourceShaderPrevIndex = -1;
        private int targetShaderIndex;

        private Vector2 rootScrollPos;
        
        [MenuItem("Window/Simple Toony URP/Material Converter")]
        public static void ShowMaterialConverter()
        {
            MaterialConverterWindow wnd = GetWindow<MaterialConverterWindow>();
            wnd.titleContent = new GUIContent("Material Converter");
            wnd.maxSize = new Vector2(400, 800);
        }


        private  MaterialData[] FindAllMaterials()
        {
            var guids = AssetDatabase.FindAssets("t:Material");
            
            var materialDatas = new List<MaterialData>();
            foreach (var g in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                if (!path.StartsWith("Assets/"))
                {
                    continue;
                }
                var mats = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath(g)).OfType<Material>();
                materialDatas.AddRange(mats.Select(mat => new MaterialData { excludeConversion = false, material = mat }));
            }

            return materialDatas.ToArray();
        }

        private string[] FindAllShaderNames()
        {
            var shaderInfos = ShaderUtil.GetAllShaderInfo();
            var shaderNames = shaderInfos.Select(shaderInfo => shaderInfo.name).ToList();
            return shaderNames.Where(shaderName => !shaderName.StartsWith("Hidden/")).ToArray();
        }

        private string[] FindProjectMaterialShaderNames()
        {
            if (allMaterials == null)
                return null;
            
            var shaderNames = new List<string>();
            foreach (var material in allMaterials)
            {
                var shaderName = material.material.shader.name;
                if (!shaderNames.Contains(shaderName))
                {
                    shaderNames.Add(shaderName);
                }
            }
            return shaderNames.ToArray();

        }

        public void CreateGUI()
        {
            FetchData();
        }

        private void OnProjectChange()
        {
            FetchData();
        }

        private void FetchData()
        {
            allMaterials = FindAllMaterials();
            allShaderNames = FindAllShaderNames();
            projectMaterialShaderNames = FindProjectMaterialShaderNames();
            sourceShaderIndex = 0;
            sourceShaderPrevIndex = -1;
            targetShaderIndex = 0;
        }

        private void OnGUI()
        {
            rootScrollPos = EditorGUILayout.BeginScrollView(rootScrollPos, false, false);
            WrapWithVertical(() =>
            {
                DrawHeader("Material Converter");
                EditorGUILayout.Space();
                
                WrapWithVertical(() =>
                {
                    EditorGUILayout.LabelField("Project Materials to Convert");
                    sourceShaderIndex = EditorGUILayout.Popup(sourceShaderIndex, projectMaterialShaderNames);
                    if (sourceShaderPrevIndex != sourceShaderIndex)
                    {
                        selectedSourceMaterials =
                            allMaterials.Where(data => data.material.shader.name == projectMaterialShaderNames[sourceShaderIndex]).ToArray();
                    }
                    sourceShaderPrevIndex = sourceShaderIndex;
                });
                
                EditorGUILayout.Space();
                
                if (selectedSourceMaterials != null && selectedSourceMaterials.Length > 0)
                {
                    DrawSubHeader("Exclude Materials");
                    EditorGUILayout.Space();
                    WrapWithVertical(DrawSourceMaterialList);
                    EditorGUILayout.Space();
                }
                WrapWithVertical(() =>
                {
                    EditorGUILayout.LabelField("Target");
                    targetShaderIndex = EditorGUILayout.Popup(targetShaderIndex, allShaderNames);
                });
                EditorGUILayout.Space();
                WrapWithVertical(DrawConvertButtons);
            });
            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader(string text)
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16
            };
            EditorGUILayout.LabelField(text, style);
        }

        private void DrawSubHeader(string text)
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14
            };
            EditorGUILayout.LabelField(text, style);
        }

        private void WrapWithVertical(Action action, bool startSpacing = true, bool endSpacing = true)
        {
            var style = new GUIStyle(GUI.skin.box);
            EditorGUILayout.BeginVertical(style, new []{GUILayout.ExpandWidth(false)});
            if (startSpacing)
            {
                EditorGUILayout.Space();
            }
            
            action.Invoke();

            if (endSpacing)
            {
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();
        }

        private void WrapWithHorizontal(Action action, bool startSpacing = true, bool endSpacing = true)
        {
            
            var style = new GUIStyle(GUI.skin.box);
            EditorGUILayout.BeginHorizontal(style, new []{GUILayout.ExpandWidth(false)});
            if (startSpacing)
            {
                EditorGUILayout.Space();
            }
            
            action.Invoke();
            
            if (endSpacing)
            {
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSourceMaterialList()
        {
            if (selectedSourceMaterials == null)
            {
                return;
            }

            for (int i = 0; i < selectedSourceMaterials.Length; i++)
            {

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                selectedSourceMaterials[i].excludeConversion = EditorGUILayout.Toggle(selectedSourceMaterials[i].excludeConversion, new []{GUILayout.MaxWidth(20)});
                GUILayout.Space(10);
                GUI.enabled = false;
                EditorGUILayout.ObjectField("", selectedSourceMaterials[i].material, typeof(Material), false);
                GUI.enabled = true;
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawConvertButtons()
        {
            if (GUILayout.Button("Convert"))
            {
                ConvertMaterials(selectedSourceMaterials.Where(data => !data.excludeConversion)
                    .Select(data => data.material)
                    .ToArray());
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Convert All Project Materials"))
            {
                ConvertMaterials(allMaterials.Where(data => !data.excludeConversion)
                    .Select(data => data.material)
                    .ToArray());
            }
        }

        private void ConvertMaterials(Material[] materials)
        {
            var targetShader = Shader.Find(allShaderNames[targetShaderIndex]);
            if (materials == null || targetShader == null)
            {
                return;
            }
            
            Undo.RecordObjects(materials, "TOON_MATERIAL_CONVERTER_MAT_UNDO");
            foreach (var material in materials)
            {
                material.shader = targetShader;
                EditorUtility.SetDirty(material);
                AssetDatabase.SaveAssetIfDirty(material);
            }
            AssetDatabase.Refresh();
            Close();
        }
    }
}

