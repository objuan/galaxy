using UnityEngine;
using System.Collections.Generic;

public class EnemyBoid : MonoBehaviour
{
    [Header("Parametri Sciame")]
    public float speed = 5f;
    public float neighborRadius = 3f; // Quanto lontano vede i compagni
    public float separationDistance = 1.5f;

    [Header("Target")]
    public Transform player;

    private void Update()
    {
        Vector3 acceleration = CalculateFlocking();

        // Movimento fluido verso la direzione calcolata
        if (acceleration != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(acceleration);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        transform.position += transform.forward * speed * Time.deltaTime;
    }

    Vector3 CalculateFlocking()
    {
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        int neighborCount = 0;

        // Trova tutti i nemici vicini
        Collider[] neighbors = Physics.OverlapSphere(transform.position, neighborRadius);

        foreach (var nb in neighbors)
        {
            if (nb.gameObject != this.gameObject && nb.CompareTag("Enemy"))
            {
                neighborCount++;

                // 1. Separazione
                Vector3 diff = transform.position - nb.transform.position;
                if (diff.magnitude < separationDistance)
                    separation += diff.normalized / diff.magnitude;

                // 2. Allineamento
                alignment += nb.transform.forward;

                // 3. Coesione
                cohesion += nb.transform.position;
            }
        }

        if (neighborCount > 0)
        {
            alignment /= neighborCount;
            cohesion = (cohesion / neighborCount) - transform.position;
        }

        // 4. Inseguimento Giocatore
        Vector3 chase = (player.position - transform.position).normalized;

        // Combina tutto (puoi regolare i moltiplicatori per cambiare il comportamento)
        return (separation * 2.5f) + (alignment * 1f) + (cohesion * 1f) + (chase * 2f);
    }
}