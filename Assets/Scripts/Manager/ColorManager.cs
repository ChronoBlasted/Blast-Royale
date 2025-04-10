using BaseTemplate.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoSingleton<ColorManager>
{
    [SerializeField] Color _healColor, _manaColor, _catchColor, _statusColor;

    [SerializeField] Sprite _activeSprite, _inactiveSprite;

    public Sprite ActiveSprite { get => _activeSprite; }
    public Sprite InactiveSprite { get => _inactiveSprite; }

    public Color GetItemColor(ItemBehaviour behaviour)
    {
        Color colorToReturn = new Color();

        switch (behaviour)
        {
            case ItemBehaviour.HEAL:
                colorToReturn = _healColor;
                break;
            case ItemBehaviour.MANA:
                colorToReturn = _manaColor;
                break;
            case ItemBehaviour.STATUS:
                colorToReturn = _statusColor;
                break;
            case ItemBehaviour.CATCH:
                colorToReturn = _catchColor;
                break;
        }

        return colorToReturn;
    }
}
