using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.Rendering.Universal.ShaderGUI
{
    internal class CustomLitShader : BaseShaderGUI
    {
        static readonly string[] workflowModeNames = Enum.GetNames(typeof(LitGUI.WorkflowMode));

        #region Mirror
        protected MaterialProperty enableMirrorProp { get; set; }
        protected MaterialProperty reflectIntensityProp { get; set; }
        protected MaterialProperty contrastProp { get; set; }
        
        public static readonly GUIContent enableMirrorText = EditorGUIUtility.TrTextContent("EnableMirror","MirrorObject for Transparent.");
        public static readonly GUIContent reflectIntensityText = EditorGUIUtility.TrTextContent("ReflectIntensity","Mirror Intensity.");
        #endregion

        #region Maziora
        protected MaterialProperty enableMazioraProp { get; set; }
        protected MaterialProperty gradMapProp { get; set; }
        protected MaterialProperty invertGradProp { get; set; }
        protected MaterialProperty blendFactorProp { get; set; }
        protected MaterialProperty gradPowerProp { get; set; }
        protected MaterialProperty gradShiftProp { get; set; }
        
        public static readonly GUIContent enableMazioraText = EditorGUIUtility.TrTextContent("EnableMaziora","MazioraColor.");
        public static readonly GUIContent invertGradText = EditorGUIUtility.TrTextContent("invertGrad","invertGrad.");
        public static readonly GUIContent gradMapText = EditorGUIUtility.TrTextContent("Grad Map", "GradationMap.");

        #endregion


        private LitGUI.LitProperties litProperties;
        private CustomLitDetailGUI.LitProperties litDetailProperties;

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
        
        public override void FillAdditionalFoldouts(MaterialHeaderScopeList materialScopesList)
        {
            materialScopesList.RegisterHeaderScope(CustomLitDetailGUI.Styles.detailInputs, Expandable.Details, _ => CustomLitDetailGUI.DoDetailArea(litDetailProperties, materialEditor));
        }

        // collect properties from the material properties
        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);
            litProperties = new LitGUI.LitProperties(properties);
            litDetailProperties = new CustomLitDetailGUI.LitProperties(properties);
            enableMirrorProp = FindProperty("_EnableMirror", properties, false);
            reflectIntensityProp = FindProperty("_ReflectIntensity", properties, false);
            contrastProp = FindProperty("_Contrast", properties, false);
            
            enableMazioraProp = FindProperty("_EnableMiaziora", properties, false);
            invertGradProp = FindProperty("_InvertGrad", properties, false);
            gradMapProp = FindProperty("_GradMap", properties, false);
            blendFactorProp = FindProperty("_BlendFactor", properties, false);
            gradPowerProp = FindProperty("_GradPower", properties, false);
            gradShiftProp = FindProperty("_GradShift", properties, false);
        }

        // material changed check
        public override void ValidateMaterial(Material material)
        {
            SetMaterialKeywords(material, LitGUI.SetMaterialKeywords, CustomLitDetailGUI.SetMaterialKeywords);
        }

        // material main surface options
        public override void DrawSurfaceOptions(Material material)
        {
            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            if (litProperties.workflowMode != null)
                DoPopup(LitGUI.Styles.workflowModeText, litProperties.workflowMode, workflowModeNames);

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
                var val = EditorGUILayout.Slider("ReflectIntensity", reflectIntensityProp.floatValue, 0, 2);
                if (material.HasProperty("_ReflectIntensity"))
                {
                    material.SetFloat("_ReflectIntensity", val);
                }
                var valContrast = EditorGUILayout.Slider("Contrast", contrastProp.floatValue, 0, 20);
                if (material.HasProperty("_Contrast"))
                {
                    material.SetFloat("_Contrast", valContrast);
                }
            }
            
            DrawFloatToggleProperty(enableMazioraText, enableMazioraProp);
            bool enableMaziora = false;
            if (material.HasProperty("_EnableMiaziora"))
            {
                enableMaziora = material.GetFloat("_EnableMiaziora") == 1.0f;
            }
            CoreUtils.SetKeyword(material, "ENABLE_MAZIORA", enableMaziora);

            if (enableMaziora)
            {
                DrawFloatToggleProperty(invertGradText, invertGradProp);
                if (gradMapProp != null) // Draw the baseMap, most shader will have at least a baseMap
                {
                    materialEditor.TextureProperty(gradMapProp, "GradMap");
                }
                
                var valBlend = EditorGUILayout.Slider("BlendFactor", blendFactorProp.floatValue, 0, 1);
                if (material.HasProperty("_BlendFactor"))
                {
                    material.SetFloat("_BlendFactor", valBlend);
                }
                
                var valGradPower = EditorGUILayout.FloatField("_GradPower", gradPowerProp.floatValue);
                if (material.HasProperty("_GradPower"))
                {
                    material.SetFloat("_GradPower", valGradPower);
                }
                
                var valGradShift = EditorGUILayout.FloatField("_GradShift", gradShiftProp.floatValue);
                if (material.HasProperty("_GradShift"))
                {
                    material.SetFloat("_GradShift", valGradShift);
                }
            }
        }

        // material main surface inputs
        public override void DrawSurfaceInputs(Material material)
        {
            base.DrawSurfaceInputs(material);
            LitGUI.Inputs(litProperties, materialEditor, material);
            DrawEmissionProperties(material, true);
            DrawTileOffset(materialEditor, baseMapProp);
        }

        // material main advanced options
        public override void DrawAdvancedOptions(Material material)
        {
            if (litProperties.reflections != null && litProperties.highlights != null)
            {
                materialEditor.ShaderProperty(litProperties.highlights, LitGUI.Styles.highlightsText);
                materialEditor.ShaderProperty(litProperties.reflections, LitGUI.Styles.reflectionsText);
            }

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
            material.SetFloat("_Blend", (float)blendMode);

            material.SetFloat("_Surface", (float)surfaceType);
            if (surfaceType == SurfaceType.Opaque)
            {
                material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            }
            else
            {
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            }

            if (oldShader.name.Equals("Standard (Specular setup)"))
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Specular);
                Texture texture = material.GetTexture("_SpecGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }
            else
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Metallic);
                Texture texture = material.GetTexture("_MetallicGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }
        }
    }
}
