using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent enemy;

    private Transform player;

    private LayerMask whatIsGround, whatIsPlayer;

    private Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    public float campoDeVisao;
    public float playerNoCampoDeVisaoX;
    public float playerNoCampoDeVisaoZ;

    public float outOfBoundsZ;
    public float outOfBoundsMinusZ;
    public float outOfBoundsX;
    public float outOfBoundsMinusX;

    void Start()
    {
        enemy = GetComponent<NavMeshAgent>();
        player = GameObject.Find("point").transform;

        walkPointRange = 20;

        campoDeVisao = Mathf.Abs(25);

        outOfBoundsMinusZ = -140;
        outOfBoundsZ = 96;
        outOfBoundsMinusX = 207;
        outOfBoundsX = 363;
    }

    
    void Update()
    {
        playerNoCampoDeVisaoZ = Mathf.Abs(transform.position.z - player.transform.position.z);
        playerNoCampoDeVisaoX = Mathf.Abs(transform.position.x - player.transform.position.x);

        

        if (playerNoCampoDeVisaoX < campoDeVisao && playerNoCampoDeVisaoZ < campoDeVisao)
        {
            Perseguir();
        }
        else
        {
            Patrulha();
        }
        
        
    }

    void Patrulha()
    {
        if (!walkPointSet)
        {
            NovoWalkPoint();
        }
        else
        {
            enemy.SetDestination(walkPoint);
        }

        Vector3 distanciaAteWalkPoint = transform.position - walkPoint;

        if (distanciaAteWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    void NovoWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);


        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        
        if(randomX < outOfBoundsMinusX || randomX > outOfBoundsX || randomZ < outOfBoundsMinusZ || randomZ > outOfBoundsZ)
        {
            walkPointSet = true;
        }
    }
    void Perseguir()
    {
        enemy.SetDestination(player.position);
        walkPointSet = false;
    }
}
