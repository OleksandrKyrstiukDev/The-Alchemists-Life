using System.Collections;
using UnityEngine;

public class WallCrackReaction : PotionReaction
{
    [Header("Crack Settings")]
    [SerializeField] private float disappearTime = 1f;

    [SerializeField] private float goodScale = 0.5f;


    private Vector3 startScale;

    private bool fixedCrack;


    private void Awake()
    {
        startScale = transform.localScale;
    }


    public override void React(BrewResult result)
    {
        if (fixedCrack)
            return;


        switch (result)
        {
            case BrewResult.Perfect:

                StartCoroutine(
                    ShrinkAndDestroy()
                );

                break;


            case BrewResult.Good:

                StartCoroutine(
                    ShrinkHalf()
                );

                break;


            default:

                Debug.Log(
                    "[Crack] Potion failed"
                );

                break;
        }
    }



    private IEnumerator ShrinkHalf()
    {
        fixedCrack = true;


        Vector3 target =
            startScale * goodScale;


        yield return ScaleRoutine(target);


        Debug.Log(
            "[Crack] Reduced"
        );
    }



    private IEnumerator ShrinkAndDestroy()
    {
        fixedCrack = true;


        yield return ScaleRoutine(
            Vector3.zero
        );


        Destroy(gameObject);
    }



    private IEnumerator ScaleRoutine(
        Vector3 target
    )
    {
        float t = 0f;

        Vector3 start =
            transform.localScale;


        while (t < disappearTime)
        {
            t += Time.deltaTime;


            transform.localScale =
                Vector3.Lerp(
                    start,
                    target,
                    t / disappearTime
                );


            yield return null;
        }


        transform.localScale = target;
    }
}