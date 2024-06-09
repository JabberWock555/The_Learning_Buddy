using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{

    [SerializeField] private List<ModelObj> modelList = new();
    [SerializeField] private CameraControllerTouch cameraController;
    [SerializeField] private GameObject MainPage;
    [SerializeField] private GameObject MenuPage;
    [SerializeField] private GameObject StartPage;
    [SerializeField] private TextMeshProUGUI infoPanel_Text;
    [SerializeField] private TextMeshProUGUI infoPanelTitle_Text;
    [SerializeField] private TextMeshProUGUI infoHeadingPanel_Text;
    [SerializeField] private GameObject animalPanelOpen;
    [SerializeField] private GameObject animalPanelClose;
    [SerializeField] private GameObject touchUi;
    [SerializeField] private GameObject zoomUi;
    
    private GameObject currentModel = null;
    private AudioClip currentInfoAudio, currentmodelAudio;
    private AudioSource audioSource, modelAudioSource;
    private GameObject infoPanel;
    public ModelType currentModelType = ModelType.None;


    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        infoPanel = infoPanel_Text.transform.parent.gameObject;
    }

    private void Start(){
        StartPage.SetActive(true);
        MenuPage.SetActive(false);
        MainPage.SetActive(false);
        InvokeAnimalPanel(true);
        if(infoPanel.activeSelf) ShowHints();
    }

    public void InvokeStart(){
        MenuPage.SetActive(true);
        StartPage.SetActive(false);
    }

    public void InvokeThemePage(){
        MenuPage.SetActive(false);
        MainPage.SetActive(true);

        InvokeAnimalPanel(true);
        cameraController.StartAutoPan();
        SetupModel(ModelType.Whale);
       // StartCoroutine(StartIntro() );  
    }

    public void InvokeHomePage(){
         MainPage.SetActive(false);
         StartPage.SetActive(true);

        Destroy(currentModel.gameObject);
        currentModelType = ModelType.None;
        currentInfoAudio = currentmodelAudio = null;
        audioSource.Stop();
        if(modelAudioSource != null) modelAudioSource.Stop();

    }

    private IEnumerator StartIntro(){ 
        touchUi.SetActive(true);
        yield return new WaitForSeconds(1);
        cameraController.OnClick += DisableIntro;
        yield return new WaitUntil(() => zoomUi.activeSelf == true);
         yield return new WaitForSeconds(1);
        cameraController.OnClick += DisableIntro;
    }

    private void DisableIntro(){

        if(touchUi != null && touchUi.activeSelf) 
        {
            cameraController.OnClick -= DisableIntro;
            touchUi.SetActive(false);
            zoomUi.SetActive(true);
        }
        else if(zoomUi != null && zoomUi.activeSelf) 
        {
            zoomUi.SetActive(false);
            cameraController.OnClick -= DisableIntro;
            return;
        }
    }

    public void SetupModel(ModelType modelType)
    {
        if (modelType == ModelType.None || modelType == currentModelType) return;

        if(currentModel) Destroy(currentModel.gameObject);
        
        ModelObj modelObj = modelList.Find( i => i.modelType == modelType );
        Debug.Log($"ModelObj : {modelObj.modelType}");
        currentModelType = modelType;
        currentModel = Instantiate(modelObj.model, Vector3.zero, Quaternion.identity);
        currentInfoAudio = modelObj.modelinfoAudio;
        currentmodelAudio = modelObj.modelAudio;
        infoPanel_Text.text = modelObj.modelInfoText;
        infoPanelTitle_Text.text = modelObj.modelName;
        infoHeadingPanel_Text.text = modelObj.modelName;
        PlayInfoAudio();
    }

    public void PlayInfoAudio(){
        if(!audioSource.isPlaying){ 
            if(currentInfoAudio == null){
                Debug.Log("No Audio");
                return;
                }
            audioSource.clip = currentInfoAudio;
            audioSource.Play(); 
        }else{
            audioSource.Stop();
        }
    }
    public void PlayModelAudio(){
        if(currentModel == null)return;
        modelAudioSource = currentModel.GetComponent<AudioSource>();
        if(!modelAudioSource.isPlaying){ 
            if(currentmodelAudio == null){
                Debug.Log("No Audio");
                return;
            }
            modelAudioSource.clip = currentmodelAudio ;
            modelAudioSource.Play(); 
        }else{
            modelAudioSource.Stop();
        }
    }

    public void ShowHints() => infoPanel.SetActive(!infoPanel.activeSelf);

    public void InvokeModelButton(ModelType modelType) 
    {
        SetupModel(modelType);
    }
    
    public void InvokeAnimalPanel(bool doCollapse){
        animalPanelClose.SetActive(doCollapse);
        animalPanelOpen.SetActive(!doCollapse);
    }


}

[Serializable] 
public class ModelObj{
    public ModelType modelType;
    public GameObject model;
    public string modelName;
    public string modelInfoText;
    public AudioClip modelAudio;
    public AudioClip modelinfoAudio;
}

public enum ModelType{
    None, 
    Whale, 
    Turtle, 
    Shark,
    JellyFish,
    Orca,
    Narwhal
}