using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    bool isRunning = false;

    int[] matchOptions;
    GameObject[] choiceInstances;
    float timeStamp = 0;
    float subTimeStamp = 0;

    enum MatchStages
    {
        Begin,
        FlashingMode, 
        Hidden,
        FinishAndCompare,
        NavigateToNextScene
    }

    MatchStages matchStage;

    GameAnimator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<GameAnimator>();
        animator.EnableAllClickables(false);
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
                            ShowTheChoicesAboveTheBoxes(false);
                            // should be a transition state
                            animator.EnableAllClickables();
                        }
                        else 
                        {
                            int subTime = (int)((Time.time - subTimeStamp) * 10.0f);
                            bool isOn = (subTime % 2 != 0) ? true:false;
                            ShowTheChoicesAboveTheBoxes(isOn);
                        }
                    }
                    break;
                case MatchStages.Hidden:
                    {
                        if(animator.GetChoices().Count == 3)
                        {
                            timeStamp = Time.time;
                            matchStage = MatchStages.FinishAndCompare;
                            animator.EnableAllClickables(false);
                        }
                    }
                    break;
                case MatchStages.FinishAndCompare:
                    {
                        if (Time.time - timeStamp > animator.timeToWaitForFruitAnim/2)
                        {
                            //ga.EnableAllClickables(false);
                            if (DoChoicesMatch() == true)
                            {
                                animator.PlaySuccessAnimation();
                                matchStage = MatchStages.NavigateToNextScene;
                            }
                            else
                            {
                                animator.PlayFailureAnimation();
                                CleanupAndReset();
                                matchStage = MatchStages.Begin;
                                timeStamp = subTimeStamp = Time.time;
                            }
                        }
                    }
                    break;
            }
            //timeStamp
        }
    }

    bool DoChoicesMatch()
    {
        var choices = animator.GetChoices();
        var matchCases = matchOptions;
        if (matchOptions.Length != choices.Count)
            return false;

        for(int i=0; i<matchOptions.Length; i++)
        {
            if (matchOptions[i] != choices[i])
                return false;
        }

        return true;
    }

    void CleanupAndReset()
    {
      /*  for(int i=0; i< choiceInstances.Length; i++)
        {
            Destroy(choiceInstances[i], 0.5f);
        }*/
        

        Vector3[] destinations;
        animator.GetDestinations(out destinations);
        GameObject[] clickableObjects;
        animator.GetClickables(out clickableObjects);

        animator.ClearPreviousSelections();
        timeStamp = Time.time;
        matchStage = MatchStages.Begin;
        /*  choiceInstances = new GameObject[destinations.Length];
          matchOptions = new int[destinations.Length];*/

        /*   for (int i = 0; i < destinations.Length; i++)
           {
               int option = matchOptions[i] ;
               Vector3 pos = destinations[i];
               pos.y += 1;
               GameObject archetype = clickableObjects[matchOptions[i]];
               choiceInstances[i] = Instantiate(archetype, pos, archetype.transform.rotation);
               //choiceInstances[i].transform.position = pos;
               //choiceInstances[i].transform.rotation = 
           }*/
    }

    void ShowTheChoicesAboveTheBoxes(bool show = true)
    {
        for (int i = 0; i < choiceInstances.Length; i++)
        {
            choiceInstances[i].active = show;
        }
    }

    public void BeginPressed()
    {
        if (isRunning == true)
            return;

        isRunning = true;
        Vector3[] destinations;
        animator.GetDestinations(out destinations);

        GameObject[] clickableObjects;
        animator.GetClickables(out clickableObjects);

        choiceInstances = new GameObject[destinations.Length];
        matchOptions = new int[destinations.Length];

        for (int i=0; i<destinations.Length; i++)
        {
            matchOptions[i] = (int)(Random.value * (float) clickableObjects.Length);
            Vector3 pos = destinations[i];
            pos.y += 1;
            GameObject archetype = clickableObjects[matchOptions[i]];
            choiceInstances[i] = Instantiate(archetype, pos, archetype.transform.rotation);
            var collider = choiceInstances[i].GetComponent<CapsuleCollider>();
            Destroy(collider);// make it not clickable
        }

        timeStamp = Time.time;
        matchStage = MatchStages.Begin;
    }
}
