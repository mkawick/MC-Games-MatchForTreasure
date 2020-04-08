using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    bool isRunning = false;

    int[] choices;
    GameObject[] choiceInstances;
    float timeStamp = 0;
    float subTimeStamp = 0;

    enum MatchStages
    {
        Begin,
        FlashingMode, 
        Hidden,
        FinishAndCompare
    }

    MatchStages matchStage;

    GameAnimator ga;
    // Start is called before the first frame update
    void Start()
    {
        ga = this.GetComponent<GameAnimator>();
        ga.EnableAllClickables(false);
        Screen.SetResolution(600, 800, false);
    }

    // Update is called once per frame
    void Update()
    {
        if(isRunning)
        {
            switch(matchStage)
            {
                case MatchStages.Begin:
                    {
                        float timeElapsed = Time.time - timeStamp;
                        if (timeElapsed > 1)
                        {
                            matchStage = MatchStages.FlashingMode;
                            timeStamp = subTimeStamp = Time.time;
                        }
                    }
                    break;
                case MatchStages.FlashingMode:
                    {
                        if(Time.time - timeStamp > 1)
                        {
                            matchStage = MatchStages.Hidden;
                            timeStamp = subTimeStamp = Time.time;
                            for (int i = 0; i < choiceInstances.Length; i++)
                            {
                                choiceInstances[i].active = false;
                            }
                        }
                        else 
                        {
                            int subTime = (int)((Time.time - subTimeStamp) * 10.0f);
                            bool isOn = (subTime % 2 != 0) ? true:false;
                            //Debug.Log("isOn = " + isOn);
                            Debug.Log("subTime = " + subTime);
                            for (int i = 0; i < choiceInstances.Length; i++)
                            {
                                choiceInstances[i].active = isOn;
                            }
                        }
                    }
                    break;
                case MatchStages.Hidden:
                    {
                        ga.EnableAllClickables();
                    }
                    break;
            }
            //timeStamp
        }
    }

    public void BeginPressed()
    {
        if (isRunning == true)
            return;

        isRunning = true;
        Vector3[] destinations;
        ga.GetDestinations(out destinations);

        GameObject[] clickableObjects;
        ga.GetClickables(out clickableObjects);

        choiceInstances = new GameObject[destinations.Length];
        choices = new int[destinations.Length];

        for (int i=0; i<destinations.Length; i++)
        {
            choices[i] = (int)(Random.value * (float) clickableObjects.Length);
            Vector3 pos = destinations[i];
            pos.y += 1;
            GameObject archetype = clickableObjects[choices[i]];
            choiceInstances[i] = Instantiate(archetype, pos, archetype.transform.rotation);
            var collider = choiceInstances[i].GetComponent<CapsuleCollider>();
            Destroy(collider);// make it not clickable
        }

        timeStamp = Time.time;
        matchStage = MatchStages.Begin;
    }
}
