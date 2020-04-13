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
    [SerializeField]
    GameObject presentAnimation;
    [SerializeField]
    GameObject toyReward;

    List<int> clickedIndices;
    List<GameObject> choicesMade;
    public Transform particleEffectSpot;

    public float hoverPositionsAboveBoxes = 0.6f;

    int currentDestinationIndex = 0;
    void Start()
    {
        int index = 0;
        foreach (var go in clickables)
        {
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

    public void EnableAllClickables(bool turnOn = true)
    {
        foreach (var go in clickables)
        {
            var comp = go.GetComponent<ClickableChoice>();
            if (comp)
            {
                comp.isClickingEnabled = turnOn;
            }
        }
        if (turnOn == true)
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
            /* Vector3 position = Vector3.zero;
             //position.x = 20;
             Quaternion rot = Quaternion.identity;
             GameObject go = Instantiate(successAnimPrefab, position, rot);
             Destroy(go, 2.5f);*/
            presentAnimation.GetComponent<BoxAnim>().particleEffectSpot = particleEffectSpot.transform;
            presentAnimation.SetActive(true);
            presentAnimation.GetComponent<BoxAnim>().Begin();
        }
    }

    public void PlayFailureAnimation()
    {
        if (successAnimPrefab != null)
        {
            Vector3 position = particleEffectSpot.transform.position;// Vector3.zero;
            //position.x = 80;
            Quaternion rot = Quaternion.identity;
            GameObject go = Instantiate(failureAnimPrefab, position, rot);
            Destroy(go, 2.5f);
        }
    }

    public float timeToWaitForFruitAnim { get { return 3.0f; } }
    public void ChoiceMade(GameObject go)
    {
        if (currentDestinationIndex >= boxes.Length)
        {
            return;
            currentDestinationIndex = 0;
        }
        int which = currentDestinationIndex++;

        Vector3 destination = boxes[which].transform.position;
        destination.y += hoverPositionsAboveBoxes;
        Vector3 position = go.transform.position;
        Quaternion rot = go.transform.rotation;

        GameObject newObj = Instantiate(go, position, rot);
        if (newObj != null)
        {
            iTween.MoveTo(newObj, destination, timeToWaitForFruitAnim);
            choicesMade.Add(newObj);
            clickedIndices.Add(go.GetComponent<ClickableChoice>().choiceIndex);
            /*    iTween tween;// = new iTween();
                tween.easeType = iTween.EaseType.easeInCubic;*/
        }
    }

    public void ClearPreviousSelections()
    {
        foreach (var go in choicesMade)
        {
            Destroy(go, 0.5f);
        }
        clickedIndices.Clear();
        currentDestinationIndex = 0;
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
