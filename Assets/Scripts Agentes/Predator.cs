using UnityEngine;

public class Predator : MonoBehaviour
{
    [Header("Predator Settings")]
    public float energy = 10f;
    public float age = 0f;
    public float maxAge = 20f;
    public float speed = 1f;
    public float visionRange = 5f;

    [Header("Predator States")]
    public bool isAlive = true;
    public PredatorState currentState = PredatorState.Exploring;

    private Vector3 destination;
    private float h; // h es el paso actual en el que estamos (tambien se puede llamar step o time)

    void Start()
    {
        destination = transform.position;
    }

    public void Simulate(float h)
    {
        if (!isAlive)
        {
            return;
        }

        this.h = h;

        switch (currentState)
        {
            case PredatorState.Exploring:
                Explore();
                break;
            case PredatorState.SearchingFood:
                SearchFood();
                break;
            case PredatorState.Eating:
                Eat();
                break;
        }

        Move();
        Age();
        CheckState();
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            destination,
            speed * h
        );

        energy -= speed * h;
    }

    void Age()
    {
        age += h;
    }

    void CheckState()
    {
        if (energy <= 0 || age >= maxAge)
        {
            isAlive = false;
            Destroy(gameObject);
        }
    }

    void Explore()
    {
        Bunny nearestBunny = FindNearestBunny();
        if (nearestBunny != null)
        {
            currentState = PredatorState.SearchingFood;
            destination = nearestBunny.transform.position;
            return;
        }

        if (Vector3.Distance(transform.position, destination) < 0.1f)
        {
            SelectNewDestination();
        }
    }

    void SelectNewDestination() //Direccion
    {
        Vector3 direction = new Vector3(
            Random.Range(-visionRange, visionRange),
            Random.Range(-visionRange, visionRange),
            0
        );

        Vector3 target = transform.position + direction;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, visionRange, LayerMask.GetMask("Obstacles"));

        if (hit.collider != null)
        {
            float offset = transform.localScale.magnitude * 0.5f;
            destination = hit.point - (Vector2)direction.normalized * offset;
        }
        else
        {
            destination = target;
        }

    }
    void SearchFood()
    {
        Bunny nearestBunny = FindNearestBunny();
        if (nearestBunny == null)
        {
            currentState = PredatorState.Exploring;
            return;
        }

        destination = nearestBunny.transform.position;

        if (Vector3.Distance(transform.position, nearestBunny.transform.position) < 0.2f)
        {
            currentState = PredatorState.Eating;
        }
    }

    void Eat()
    {
        Collider2D foodHit = Physics2D.OverlapCircle(transform.position, 0.2f, LayerMask.GetMask("Bunnies")); //Si esta en contacto con la comida
        if (foodHit != null)
        {
            Bunny food = foodHit.GetComponent<Bunny>();
            if (food != null)
            {
                energy += food.age;
                Destroy(food.gameObject);
            }
        }

        currentState = PredatorState.Exploring;
    }

    Bunny FindNearestBunny()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, visionRange, LayerMask.GetMask("Food"));

        Bunny nearestFood = null;
        float distance = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            Bunny food = hit.GetComponent<Bunny>();
            if (food != null)
            {
                float dist = Vector2.Distance(transform.position, food.transform.position);
                if (dist < distance)
                {
                    distance = dist;
                    nearestFood = food;
                }
            }
        }

        return nearestFood;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(destination, 0.2f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, destination);
    }
}
