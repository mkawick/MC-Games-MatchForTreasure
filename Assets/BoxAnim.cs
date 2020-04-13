using System;
using UnityEngine;
using UnityEngine.UI;

public class BoxAnim : MonoBehaviour
{
    [SerializeField]
    GameObject parentPrefabInstance;
   /* [SerializeField]
    GameObject box;*/
    [SerializeField]
    GameObject boxPrefab, lidPrefab, wrappedRibbonPrefab, unwrappedRibbonPrefab;
    GameObject box, lid, wrappedRibbon, unwrappedRibbon, parentInstance;
    [SerializeField]
    GameObject successAnimPrefab;
    public Transform particleEffectSpot;
    [SerializeField]
    GameObject [] toyRewards;
    GameObject animatedToyReward;
    public Transform toyStartSpot;
    public Transform toyEndSpot;
    public bool enableKeypressForTest = false;

    float timeStamp;
    float randomDuration;
    public float pauseTimeBeforeUnwrap = 1.0f;
    public float pauseTimeBeforeLidOpen = 1.0f;
    public float pauseTimeBeforeConfetti = 1.0f;
    public float howLongToPlayUnwrap = 1.0f;
    public float howLongToPlayLidOpen = 1.0f;
    public float howLongToPlayConfetti = 1.0f;
    Stages animStage = Stages.Waiting;
    ShakingState shakingStage = ShakingState.NotShaking;

    enum Stages
    {
        Waiting,
        PauseBeforeUnwrap,
        Unwrapping,
        PauseBeforeLidOpens,
        LidOpening, 
        PauseBeforeConfetti,
        PlayingConfetti,
        ShowAndAnimateToy,
        Finished
    }
    enum ShakingState
    {
        NotShaking,
        SettingUpShake,
        Skaking
    }
    // Start is called before the first frame update
    void Start()
    {
        CopyAllPrefabs();

        if (parentPrefabInstance != null)
            parentPrefabInstance.SetActive(false);

        if (unwrappedRibbon != null)
            unwrappedRibbon.SetActive(false);

        if (wrappedRibbon != null)
            wrappedRibbon.SetActive(true);
        if (lid != null)
            lid.SetActive(true);
        if (box != null)
            box.SetActive(true);
        timeStamp = Time.time;
    }

