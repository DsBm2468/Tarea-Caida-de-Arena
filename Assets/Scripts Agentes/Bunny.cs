using UnityEditor;
using UnityEngine;

public class Bunny : MonoBehaviour
{
    [Header("Bunny Settings")]
    public float energy = 10f;
    public float age = 0f;
    public float maxAge = 20f;
    public float speed = 1f;
    public float visionRange = 5f;

    [Header("Bunny States")]
    public bool isAlive = true;
    public BunnyState currentState = BunnyState.Exploring;

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

        EvaluateState();

        switch (currentState)
        {
            case BunnyState.Exploring:
                Explore();
                break;
            case BunnyState.SearchingFood:
                SearchFood();
                break;
            case BunnyState.Eating:
                Eat();
                break;
            case BunnyState.Fleeing:
                Flee();
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

    void SearchFood()
    {
        Food nearestFood = FindNearestFood();
        if (nearestFood != null)
        {
            currentState = BunnyState.Exploring;
            return;
        }

        destination = nearestFood.transform.position;
        
        if (Vector3.Distance(transform.position, nearestFood.transform.position) < 0.2f)
        {
            currentState = BunnyState.Eating;
        }
    }

    void Eat()
    {
        Collider2D foodHit = Physics2D.OverlapCircle(transform.position, 0.2f, LayerMask.GetMask("Food")); //Si esta en contacto con la comida
        if (foodHit != null)
        {
            Food food = foodHit.GetComponent<Food>();
            if (food != null)
            {
                energy += food.nutrition;
                Destroy(food.gameObject);
            }
        }

        currentState = BunnyState.Exploring;
    }

    Food FindNearestFood()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, visionRange, LayerMask.GetMask("Food"));

        Food nearestFood = null;
        float distance = Mathf.Infinity;

        foreach (Collider2D hit in hits) 
        {
            Food food = hit.GetComponent<Food>();
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
}
