using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class FrameWaiter : MonoBehaviour
{
    public static async Task WaitForEndOfFrameAsync()
    {
        var tcs = new TaskCompletionSource<bool>();
        Instance.StartCoroutine(WaitEndOfFrameCoroutine(tcs));
        await tcs.Task;
    }

    private static FrameWaiter _instance;
    private static FrameWaiter Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("FrameWaiter");
                _instance = obj.AddComponent<FrameWaiter>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    private static IEnumerator WaitEndOfFrameCoroutine(TaskCompletionSource<bool> tcs)
    {
        yield return new WaitForEndOfFrame();
        tcs.SetResult(true);
    }
}
