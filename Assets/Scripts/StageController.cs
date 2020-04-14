using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageController : MonoBehaviour
{
    bool isRunning = false;

    int[] matchOptions;
    GameObject[] choiceInstances;
    float timeStamp = 0;
    float subTimeStamp = 0;
    public GameObject startButton;
    public GameObject nextScreenButton;

    public float offsetVerticalFromBoxToPlaceChoices = 1.2f;

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
        ResetGame();
        //nextScreenButton.SetActive(false);
        //Screen.SetResolution(600, 800, false);
    }

    // Update is called once per frame
    void Update()
    {
        if(isRunning)
        {
            switch (matchStage)
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
                        if (Time.time - timeStamp > 1)
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
                            bool isOn = (subTime % 2 != 0) ? true : false;
                            ShowTheChoicesAboveTheBoxes(isOn);
                        }
                    }
                    break;
                case MatchStages.Hidden:
                    {
                        if (animator.GetChoicesMade().Count == 3)
                        {
                            timeStamp = Time.time;
                            matchStage = MatchStages.FinishAndCompare;
                            animator.EnableAllClickables(false);
                        }
                    }
                    break;
                case MatchStages.FinishAndCompare:
                    {
                        if (Time.time - timeStamp > animator.timeToWaitForFruitAnim / 2)
                        {
                            //ga.EnableAllClickables(false);
                            if (DoChoicesMatch() == true)
                            {
                                animator.PlaySuccessAnimation();
                                matchStage = MatchStages.NavigateToNextScene;
                                timeStamp = subTimeStamp = Time.time;
                                
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
                case MatchStages.NavigateToNextScene:
                    {
                        if(Time.time - timeStamp > animator.timeToWaitForFruitAnim)
                        {
                            nextScreenButton.SetActive(true);
                        }
                    }
                    break;
            }
            //timeStamp
        }
    }

    bool DoChoicesMatch()
    {
        var choices = animator.GetChoicesMade();
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
        isRunning = false;
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

    public void ResetGame()
    {
        nextScreenButton.SetActive(false);
        animator.EnableAllClickables(false);
        animator.ResetRewardBox();
        
        nextScreenButton.SetActive(false);
        startButton.SetActive(true);
        CleanupAndReset();
    }

    public void BeginPressed()
    {
        if (isRunning == true)
            return;

        if (startButton != null)
        {
            //startButton.GetComponent<GameObject>().SetActive(false);
            startButton.SetActive(false);
        }

        //animator.ResetRewardBox();

        isRunning = true;
        ChooseRandomObjectsForPlacement();

        timeStamp = Time.time;
        matchStage = MatchStages.Begin;
    }

    void ChooseRandomObjectsForPlacement()
    {
        Vector3[] destinations;
        animator.GetDestinations(out destinations);

        GameObject[] clickableObjects;
        animator.GetClickables(out clickableObjects);

        choiceInstances = new GameObject[destinations.Length];
        matchOptions = new int[destinations.Length];

        for (int i = 0; i < destinations.Length; i++)
        {
            GameObject archetype;
            do
            {
                matchOptions[i] = (int)(Random.value * (float)clickableObjects.Length);
                archetype = clickableObjects[matchOptions[i]];
            } while (archetype != null && archetype.activeSelf == false);

            Vector3 pos = destinations[i];
            pos.y -= offsetVerticalFromBoxToPlaceChoices;// just an offset
            choiceInstances[i] = Instantiate(archetype, pos, archetype.transform.rotation);
            var collider = choiceInstances[i].GetComponent<CapsuleCollider>();
            Destroy(collider);// make it not clickable
        }
    }
}
