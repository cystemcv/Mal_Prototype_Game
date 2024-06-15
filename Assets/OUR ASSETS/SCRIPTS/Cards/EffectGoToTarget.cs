using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectGoToTarget : MonoBehaviour
{

    public LTDescr moveTween;

    public float waitTime = 0.2f;
    public float timeToReachTarget = 0.25f;

    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitTime());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveToTarget()
    {
        moveTween = LeanTween.move(gameObject, target.transform.position, timeToReachTarget).setOnComplete(OnTargetReached); 
    }

    private void OnTargetReached()
    {
        // Code to execute when the target is reached
        Debug.Log("Target reached!");
        Destroy(this.gameObject,0.2f);
        // Add any additional logic you want to execute here
    }

    IEnumerator WaitTime()
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(waitTime);
        MoveToTarget();
    }
}
