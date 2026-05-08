using UnityEngine;

public class ShowRoom : MonoBehaviour
{
    private const string PlayerTag = "Player";

    [SerializeField] private GameObject prefabToToggle;

    public GameObject GetContentParent()
    {
        return prefabToToggle;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(PlayerTag))
            return;

        if (prefabToToggle != null)
            prefabToToggle.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(PlayerTag))
            return;

        if (prefabToToggle != null)
            prefabToToggle.SetActive(false);
    }
}
