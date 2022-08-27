using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
public class CreateEnemy : MonoBehaviour
{

    public float time = 1f;

    private MMMultipleObjectPooler mMSimpleObjectPooler;
    // Start is called before the first frame update
    void Awake()
    {
        mMSimpleObjectPooler = GetComponent<MMMultipleObjectPooler>();
    }

    // Update is called once per frame
    void Start()
    {
        StartCoroutine(Create());
    }

    private IEnumerator Create()
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            var obj = mMSimpleObjectPooler.GetPooledGameObject();
            if (obj != null)
            {
                obj.SetActive(true);
                obj.GetComponent<Health>()?.Revive();
            }

        }

    }
}
