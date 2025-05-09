using UnityEngine;
using System.Collections;

// 총알 오브젝트의 컴포넌트
public class Bullet : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 오브젝트의 태그가 "Ground" 또는 "walls"라면 총알을 파괴
        if (other.CompareTag("Ground") || other.CompareTag("walls"))
        {
            // 총알 오브젝트를 파괴
            Destroy(gameObject);
        }
    }
}
