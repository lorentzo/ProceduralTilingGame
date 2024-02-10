
// This shader fills the mesh shape with a color predefined in the code.
Shader "Custom/NormalBasedColor"
{
    // The _BaseColor variable is visible in the Material's Inspector, as a field
    // called Base Color. You can use it to select a custom color. This variable
    // has the default value (1, 1, 1, 1).
    Properties
    { 
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
    }

    // The SubShader block containing the Shader code.
    SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            // The HLSL code block. Unity SRP uses the HLSL language.
            HLSLPROGRAM
            // This line defines the name of the vertex shader.
            #pragma vertex vert
            // This line defines the name of the fragment shader.
            #pragma fragment frag

            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
            struct Attributes
            {
                // The positionOS variable contains the vertex positions in object
                // space.
                float4 positionOS   : POSITION;
                half3 normal        : NORMAL;
            };

            struct Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionHCS  : SV_POSITION;
                half3 normal        : TEXCOORD0;
            };

            // To make the Unity shader SRP Batcher compatible, declare all
            // properties related to a Material in a a single CBUFFER block with
            // the name UnityPerMaterial.
            CBUFFER_START(UnityPerMaterial)
                // The following line declares the _BaseColor variable, so that you
                // can use it in the fragment shader.
                half4 _BaseColor;
            CBUFFER_END

            // The vertex shader definition with properties defined in the Varyings
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
            Varyings vert(Attributes IN)
            {
                // Declaring the output object (OUT) with the Varyings struct.
                Varyings OUT;
                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous clip space.
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                // Use the TransformObjectToWorldNormal function to transform the
                // normals from object to world space. This function is from the
                // SpaceTransforms.hlsl file, which is referenced in Core.hlsl.
                //OUT.normal = TransformObjectToWorldNormal(IN.normal);
                OUT.normal = IN.normal;
                // Returning the output.
                return OUT;
            }

            // The fragment shader definition.
            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = 0;
                color.rgb = IN.normal * 0.5 + 0.5;
                return color;
            }
            ENDHLSL
        }
    }
}
