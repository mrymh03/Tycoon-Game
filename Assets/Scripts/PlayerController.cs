using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof (CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FloatingJoystick _joystick;
    [SerializeField] private Animator _animator;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] public int maxDamage = 10;
    public GameObject HitDamageText;
    public GameObject target;
    EnemyController targetData;
    public float range = 3f;
    public float punchCooldown = 0.1f;
    float currentCooldown = 0f;
    bool isTouching = false;

    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = new Vector3(_joystick.Horizontal * _moveSpeed, _rigidbody.velocity.y, _joystick.Vertical * _moveSpeed);

        if (_joystick.Horizontal != 0 || _joystick.Vertical != 0)
        {
            if (target == null)
            {
                transform.rotation = Quaternion.LookRotation(_rigidbody.velocity);
            }
            _animator.SetBool("Walking", true);
        }
        else
            _animator.SetBool("Walking", false);
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                EnemyController tempEnemyData = enemy.GetComponent<EnemyController>();
                if (tempEnemyData.ReturnHealth() > 0) {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                }
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy;
            targetData = nearestEnemy.GetComponent<EnemyController>();
        } else
        {
            target = null;
            targetData = null;
        }
    }

    // Update is called once per frame
    void Update () 
    {
        if (target == null)
        {
            return;
        }

        // Make sure the enemy is alive
        if (targetData.ReturnHealth() <= 0)
        {
            target = null;
            targetData = null;
            return;
        }

        LockOnTarget();

        float gotDistance = (target.transform.position - transform.position).sqrMagnitude;

        if ((gotDistance <= 3f) && Time.time - currentCooldown >= punchCooldown)
        {
            currentCooldown = Time.time;
            _animator.Play("Attack");
        }
    }

    void Hit()
    {
        if (isTouching)
        {
            float gotDamage = Mathf.Ceil(Random.Range(maxDamage/2, maxDamage));
            ShowDamageText(gotDamage);
            Animator gotAnimation = target.GetComponent<Animator>();
            gotAnimation.Play("Hit");
            targetData.GotDamage(gotDamage);
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if (!targetData) {
            isTouching = false;
            return;
        }
        if (other.tag == "Enemy" && other.transform == target.transform){
            if (targetData.ReturnHealth() > 0)
            {
                isTouching = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!targetData) {
            isTouching = false;
            return;
        }
         if (other.tag == "Enemy" && other.transform == target.transform){
            isTouching = false;
        }
    }

    void ShowDamageText(float gotDamage) 
    {
        GameObject hitLabel = Instantiate(HitDamageText, target.transform.position, Quaternion.identity, target.transform);
        hitLabel.GetComponent<TMPro.TextMeshPro>().text = gotDamage.ToString();
    }

    void LockOnTarget ()
    {
        Vector3 dir = target.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 10f).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, range);
	}
}