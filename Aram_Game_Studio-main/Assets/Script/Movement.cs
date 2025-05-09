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

    //[SerializeField] private float dashDelay = 0f; // 대쉬 지연 시간 카운터
    [SerializeField] private float dashTime = 0f; // 대쉬 쿨타임
    [SerializeField] private float dashSpeed = 0.1f; // 대쉬 속도
    [SerializeField] private float dashLoop = 100f; // 대쉬 지속 시간
    [SerializeField] private float dashLoopTime = 0.005f; // 대쉬 지속 시간
    private float delayTime = 0f; // 대쉬 지연 시간 카운터

    bool isDashing = false; // 대쉬 상태 확인 변수
    private int dashDirection = 1; // 1: 오른쪽, -1: 왼쪽

    private bool dashInput = false;

    bool ifDash = true;
    bool ifJump = true;
    float dashValue = 0f;
    float dashValue2 = 0f;
    bool moveValue = true;
    public Vector3 direction = Vector3.right;
    public LayerMask groundLayer; // 인스펙터에서 Ground 레이어를 
    [SerializeField] private GameObject dashTransform1;
    [SerializeField] private GameObject dashTransform2;

    // 매 프레임마다 호출되는 함수
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            dashInput = true;
        }
        
        // 플레이어 레이어를 제외하고 Ground 레이어만 감지
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
        isGrounded = hit.collider != null;

        if (hit.collider != null && hit.collider.CompareTag("Ground"))
        {
            if (Input.GetAxisRaw("Vertical") > 0 && ifJump) {
                isJumping = true;
            }
        }
    }

    // 물리 연산이 필요한 경우 호출되는 함수
    void FixedUpdate ()
    {
        if (moveValue) {
            Move(); // 이동 처리
            Jump(); // 점프 처리
        }
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
        Vector3 moveVelocity = Vector3.zero;
        // 왼쪽 방향키 또는 A키 입력 시 왼쪽으로 이동
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            moveVelocity = Vector3.left;
            dashDirection = -1; // 왼쪽 입력 시 대시 방향도 
            direction = Vector3.left; // 바라보는 방향을 왼쪽으로
        }
        // 오른쪽 방향키 또는 D키 입력 시 오른쪽으로 이동
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            moveVelocity = Vector3.right;
            dashDirection = 1; // 오른쪽 입력 시 대시 방향도 오른쪽
            direction = Vector3.right; // 바라보는 방향을 오른쪽으로
        }
        // 실제 위치 이동
        transform.position += moveVelocity * movePower * Time.deltaTime;

        RaycastHit2D hit = Physics2D.Raycast(dashTransform2.transform.position, Vector3.right * dashDirection, dashSpeed);
        hit = Physics2D.Raycast(dashTransform1.transform.position, Vector3.right * dashDirection, dashSpeed);
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
        if (collision.gameObject.CompareTag("walls")) {
            if (ifDash) {
                ifDash = false;
            }
        }
    }

    // 바닥에서 떨어졌을 때 호출되는 함수
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("walls")) {
            if (!ifDash) {
                ifDash = true;
            }
        }
    }

    // 대시(dash) 동작을 실행하는 함수
    public void Execute() {
        // 대시 중이거나 쿨타임이 남아있으면 실행 불가
        if (isDashing) {
            return;
        }

        // 대시 시작 전 벽 충돌 체크
        RaycastHit2D wallCheck0 = Physics2D.Raycast(transform.position, Vector3.right * dashDirection, 0.5f, groundLayer);
        RaycastHit2D wallCheck1 = Physics2D.Raycast(dashTransform1.transform.position, Vector3.right * dashDirection, 0.5f, groundLayer);
        RaycastHit2D wallCheck2 = Physics2D.Raycast(dashTransform2.transform.position, Vector3.right * dashDirection, 0.5f, groundLayer);

        // 벽에 붙어있으면 대시 시작하지 않음
        if (wallCheck1.collider != null || wallCheck2.collider != null) {
            return;
        }

        delayTime = 0;
        isDashing = true;
        ifJump = false;
        StartCoroutine(DashCoroutine());
    }

    // 실제 대시 상태를 일정 시간 유지하는 코루틴 함수
    private IEnumerator DashCoroutine() {
        // 대시 시작: Y축 이동 고정
        rigid.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        dashValue = 0.01f - dashLoopTime;
        moveValue = false;
        
        // 대시 시작 전 위치 저장
        Vector3 startPosition = transform.position;
        
        for (int i = 0; i < dashLoop; i++) {
            if (ifDash) {
                // 다음 위치 계산
                Vector3 nextPosition = transform.position + Vector3.right * dashDirection * dashSpeed;
                
                // 다음 위치에서 충돌 체크
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.right * dashDirection, dashSpeed, groundLayer);
                // 충돌이 감지되고 벽이나 땅이라면 대시 중단
                if (hit.collider != null && (hit.collider.CompareTag("Ground")))
                {
                    break;
                }

                hit = Physics2D.Raycast(dashTransform1.transform.position, Vector3.right * dashDirection, dashSpeed, groundLayer);
                if (hit.collider != null && (hit.collider.CompareTag("Ground")))
                {
                    break;
                }

                hit = Physics2D.Raycast(dashTransform2.transform.position, Vector3.right * dashDirection, dashSpeed, groundLayer);
                if (hit.collider != null && (hit.collider.CompareTag("Ground")))
                {
                    break;
                }

                // 충돌이 없으면 이동
                transform.position = nextPosition;
                yield return new WaitForSeconds(dashValue);
                dashValue2 += 1.5f;
                dashValue += 0.001f * dashValue2;
            }
            else break;
        }
        
        // 대시 끝: Y축 이동 고정 해제(회전만 고정)
        ifJump = true;
        moveValue = true;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        dashValue = 0f;
        dashValue2 = 0f;
    }
}
