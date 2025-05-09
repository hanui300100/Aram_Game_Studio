using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] Transform player; // 카메라가 따라갈 대상(플레이어). Inspector에서 직접 할당해야 함.
    [SerializeField] float smoothing = 0.2f; // 카메라가 목표 위치로 이동할 때의 부드러움 정도. 값이 작을수록 더 천천히 따라감.
    [SerializeField] Vector2 minCameraBoundary; // 카메라가 이동할 수 있는 최소 x, y 좌표. 맵의 왼쪽 아래 등 경계 제한에 사용.
    [SerializeField] Vector2 maxCameraBoundary; // 카메라가 이동할 수 있는 최대 x, y 좌표. 맵의 오른쪽 위 등 경계 제한에 사용.
    [SerializeField] float followThresholdY = 0f; // 플레이어가 이 y값을 넘어서야 카메라가 y축으로 따라가기 시작함. 그 전까지는 y가 고정됨.

    // FixedUpdate는 물리 연산이 일어나는 일정한 시간 간격마다 호출됨. 카메라 이동은 물리 연산과 맞추는 것이 자연스러움.
    private void FixedUpdate()
    {
        // targetY는 카메라가 이동할 목표 y좌표. 기본적으로 현재 카메라의 y값을 유지함.
        float targetY = transform.position.y;

        // 플레이어의 y좌표가 followThresholdY보다 높으면 카메라가 플레이어를 따라감.
        // 그렇지 않으면 카메라의 y좌표는 followThresholdY에 고정되어, 플레이어가 낮은 곳에 있을 때 화면이 불필요하게 움직이지 않음.
        if (player.position.y > followThresholdY)
            targetY = player.position.y; // 플레이어가 충분히 높이 올라갔을 때만 카메라가 y축을 따라감
        else
            targetY = followThresholdY; // 그 전까지는 y축이 고정되어 안정적인 화면 제공

        // 목표 위치 계산:
        // x: 플레이어의 x좌표가 맵 경계(min~max) 밖으로 나가지 않도록 제한하는 코드
        // Mathf.Clamp(값, 최소, 최대)는 값이 최소보다 작으면 최소, 최대보다 크면 최대, 그 사이면 그대로 반환함
        // 예시: player.position.x가 -5면 minCameraBoundary.x(예:0) 반환, 15면 maxCameraBoundary.x(예:10) 반환, 2면 2 반환
        Vector3 targetPos = new Vector3(
            Mathf.Clamp(player.position.x, minCameraBoundary.x, maxCameraBoundary.x), // x축: 플레이어를 따라가지만 맵 경계 내에서만 이동
            Mathf.Clamp(targetY, minCameraBoundary.y, maxCameraBoundary.y),           // y축: 플레이어가 임계값 이상일 때만 따라가고, 경계 내에서만 이동
            this.transform.position.z // z축: 2D 게임에서는 항상 고정(예: -10)
        );

        // 실제 카메라 위치를 목표 위치로 부드럽게 이동시킴.
        // Vector3.Lerp는 현재 위치에서 목표 위치까지 smoothing 비율만큼만 이동하게 해줌.
        // 값이 1에 가까울수록 즉시 이동, 0에 가까울수록 천천히 따라감.
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);
    }
}
