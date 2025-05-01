Shader "Custom/DepthMask" {
    SubShader {
        Tags {"Queue" = "Geometry-101" }
        ColorMask 0
        //Cull Back
        //Cull Front 
        Cull Off
        ZWrite On

        Pass {}
    }
}