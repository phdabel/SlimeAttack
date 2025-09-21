using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public enum EnemyState
{
    IDLE,
    ALERT,
    EXPLORE,
    PATROL,
    FOLLOW,
    FURY,
    DIED
}

public enum GameState
{
    GAMEPLAY,
    PAUSED,
    DIED
}


public class GameManager : MonoBehaviour
{

    public GameState GameState = GameState.GAMEPLAY;

    [Header("InfoPlayer")]
    public Transform player;
    private int gems;

    [Header("UI")]
    public Text txtGems;
    public Text txtHP;

    [Header("Slime IA")]
    public float slimeIdleWaitingTime;
    public Transform[] SlimeWayPoints;
    public float slimeDistanceToAttack = 2.3f;
    public float slimeAttackDelay = 1f;

    [Header("Rain Manager")]
    public PostProcessVolume postB;
    public ParticleSystem rainParticle;
    private ParticleSystem.EmissionModule rainModule;
    public int rainRateOverTime;
    public int rainIncrement;
    public float rainIncrementDelay;

    [Header("Drop Item")]
    public GameObject gemPrefab;
    public int chanceDrop = 100;

    public void OnOffRain(bool isRainy)
    {
        StopCoroutine("RainManager");
        StopCoroutine("PostBManager");
        StartCoroutine("RainManager", isRainy);
        StartCoroutine("PostBManager", isRainy);
    }

    IEnumerator RainManager(bool isRainy)
    {
        switch (isRainy)
        {
            case true: // aumenta a chuva
                Debug.Log("Chuva aumentando");
                for (float r = rainModule.rateOverTime.constant; r < rainRateOverTime; r += rainIncrement)
                {
                    rainModule.rateOverTime = r;
                    yield return new WaitForSeconds(rainIncrementDelay);
                }
                rainModule.rateOverTime = rainRateOverTime;
                break;
            case false: // diminui a chuva
                Debug.Log("Chuva diminuindo");
                for (float r = rainModule.rateOverTime.constant; r > 0; r -= rainIncrement)
                {
                    rainModule.rateOverTime = r;
                    yield return new WaitForSeconds(rainIncrementDelay);
                }
                rainModule.rateOverTime = 0;
                break;
        }
    }

    IEnumerator PostBManager(bool isRainy)
    {
        switch (isRainy)
        {
            case true:
                for (float w = postB.weight; w < 1; w += 1 * Time.deltaTime)
                {
                    postB.weight = w;
                    yield return new WaitForEndOfFrame();
                }
                postB.weight = 1;
                break;
            case false:
                for (float w = postB.weight; w > 0; w -= 1 * Time.deltaTime)
                {
                    postB.weight = w;
                    yield return new WaitForEndOfFrame();
                }
                postB.weight = 0;
                break;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rainModule = rainParticle.emission;
        txtGems.text = "Gems: " + gems.ToString();
        txtHP.text = "HP: " + player.GetComponent<PlayerController>().HP.ToString();
    }

    void Update()
    {
        txtGems.text = "Gems: " + gems.ToString();
        txtHP.text = "HP: " + player.GetComponent<PlayerController>().HP.ToString();
    }

    public void ChangeGameState(GameState newState)
    {
        GameState = newState;
    }

    public void SetGems(int amount)
    {
        gems += amount;
    }

}
