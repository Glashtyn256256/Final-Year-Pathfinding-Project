using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This holds data of what our map will look and is then passed on to our grid.
//We can make this read data from a text file with custom maps, currenlty it's basic.
public class MapData : MonoBehaviour
{
    //Here is the width and height of our map. We can alter this to make it bigger or smaller.
    public int width = 10;
    public int height = 5;

    //This function puts our mapadata into the 2d array. 0 is for out floor and 1 is for our wall
    //This will eventually be used in our grid and converted into a enum to let us know if the node is blocked or not. 
    public int[,] MakeMap() //2d array
    {
        int[,] map = new int[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if(x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = 0;
                }
            }
        }
        return map;
    }
}
