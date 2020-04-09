using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAnimator : MonoBehaviour
{
    [SerializeField]
    GameObject[] clickables;
    [SerializeField]
    GameObject[] boxes;
    [SerializeField]
    GameObject successAnimPrefab;
    [SerializeField]
    GameObject failureAnimPrefab;
    List<int> clickedIndices;
    List<GameObject> choicesMade;

    int currentDestinationIndex = 0;
    void Start()
    {
        foreach(var go in clickables)
        {
            int index = 0;
            var comp = go.GetComponent<ClickableChoice>();
            if (comp)
            {
                comp.gameAnimator = this;
                comp.isClickingEnabled = false;
                comp.choiceIndex = index++;
            }
        }
        clickedIndices = new List<int>();
        choicesMade = new List<GameObject>();
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
        if (enabled == true)
        {
            clickedIndices = new List<int>();
            choicesMade = new List<GameObject>();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public List<int> GetChoices()
    {
        return clickedIndices;
    }

    public void PlaySuccessAnimation()
    {
        if(successAnimPrefab != null)
        {
            Vector3 position = Vector3.zero;
            Quaternion rot = Quaternion.identity;
            GameObject go = Instantiate(successAnimPrefab, position, rot);
            Destroy(go, 2.5f);
        }
    }

    public void PlayFailureAnimation()
    {
        if (successAnimPrefab != null)
        {
            Vector3 position = Vector3.zero;
            Quaternion rot = Quaternion.identity;
            GameObject go = Instantiate(successAnimPrefab, position, rot);
            Destroy(go, 2.5f);
        }
    }

    public void ChoiceMade(GameObject go)
    {
        if (currentDestinationIndex >= boxes.Length)
        {
            return;
            currentDestinationIndex = 0;
        }
        int which = currentDestinationIndex++;

        Vector3 destination = boxes[which].transform.position;
        destination.y += 1;
        Vector3 position = go.transform.position;
        Quaternion rot = go.transform.rotation;

        GameObject newObj = Instantiate(go, position, rot);
        if (newObj != null)
        {
            iTween.MoveTo(newObj, destination, 3.0f);
            choicesMade.Add(newObj);
            clickedIndices.Add(go.GetComponent<ClickableChoice>().choiceIndex);
            /*    iTween tween;// = new iTween();
                tween.easeType = iTween.EaseType.easeInCubic;*/
        }
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
