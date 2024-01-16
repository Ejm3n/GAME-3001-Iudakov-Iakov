using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDragScript : MonoBehaviour
{
    private bool isDragging = false;
    private Rigidbody2D currentDraggedObject;
    private Vector2 offset;

    void Update()
    {
        //First we check if player clicked on screen
        if(Input.GetMouseButtonDown(0))
        {
            //raycast to check if the mouse is over a collider
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Vector2.zero);

            if(hit.collider != null)
            {
                //check if the clicked game object has a rigidbody2d
                currentDraggedObject = hit.collider.GetComponent<Rigidbody2D>();

                if(currentDraggedObject != null)
                {
                    // start dragging only if no object is currently being dragged
                    isDragging = true;
                    //currentDraggedObject = rb2d; - old line i want to check if we really need to create rb2d

                    offset = currentDraggedObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            //stop dragging
            isDragging=false;
            currentDraggedObject = null;
        }
        //drag object according to mouse
        if(isDragging && currentDraggedObject != null)
        {
            //move the dragged gameobject based on the mouse position
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentDraggedObject.MovePosition(mousePosition + offset);
        }
    }
}
