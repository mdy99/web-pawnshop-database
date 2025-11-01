using UnityEngine;

public class FixedAspect : MonoBehaviour
{
    public float targetAspect = 16f / 9f;

    private int lastScreenWidth = 0;
    private int lastScreenHeight = 0;

    private void Start() {
        lastScreenHeight = Screen.height;
        lastScreenWidth = Screen.width;
        ApplyAspect();
    }

    private void Update() {
        if (lastScreenHeight != Screen.height || lastScreenWidth != Screen.width) {
            lastScreenHeight = Screen.height;
            lastScreenWidth = Screen.width;
            ApplyAspect();
        }
    }

    private void ApplyAspect() {
        Camera cam = GetComponent<Camera>();
        // 현재 창의 실제 화면 비율 계산
        float windowAspect = (float)Screen.width / (float)Screen.height;
        // 현재 비율과 목표 비율 비교. Scale 얻기
        float scaleHeight = windowAspect / targetAspect;

        // 레터박스 추가
        if(scaleHeight < 1.0f) {
            Rect rect = cam.rect;
            
            rect.width = 1.0f; // 가로 꽉 채우고
            rect.height = scaleHeight; // 세로는 비율에 맞게
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f; // 위아래 중앙 정렬

            cam.rect = rect;
        } else {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = cam.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            cam.rect = rect;
        }
    }
}
