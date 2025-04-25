using DG.Tweening;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BlastInWorld : MonoBehaviour
{
    [SerializeField] bool _isPlayer;
    [SerializeField] ChronoTweenHelper _chronoTweenHelper;
    public SpriteRenderer BlastRender;
    public PlatformLayout PlatformLayout;
    Material _flashMaterial;
    GameObject _currentTrap;

    public void Init(Sprite sprite)
    {
        BlastRender.sprite = sprite;

        string targetLayerName = _isPlayer ? "Player" : "Opponent";

        SortingLayer targetLayer = SortingLayer.layers.FirstOrDefault(l => l.name == targetLayerName);

        BlastRender.sortingLayerID = targetLayer.id;

        transform.localScale = Vector3.one;
        BlastRender.transform.localScale = Vector3.one;

        _chronoTweenHelper.TweenAction?.Invoke();

        if (_currentTrap != null) Destroy(_currentTrap);

        _flashMaterial = BlastRender.material;
        _flashMaterial.SetFloat("_FlashAmount", 1);
    }

    public IEnumerator SetPos()
    {
        yield return new WaitForEndOfFrame();

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

    public async Task DoCatchBlastTrap(int amount, GameObject trapGO)
    {
        _currentTrap = Instantiate(trapGO, new Vector3(-16, -64, 0), Quaternion.identity);

        await _currentTrap.transform.DOJump(BlastRender.transform.position + new Vector3(0, .5f, 0), 2, 1, .5f).SetEase(Ease.OutSine).AsyncWaitForCompletion();

        DOVirtual.Float(_flashMaterial.GetFloat("_FlashAmount"), 0, .2f, x =>
       {
           _flashMaterial.SetFloat("_FlashAmount", x);
       });

        DOTween.Kill(BlastRender.transform);
        await BlastRender.transform.DOScale(0, .5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _currentTrap.transform.DOPunchPosition(new Vector3(0, -.25f, 0), .2f, 1, 1);
        }).AsyncWaitForCompletion();

        CameraManager.Instance.SetCameraPosition(_currentTrap.transform.position);
        CameraManager.Instance.SetCameraZoom(6);

        UIManager.Instance.GameView.HideHUD();

        await Task.Delay(500);

        EnvironmentManager.Instance.SetDarkBackground(true);

        float delay = 1f;

        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < amount - 1; i++)
        {
            seq.Append(_currentTrap.transform.DOPunchScale(new Vector3(.25f, .25f, 0), .2f, 1, 1).SetEase(Ease.OutSine).OnComplete(() =>
            {
                CameraManager.Instance.DoShakeCamera();
                CameraManager.Instance.AddCameraZoom(-.5f);
            }));

            seq.AppendInterval(delay);
        }

        await PlatformLayout.CatchAnimation(amount, delay);

        if (amount == 4)
        {
            Color targetColor = Color.Lerp(Color.white, Color.black, .5f);

            _currentTrap.GetComponentInChildren<SpriteRenderer>().color = targetColor;

            _currentTrap.transform.DOPunchScale(new Vector3(.5f, .5f, 0), .2f, 1, 1);

            Instantiate(ResourceObjectHolder.Instance.GetResourceByType(ResourceType.CatchSuccess).Prefab, _currentTrap.transform.position, Quaternion.identity);

            UIManager.Instance.GameView.ShowHUD(false);
        }
        else
        {
            DOVirtual.Float(_flashMaterial.GetFloat("_FlashAmount"), 1, .15f, x =>
            {
                _flashMaterial.SetFloat("_FlashAmount", x);
            });

            _currentTrap.transform.DOMoveY(-64, .5f).SetEase(Ease.OutSine);

            Instantiate(ResourceObjectHolder.Instance.GetResourceByType(ResourceType.CatchFailure).Prefab, _currentTrap.transform.position, Quaternion.identity);

            _chronoTweenHelper.TweenAction?.Invoke();

            Destroy(_currentTrap, .5f);

            UIManager.Instance.GameView.ShowHUD(true);
        }

        TimeManager.Instance.DoLagTime();

        EnvironmentManager.Instance.SetDarkBackground(false);

        CameraManager.Instance.Reset();
    }
}
