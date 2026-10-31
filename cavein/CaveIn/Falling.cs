using UnityEngine;

// Makes a rock or gold bar fall, and shows a circle on the floor where it will land.
// The circle grows as the object gets closer, so the player can see how much time is left.
public class Falling : MonoBehaviour
{
    public float fallSpeed = 6f;

    GameObject marker;
    float startHeight;
    float maxCircleSize;

    void Start()
    {
        startHeight = transform.position.y;
        maxCircleSize = name == "Rock" ? 1.4f : 0.8f;

        // A flat circle on the floor under the object: red for rocks, yellow for gold
        Color markerColor = name == "Rock" ? Color.red : Color.yellow;
        Vector3 markerPosition = new Vector3(transform.position.x, 0.01f, transform.position.z);
        marker = GameManager.MakeShape(PrimitiveType.Cylinder, markerPosition, new Vector3(0.3f, 0.01f, 0.3f), markerColor);
        Destroy(marker.GetComponent<Collider>());
    }

    void Update()
    {
        if (!GameManager.Instance.playing) return;

        // Fall down. Speed grows with time and with the gold collected (greed mechanic).
        transform.position += Vector3.down * fallSpeed * GameManager.Instance.Speed * Time.deltaTime;

        // Grow the circle: 0 = just dropped, 1 = about to land
        float progress = 1f - (transform.position.y - 0.3f) / (startHeight - 0.3f);
        float size = Mathf.Lerp(0.3f, maxCircleSize, progress);
        marker.transform.localScale = new Vector3(size, 0.01f, size);

        // Remove it when it reaches the floor
        if (transform.position.y < 0.3f) Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (marker != null) Destroy(marker);
    }
}
