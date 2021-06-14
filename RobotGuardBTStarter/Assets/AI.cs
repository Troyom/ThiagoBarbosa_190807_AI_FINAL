using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{
    public Transform player;
    public Transform bulletSpawn;
    public Slider healthBar;
    public GameObject bulletPrefab;
    public Transform[] patrulha;

    NavMeshAgent agent;

    //Ajeita a movimentaçao 
    public Vector3 destination;

    //Aejita a mira
    public Vector3 target;

    //Ajeita a vida
    float health = 100.0f;

    //Ajeita a velocidade de rotaçao
    float rotSpeed = 5.0f;

    float visibleRange = 80.0f;

    //Alcance do tiro
    public float shotRange = 40.0f;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = shotRange - 5; //for a little buffer
        InvokeRepeating("UpdateHealth", 5, 0.5f);
    }

    void Update()
    {
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
        healthBar.value = (int)health;
        healthBar.transform.position = healthBarPos + new Vector3(0, 60, 0);
    }

    void UpdateHealth()
    {
        if (health < 100)
            health++;
    }

    //Faz o player perder vida ao ser atingido
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "bullet")
        {
            health -= 10;
        }
    }

    [Task]
    public void PickRandomDestination()
    {
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        agent.SetDestination(dest);
        Task.current.Succeed();
    }


    //Faz o tanque se mover
    [Task]
    public void MoveToDestination()
    {
        if (Task.isInspected)
        {
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);
        }
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public void PickDestination(int x, int z)
    {

        Vector3 dest = new Vector3(x, 0, z);

        agent.SetDestination(dest);

        Task.current.Succeed();
    }

    [Task]
    public void TargetPlayer()
    {
        target = player.transform.position;
        Task.current.Succeed();
    }

    [Task]
    public bool Fire()
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab,
            bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);

        return true;
    }

    [Task]
    public void LookAtTarget()
    {
        Vector3 direction = target - this.transform.position;

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
            Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);

        if (Task.isInspected) Task.current.debugInfo = string.Format("angle={0}",
            Vector3.Angle(this.transform.forward, direction));

        if (Vector3.Angle(this.transform.forward, direction) < 5.0f)
        {
            Task.current.Succeed();
        }
    }

    [Task]
    bool SeePlayer()
    {
        Vector3 distance = player.transform.position - this.transform.position;
        RaycastHit hit; bool seeWall = false; 
        Debug.DrawRay(this.transform.position, distance, Color.red);

        if (Physics.Raycast(this.transform.position, distance, out hit))
        {
            if (hit.collider.gameObject.tag == "wall")
            {
                seeWall = true;
            }
        }
        if (Task.isInspected) Task.current.debugInfo = string.Format("wall={0}", seeWall); 
        
        if (distance.magnitude < visibleRange && !seeWall) return true; else return false;
    }
    [Task] bool Turn(float angle){ 
        var p = this.transform.position + Quaternion.AngleAxis
            (angle, Vector3.up) * this.transform.forward; 
        target = p; return true; 
    }
    [Task]
    public bool IsHealthLessThan(float health)
    {
        return this.health < health;
    }
    [Task]
    public bool Explode()
    {
        Destroy(healthBar.gameObject);
        Destroy(this.gameObject);
        return true;
    }
    [Task]
    public void Chase()
    {
        agent.SetDestination(player.position);
        if (agent.remainingDistance < 30)
        {
            Task.current.Succeed();
        }
    }
    [Task]
    public void Fugir()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= 30)
        {
            Vector3 dirToPlayer = player.transform.position - transform.position;

            Vector3 newPos = transform.position - dirToPlayer;

            agent.SetDestination(newPos);
            agent.speed = 30;
        }

        if (distance > 30f)
        {
            Task.current.Succeed();
        }

    }
    [Task]
    public void Patrulha(int i)
    {
        agent.SetDestination(patrulha[i].position);

        if (agent.remainingDistance <= 5)
        {
            Task.current.Succeed();
        }
    }


}

