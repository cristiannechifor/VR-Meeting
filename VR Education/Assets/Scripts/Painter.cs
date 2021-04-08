using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painter : MonoBehaviour
{
    /// <summary>
    ///The color of the brush
    /// </summary>
    public Color32 penColor;

    public Transform rayOrigin;

    private RaycastHit hitInfo;
    //Is the brush being held by the handle
    private bool IsGrabbing;
    public static Board board; // set to a member of type instead of a member of type instance, because all brushes use the same board

 private void Start()
    {
        //Set the brush part to the color of the brush to identify the color of the brush
        foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
        {
            if (renderer.transform == transform)
            {
                continue;
            }
            renderer.material.color = penColor;
        }
        if (!board)
        {
            board = FindObjectOfType<Board>();
        }

    }

    private void Update()
    {
        Ray r = new Ray(rayOrigin.position, rayOrigin.forward);
        if (Physics.Raycast(r, out hitInfo, 0.1f))
        {
            if (hitInfo.collider.tag == "Board")
            {
                //Set the UV coordinates of the corresponding board picture where the brush is located 
                board.SetPainterPositon(hitInfo.textureCoord.x, hitInfo.textureCoord.y);
                //Current pen color
                board.SetPainterColor(penColor);
                board.IsDrawing = true;
                IsGrabbing = true;
            }
        }
        else if (IsGrabbing)
        {
            board.IsDrawing = false;
            IsGrabbing = false;
        }
    }

}