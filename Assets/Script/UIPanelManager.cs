using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class UIPanelManager : MonoBehaviour
{
    public enum PanelId
    {
        NONE,
        START_PANEL,
        MAIN_MENU,
        ANIMAL_MENU,

    }

    [Serializable]
    private struct PanelInfo
    {
        public PanelId panelId; 
        public GameObject panel;
        public Button button; 
    }

    [SerializeField] private List<PanelInfo> panels;

    private Dictionary<PanelId, GameObject> panelDictionary;
    private GameObject previousPanel;

    void Start()
    {
        panelDictionary = new Dictionary<PanelId, GameObject>();

        foreach (PanelInfo panelInfo in panels)
        {
            if (panelInfo.panel != null && panelInfo.button != null)
            {
                panelDictionary[panelInfo.panelId] = panelInfo.panel;
                panelInfo.button.onClick.AddListener(() => ShowPanel(panelInfo.panelId));
            }
            else
            {
                Debug.LogWarning("Panel or button reference is missing for panel: " + panelInfo.panelId);
            }
        }
    }

    public void ShowPanel(PanelId panelName)
    {
        foreach (KeyValuePair<PanelId, GameObject> panelEntry in panelDictionary)
        {
            panelEntry.Value.SetActive(panelEntry.Key == panelName);
        }
    }
}
