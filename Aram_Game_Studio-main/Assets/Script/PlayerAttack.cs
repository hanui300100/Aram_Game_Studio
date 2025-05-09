using UnityEngine;
using System.Collections;

// 플레이어의 공격 기능을 담당하는 클래스
public class PlayerAttack : MonoBehaviour
{
    // 총알 프리팹을 저장할 변수
    public GameObject bulletPrefab;
    // 총알이 생성될 위치를 지정하는 Transform
    public Transform bulletSpawnPoint;
    // 총알의 이동 속도 (초당 이동하는 유니티 단위)
    public float bulletSpeed = 10f;
    // 연사 속도 (초당 발사 가능한 횟수)
    // 예: fireRate = 10f인 경우, 1초에 10발 발사 가능
    public float fireRate = 10f;
    // 다음 발사 가능한 시간을 저장하는 변수
    // Time.time은 게임 시작 후 경과한 시간(초)을 반환
    private float nextFireTime = 0f;
    // 총알의 물리 컴포넌트
    public Rigidbody2D bulletRb;
    // Movement 컴포넌트 참조
    private Movement movement;

    // 게임 시작 시 초기화
    void Start()
    {
        // 총알 프리팹에서 Rigidbody2D 컴포넌트를 가져옴
        bulletRb = bulletPrefab.GetComponent<Rigidbody2D>();
        // 같은 게임 오브젝트에서 Movement 컴포넌트를 가져옴
        movement = GetComponent<Movement>();
    }

    // 매 프레임마다 실행되는 업데이트 함수
    // Time.deltaTime: 이전 프레임과 현재 프레임 사이의 시간 간격(초)
    void Update()
    {
        // 마우스 왼쪽 버튼을 누르고, 다음 발사 시간이 되었을 때
        // Time.time >= nextFireTime: 현재 시간이 다음 발사 가능 시간보다 크거나 같을 때
        if (Input.GetAxisRaw("Fire1") > 0 && Time.time >= nextFireTime)
        {
            // 총알 발사
            Shoot();
            // 다음 발사 가능 시간을 설정
            // 현재 시간 + (1초 / 연사 속도)
            // 예: fireRate = 10f인 경우, 0.1초 후에 다음 발사 가능
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    // 총알을 발사하는 함수
    void Shoot()
    {
        // 총알의 생성 위치를 계산
        Vector3 spawnPosition = bulletSpawnPoint.position;
        
        // 계산된 위치에 총알 생성
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, bulletSpawnPoint.rotation);
        
        // 생성된 총알의 Rigidbody2D 컴포넌트를 가져옴
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        
        // 총알의 속도를 설정 (플레이어의 이동 방향으로 발사)
        // movement.direction: 플레이어의 현재 이동 방향 벡터
        // * bulletSpeed: 속도 크기 조절
        // linearVelocity: 2D 물리 시스템에서 속도와 방향을 나타내는 벡터
        rb.linearVelocity = movement.direction * bulletSpeed;
    }
}
