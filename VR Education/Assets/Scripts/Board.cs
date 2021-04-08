using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    //When the brush moves very fast, in order to avoid intermittent points, it is necessary to interpolate between two points, and LERP is the interpolation coefficient
    [Range(0, 1)]
    public float lerp = 0.05f;
    //Initialize picture of background
    public Texture2D initailizeTexture;
    //Picture of current background
    private Texture2D currentTexture;
    //The position of the brush is mapped to the UV coordinates of the board picture
    private Vector2 paintPos;

    private bool isdrawing = false; // is the current brush on the palette
                                    //Where the brush is when you leave 
    private int lastPaintX;
    private int lastPaintY;
    //The size of the color block represented by the brush
    private int painterTipsWidth = 30;
    private int painterTipsHeight = 15;
    //The size of the background picture of the current palette
    private int textureWidth;
    private int textureHeight;

    //The color of the brush
    private Color32[] painterColor;

    private Color32[] currentColor;
    private Color32[] originColor;
    public bool isDrawing;

    private void Start()
    {
        //Get the size of the original picture 
        Texture2D originTexture = GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
        textureWidth = originTexture.width;//1920 
        textureHeight = originTexture.height;//1080

        //Set current picture
        currentTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false, true);
        currentTexture.SetPixels32(originTexture.GetPixels32());
        currentTexture.Apply();

        //Assign to blackboard
        GetComponent<MeshRenderer>().material.mainTexture = currentTexture;

        //Initialize brush color
        painterColor = Enumerable.Repeat<Color32>(new Color32(255, 0, 0, 255), painterTipsWidth * painterTipsHeight).ToArray<Color32>();
    }

    private void LateUpdate()
    {
        //Calculate the starting point of the color block represented by the current brush
        int texPosX = (int)(paintPos.x * (float)textureWidth - (float)(painterTipsWidth / 2));
        int texPosY = (int)(paintPos.y * (float)textureHeight - (float)(painterTipsHeight / 2));
        if (isDrawing)
        {
            //Change the pixel value of the block where the brush is located
            currentTexture.SetPixels32(texPosX, texPosY, painterTipsWidth, painterTipsHeight, painterColor);
            //If you move the brush quickly, there will be intermittent phenomenon, so interpolation is needed
            if (lastPaintX != 0 && lastPaintY != 0)
            {
                int lerpCount = (int)(1 / lerp);
                for (int i = 0; i <= lerpCount; i++)
                {
                    int x = (int)Mathf.Lerp((float)lastPaintX, (float)texPosX, lerp);
                    int y = (int)Mathf.Lerp((float)lastPaintY, (float)texPosY, lerp);
                    currentTexture.SetPixels32(x, y, painterTipsWidth, painterTipsHeight, painterColor);
                }
            }
            currentTexture.Apply();
            lastPaintX = texPosX;
            lastPaintY = texPosY;
        }
        else
        {
            lastPaintX = lastPaintY = 0;
        }

    }

    /// <summary>
    ///Set the UV position of the current brush
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetPainterPositon(float x, float y)
    {
        paintPos.Set(x, y);
    }

    /// <summary>
    ///Is the brush drawing at present
    /// </summary>
    public bool IsDrawing
    {
        get
        {
            return isDrawing;
        }
        set
        {
            isDrawing = value;
        }
    }

    /// <summary>
    ///Use the color of the brush currently on the palette
    /// </summary>
    /// <param name="color"></param>
    public void SetPainterColor(Color32 color)
    {
        if (!painterColor[0].IsEqual(color))
        {
            for (int i = 0; i < painterColor.Length; i++)
            {
                painterColor[i] = color;
            }
        }
    }


}
public static class MethodExtention
{
    /// <summary>
    ///Used to compare two color32 types for the same color
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="compare"></param>
    /// <returns></returns>
    public static bool IsEqual(this Color32 origin, Color32 compare)
    {
        if (origin.g == compare.g && origin.r == compare.r)
        {
            if (origin.a == compare.a && origin.b == compare.b)
            {
                return true;
            }
        }
        return false;
    }
}
