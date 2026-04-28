using UnityEngine;

public class ShowRoom : MonoBehaviour
{
    [SerializeField] private GameObject prefabToToggle;

    public GameObject GetContentParent()
    {
        return prefabToToggle;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (prefabToToggle != null)
            prefabToToggle.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (prefabToToggle != null)
            prefabToToggle.SetActive(false);
    }
}