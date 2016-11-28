using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UniBox2D_PlayerTileByTileMovement : MonoBehaviour {

    enum direction
    {
        R,
        L
    };

	private Rigidbody2D _rig;
    public Vector2 pos;
    public List<string> map;
    private Transform myTransform;

    public char[] freeChars;
    private direction dir;
    public float speed;


	void Start()
	{
        dir = direction.R;
		_rig = this.GetComponent<Rigidbody2D> ();
        myTransform = GetComponent<Transform>();
        string tmp = "E ";
        freeChars = tmp.ToCharArray();
		map = GameObject.Find("DungeonGenDisp").GetComponent<UniBox2D_DisplayGeneratedDungeon>().map;
	}

    void Update()
    {
        myTransform.position = Vector3.Lerp(myTransform.position, new Vector3(pos.x, pos.y, 0), speed);
    }

    public bool tileIsFree(int y, int x)
    {
        char[] tmp = map[x].ToCharArray();
        for (int i = 0; i < freeChars.Length;i++)
        {
            if (tmp[y] == freeChars[i])
                return true;
        }
            return false;
    }

    public void MoveUp()
    {
        if (tileIsFree((int)pos.x, (int)(pos.y + 1)))
            pos = new Vector2(pos.x, pos.y + 1);
    }

    public void MoveDown()
    {
        if (tileIsFree((int)pos.x, (int)(pos.y - 1)))
            pos = new Vector2(pos.x, pos.y - 1);
    }

    public void MoveRight()
    {
        if (tileIsFree((int)(pos.x + 1), (int)pos.y))
        {
            pos = new Vector2(pos.x + 1, pos.y);
            if (dir == direction.L)
            {
                dir = direction.R;
            }
        }
    }

    public void MoveLeft()
    {
        if (tileIsFree((int)(pos.x - 1), (int)pos.y))
        {
            pos = new Vector2(pos.x - 1, pos.y);
            if (dir == direction.R)
            {
                dir = direction.L;
            }
        }
    }
}
