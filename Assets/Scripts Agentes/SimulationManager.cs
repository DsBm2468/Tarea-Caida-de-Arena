using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public float secondsPerIteration = 1.0f;
    private float time = 0;
    
    void Start()
    {
        // Lista de conejos
        // Lista de zorros
        // Como aparece la comida
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
        // Por cada zorro, simule la logica del zorro
    }
}
