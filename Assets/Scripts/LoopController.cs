using UnityEngine;

public class LoopController : MonoBehaviour
{
    public GameObject player;

    private float offsetX;

    public void Start()
    {
        this.offsetX = this.transform.position.x
            - this.player.transform.position.x;
    }

    public void Update()
    {
        var position = this.transform.position;
        position.x = this.player.transform.position.x + offsetX;
        this.transform.position = position;
    }
}
