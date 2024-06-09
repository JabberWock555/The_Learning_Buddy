using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelButton : MonoBehaviour
{
    [SerializeField] private ModelType modelType;
    [SerializeField] private UIManager uI;
    [SerializeField] private GameObject selectedImage;
    private Button button;
    private static Action<ModelType> OnSelect;

    void Start()
    {
        button = GetComponent<Button>();
        selectedImage.SetActive(false);
        OnSelect += OnSelected;
    }

    private void OnSelected(ModelType modelType)
    {
        selectedImage.SetActive(this.modelType == modelType);

    }
    public void InvokeButton() 
    {
        uI.InvokeModelButton(modelType);
        OnSelect?.Invoke(modelType);
    }

    private void OnDestroy() {
        OnSelect -= OnSelected;
    }
}
