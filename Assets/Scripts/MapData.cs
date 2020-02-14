using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This holds data of what our map will look and is then passed on to our grid.
//We can make this read data from a text file with custom maps, currenlty it's basic.
public class MapData : MonoBehaviour
{
    //Here is the width and height of our map. 
    //We can alter this to make it bigger or smaller in the unity editor.
    public int mapWidth = 10;
    public int mapHeight = 5;

    public Texture2D textureMap;
    public string resourcePath = "MapData";
    public string levelName;
    //This function puts our mapadata into the 2d array. 0 is for out floor and 1 is for our wall
    //This will eventually be used in our grid and converted into a enum to let us know if the node is blocked or not. 

   
    public int[,] MakeMap() //2d array
    {

        int[,] map;
        if (textureMap != null)
        {
            List<string> lines = new List<string>();
            lines = GetMapFromTexture(textureMap);
            SetDimensions(lines);
            map = new int[mapWidth, mapHeight];
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    if (lines[y].Length > x)
                    {
                        map[x, y] = (int)char.GetNumericValue(lines[y][x]);
                    }

                }
            }
        }
        else
        {
            map = new int[mapWidth, mapHeight];
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    if (x == 0 || y == 0 || x == mapWidth - 1 || y == mapHeight - 1)
                    {
                        map[x, y] = 1;
                    }
                    else
                    {
                        map[x, y] = 0;
                    }
                }
            }
        }
        return map;
    }

    public void SetDimensions(List<string> textLines)
    {
        mapHeight = textLines.Count;

        foreach (string line in textLines)
        {
            if (line.Length > mapWidth)
            {
                mapWidth = line.Length;
            }
        }
    }

    public List<string> GetMapFromTexture(Texture2D texture)
    {
        List<string> lines = new List<string>();

        if (textureMap != null)
        {
            for (int y = 0; y < texture.height; y++)
            {
                string newLine = "";

                for (int x = 0; x < texture.width; x++)
                {
                    if (texture.GetPixel(x, y) == Color.black)
                    {
                        newLine += '1';
                    }
                    else if (texture.GetPixel(x, y) == Color.white)
                    {
                        newLine += '0';
                    }
                    else if (texture.GetPixel(x, y) == Color.blue)
                    {
                        newLine += '9';
                    }
                    else if (texture.GetPixel(x, y) == Color.green)
                    {
                        newLine += '3';
                    }
                    else
                    {
                        newLine += '0';
                    }
                }
                lines.Add(newLine);
            }
        }
        return lines;
    }
}
