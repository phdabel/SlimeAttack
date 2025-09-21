using UnityEngine;

public class RainManager : MonoBehaviour
{

    private GameManager _GameManager;
    public bool isRainy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _GameManager = FindAnyObjectByType<GameManager>();

        if (_GameManager == null)
        {
            Debug.LogError("RainManager: GameManager não encontrado na cena!");
        }
        else
        {
            Debug.Log("RainManager: GameManager encontrado com sucesso!");
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("RainManager: OnTriggerEnter chamado.");
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("RainManager: Player entrou na área de chuva.");
            _GameManager.OnOffRain(isRainy);
        }
    }

}
