using UnityEngine;

public class RoomMaterialManager : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;

    [Header("Materials")]
    [SerializeField] private Material deadEndMaterial;
    [SerializeField] private Material straightVerticalMaterial;
    [SerializeField] private Material straightHorizontalMaterial;
    [SerializeField] private Material cornerUpRightMaterial;
    [SerializeField] private Material cornerUpLeftMaterial;
    [SerializeField] private Material cornerDownRightMaterial;
    [SerializeField] private Material cornerDownLeftMaterial;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>(); // OPTIMIZED: cache local renderer fallback once.
    }

    public void ApplyFromConnections(bool openLeft, bool openRight, bool openTop, bool openBottom)
    {
        if (targetRenderer == null)
            return;

        Material chosenMaterial = GetMaterial(openLeft, openRight, openTop, openBottom);

        if (chosenMaterial != null)
            targetRenderer.material = chosenMaterial;
    }

    private Material GetMaterial(bool left, bool right, bool top, bool bottom)
    {
        int count = (left ? 1 : 0) + (right ? 1 : 0) + (top ? 1 : 0) + (bottom ? 1 : 0);

        if (count == 1)
            return deadEndMaterial;

        if (top && bottom && !left && !right)
            return straightVerticalMaterial;

        if (left && right && !top && !bottom)
            return straightHorizontalMaterial;

        if (top && right && !left && !bottom)
            return cornerDownLeftMaterial;

        if (top && left && !right && !bottom)
            return cornerDownRightMaterial;

        if (bottom && right && !left && !top)
            return cornerUpLeftMaterial;

        if (bottom && left && !right && !top)
            return cornerUpRightMaterial;

        return deadEndMaterial;
    }
}
