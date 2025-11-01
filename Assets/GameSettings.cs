using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    public int mapSize = 20;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMapSize(int size)
    {
        mapSize = size;
        Debug.Log("Map size set to: " + size + "x" + size);
    }

    public int GetMapSize()
    {
        return mapSize;
    }
}
