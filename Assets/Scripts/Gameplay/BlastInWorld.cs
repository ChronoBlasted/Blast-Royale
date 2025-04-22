using DG.Tweening;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BlastInWorld : MonoBehaviour
{
    [SerializeField] bool _isPlayer;
    public SpriteRenderer BlastRender;
    public PlatformLayout PlatformLayout;
    Material _flashMaterial;
    public void Init(Sprite sprite)
    {
        BlastRender.sprite = sprite;

        string targetLayerName = _isPlayer ? "Player" : "Opponent";

        SortingLayer targetLayer = SortingLayer.layers.FirstOrDefault(l => l.name == targetLayerName);

        BlastRender.sortingLayerID = targetLayer.id;

        transform.localScale = Vector3.one;

        _flashMaterial = BlastRender.material;
        _flashMaterial.SetFloat("_FlashAmount", 1);
    }

    void Update()
    {
        var target = _isPlayer ? UIManager.Instance.GameView.PlayerHUD.BlastTransformInUI : UIManager.Instance.GameView.OpponentHUD.BlastTransformInUI;

        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, target.position);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane));

        worldPos.z = 0;

        transform.position = worldPos;
    }

    #region AA Action
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
    #endregion

    public void DoTakeDamageRender()
    {

        DOVirtual.Float(_flashMaterial.GetFloat("_FlashAmount"), 0, .15f, x =>
        {
            _flashMaterial.SetFloat("_FlashAmount", x);
        }).SetEase(Ease.OutSine).SetLoops(2, LoopType.Yoyo);

        BlastRender.transform.DOPunchPosition(new Vector3(.25f, 0), .5f, 15);
    }

    public async Task DoCatchBlastTrap(int amount, Sprite trapSprite)
    {
        DOVirtual.Float(_flashMaterial.GetFloat("_FlashAmount"), 0, .15f, x =>
        {
            _flashMaterial.SetFloat("_FlashAmount", x);
        });

        await PlatformLayout.CatchAnimation(amount);

        if (amount == 4)
        {
            Instantiate(trapSprite, BlastRender.transform.position, Quaternion.identity);

            transform.DOScale(0f, .5f).SetEase(Ease.InBack);
        }
        else
        {
            DOVirtual.Float(_flashMaterial.GetFloat("_FlashAmount"), 1, .15f, x =>
            {
                _flashMaterial.SetFloat("_FlashAmount", x);
            });

            // TODO Add vfx out trap
        }

    }
}
