using UnityEngine;

// Moves the player and reacts when it touches a rock or a gold bar.
public class Player : MonoBehaviour
{
    public float speed = 6f;
    public float safeTime = 1f;    // seconds you can't be hit again after a hit (fairness)

    Renderer body;
    Color normalColor;
    float flashTimer;              // how long the red/yellow flash lasts
    float safeTimer;               // counts down the safe time after a hit

    void Start()
    {
        body = GetComponent<Renderer>();
        normalColor = body.material.color;

        // A kinematic Rigidbody lets Unity tell us when we touch something (OnTriggerEnter),
        // without gravity or physics pushing the player around.
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Update()
    {
        // Feedback: go back to the normal colour after a flash
        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0f) body.material.color = normalColor;
        }

        // Feedback: blink while safe after a hit
        if (safeTimer > 0f)
        {
            safeTimer -= Time.deltaTime;
            body.enabled = safeTimer <= 0f || Mathf.Repeat(safeTimer, 0.2f) > 0.1f;
        }

        if (!GameManager.Instance.playing) return;

        // Move on the floor and stay inside it
        Vector3 move = GameManager.GetMoveInput();
        Vector3 newPosition = transform.position + move * speed * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, -5.5f, 5.5f);
        newPosition.z = Mathf.Clamp(newPosition.z, -3.5f, 3.5f);
        transform.position = newPosition;
    }

    // Unity calls this when the player touches a rock or a gold bar
    void OnTriggerEnter(Collider other)
    {
        if (!GameManager.Instance.playing) return;

        if (other.name == "Gold")
        {
            GameManager.Instance.CollectGold();
            Flash(Color.yellow);                 // feedback: you got gold
            Destroy(other.gameObject);
        }
        else if (other.name == "Rock" && safeTimer <= 0f)
        {
            GameManager.Instance.HitByRock();
            Flash(Color.red);                    // feedback: you got hit
            safeTimer = safeTime;                // can't be hit again for 1 second
            Destroy(other.gameObject);
        }
    }

    void Flash(Color color)
    {
        body.material.color = color;
        flashTimer = 0.2f;
    }

    // Called by the GameManager when a new round starts
    public void ResetPlayer()
    {
        transform.position = new Vector3(0f, 1f, 0f);
        safeTimer = 0f;
        flashTimer = 0f;
        body.enabled = true;
        body.material.color = normalColor;
    }
}
