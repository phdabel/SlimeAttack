using UnityEngine;

public class Collectible : MonoBehaviour
{

    private GameManager _GameManager;

    void Start()
    {
        _GameManager = FindAnyObjectByType<GameManager>() as GameManager;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Collectible":
                Destroy(other.gameObject);
                _GameManager.SetGems(5);
                break;
        }
    }
}
