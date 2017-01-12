#include "UnityCG.cginc"

void CrossFade(float2 screenPos, float _Alpha, float _Ind = 0)
{
	//float textur[5][5] = {{0.99f, 0.81f, 0.48f, 0.75f, 0.95f}, {0.82f, 0.65f, 0.25f, 0.47f, 0.78f}, {0.53f, 0.35f, 0.00f, 0.28f, 0.60f}, {0.88f, 0.65f, 0.22f, 0.56f, 0.73f}, {0.97f, 0.71f, 0.50f, 0.85f, 0.92f}};
	//float textur[25] = {0.99f, 0.81f, 0.48f, 0.75f, 0.95f, 0.82f, 0.65f, 0.25f, 0.47f, 0.78f, 0.53f, 0.35f, 0.00f, 0.28f, 0.60f, 0.88f, 0.65f, 0.22f, 0.56f, 0.73f, 0.97f, 0.71f, 0.50f, 0.85f, 0.92f};
	float textur[4] = { 0.99f, 0.75f, 0.50f, 0.25f };

    float pxX = screenPos.x - floor(screenPos.x / 2.0f ) * 2.0f;
    float pxY = screenPos.y - floor(screenPos.y / 2.0f ) * 2.0f;

	clip(abs(_Ind * 1 - textur[pxX + pxY * 2]) - (1.0f - _Alpha));
}

void CrossFadeUV(float4 screenPos, float _Alpha, float _Ind = 0)
{
	float2 screenPosition = (screenPos.xy/screenPos.w);
    screenPosition = floor(screenPosition * _ScreenParams.xy * 1.0f);
	CrossFade(screenPosition, _Alpha, _Ind);
}