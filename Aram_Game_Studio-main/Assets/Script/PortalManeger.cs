using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalManeger : MonoBehaviour
{
    public Object scene;
    bool isScene = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isScene = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isScene = false;
        }
    }

    void Update()
    {
        if (isScene)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                SceneManager.LoadScene(scene.name);
            }
        }
    }
}
