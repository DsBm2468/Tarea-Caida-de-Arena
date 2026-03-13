using System; //Permite usar eventos
using UnityEngine; //Necesario para cualquier script de Unity para que funcione

public class InputManager : MonoBehaviour //Es el sistema que detecta cuando presionas teclas o mueves el mouse
{
    public static InputManager Instance { get; private set; } //public static InputManager Instance crea un "Singleton"
    //Los singletons son clases que se pueden instanciar una sola vez y a las que se puede acceder globalmente
    
    private PlayerController controls; //Mapa de las geclas

    //event action son la forma en la qeu otros scripts pueden sincronizarse para saber cuando algo pasa

    // Eventos de cámara
    public event Action<Vector2> OnCameraMove;
    public event Action<float> OnCameraZoom;

    // Eventos de gameplay
    public event Action OnPause;
    public event Action OnRestart;
    public event Action OnClear;
    public event Action OnToggleCell;
    public event Action OnManualStep; //Paso manual

    void Awake() //Awake() se ejecuta inmediatamente cuando el objeto se crea (antes que Start)
    {
        //Instance es el objeto real con características propias
        if (Instance != null && Instance != this) //SI existe otro InputManager y ese no es este mismo script
        {
            Destroy(gameObject);//Se elimina este objeto duplicado
            return;
        }

        Instance = this; //Guarda este objeto como la instancia única, el objeto oficial
        DontDestroyOnLoad(gameObject); //DontDestroyOnLoad() hace que el objeto sobreviva incluso si se cambia de escena

        controls = new PlayerController(); //Crea el objeto que maneja los inputs (la libreta de teclas, estará vacia al inicio)

        // Cámara
        controls.Camera.Move.performed += ctx => OnCameraMove?.Invoke(ctx.ReadValue<Vector2>());
        //controls.Camera.Move es la ubicacion del boton, el .performed indica cuando presionas la tecla
        //+= añade esta accion a la lista de cosas que hacer
        //ctx => (ctx es "Context") toma la información sobre el input, se transforma en ...
        //?.Invoke() avisa a quienes estén escuchando este evento
        //ctx.ReadValue<Vector2>() lee el valor del movimiento (x, y en este caso)
        controls.Camera.Move.canceled += ctx => OnCameraMove?.Invoke(Vector2.zero); //.canceled es cuando dejas de mover/presionar el boton, Vector2.zero envia (0,0) para indicar que no te mueves
        controls.Camera.Zoom.performed += ctx => OnCameraZoom?.Invoke(ctx.ReadValue<float>()); //ReadValue<float>() lee el zoom como numero positivo o negativo
        controls.Camera.Zoom.canceled += ctx => OnCameraZoom?.Invoke(0);

        // Gameplay
        controls.Gameplay.Pause.performed += _ => OnPause?.Invoke();
        //_ significa que no importan los demás detalles, solo lo hace
        controls.Gameplay.Restart.performed += _ => OnRestart?.Invoke();
        controls.Gameplay.Clear.performed += _ => OnClear?.Invoke();
        controls.Gameplay.ToggleCell.performed += _ => OnToggleCell?.Invoke();
        controls.Gameplay.ManualStep.performed += _ => OnManualStep?.Invoke();
    }
    void OnEnable() //OnEnable() es cuando un objeto se activa (o el juego empieza), empieza a escuchar teclas
    {
        controls.Camera.Enable();
        controls.Gameplay.Enable();
    }
    void OnDisable() //OnDisable() es cuando el objeto se desactiva, es decir que deja de escuchar
    {
        controls.Camera.Disable();
        controls.Gameplay.Disable();
    }
}