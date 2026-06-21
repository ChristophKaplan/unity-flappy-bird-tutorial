using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Sprite[] sprites;
    public float strength = 5f;
    public float gravity = -9.81f;
    public float tilt = 5f;

    private SpriteRenderer spriteRenderer;
    private Vector3 direction;
    private int spriteIndex;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
    }

    private void OnEnable()
    {
        Vector3 position = transform.position;
        position.y = 0f;
        transform.position = position;
        direction = Vector3.zero;
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame) {
            direction = Vector3.up * strength;
        }

        // Apply gravity and update the position
        direction.y += gravity * Time.deltaTime;
        transform.position += direction * Time.deltaTime;

        // Neigung des Vogels basierend auf der Bewegungsrichtung:
        // eulerAngles beschreibt die Rotation als x/y/z-Winkel in Grad.
        // Wir lesen die aktuelle Rotation aus, ändern nur die z-Achse (Kippen links/rechts)
        // und schreiben sie zurück – x und y bleiben unberührt.
        // direction.y ist positiv beim Aufsteigen und negativ beim Fallen.
        // Multipliziert mit tilt ergibt das einen Neigungswinkel:
        //   Vogel steigt  → direction.y > 0 → rotation.z > 0 → Nase zeigt nach oben
        //   Vogel fällt   → direction.y < 0 → rotation.z < 0 → Nase zeigt nach unten
        Vector3 rotation = transform.eulerAngles;
        rotation.z = direction.y * tilt;
        transform.eulerAngles = rotation;
    }

    private void AnimateSprite()
    {
        if (sprites.Length == 0) return;

        // Der Modulo-Operator (%) gibt den Rest einer ganzzahligen Division zurück.
        // Beispiel mit 3 Sprites (Index 0, 1, 2):
        //   (0+1) % 3 = 1
        //   (1+1) % 3 = 2
        //   (2+1) % 3 = 0  ← springt automatisch zurück auf 0
        // So wird ein Index-out-of-bounds-Fehler vermieden und die Animation läuft endlos.
        spriteIndex = (spriteIndex + 1) % sprites.Length;
        spriteRenderer.sprite = sprites[spriteIndex];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle")) {
            GameManager.Instance.GameOver();
        } else if (other.gameObject.CompareTag("Scoring")) {
            GameManager.Instance.IncreaseScore();
        }
    }
}
