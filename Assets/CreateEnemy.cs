using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
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
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(time);
            var obj = mMSimpleObjectPooler.GetPooledGameObject();
            obj.SetActive(true);
        }
    }
}
