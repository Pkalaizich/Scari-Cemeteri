using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Tombs : MonoBehaviour
{
    [SerializeField] private List<Animator> tombsAnimators;
    [SerializeField] private List<SpriteRenderer> outlines;
    [SerializeField] private List<Light2D> lights1;
    [SerializeField] private List<Light2D> lights2;

    #region Singleton
    static private Tombs instance;
    static public Tombs Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        instance = this;
        foreach(SpriteRenderer sr in outlines)
        {
            sr.gameObject.SetActive(false);
        }
    }
    #endregion
    public void SetAnimation(int index, int animationToTrigger)
    {
        tombsAnimators[index].SetInteger("parameter", animationToTrigger);
        if(animationToTrigger ==0)
        {
            lights1[index].gameObject.SetActive(false);
            lights2[index].gameObject.SetActive(false);
        }
    }    

    public void SetTombColor(int index, Color color)
    {
        lights1[index].gameObject.SetActive(true);
        lights2[index].gameObject.SetActive(true);
        outlines[index].gameObject.SetActive(true);
        outlines[index].color= color;
    }

    public void DeactivateOutline(int index)
    {
        outlines[index].gameObject.SetActive(false);
    }
}
