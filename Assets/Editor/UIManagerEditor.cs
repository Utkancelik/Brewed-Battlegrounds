using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[CustomEditor(typeof(UIManager))]
public class UIManagerEditor : Editor
{
    SerializedProperty waveTextObject;
    SerializedProperty startBattleButton;
    SerializedProperty currentGoldText;
    SerializedProperty foodText;
    SerializedProperty totalGoldVaultText;
    SerializedProperty gameOverRoundGoldText;
    SerializedProperty roundGoldInGameText;
    SerializedProperty unitButtonPrefab;
    SerializedProperty unitButtonContainer;
    SerializedProperty mainBattlePanel;
    SerializedProperty battleBottomPanel;
    SerializedProperty upgradePanel;
    SerializedProperty mainButtonsPanel;
    SerializedProperty goToBattleButton;
    SerializedProperty upgradeButton;
    SerializedProperty unlockSoldierButtons;
    SerializedProperty soldierCards;
    SerializedProperty gameOverPanel;
    SerializedProperty closeGameOverPanelButton;
    SerializedProperty fadeOverlay;
    SerializedProperty increaseFoodProductionButton;
    SerializedProperty increaseBaseHealthButton;
    SerializedProperty foodProductionRateText;
    SerializedProperty baseHealthText;
    SerializedProperty foodFillingImage;

    bool foldoutGeneral = true;
    bool foldoutPanels = true;
    bool foldoutButtons = true;
    bool foldoutTexts = true;

    void OnEnable()
    {
        waveTextObject = serializedObject.FindProperty("waveTextObject");
        startBattleButton = serializedObject.FindProperty("startBattleButton");
        currentGoldText = serializedObject.FindProperty("currentGoldText");
        foodText = serializedObject.FindProperty("foodText");
        totalGoldVaultText = serializedObject.FindProperty("totalGoldVaultText");
        gameOverRoundGoldText = serializedObject.FindProperty("gameOverRoundGoldText");
        roundGoldInGameText = serializedObject.FindProperty("roundGoldInGameText");
        unitButtonPrefab = serializedObject.FindProperty("unitButtonPrefab");
        unitButtonContainer = serializedObject.FindProperty("unitButtonContainer");
        mainBattlePanel = serializedObject.FindProperty("mainBattlePanel");
        battleBottomPanel = serializedObject.FindProperty("battleBottomPanel");
        upgradePanel = serializedObject.FindProperty("upgradePanel");
        mainButtonsPanel = serializedObject.FindProperty("mainButtonsPanel");
        goToBattleButton = serializedObject.FindProperty("goToBattleButton");
        upgradeButton = serializedObject.FindProperty("upgradeButton");
        unlockSoldierButtons = serializedObject.FindProperty("unlockSoldierButtons");
        soldierCards = serializedObject.FindProperty("soldierCards");
        gameOverPanel = serializedObject.FindProperty("gameOverPanel");
        closeGameOverPanelButton = serializedObject.FindProperty("closeGameOverPanelButton");
        fadeOverlay = serializedObject.FindProperty("fadeOverlay");
        increaseFoodProductionButton = serializedObject.FindProperty("increaseFoodProductionButton");
        increaseBaseHealthButton = serializedObject.FindProperty("increaseBaseHealthButton");
        foodProductionRateText = serializedObject.FindProperty("foodProductionRateText");
        baseHealthText = serializedObject.FindProperty("baseHealthText");
        foodFillingImage = serializedObject.FindProperty("foodFillingImage");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        foldoutGeneral = EditorGUILayout.Foldout(foldoutGeneral, "General Settings");
        if (foldoutGeneral)
        {
            EditorGUILayout.PropertyField(waveTextObject);
            EditorGUILayout.PropertyField(startBattleButton);
        }

        foldoutPanels = EditorGUILayout.Foldout(foldoutPanels, "Panels");
        if (foldoutPanels)
        {
            EditorGUILayout.PropertyField(mainBattlePanel);
            EditorGUILayout.PropertyField(battleBottomPanel);
            EditorGUILayout.PropertyField(upgradePanel);
            EditorGUILayout.PropertyField(mainButtonsPanel);
            EditorGUILayout.PropertyField(gameOverPanel);
        }

        foldoutButtons = EditorGUILayout.Foldout(foldoutButtons, "Buttons");
        if (foldoutButtons)
        {
            EditorGUILayout.PropertyField(goToBattleButton);
            EditorGUILayout.PropertyField(upgradeButton);
            EditorGUILayout.PropertyField(closeGameOverPanelButton);
            EditorGUILayout.PropertyField(increaseFoodProductionButton);
            EditorGUILayout.PropertyField(increaseBaseHealthButton);
            EditorGUILayout.PropertyField(unlockSoldierButtons, true);
        }

        foldoutTexts = EditorGUILayout.Foldout(foldoutTexts, "Texts and Images");
        if (foldoutTexts)
        {
            EditorGUILayout.PropertyField(currentGoldText);
            EditorGUILayout.PropertyField(foodText);
            EditorGUILayout.PropertyField(totalGoldVaultText);
            EditorGUILayout.PropertyField(gameOverRoundGoldText);
            EditorGUILayout.PropertyField(roundGoldInGameText);
            EditorGUILayout.PropertyField(foodProductionRateText);
            EditorGUILayout.PropertyField(baseHealthText);
            EditorGUILayout.PropertyField(fadeOverlay);
            EditorGUILayout.PropertyField(foodFillingImage);
            EditorGUILayout.PropertyField(unitButtonPrefab);
            EditorGUILayout.PropertyField(unitButtonContainer);
            EditorGUILayout.PropertyField(soldierCards, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
