using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAnimator : MonoBehaviour
{
    [SerializeField]
    GameObject[] clickables;
    [SerializeField]
    GameObject[] boxes;

    int currentDestinationIndex = 0;
    void Start()
    {
        foreach(var go in clickables)
        {
            var comp = go.GetComponent<ClickableChoice>();
            if (comp)
            {
                comp.gameAnimator = this;
                comp.isClickingEnabled = false;
            }
        }
    }

    public void EnableAllClickables(bool enable = true)
    {
        foreach (var go in clickables)
        {
            var comp = go.GetComponent<ClickableChoice>();
            if (comp)
            {
                comp.isClickingEnabled = enable;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChoiceMade(GameObject go)
    {
        int which = currentDestinationIndex++;
        if (currentDestinationIndex >= boxes.Length)
            currentDestinationIndex = 0;
        iTween.MoveTo(go, boxes[which].transform.position, 3.0f);
    /*    iTween tween;// = new iTween();
        tween.easeType = iTween.EaseType.easeInCubic;*/
    }

    public void GetDestinations(out Vector3 [] positions)
    {

        int num = boxes.Length;
        positions = new Vector3[num];

        int index = 0;
        foreach (var go in boxes)
        {
            positions[index++] = (go.transform.position);
        }
    }
    public void GetClickables(out GameObject[] clickableObjects)
    {

        int num = clickables.Length;
        clickableObjects = new GameObject[num];

        int index = 0;
        foreach (var go in clickables)
        {
            clickableObjects[index++] = (go);
        }
    }
}
