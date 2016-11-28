using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;

public class UniBox2D_LevelDrawer : EditorWindow {

    // SECTIONS //
    // Tiles size section
    // Map size section
    // Level Drawer Section
    // Sprite Selection Section
    // Spacing Section
    // Generate Map Button
    // Clear Map Button
    // Clear datas

    Vector3 tile_size = new Vector3(0, 0, 0);
    Vector3 tile_pos = new Vector3(0, 0, 0);
    Sprite _sprite;
    Sprite drawing_sprite;
	float tile_width = 1;
	float tile_height = 1;
	float map_width = 10;
	float map_height = 10;

    float slider_value_x = 0;
    float slider_value_y = 0;
    float tiles_spacing_x = 0;
    float tiles_spacing_y = 0;

    bool brushing = false;
    bool erasing = false;
    bool coloring = false;


	[MenuItem ("Window/LevelDrawer")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		UniBox2D_LevelDrawer window = (UniBox2D_LevelDrawer)EditorWindow.GetWindow (typeof (UniBox2D_LevelDrawer));
		window.Show();
	}

	void OnGUI()
    {
        #region Map Generation
        GUILayout.Label("Map Settings", EditorStyles.boldLabel);

        // Sprite Selection Section
        _sprite = EditorGUI.ObjectField(new Rect(50, 30, 150, 150), _sprite, typeof(Sprite), true) as Sprite;

		// Tiles size section
        GUI.Box(new Rect(new Vector2(10, 200), new Vector2(220, 105)), "Tiles size");

        GUI.Box(new Rect(new Vector2(20, 230), new Vector2(200, 30)), "");
        GUI.Label(new Rect(30, 235, 20, 20), "X");
		tile_width = EditorGUI.Slider(new Rect(60,235,150,20),tile_width,0.1f, 10f);

        GUI.Box(new Rect(new Vector2(20, 265), new Vector2(200, 30)), "");
        GUI.Label(new Rect(30, 270, 20, 20), "Y");
		tile_height = EditorGUI.Slider(new Rect(60,270,150,20),tile_height,0.1f, 10f);

        // Map size section
        GUI.Box(new Rect(new Vector2(10, 310), new Vector2(220, 105)), "Map size");

        GUI.Box(new Rect(new Vector2(20, 340), new Vector2(200, 30)), "");
        GUI.Label(new Rect(30, 345, 20, 20), "X");
		map_width = EditorGUI.Slider(new Rect(60,345,150,20),map_width,1, 100);

        GUI.Box(new Rect(new Vector2(20, 375), new Vector2(200, 30)), "");
        GUI.Label(new Rect(30, 380, 20, 20), "Y");
		map_height = EditorGUI.Slider(new Rect(60,380,150,20),map_height,1, 100);

        // Spacing Section
        GUI.Box(new Rect(new Vector2(10, 420), new Vector2(220, 105)), "Spacing");

        GUI.Box(new Rect(20, 445, 200, 30), "");
        GUI.Box(new Rect(20, 480, 200, 30), "");
        GUI.Label(new Rect(30, 450, 150, 20), "X");
        slider_value_x = EditorGUI.Slider(new Rect(60, 450, 150, 20), slider_value_x, -1f, 1f);
        
        GUI.Label(new Rect(30, 485, 150, 20), "Y");
        slider_value_y = EditorGUI.Slider(new Rect(60, 485, 150, 20), slider_value_y, -1f, 1f);


		// Generate Map Button
		if (GUI.Button(new Rect(20, 540, 200, 25), "Generate map grid"))
            Generate_map();

        // Clear Map Button
        if (GUI.Button(new Rect(20, 570, 200, 25), "Clear map grid"))
            Clear_map();

        // Clear datas
        if (GUI.Button(new Rect(40, 600, 160, 25), "Clear datas"))
            Clear_datas();
        #endregion

        #region Level Drawer
        // Level Drawer Section
        GUI.Box(new Rect(new Vector2(250, 30), new Vector2(Screen.width - 260, Screen.height - 70)), "Level Drawer");

        drawing_sprite = EditorGUI.ObjectField(new Rect(Screen.width - 200, 60, 150, 150), drawing_sprite, typeof(Sprite), true) as Sprite;

        if (GUI.Button(new Rect(Screen.width - 200, 220, 50, 50), AssetDatabase.LoadAssetAtPath("Assets/Plugins/2D_LevelDrawerCS/Resources/brush.gif", typeof(Texture2D)) as Texture2D))
        {
            brushing = true;
            coloring = false;
            erasing = false;
        }
        if (GUI.Button(new Rect(Screen.width - 148, 220, 50, 50), AssetDatabase.LoadAssetAtPath("Assets/Plugins/2D_LevelDrawerCS/Resources/eraser.gif", typeof(Texture2D)) as Texture2D))
        {
            brushing = false;
            coloring = false;
            erasing = true;
        }
        if (GUI.Button(new Rect(Screen.width - 96, 220, 50, 50), AssetDatabase.LoadAssetAtPath("Assets/Plugins/2D_LevelDrawerCS/Resources/coloring.gif", typeof(Texture2D)) as Texture2D))
        {
            brushing = false;
            coloring = true;
            erasing = false;
        }
        #endregion

    }

