using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public static MapLoader instance;
    public GameObject[] maps;
    private LevelMap currentMap;
    private int index = 0;
    
    // Unloads current map and loads next
    public void NextMap()
    {
        currentMap.UnloadMap();
        index = (index + 1) % maps.Length;
        LoadMap();
    }

    void LoadMap()
    {
        currentMap = Instantiate(maps[index]).GetComponent<LevelMap>();
    }

    void Awake()
    {
        instance = this;
        LoadMap();
    }
}
