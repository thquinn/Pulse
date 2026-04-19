using UnityEngine;

public class BorderScript : MonoBehaviour
{
    public GameObject prefabWall;
    public Material materialPulse;

    public Vector2 size;
    public float expand;

    void Start() {
        Vector2 inflated = size + new Vector2(expand, expand);
        // Right and left walls.
        Instantiate(prefabWall).GetComponent<BorderWallScript>().Init(new Vector2(inflated.x / 2, 0), Quaternion.identity, size.y + expand - .5f);
        Instantiate(prefabWall).GetComponent<BorderWallScript>().Init(new Vector2(-inflated.x / 2, 0), Quaternion.Euler(0, 0, 180), size.y + expand - .5f);
        // Top and bottom walls.
        Instantiate(prefabWall).GetComponent<BorderWallScript>().Init(new Vector2(0, inflated.y / 2), Quaternion.Euler(0, 0, 90), size.x + expand - .5f);
        Instantiate(prefabWall).GetComponent<BorderWallScript>().Init(new Vector2(0, -inflated.y / 2), Quaternion.Euler(0, 0, 270), size.x + expand - .5f);
    }

    void Update() {
        materialPulse.SetVector("_Extents", new Vector2((size.x + expand) / 2, (size.y + expand) / 2));
    }
}
