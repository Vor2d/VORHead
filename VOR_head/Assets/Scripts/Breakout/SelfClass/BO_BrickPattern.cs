using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_BrickPattern
{
    public bool[,] Binary_pattern { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Vector2Int Center { get; private set; }

    public BO_BrickPattern()
    {
        this.Binary_pattern = new bool[0,0];
        this.Width = 0;
        this.Height = 0;
        this.Center = new Vector2Int();
    }

    /// <summary>
    /// Generate binary pattern from texture, tex: index from bottom-left to top-right;
    /// </summary>
    /// <param name="tex"></param>
    public void generate_pattern_binary(Texture2D tex)
    {
        Width = tex.width;
        Height = tex.height;
        Center = new Vector2Int(tex.width / 2, tex.height / 2);
        Binary_pattern = new bool[tex.height, tex.width];
        int prow = 0;
        for(int r = 0;r<tex.height;r++)
        {
            //prow = tex.height - 1 - r;
            prow = r;
            for (int c = 0;c < tex.width;c++)
            {
                Binary_pattern[r,c] = (tex.GetPixel(c, prow) != Color.white);
            }
        }
    }
}
