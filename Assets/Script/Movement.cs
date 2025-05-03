using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    public float movePower = 1f; // 이동 속도 조절 변수
    public float jumpPower = 1f; // 점프 힘 조절 변수

    public Rigidbody2D rigid; // 플레이어의 Rigidbody2D 컴포넌트 참조

    Vector3 movement; // 이동 방향을 저장하는 변수
    bool isJumping = false; // 점프 입력이 들어왔는지 확인하는 변수
    bool isGrounded = false; // 바닥에 닿아있는지 확인하는 변수

    [SerializeField] private float dashDelay = 0f; // 대쉬 지연 시간 카운터
    [SerializeField] private float dashTime = 0f; // 대쉬 쿨타임
    [SerializeField] private float dashSpeed = 0.1f; // 대쉬 속도
    [SerializeField] private float dashLoop = 10f; // 대쉬 지속 시간
    [SerializeField] private float dashLoopTime = 0.05f; // 대쉬 지속 시간
    private float delayTime = 0f; // 대쉬 지연 시간 카운터

    bool isDashing = false; // 대쉬 상태 확인 변수
    private int dashDirection = 1; // 1: 오른쪽, -1: 왼쪽

    private bool dashInput = false;

    // 매 프레임마다 호출되는 함수
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            dashInput = true;
        }
        // 위쪽 입력(↑, W)이 눌렸고, 바닥에 있을 때만 점프 가능
        if (Input.GetAxisRaw("Vertical") > 0 && isGrounded) {
            isJumping = true;
        }
        // 만약 현재 캐릭터가 대시(dash) 중이라면
    }

    // 물리 연산이 필요한 경우 호출되는 함수
    void FixedUpdate ()
    {
        Move(); // 이동 처리
        Jump(); // 점프 처리
        if (dashInput) {
            Execute();
            dashInput = false;
        }
        if (!isDashing)
            delayTime += Time.deltaTime;
    }

    // 좌우 이동을 처리하는 함수
    void Move() 
    {
        Vector3 moveVelocity = Vector3.zero; // 이동 방향 초기화

        // 왼쪽 방향키 또는 A키 입력 시 왼쪽으로 이동
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            moveVelocity = Vector3.left;
            dashDirection = -1; // 왼쪽 입력 시 대시 방향도 왼쪽
        }
        // 오른쪽 방향키 또는 D키 입력 시 오른쪽으로 이동
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            moveVelocity = Vector3.right;
            dashDirection = 1; // 오른쪽 입력 시 대시 방향도 오른쪽
        }
        // 실제 위치 이동
        transform.position += moveVelocity * movePower * Time.deltaTime;
    }

    // 점프를 처리하는 함수
    void Jump ()
    {
        if (!isJumping) // 점프 입력이 없으면 실행하지 않음
            return;

        rigid.linearVelocity = Vector2.zero; // 기존 속도를 초기화

        Vector2 jumpVelocity = new Vector2 (0, jumpPower); // 위쪽으로 점프 힘 적용
        rigid.AddForce (jumpVelocity, ForceMode2D.Impulse); // 힘을 순간적으로 가함

        isJumping = false; // 점프 입력 초기화
        isGrounded = false; // 점프 중에는 바닥에 있지 않음
    }

    // 바닥에 닿았을 때 호출되는 함수
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트의 태그가 "Ground"라면 바닥에 닿은 것으로 간주
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // 바닥에서 떨어졌을 때 호출되는 함수
    void OnCollisionExit2D(Collision2D collision)
    {
        // 충돌이 끝난 오브젝트의 태그가 "Ground"라면 바닥에서 떨어진 것으로 간주
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // 대시(dash) 동작을 실행하는 함수
    public void Execute() {
        // 대시 중이거나 쿨타임이 남아있으면 실행 불가
        if (delayTime < dashDelay || isDashing) {
            Debug.Log("대시 불가능");
            return;
        }
        delayTime = 0;
        isDashing = true;
        Debug.Log("대시");
        StartCoroutine(DashCoroutine());
    }

    // 실제 대시 상태를 일정 시간 유지하는 코루틴 함수
    private IEnumerator DashCoroutine() {
        // 대시 시작: Y축 이동 고정
        rigid.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

        for (int i = 0; i < dashLoop; i++) {
            transform.position += Vector3.right * dashDirection * dashSpeed;
            yield return new WaitForSeconds(dashLoopTime); // 이동 후 잠깐 대기
        }
        // 대시 끝: Y축 이동 고정 해제(회전만 고정)
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
    }
}
