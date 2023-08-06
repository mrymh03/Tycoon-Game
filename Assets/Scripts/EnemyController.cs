using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float Health = 100;
    public float lookRadius = 10f;
    public float attackCooldown = 0.1f;
    float currentCooldown = 0f;
    float currentHealth = 100;

	Transform target;
	NavMeshAgent agent;
    public GameObject gotPlayer;

	void Start()
	{
		target = gotPlayer.transform;
		agent = GetComponent<NavMeshAgent>();
        currentHealth = Health;
	}

	void Update()
	{
		// Get the distance to the player
		float distance = Vector3.Distance(target.position, transform.position);

		// If inside the radius
		if (distance <= lookRadius && currentHealth > 0)
		{
			// Move towards the player
			agent.SetDestination(target.position);
			if (distance <= agent.stoppingDistance && Time.time - currentCooldown >= attackCooldown)
			{
                currentCooldown = Time.time;
                agent.SetDestination(transform.position);
				// Attack
				//combatManager.Attack(Player.instance.playerStats);
				LockOnTarget();
                _animator.Play("Attack");
			}
		}
	}

    public void GotDamage(float Damage)
    {
        currentHealth -= Damage;
        if (currentHealth <= 0)
        {
            // Time to die
            _animator.Play("Death");
            Destroy(gameObject, 1.5f);
        }
    }

    public float ReturnHealth()
    {
        return currentHealth;
    }

    void LockOnTarget()
    {
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 10f).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

	void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, lookRadius);
	}
}
