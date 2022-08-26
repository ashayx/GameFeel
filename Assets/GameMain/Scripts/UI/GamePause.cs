using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using UnityEngine;

public class GamePause : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GamePause()
    {
        CorgiEngineEvent.Trigger(CorgiEngineEventTypes.Pause);

    }
}
