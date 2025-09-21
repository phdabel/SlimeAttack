using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoVDetection : MonoBehaviour
{
    public Transform player;
    public float maxAngle;
    public float maxRadius;
    [Header("Detection Settings")]
    public LayerMask obstacleLayer = -1; // Camadas que bloqueiam visão
    public float checkHeight = 1f; // Altura da verificação (evitar verificar no chão)
    
    private bool isInFieldOfView = false;

    private void Update()
    {
        isInFieldOfView = IsInFieldOfView(transform, player, maxAngle, maxRadius);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRadius);

        Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, transform.up) * transform.forward * maxRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, transform.up) * transform.forward * maxRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        if (!isInFieldOfView)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, (player.position - transform.position).normalized * maxRadius);

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * maxRadius);
    }

    public static bool IsInFieldOfView(Transform checkingObject, Transform target, float maxAngle, float maxRadius)
    {
        // Verificação de distância primeiro (mais eficiente)
        Vector3 directionToTarget = target.position - checkingObject.position;
        float distanceToTarget = directionToTarget.magnitude;
        
        if (distanceToTarget > maxRadius)
        {
            return false; // Target muito longe
        }

        // Verificação de ângulo
        directionToTarget.y = 0; // Ignora diferença de altura para cálculo do ângulo
        directionToTarget.Normalize();
        
        Vector3 forward = checkingObject.forward;
        forward.y = 0;
        forward.Normalize();
        
        float angle = Vector3.Angle(forward, directionToTarget);
        
        if (angle > maxAngle)
        {
            return false; // Target fora do cone de visão
        }

        // Verificação de linha de visão (raycast)
        Vector3 rayStart = checkingObject.position + Vector3.up * 0.5f; // Altura dos "olhos"
        Vector3 rayDirection = (target.position + Vector3.up * 0.5f) - rayStart;
        
        RaycastHit hit;
        if (Physics.Raycast(rayStart, rayDirection.normalized, out hit, distanceToTarget))
        {
            // Se o raycast atingiu algo e não é o target, há um obstáculo
            if (hit.transform != target)
            {
                return false; // Obstáculo bloqueando a visão
            }
        }

        return true; // Target está no campo de visão
    }
}
