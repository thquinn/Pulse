using UnityEngine;

public class BorderScript : MonoBehaviour
{
    public GameObject prefabWall;
    public Material materialPulse, materialParticlePulseDissolve, materialBackground;

    public Vector2 size;
    [HideInInspector] public Vector2 targetSize, initialSize;
    public float expand;
    Vector2 vSize;

    void Start() {
        targetSize = size;
        initialSize = size;
        Instantiate(prefabWall, transform).GetComponent<BorderWallScript>().Init(this, BorderWallSide.Right);
        Instantiate(prefabWall, transform).GetComponent<BorderWallScript>().Init(this, BorderWallSide.Left);
        Instantiate(prefabWall, transform).GetComponent<BorderWallScript>().Init(this, BorderWallSide.Top);
        Instantiate(prefabWall, transform).GetComponent<BorderWallScript>().Init(this, BorderWallSide.Bottom);
    }

    void Update() {
        size = Vector2.SmoothDamp(size, targetSize, ref vSize, .5f);
        Vector2 extents = new Vector2((size.x + expand) / 2, (size.y + expand) / 2);
        materialPulse.SetVector("_Extents", extents);
        materialParticlePulseDissolve.SetVector("_Extents", extents);
        materialBackground.SetVector("_Extents", extents);
    }
}
