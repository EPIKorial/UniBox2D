using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DungeonGenerator
{
    public class DungeonGen : MonoBehaviour
    {
            static public List<string> newMap(int RoomNumber, int MapHeight, int MapWidth, int RoomMinHeight, int RoomMinWidth, int RoomMaxHeight, int RoomMaxWidth)
            {
                Map map;
                List<string> mapT;

                map = new Map(RoomNumber, new Size(MapWidth, MapHeight), new Size(RoomMaxWidth, RoomMaxHeight), new Size(RoomMinWidth, RoomMinHeight));
                map.generate_map();
                mapT = map._map;
                return mapT;
            }
    }

    public class Map
    {
        public int room_nbr;
        public Size map_size;
        public Size room_max_size;
        public Size room_min_size;
        public List<Leaf> _leafs;
        public List<Rooms> _rooms;
        public char[] tmpCharArray;
        public List<int> intArray;

        public List<string> _map;

        public Map(int nbr, Size mapSize, Size roomMax, Size roomMin)
        {
            room_nbr = nbr;
            map_size = mapSize;
            room_max_size = roomMax;
            room_min_size = roomMin;
            _leafs = new List<Leaf>();
            _rooms = new List<Rooms>();
            _map = new List<string>();
            intArray = new List<int>();
        }
        ~Map() { }


        public int get_room_nbr()
        {
            return room_nbr;
        }

        public int get_room_max_x()
        {
            return room_max_size.x;
        }

        public int get_room_max_y()
        {
            return room_max_size.y;
        }

        public Size get_map_size()
        {
            return map_size;
        }

        public void generate_map()
        {
            Leaf _root = new Leaf(new Size(0, 0), new Size(map_size.x, map_size.y));
            List<Rooms> tmpRooms = new List<Rooms>();

            _leafs.Add(_root);
            split();
            for (int i = 0; i < _leafs.Count; i++)
                _leafs[i].create_rooms(tmpRooms, room_max_size);
            for (int i = 0; i < room_nbr; i++)
                _rooms.Add(tmpRooms[getNewInt(tmpRooms)]);
            for (int i = 0; i < _leafs.Count; i++)
            {
                _leafs[i].leftChild = null;
                _leafs[i].rightChild = null;
            }
            blank_map();
            draw_rooms();
            draw_better_walls();
            draw_hallways();
            draw_hallways_walls();
            draw_angles();
            draw_in_out();
            //// On Windows
           /* string fileName = Application.dataPath + "\\map.txt";
            if (File.Exists(fileName))
                File.Delete(fileName);
            var sr = File.CreateText(fileName);
            for (int i = 0; i < _map.Count; i++)
                sr.WriteLine(_map[i]);
            sr.Close();*/
        }

        public int getNewInt(List<Rooms> tmpRooms)
        {
            int tmp = (int)Random.Range(0, tmpRooms.Count);
            while (isInList(tmp))
                tmp = (int)Random.Range(0, tmpRooms.Count);
            intArray.Add(tmp);
            return tmp;
        }

        public bool isInList(int nbr)
        {
            for (int i = 0; i < intArray.Count;i++ )
            {
                if (intArray[i] == nbr)
                    return true;
            }
                return false;
        }

        public void split()
        {
            bool split = true;

            while (split)
            {
                split = false;
                for (int i = 0; i < _leafs.Count; i++)
                {
                    if ((_leafs[i].leftChild == null && _leafs[i].rightChild == null)
                        && (_leafs[i]._size.x > room_max_size.x || _leafs[i]._size.y > room_max_size.y)
                        && (_leafs[i].split(room_min_size)))
                    {
                        _leafs.Add(_leafs[i].leftChild);
                        _leafs.Add(_leafs[i].rightChild);
                        split = true;
                    }
                }
            }
        }

        public void blank_map()
        {
            string tmp = "";
            for (int x = 0; x < map_size.x + 4; x++)
                tmp += ".";
            for (int i = 0; i < map_size.y + 4; i++)
                _map.Add(tmp);
        }

        public void draw_rooms()
        {
            int x = 0;
            int y = 0;

            for (int i = 0; i < _rooms.Count; i++)
            {
                x = _rooms[i]._size.x + _rooms[i]._pos.x + 2;
                y = _rooms[i]._size.y + _rooms[i]._pos.y + 2;
                for (int p = _rooms[i]._pos.y + 1; p <= y; p++)
                {
                    for (int o = _rooms[i]._pos.x + 1; o <= x; o++)
                    {
                        if (o == x || o == _rooms[i]._pos.x + 1)
                        {
                            tmpCharArray = _map[p].ToCharArray();
                            if (tmpCharArray[o] == '.')
                                tmpCharArray[o] = '#';
                            _map[p] = new string(tmpCharArray);
                        }
                        else if (p == y || p == _rooms[i]._pos.y + 1)
                        {
                            tmpCharArray = _map[p].ToCharArray();
                            if (tmpCharArray[o] == '.')
                                tmpCharArray[o] = '#';
                            _map[p] = new string(tmpCharArray);
                        }
                        else
                        {
                            tmpCharArray = _map[p].ToCharArray();
                            tmpCharArray[o] = ' ';
                            _map[p] = new string(tmpCharArray);
                        }
                    }
                }
            }
        }

        public void draw_hallways()
        {
            Size r1 = new Size();
            Size r2 = new Size();
            int w = 0;
            int h = 0;

            for (int i = 0; i < _rooms.Count-1; i++)
            {
                r1.y = (_rooms[i]._size.y / 2) + _rooms[i]._pos.y + 2;
                r1.x = (_rooms[i]._size.x / 2) + _rooms[i]._pos.x + 2;
                r2.y = (_rooms[i + 1]._size.y / 2) + _rooms[i + 1]._pos.y + 2;
                r2.x = (_rooms[i + 1]._size.x / 2) + _rooms[i + 1]._pos.x + 2;

                w = r2.x - r1.x;
                h = r2.y - r1.y;
                if (w > 0 && h > 0)
                {
                    for (int x = 0; x < w; x++)
                    {
                        tmpCharArray = _map[r1.y].ToCharArray();
                        tmpCharArray[r1.x + x] = ' ';
                        _map[r1.y] = new string(tmpCharArray);
                    }
                    for (int y = h; y > 0; y--)
                    {
                        tmpCharArray = _map[r2.y - y].ToCharArray();
                        tmpCharArray[r2.x] = ' ';
                        _map[r2.y - y] = new string(tmpCharArray);
                    }
                }
                else if (w > 0 && h < 0)
                {
                    for (int x = 0; x < w; x++)
                    {
                        tmpCharArray = _map[r1.y].ToCharArray();
                        tmpCharArray[r1.x + x] = ' ';
                        _map[r1.y] = new string(tmpCharArray);
                    }
                    for (int y = (h * -1); y > 0; y--)
                    {
                        tmpCharArray = _map[r2.y + y].ToCharArray();
                        tmpCharArray[r2.x] = ' ';
                        _map[r2.y + y] = new string(tmpCharArray);
                    }
                }
                else if (w < 0 && h > 0)
                {
                    for (int x = 0; x < (w * -1); x++)
                    {
                        tmpCharArray = _map[r1.y].ToCharArray();
                        tmpCharArray[r1.x - x] = ' ';
                        _map[r1.y] = new string(tmpCharArray);
                    }
                    for (int y = h; y > 0; y--)
                    {
                        tmpCharArray = _map[r2.y - y].ToCharArray();
                        tmpCharArray[r2.x] = ' ';
                        _map[r2.y - y] = new string(tmpCharArray);
                    }
                }
                else
                {
                    for (int x = 0; x < (w * -1); x++)
                    {
                        tmpCharArray = _map[r1.y].ToCharArray();
                        tmpCharArray[r1.x - x] = ' ';
                        _map[r1.y] = new string(tmpCharArray);
                    }
                    for (int y = (h * -1); y > 0; y--)
                    {
                        tmpCharArray = _map[r2.y + y].ToCharArray();
                        tmpCharArray[r2.x] = ' ';
                        _map[r2.y + y] = new string(tmpCharArray);
                    }
                }
            }
        }

        public void draw_in_out()
        {
            tmpCharArray = _map[(_rooms[0]._size.y / 2) + _rooms[0]._pos.y + 2].ToCharArray();
            tmpCharArray[(_rooms[0]._size.x / 2) + _rooms[0]._pos.x + 2] = 'E';
            _map[(_rooms[0]._size.y / 2) + _rooms[0]._pos.y + 2] = new string(tmpCharArray);

            tmpCharArray = _map[(_rooms[_rooms.Count - 1]._size.y / 2) + _rooms[_rooms.Count - 1]._pos.y + 2].ToCharArray();
            tmpCharArray[(_rooms[_rooms.Count - 1]._size.x / 2) + _rooms[_rooms.Count - 1]._pos.x + 2] = 'S';
            _map[(_rooms[_rooms.Count - 1]._size.y / 2) + _rooms[_rooms.Count - 1]._pos.y + 2] = new string(tmpCharArray);
        }

        public void draw_hallways_walls()
        {
            for (int i = 1; i < _map.Count - 1; i++)
            {
                for (int x = 1; x < _map[i].Length - 1; x++)
                {
                    if ((_map[i][x] == '.' && _map[i][x + 1] == ' ')
                        || (_map[i][x] == '.' && _map[i][x - 1] == ' '))
                    {
                        tmpCharArray = _map[i].ToCharArray();
                        if (tmpCharArray[x] == '.')
                            tmpCharArray[x] = '|';
                        _map[i] = new string(tmpCharArray);
                    }
                    else if ((_map[i][x] == '.' && _map[i - 1][x] == ' ')
                        || (_map[i][x] == '.' && _map[i + 1][x] == ' '))
                    {
                        tmpCharArray = _map[i].ToCharArray();
                        if (tmpCharArray[x] == '.')
                            tmpCharArray[x] = '-';
                        _map[i] = new string(tmpCharArray);
                    }
                }
            }
        }

        public void draw_better_walls()
        {
            for (int i = 1; i < _map.Count - 1; i++)
            {
                for (int x = 1; x < _map[i].Length - 1; x++)
                {
                    if (_map[i][x] == '#' && _map[i][x + 1] == '#' && _map[i + 1][x] == '#')
                    {
                        tmpCharArray = _map[i].ToCharArray();
                        tmpCharArray[x] = 'X';
                        _map[i] = new string(tmpCharArray);
                    }
                    else if (_map[i][x] == '#' && _map[i][x - 1] == '-' && _map[i + 1][x] == '#')
                    {
                        tmpCharArray = _map[i].ToCharArray();
                        tmpCharArray[x] = 'X';
                        _map[i] = new string(tmpCharArray);
                    }
                    else if ((_map[i][x] == '#' && _map[i][x - 1] == '-' && _map[i - 1][x] == '|')
                         || (_map[i][x] == '#' && _map[i][x - 1] == 'X' && _map[i - 1][x] == '|')
                         || (_map[i][x] == '#' && _map[i][x - 1] == '-' && _map[i - 1][x] == 'X'))
                    {
                        tmpCharArray = _map[i].ToCharArray();
                        tmpCharArray[x] = 'X';
                        _map[i] = new string(tmpCharArray);
                    }
                    else if (_map[i][x] == '#' && _map[i - 1][x] == '|' && _map[i][x + 1] == '#')
                    {
                        tmpCharArray = _map[i].ToCharArray();
                        tmpCharArray[x] = 'X';
                        _map[i] = new string(tmpCharArray);
                    }
                    else if (_map[i][x] == '#' && _map[i][x + 1] == '#')
                    {
                        tmpCharArray = _map[i].ToCharArray();
                        tmpCharArray[x] = '-';
                        _map[i] = new string(tmpCharArray);
                    }
                    else if (_map[i][x] == '#' && _map[i + 1][x] == '#')
                    {
                        tmpCharArray = _map[i].ToCharArray();
                        tmpCharArray[x] = '|';
                        _map[i] = new string(tmpCharArray);
                    }
                }
            }
        }

        public void draw_angles()
        {
            for (int i = 1; i < _map.Count - 1; i++)
            {
                for (int x = 1; x < _map[i].Length - 1; x++)
                {
                    if ((_map[i][x] == '|' && _map[i][x + 1] == 'O')
                        || (_map[i][x] == '|' && _map[i][x - 1] == 'O')
                        || (_map[i][x] == '-' && _map[i + 1][x] == 'O')
                        || (_map[i][x] == '-' && _map[i - 1][x] == 'O'))
                    {
                        tmpCharArray = _map[i].ToCharArray();
                        tmpCharArray[x] = 'X';
                        _map[i] = new string(tmpCharArray);
                    }
                }
            }
            for (int i = 1; i < _map.Count - 1; i++)
            {
                for (int x = 1; x < _map[i].Length - 1; x++)
                {
                    if ((_map[i][x] == '|' && _map[i + 1][x] == ' ')
                        || (_map[i][x] == '|' && _map[i - 1][x] == ' ')
                        || (_map[i][x] == '-' && _map[i][x + 1] == ' ')
                        || (_map[i][x] == '-' && _map[i][x - 1] == ' '))
                    {
                        tmpCharArray = _map[i].ToCharArray();
                        tmpCharArray[x] = 'X';
                        _map[i] = new string(tmpCharArray);
                    }
                }
            }
            for (int i = 1; i < _map.Count - 1; i++)
            {
                for (int x = 1; x < _map[i].Length - 1; x++)
                {
                    if ((_map[i][x] == '.' && _map[i + 1][x] == '|')
                        || (_map[i][x] == '.' && _map[i - 1][x] == '|')
                        || (_map[i][x] == '.' && _map[i][x + 1] == '-')
                        || (_map[i][x] == '.' && _map[i][x - 1] == '-'))
                    {
                        tmpCharArray = _map[i].ToCharArray();
                        tmpCharArray[x] = 'X';
                        _map[i] = new string(tmpCharArray);
                    }
                }
            }
            for (int i = 1; i < _map.Count - 1; i++)
            {
                for (int x = 1; x < _map[i].Length - 1; x++)
                {
                    if (_map[i][x] == '#')
                    {
                        tmpCharArray = _map[i].ToCharArray();
                        tmpCharArray[x] = 'X';
                        _map[i] = new string(tmpCharArray);
                    }
                }
            }
        }
    };

    public class Size
    {
        public int x;
        public int y;

        public Size()
        {
            x = 0;
            y = 0;
        }

        public Size(int X, int Y)
        {
            x = X;
            y = Y;
        }

    };

    public class Rooms
    {
        public Size _pos;
        public Size _size;
        public int _id;

        public Rooms(Size pos, Size size, int id)
        {
            _pos = new Size(pos.x, pos.y);
            _size = new Size(size.x, size.y);
            _id = id;
        }
        ~Rooms() { }
    };

    public class Leaf
    {
        public Size _pos;
        public Size _size;
        public Leaf leftChild;
        public Leaf rightChild;
        static public int ids = 0;

        public Leaf(Size pos, Size size)
        {
            _pos = new Size(pos.x, pos.y);
            _size = new Size(size.x, size.y);
            leftChild = null;
            rightChild = null;
        }
        ~Leaf() { }

        public bool split(Size roomMin)
        {
            bool splitB;

            if (leftChild != null && rightChild != null)
                return false;
            if (_size.x > _size.y)
                splitB = false;
            else
                splitB = true;
            int max = (splitB ? _size.y : _size.x) - (splitB ? roomMin.y : roomMin.x);
            if (max < (splitB ? roomMin.y : roomMin.x))
                return false;
            int splitPos = Random.Range((splitB ? roomMin.y : roomMin.x), max);
            if (splitB)
            {
                leftChild = new Leaf(new Size(_pos.x, _pos.y), new Size(_size.x, splitPos));
                rightChild = new Leaf(new Size(_pos.x, _pos.y + splitPos), new Size(_size.x, _size.y - splitPos));
            }
            else
            {
                leftChild = new Leaf(new Size(_pos.x, _pos.y), new Size(splitPos, _size.y));
                rightChild = new Leaf(new Size(_pos.x + splitPos, _pos.y), new Size(_size.x - splitPos, _size.y));
            }
            return true;
        }

        public void create_rooms(List<Rooms> _rooms, Size maxS)
        {
            if (leftChild != null || rightChild != null)
            {
                if (leftChild != null)
                    leftChild.create_rooms(_rooms, maxS);
                if (rightChild != null)
                    rightChild.create_rooms(_rooms, maxS);
            }
            else
            {
                if (_size.x <= maxS.x && _size.y <= maxS.y)
                {
                    ++ids;
                    _rooms.Add(new Rooms(_pos, _size, ids));
                }
            }
        }


        public void display_node_info()
        {
            Debug.Log("Leaf pos : " + _pos.x + "/" + _pos.y);
            Debug.Log("Leaf _size : " + _size.x + "/" + _size.y);
        }
    };
}
