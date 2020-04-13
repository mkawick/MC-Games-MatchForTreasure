using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAnim : MonoBehaviour
{
    [SerializeField]
    GameObject parentPrefabInstance;
    [SerializeField]
    GameObject box;
    [SerializeField]
    GameObject lid, wrappedRibbon, unwrappedRibbon;
    [SerializeField]
    GameObject successAnimPrefab;
    public Transform particleEffectSpot;
    [SerializeField]
    GameObject toyReward;
    public Transform toyStartSpot;
    public Transform toyEndSpot;

    float timeStamp;
    public float pauseTimeBeforeUnwrap = 1.0f;
    public float pauseTimeBeforeLidOpen = 1.0f;
    public float pauseTimeBeforeConfetti = 1.0f;
    public float howLongToPlayUnwrap = 1.0f;
    public float howLongToPlayLidOpen = 1.0f;
    public float howLongToPlayConfetti = 1.0f;
    Stages animStage = Stages.Waiting;

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
    // Start is called before the first frame update
    void Start()
    {
        if (unwrappedRibbon != null)
            unwrappedRibbon.SetActive(false);

        if (wrappedRibbon != null)
            wrappedRibbon.SetActive(true);
        if (lid != null)
            lid.SetActive(true);
        if (box != null)
            box.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
      /*  if( Input.GetKeyDown(KeyCode.Return) == true)
        {
            Begin();
        }*/
        if (animStage == Stages.Waiting)
            return;
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
                    if(toyReward!= null)
                    {
                        toyReward.SetActive(true);
                        toyReward.transform.position = toyStartSpot.position;
                        iTween.MoveTo(toyReward,  toyEndSpot.position, howLongToPlayLidOpen);
}
                }
                break;
            case Stages.Finished:
                break;
            
        }
    }

   /* private void Reset()
    {
        
    }*/

    public void Begin()
    {
        timeStamp = Time.time;
        animStage = Stages.PauseBeforeUnwrap;

        if (parentPrefabInstance != null)
            parentPrefabInstance.SetActive(true);
        if (wrappedRibbon != null)
            wrappedRibbon.SetActive(true);
        if (lid != null)
            lid.SetActive(true);
        if (box != null)
            box.SetActive(true);
    }

}
