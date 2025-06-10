using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "NewQuestDataRef", menuName = "ScriptableObjects/NakamaDataRef/NewQuestDataRefObject", order = 0)]
public class QuestDataRef : ScriptableObject
{
    public QuestIds QuestIds;
    public Sprite QuestIco;
    public LocalizedString QuestName;
    public LocalizedString QuestDesc;
}