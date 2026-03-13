using UnityEngine;
using UnityEngine.InputSystem;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class GameManager : MonoBehaviour
{
    [Header("Grid Settings")] //Son letreros en Unity para organizar variables, no hace nada en el código
    public int width = 50;
    public int height = 30;
    public float updateTime = 0.1f; //cada 0.1 segundos avanza
    public GameObject cellPrefab;

    private bool[,] grid; //Estado actual (vivo o muerto)
    //bool[,]grid es una hoja de calculo deverdadero o falso.
    // grid[0, 0] = true        (viva) grid[0, 1] = false(muerta)
    // grid[1, 0] = false       (muerta) grid[1, 1] = true(viva)
    private bool[,] nextGrid; //Estado después
    private GameObject[,] cellObjects; //Granos de arena visibles
    //GameObject[,] cellObjects; guarda cada grano de arena para poder cambiarles el color después
    private float timer; //Reloj interno
    private bool isPaused = false; //Esta pausado?

    void Start()
    {
        grid = new bool[width, height]; //crea la matriz vacía
        nextGrid = new bool[width, height]; //crea la matriz futura
        cellObjects = new GameObject[width, height]; //Crea la matriz de objeto

        //InputManager.Instance.mmm += hace que cuando alguien presione el boton (mmm), llama a la acción correspondiente
        InputManager.Instance.OnPause += TogglePause;
        InputManager.Instance.OnRestart += RestartSimulation;
        InputManager.Instance.OnClear += ClearSimulation;
        InputManager.Instance.OnToggleCell += ToggleCellInput;

        //Tecla para el ManualStep
        InputManager.Instance.OnManualStep += ManualStep;

        GenerateGrid(); //contruye los granos de arena
        Debug.Log("SIMULACIÓN DE ARENA INICIADA");
    }

    void Update()
    {
        if (isPaused) return; //Si está pausado, no hace nada

        timer += Time.deltaTime; //Time.deltaTime es el tiempo que pasó entre el frame anterior y este frame
        if (timer >= updateTime) //Si el tiempo es mayor o igual al updateTime(0,1. lo configuré al inicio), entonces ...
        {
            Step(); //Calcula la siguiente generación de granos de arena
            UpdateVisuals(); //cambia los colores
            timer = 0f; //Reinicia el reloj
        }
    }

    void TogglePause() //TogglePause() permite cambiar el estado de pausa
    {
        isPaused = !isPaused; //Invierte el estado del objeto
        Debug.Log(isPaused ? "Simulación pausada" : "Simulación reanudada");
    }

    void ToggleCellInput() //Toggle significa cambiar entre 2 estados
    {
        //Al mantener presionado el mouse se crean granos de arena de forma continua, entoces se ejecuta este void
        HandleMouseClick();
    }


    void ClearSimulation()
    {
        Debug.Log("Limpiando simulación...");
        ClearGrid(); //Llama al metodo para borrar todas los granos de arena
        timer = 0f;
    }

    void RestartSimulation()
    {
        Debug.Log("Reiniciando simulación...");
        ClearGrid();
        timer = 0f;
    }

    void GenerateGrid() //Solo se llama una vez, al inicio del juego (en Start)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject cell = Instantiate(cellPrefab, new Vector3(x, y, 0), Quaternion.identity); //guarda la celda en una vafiable llamada cell, crea una copia de ese modelo en la posicion que le diga y sin rotación
                cell.transform.parent = transform; //Hace que la celda sea hija del gamemanager, esto permite mehor orden
                cellObjects[x, y] = cell; //Guardar referencia
            }
        }
    }

    public void ClearGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = false;
            }
        }
        UpdateVisuals();
    }

    void ManualStep()
    {
        Debug.Log("Progresión manual de la simulación ejecutada");

        Step(); //Calcula el siguiente estado
        UpdateVisuals(); //Actualiza la pantalla
    }

    void Step() //Este metodo se llama cada vez que se quiere avanzar a la siguiente generacion
    {
        //Vaciar el tablero
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                nextGrid[x, y] = false; //Recorre todas las celdas del tablero y las deja vacias
            }
        }

        int movedSand = 0; //Cuenta cuantos granos de arena se movieron en este paso
        int staticSand = 0; //Cuenta cuantos granos de arena se quedaron quietos

        //Recorrer el tablero de arriba a abajo
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y]) //Si hay arena en la posición actual, entonces...
                {
                    //REGLA 1.Puede bajar directamente (la celda debajo está vacia), entonces...
                    if (y > 0 && !grid[x, y - 1]) //y=0 es el suelo, !grid[x,y - 1] es para saber si el espacio de abajo está vacio actualmente
                    {
                        nextGrid[x, y - 1] = true; //El grano de arena se pondrá abajo
                        movedSand++;
                    }
                    else //REGLA 2. Si la celda está ocupada, la arena intenta moverse diagonalmente hacia abajo 
                    {
                        //verifica las diagonales para ver si alguna está libre
                        bool leftCellFree = (x > 0 && y > 0 && !grid[x - 1, y - 1]); //LA DIAGONAL IZQUIERDA ESTA LIBRE SI: No esta en el borde izq, no está en el piso y la celda (x-1, y-1) está vacia
                        bool rightCellFree = (x < width - 1 && y > 0 && !grid[x + 1, y - 1]); //LA DIAGONAL DERECHA ESTA LIBRE SI: No esta en el borde der, no está en el piso y la celda (x-1, y-1) está vacia

                        //REGLA 3. Movimiento diagonal

                        //3.1. Si en ambos lados están libres, escoger una dirección al azar
                        if (leftCellFree && rightCellFree)
                        {
                            if (Random.value < 0.5f) //Si el número aleatorio es menor que 0,5 se va a la izq, si no, entonces a la der
                            {
                                nextGrid[x - 1, y - 1] = true;
                            }
                            else
                            {
                                nextGrid[x + 1, y - 1] = true;
                            }
                            movedSand++;
                        }

                        //3.2. Si solo el lado izquierdo está libre, se moverá el grano de arena a la izquierda
                        else if (leftCellFree)
                        {
                            nextGrid[x - 1, y - 1] = true;
                            movedSand++;
                        }

                        //3.3. Si solo el lado derecho está libre, se moverá el grano de arena a la derecha
                        else if (rightCellFree)
                        {
                            nextGrid[x + 1, y - 1] = true;
                            movedSand++;
                        }

                        //REGLA 4. Si las 3 opciones están ocupadas, el grano de arena permanece en su lugar
                        else
                        {
                            nextGrid[x, y] = true;
                            staticSand++;
                        }
                    }
                }
            }
        }

        // Swap grids
        var temp = grid;
        grid = nextGrid;
        nextGrid = temp;

        //Estádísticas en consola (Contador de granos de arena)
        int total = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y]) //Si hay arena en esta celda
                {
                    total++;
                }
            }
        }
        Debug.Log($"EVOLUCIÓN DEL SISTEMA DE ARENA: {movedSand} movidas, {staticSand} quietas, Granos de arena en total: {total}");
    }

    void HandleMouseClick() //Click del mouse
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()); //Convertir clic a posición mundial,Camera.main.ScreenToWorldPoint(...)convierte los píxeles a coordenadas del mundo de Unity.
        int x = Mathf.RoundToInt(worldPos.x);
        int y = Mathf.RoundToInt(worldPos.y);

        if (x < 0 || x >= width || y < 0 || y >= height) //Si haces click fuera del tablero, no hace nada
            return;

        grid[x, y] = true; //Crear arena
        UpdateVisuals();
    }


    //Actualizar
    void UpdateVisuals()
    {
        //Recorrer todas las celdas
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Obtener el componente que pinta
                var rend = cellObjects[x, y].GetComponent<SpriteRenderer>(); //.GetComponent<SpriteRenderer>() Da el componente que sabe pintar sprites
                //rend.color = grid[x, y] ? Color.black : Color.white; //Forma corta de escribir un if/else
                rend.color = grid[x, y] ? new Color(0.902f, 0.725f, 0.439f) : Color.white; //Forma corta de escribir un if/else
                //Si se cumple la condicion (grid[x. y] esta viva, el color es ..., si no es blanco)
            }
        }
    }
}
