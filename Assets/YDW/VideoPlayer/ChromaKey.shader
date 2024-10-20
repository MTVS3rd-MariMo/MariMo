Shader "Custom/ChromaKey"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // 텍스처를 받는 프로퍼티
        _ChromaColor ("Chroma Key Color", Color) = (0,1,0,1) // 크로마키 색상 (기본: 녹색)
        _Threshold ("Threshold", Range(0, 1)) = 0.1 // 크로마키 탐지에 사용할 임계값
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" } // 투명 객체로 처리
        LOD 100
        
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha // 투명 처리를 위한 블렌딩 설정
            Cull Off // 뒤집힌 면을 제거하지 않음

            CGPROGRAM
            #pragma vertex vert // 버텍스 셰이더 정의
            #pragma fragment frag // 프래그먼트 셰이더 정의
            #include "UnityCG.cginc"

            // 버텍스 구조체
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            // 프래그먼트 구조체
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex; // 텍스처 (동영상)
            float4 _ChromaColor; // 크로마키 색상
            float _Threshold; // 색상 탐지 허용 오차 값

            // 버텍스 셰이더: 오브젝트의 정점 데이터를 화면 좌표로 변환
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // 3D 좌표를 2D 화면 좌표로 변환
                o.uv = v.uv; // 텍스처 좌표 유지
                return o;
            }

            // 프래그먼트 셰이더: 각 픽셀을 처리
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv); // 텍스처(동영상)에서 색상 가져오기
                float dist = distance(texColor.rgb, _ChromaColor.rgb); // 텍스처의 색상과 크로마키 색상 비교
                
                if (dist < _Threshold) // 크로마키 색상과의 거리가 임계값보다 작으면
                    discard; // 픽셀을 제거하여 투명하게 만듦

                return texColor; // 나머지 색상은 그대로 유지
            }
            ENDCG
        }
    }
}
