using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;

public class GameAnalytics : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnityServices.InitializeAsync();
    }


}
