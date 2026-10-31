using UnityEngine;

// Drops rocks from above. Sometimes drops a gold bar right next to a rock (risk vs reward).
public class Spawner : MonoBehaviour
{
    public float timeBetweenRocks = 0.8f;
    public float goldChance = 0.4f;     // 40% of rocks come with a gold bar

    float timer;

    void Update()
    {
        if (!GameManager.Instance.playing) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        // The higher the danger (Speed), the more often rocks appear
        timer = timeBetweenRocks / GameManager.Instance.Speed;

        // Drop a rock at a random spot above the floor
        Vector3 rockPosition = new Vector3(Random.Range(-5f, 5f), 12f, Random.Range(-3.5f, 3.5f));
        Spawn("Rock", PrimitiveType.Sphere, rockPosition, 1.2f, new Color(0.5f, 0.45f, 0.4f));

        // Sometimes drop gold right next to the rock, so grabbing it is risky
        if (Random.value < goldChance)
        {
            Vector2 direction = Random.insideUnitCircle.normalized * 1.4f;
            Vector3 goldPosition = rockPosition + new Vector3(direction.x, 0f, direction.y);
            goldPosition.x = Mathf.Clamp(goldPosition.x, -5.5f, 5.5f);
            goldPosition.z = Mathf.Clamp(goldPosition.z, -3.5f, 3.5f);
            Spawn("Gold", PrimitiveType.Cube, goldPosition, 0.5f, new Color(1f, 0.8f, 0.1f));
        }
    }

    void Spawn(string objectName, PrimitiveType type, Vector3 position, float size, Color color)
    {
        GameObject obj = GameManager.MakeShape(type, position, Vector3.one * size, color);
        obj.name = objectName;
        obj.GetComponent<Collider>().isTrigger = true;          // so the player can detect touching it
        obj.transform.SetParent(GameManager.Instance.transform); // so restart can remove it
        obj.AddComponent<Falling>();
    }
}
