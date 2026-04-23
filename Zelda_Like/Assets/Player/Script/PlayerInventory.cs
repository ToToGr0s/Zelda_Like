using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public bool hasKey;

    public void AddKey()
    {
        hasKey = true;
    }

    public void RemoveKey()
    {
        hasKey = false;
    }

    public bool HasKey()
    {
        return hasKey;
    }

    public bool UseKey()
    {
        if (!hasKey)
            return false;

        hasKey = false;
        return true;
    }
}