    void CopyAllPrefabs()
    {
        parentInstance = Instantiate(parentPrefabInstance);
        parentInstance.transform.parent = this.transform;
        parentInstance.transform.position = parentPrefabInstance.transform.position;

        box = GetChildObject(parentInstance.transform, "PresentBox");
        lid = GetChildObject(parentInstance.transform, "PresentCap");
        wrappedRibbon = GetChildObject(parentInstance.transform, "PresentWrappedBand");
        unwrappedRibbon = GetChildObject(parentInstance.transform, "PresentUnwrappedBand");

        /* parentInstance.GetComponentInChildren

         box = Instantiate(boxPrefab);s
         lid = Instantiate(lidPrefab);
         wrappedRibbon = Instantiate(wrappedRibbonPrefab);
         unwrappedRibbon = Instantiate(unwrappedRibbonPrefab);*/

    }
    public GameObject GetChildObject(Transform parent, string _tag)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.tag == _tag)
            {
                return child.gameObject;
            }
            if (child.childCount > 0)
            {
                return GetChildObject(child, _tag);
            }
        }
        return null;
    }

    private void Reset()
    {
        if (animatedToyReward != null)
            Destroy(animatedToyReward);
    }

    // Update is called once per frame
    void Update()
    {
        if(enableKeypressForTest == true && Input.GetKeyDown(KeyCode.Return) == true)
        {
            Begin();
        }
        if (animStage == Stages.Waiting)
        {
            HandleShakingAnim();
            return;
        }
        switch(animStage)
        {
            case Stages.PauseBeforeUnwrap:
                if(Time.time - timeStamp > pauseTimeBeforeUnwrap)
                {
                    timeStamp = Time.time;
                    animStage = Stages.Unwrapping;
                }
                break;
            case Stages.Unwrapping:
                {
                    if (unwrappedRibbon != null)
                        unwrappedRibbon.SetActive(true);

                    if (wrappedRibbon != null)
                        wrappedRibbon.SetActive(false);
                    timeStamp = Time.time;
                    animStage = Stages.PauseBeforeLidOpens;
                }
                break;
            case Stages.PauseBeforeLidOpens:
                if (Time.time - timeStamp > pauseTimeBeforeLidOpen)
                {
                    timeStamp = Time.time;
                    animStage = Stages.LidOpening;
                }
                break;
            case Stages.LidOpening:
                {
                    Vector3 destination = lid.transform.position;
                    destination.y += 2;
                    var rot = lid.transform.rotation;
                    var rot2 = rot.eulerAngles;
                    rot2.x -=35;
                    //ßrot.eulerAngles
                    //rot.x += 4;

                    iTween.RotateTo(lid, rot2, howLongToPlayLidOpen);
                    iTween.MoveTo(lid, destination, howLongToPlayLidOpen);

                    timeStamp = Time.time;
                    animStage = Stages.PauseBeforeConfetti;
                }
                break;
            case Stages.PauseBeforeConfetti:
                if (Time.time - timeStamp > pauseTimeBeforeConfetti)
                {
                    timeStamp = Time.time;
                    animStage = Stages.PlayingConfetti;
                }
                break;
            case Stages.PlayingConfetti:
                {
                    Quaternion rot = Quaternion.identity;
                    /*  Vector3 position = (lid.transform.position + box.transform.position) ; // Vector3.zero;
                      position *= 0.8f;
                      position += box.transform.position;*/
                    Vector3 position = particleEffectSpot.transform.position;//
                    //position.x -= 1.0f;
                    GameObject go = Instantiate(successAnimPrefab, position, rot);
                    go.transform.localScale *= 0.4f;
                    Destroy(go, howLongToPlayConfetti);
                    animStage = Stages.ShowAndAnimateToy;
                }
                break;
            case Stages.ShowAndAnimateToy:
                {
                    if(toyRewards!= null)
                    {
                        GameObject archetype;
                        int which;
                        do
                        {
                            which = (int)(UnityEngine.Random.value * (float)toyRewards.Length);
                            archetype = toyRewards[which];
                        } while (archetype == null);

                        animatedToyReward = Instantiate(archetype);
                        animatedToyReward.SetActive(true);
                        animatedToyReward.transform.rotation = toyRewards[which].transform.rotation;
                        animatedToyReward.transform.position = toyStartSpot.position;
                        animatedToyReward.transform.parent = this.transform;
                        iTween.MoveTo(animatedToyReward, toyEndSpot.position, howLongToPlayLidOpen);
                        animStage = Stages.Finished;
                        timeStamp = Time.time;
                    }
                }
                break;
            case Stages.Finished:
                break;
            
        }
    }

    private void HandleShakingAnim()
    {
        if (shakingStage == ShakingState.NotShaking)
        {
            timeStamp = Time.time;
            shakingStage = ShakingState.SettingUpShake;
            randomDuration = UnityEngine.Random.value * 3 + 2;
        }
        else if (shakingStage == ShakingState.SettingUpShake)
        {
            // essentially do nothing until time passes
            if (Time.time - timeStamp > randomDuration)
            {
                timeStamp = Time.time;
                shakingStage = ShakingState.Skaking;
                randomDuration = UnityEngine.Random.value * 1.5f + 1;
                float shakeRotAmount = UnityEngine.Random.value * 8;
                float shakePosAmount = UnityEngine.Random.value / 10;
                iTween.ShakeRotation(parentInstance, new Vector3(0, 0, shakeRotAmount), randomDuration);
                iTween.ShakePosition(parentInstance, new Vector3(shakePosAmount, shakePosAmount, 0), randomDuration);
            }
        }
        else
        {
            if (Time.time - timeStamp > randomDuration)
            {
                shakingStage = ShakingState.NotShaking;
            }
        }
    }

    /* private void Reset()
     {

     }*/

    public void Begin()
    {
        timeStamp = Time.time;
        animStage = Stages.PauseBeforeUnwrap;

        if (parentInstance != null)
            parentInstance.SetActive(true);
        if (wrappedRibbon != null)
            wrappedRibbon.SetActive(true);
        if (lid != null)
            lid.SetActive(true);
        if (box != null)
            box.SetActive(true);

        if (unwrappedRibbon != null)
            unwrappedRibbon.SetActive(false);
    }

}
