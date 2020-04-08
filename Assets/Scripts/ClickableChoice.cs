using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableChoice : MonoBehaviour
{
    public GameAnimator gameAnimator;
    bool isMouseButtonDown = false;
    public bool isClickingEnabled { get; set;  }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

      /*  if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.transform == gameObject.transform)
                {
                    if (gameAnimator != null)
                        gameAnimator.ChoiceMade(gameObject);
                }
            }
            isMouseButtonDown = true;
        }*/
    }

    private void OnMouseDown()
    {
       /* if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isMouseButtonDown = true;
        }*/
    }
    private void OnMouseUp()
    {
        if (isClickingEnabled == false)
            return;

        if (gameAnimator != null)
            gameAnimator.ChoiceMade(gameObject);

        isMouseButtonDown = false;
    }
}
