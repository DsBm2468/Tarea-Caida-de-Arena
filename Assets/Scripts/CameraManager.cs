using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 10f; //Que tan rápido se mueve la camara cuando presionas las teclas
    //10f se refiere a que se mueve 10 unidades por segundo

    [Header("Zoom")]
    public float zoomSpeed = 5f;//Que tan rápido se acerca o aleja la camara cuando usas la rueda del mouse
    public float minZoom = 2f;
    public float maxZoom = 30f;

    private Camera cam; //Variable privada que guardará un referncia a la camara para usarla rapido
    private Vector2 moveInput = Vector2.zero; //Guarda la dirección en la que el jugador QUIERE mover la cámara.
    private float zoomInput = 0f;//Guarda cuánto zoom QUIERE hacer el jugador.

    void Start()
    {
        cam = Camera.main;
        InputManager.Instance.OnCameraMove += val => moveInput = val;
        InputManager.Instance.OnCameraZoom += val => zoomInput = val;
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    void HandleMovement()
    {
        Vector3 delta = new Vector3(moveInput.x, moveInput.y, 0f); //crea el vector de movimiento
        //Vector3 delta es una variable que guarda 3 números (x, y, z)
        //(moveInput.x, moveInput.y, 0f) es lo que el usuario movio en horizontal, vertical y en z siempre es 0
        cam.transform.position += delta * moveSpeed * Time.deltaTime;
    }

    void HandleZoom()
    {
        cam.orthographicSize -= zoomInput * zoomSpeed * Time.deltaTime; //orthographicSize controla el zoom, en valor pequeño MAS ZOOM, en valor grande MENOS ZOOM
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom); //Limita el zoom
    }
}