    void Update()
    {
        Manage_spacing();
        Manage_drawing();
    }
    
    void Generate_map()
    {
        GameObject go_tmp = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Plugins/2D_LevelDrawerCS/Resources/LD-Sprite.prefab", typeof(GameObject)) as GameObject, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        go_tmp.transform.localScale = new Vector3(tile_width, tile_height, 1);
        if (_sprite)
            go_tmp.GetComponent<SpriteRenderer>().sprite = _sprite;
        tile_size = go_tmp.GetComponent<SpriteRenderer>().bounds.size;
        DestroyImmediate(go_tmp.gameObject);
          
        if (GameObject.FindGameObjectWithTag("LD"))
            return;
        for (int y = 0; y < map_height; y++)
        {
            for (int x = 0; x < map_width; x++)
            {
                tile_pos = new Vector3(x * tile_size.x + (x * tiles_spacing_x), y * tile_size.y + (y * tiles_spacing_y), 0);
                GameObject go = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Plugins/2D_LevelDrawerCS/Resources/LD-Sprite.prefab", typeof(GameObject)) as GameObject, new Vector3(tile_pos.x, tile_pos.y, 0), new Quaternion(0, 0, 0, 0));
                go.transform.localScale = new Vector3(tile_width, tile_height, 1);
                if (_sprite)
                    go.GetComponent<SpriteRenderer>().sprite = _sprite;
            }
        }
     }

    void Clear_map()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("LD");
        if (gos.Length == 0)
            return;
        foreach (GameObject go in gos)
        {
            DestroyImmediate(go.gameObject);
        }
    }

    void Clear_datas()
    {
        _sprite = null;
        drawing_sprite = null;
        tile_width = 1;
        tile_height = 1;
        map_width = 10;
        map_height = 10;
        slider_value_x = 0;
        slider_value_y = 0;
        tiles_spacing_x = 0;
        tiles_spacing_y = 0;
        coloring = false;
        brushing = false;
        erasing = false;
    }

    void Manage_spacing()
    {
        if (!GameObject.FindGameObjectWithTag("LD"))
            return;
        if (slider_value_x != tiles_spacing_x)
        {
            tiles_spacing_x = slider_value_x;
            Clear_map();
            Generate_map();
        }
        if (slider_value_y != tiles_spacing_y)
        {
            tiles_spacing_y = slider_value_y;
            Clear_map();
            Generate_map();
        }
    }

    void Manage_drawing()
    {
        if (brushing)
        {
            Debug.Log("Brushing");
            // Not working at all !!!
           // Cursor.SetCursor(Resources.Load("Assets/Plugins/2D_LevelDrawerCS/Resources/brush.gif") as Texture2D, Vector2.zero, CursorMode.Auto);

            // Not working at all !!!
           /* Ray ray = Camera.main.ScreenPointToRay(Event.current.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                Debug.Log(Event.current.mousePosition);
                Vector3 newTilePosition = hit.point;
            }
            else
                Debug.Log("peau de zob");*/
        }
        if (erasing)
        {
            Debug.Log("Erasing");
        }
        if (coloring)
        {
            Debug.Log("Coloring");
        }
    }
}
