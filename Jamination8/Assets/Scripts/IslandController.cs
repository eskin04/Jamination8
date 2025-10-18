using UnityEngine;

public class IslandController : MonoBehaviour
{
    private bool isActive = true;

    public bool IsActive()
    {
        return isActive;
    }

    public void SetIsActive(bool active)
    {
        isActive = active;
    }
}
