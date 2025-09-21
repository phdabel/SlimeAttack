using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{

    public ParticleSystem fxHit;
    private bool isCut = false;

    void GetHit(int amount)
    {
        if (isCut) return; // Se já foi cortada, não faz nada

        transform.localScale = new Vector3(1f, 1f, 1f);
        fxHit.Emit(10);
        isCut = true;
        Debug.Log("Grama cortada!");
    }
}
