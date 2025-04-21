using DG.Tweening;
using System.Linq;
using UnityEngine;

public class BlastInWorld : MonoBehaviour
{
    [SerializeField] bool _isPlayer;
    public SpriteRenderer BlastRender;
    public PlatformLayout PlatformLayout;


    public void Init(Sprite sprite)
    {
        BlastRender.sprite = sprite;

        string targetLayerName = _isPlayer ? "Player" : "Opponent";

        SortingLayer targetLayer = SortingLayer.layers.FirstOrDefault(l => l.name == targetLayerName);

        BlastRender.sortingLayerID = targetLayer.id;
    }

    void Update()
    {
        var target = _isPlayer ? UIManager.Instance.GameView.PlayerHUD.BlastTransformInUI : UIManager.Instance.GameView.OpponentHUD.BlastTransformInUI;

        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, target.position);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane));

        worldPos.z = 0;

        transform.position = worldPos;
    }

    public void DoMoveToOpponentRender(Vector3 position)
    {
        BlastRender.transform.DOMove(position, 0.5f).SetEase(Ease.OutSine).SetLoops(2, LoopType.Yoyo);
    }

    public void DoSelfRender()
    {
        BlastRender.transform.DOPunchPosition(new Vector3(0, .25f, 0), .5f, 1, 1);
    }

    public void DoCastProjectile(Vector3 direction)
    {
        BlastRender.transform.DOPunchPosition(direction, .5f, 1, 1);

    }

    public void DoCastLaser()
    {
        BlastRender.transform.DOShakePosition(.2f, new Vector3(.2f, .2f));
    }

    public void DoTakeDamageRender()
    {
        var tweenLoopDuration = .1f;

        BlastRender.DOColor(Color.black, tweenLoopDuration)
                                    .SetLoops(2, LoopType.Yoyo)
                                    .SetEase(Ease.OutSine);

        BlastRender.transform.DOPunchPosition(new Vector3(.25f, 0), .5f, 15);
    }
}
