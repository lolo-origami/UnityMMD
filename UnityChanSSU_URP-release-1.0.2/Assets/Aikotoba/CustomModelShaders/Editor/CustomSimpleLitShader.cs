using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEditor.Rendering.Universal.ShaderGUI
{
    internal class CustomSimpleLitShader : BaseShaderGUI
    {
        public static readonly GUIContent enableMirrorText = EditorGUIUtility.TrTextContent("EnableMirror","MirrorObject for Transparent.");
        public static readonly GUIContent reflectIntensityText = EditorGUIUtility.TrTextContent("ReflectIntensity","Mirror Intensity.");
        protected MaterialProperty enableMirrorProp { get; set; }
        protected MaterialProperty reflectIntensity { get; set; }
        
        // Properties
        private SimpleLitGUI.SimpleLitProperties shadingModelProperties;

        static void DrawFloatToggleProperty(GUIContent styles, MaterialProperty prop)
        {
            if (prop == null)
                return;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            bool newValue = EditorGUILayout.Toggle(styles, prop.floatValue == 1.0f);
            if (EditorGUI.EndChangeCheck())
                prop.floatValue = newValue ? 1.0f : 0.0f;
            EditorGUI.showMixedValue = false;
        }

        // collect properties from the material properties
        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);
            shadingModelProperties = new SimpleLitGUI.SimpleLitProperties(properties);
            enableMirrorProp = FindProperty("_EnableMirror", properties, false);
            reflectIntensity = FindProperty("_ReflectIntensity", properties, false);
        }

        // material changed check
        public override void ValidateMaterial(Material material)
        {
            SetMaterialKeywords(material, SimpleLitGUI.SetMaterialKeywords);
        }

        // material main surface options
        public override void DrawSurfaceOptions(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;
            base.DrawSurfaceOptions(material);
            
            DrawFloatToggleProperty(enableMirrorText, enableMirrorProp);
            bool enableMirror = false;
            if (material.HasProperty("_EnableMirror"))
            {
                enableMirror = material.GetFloat("_EnableMirror") == 1.0f;
            }
            CoreUtils.SetKeyword(material, "ENABLE_MIRROR", enableMirror);

            if (enableMirror)
            {
                var val = EditorGUILayout.Slider("ReflectIntensity", reflectIntensity.floatValue, 0, 1);
                if (material.HasProperty("_ReflectIntensity"))
                {

                    material.SetFloat("_ReflectIntensity", val);
                }
            }
        }

        // material main surface inputs
        public override void DrawSurfaceInputs(Material material)
        {
            base.DrawSurfaceInputs(material);
            SimpleLitGUI.Inputs(shadingModelProperties, materialEditor, material);
            DrawEmissionProperties(material, true);
            DrawTileOffset(materialEditor, baseMapProp);
        }

        public override void DrawAdvancedOptions(Material material)
        {
            SimpleLitGUI.Advanced(shadingModelProperties);
            base.DrawAdvancedOptions(material);
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // _Emission property is lost after assigning Standard shader to the material
            // thus transfer it before assigning the new shader
            if (material.HasProperty("_Emission"))
            {
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            }

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                SetupMaterialBlendMode(material);
                return;
            }

            SurfaceType surfaceType = SurfaceType.Opaque;
            BlendMode blendMode = BlendMode.Alpha;
            if (oldShader.name.Contains("/Transparent/Cutout/"))
            {
                surfaceType = SurfaceType.Opaque;
                material.SetFloat("_AlphaClip", 1);
            }
            else if (oldShader.name.Contains("/Transparent/"))
            {
                // NOTE: legacy shaders did not provide physically based transparency
                // therefore Fade mode
                surfaceType = SurfaceType.Transparent;
                blendMode = BlendMode.Alpha;
            }
            material.SetFloat("_Surface", (float)surfaceType);
            material.SetFloat("_Blend", (float)blendMode);
        }
    }
}
