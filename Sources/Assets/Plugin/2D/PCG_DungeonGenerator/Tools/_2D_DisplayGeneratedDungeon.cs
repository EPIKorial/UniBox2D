using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DungeonGenerator;

public class _2D_DisplayGeneratedDungeon : MonoBehaviour 
{

    public int roomNbr;
    public int mapSizeX;
    public int mapSizeY;
    public int roomMaxSizeX;
    public int roomMaxSizeY;
    public int roomMinSizeX;
    public int roomMinSizeY;

    public List<string> map;
    public GameObject ground;
    public GameObject wall;
    public GameObject corner;
    public GameObject In;
    public GameObject Out;

    private GameObject _go;


	public void GenerateMap()
	{
		foreach (var gameObj in FindObjectsOfType(typeof(GameObject)) as GameObject[])
		{
			if(gameObj.name.Contains("(Clone)"))
			{
				Destroy (gameObj);
			}
		}
		map = DungeonGen.newMap(roomNbr, mapSizeY, mapSizeX, roomMinSizeY, roomMinSizeX, roomMaxSizeY, roomMaxSizeX);
		displayMap(map);

		GameObject.Find ("Camera").transform.position = new Vector3 (mapSizeX / 2 - mapSizeX / 2.8f, mapSizeY / 2 + mapSizeY / 10, -3);
	}
		
	void displayMap(List<string> map)
    {
        for (int i = 0; i < map.Count; i++)
        {
            char[] tmp = map[i].ToCharArray();
            for (int x = 0; x < tmp.Length; x++)
            {
                if (tmp[x] == ' ')
                   Instantiate(ground, new Vector3(x, i, 0), new Quaternion(0, 0, 0, 0));
                if (tmp[x] == '-' || tmp[x] == '|')
                    Instantiate(wall, new Vector3(x, i, 0), new Quaternion(0, 0, 0, 0));
                if (tmp[x] == '#' || tmp[x] == 'X')
                    Instantiate(corner, new Vector3(x, i, 0), new Quaternion(0, 0, 0, 0));
                if (tmp[x] == 'E')
                {
                    Instantiate(In, new Vector3(x, i, 0), new Quaternion(0, 0, 0, 0));
                    Instantiate(ground, new Vector3(x, i, 0), new Quaternion(0, 0, 0, 0));
                }
                if (tmp[x] == 'S')
                {
                    Instantiate(Out, new Vector3(x, i, 0), new Quaternion(0, 0, 0, 0));
                    Instantiate(ground, new Vector3(x, i, 0), new Quaternion(0, 0, 0, 0));
                }
            }
        }
    }
		
}
