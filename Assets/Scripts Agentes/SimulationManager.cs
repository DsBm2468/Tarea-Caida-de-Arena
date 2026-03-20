using UnityEngine;
using System.Collections.Generic;

public class SimulationManager : MonoBehaviour
{
    public float secondsPerIteration = 1.0f;
    private float time = 0;

    private List<Bunny> bunnies = new List<Bunny>();
    private List<Predator> foxes = new List<Predator>();

    void Start()
    {
        // To do: Lista de conejos
        Bunny[] foundBunnies = FindObjectsByType<Bunny>(FindObjectsSortMode.InstanceID);
        bunnies = new List<Bunny>(foundBunnies);

        // To do: Lista de zorros
        Predator[] foundFoxes = FindObjectsByType<Predator>(FindObjectsSortMode.InstanceID);
        foxes = new List<Predator>(foundFoxes);
        // To do: Como aparece la comida
    }

    void Update()
    {
        time += Time.deltaTime;

        if (time > secondsPerIteration)
        {
            time = 0;
            Simulate();
        }
    }

    void Simulate()
    {
        // Por cada conejo, simule la logica del conejo
        foreach (Bunny b in bunnies) if (b != null && b.isAlive) b.Simulate(secondsPerIteration);
        foreach (Predator f in foxes) if (f != null && f.isAlive) f.Simulate(secondsPerIteration);

        // Por cada zorro, simule la logica del zorro
    }
}
