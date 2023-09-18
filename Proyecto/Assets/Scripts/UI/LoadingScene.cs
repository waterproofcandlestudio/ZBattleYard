using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    float progress;

    [Header("Testing graf")]
    [SerializeField] bool fakeLoading;

    [Header("References")]
    [SerializeField] GameObject loadingContent;
    [SerializeField] Transform movingLoadingThing;
    //[SerializeField] Image loadingFillImage;
    [SerializeField] Text loadingText;
    [SerializeField] Slider loadingSlider;

    [Header("Loading knob positions")]
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;

    void Awake()
    {
        if (fakeLoading)
            loadingContent.SetActive(true);

        else
            loadingContent.SetActive(false);
    }

    void Update()
    {
        if (fakeLoading)
        {
            if (progress < 1f)
            {
                progress += Time.deltaTime * 0.1f;
                float _prosentProgress = progress * 100f;
                loadingText.text = _prosentProgress.ToString("F0") + "%";

                //loadingFillImage.fillAmount = progress;
                loadingSlider.value = progress;
                movingLoadingThing.localPosition = new Vector3(Mathf.Lerp(startPoint.localPosition.x, endPoint.localPosition.x, progress), 0f, 0f);
            }
        }
    }

    public void LoadScene(int _sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(_sceneIndex));
    }

    IEnumerator LoadAsynchronously(int _sceneIndex)
    {
        AsyncOperation _opearation = SceneManager.LoadSceneAsync(_sceneIndex);

        loadingContent.SetActive(true);

        while (!_opearation.isDone)
        {
            float _progress = Mathf.Clamp01(_opearation.progress / 0.9f);
            float _prosentProgress = _progress * 100f;
            //loadingFillImage.fillAmount = _progress;
            loadingSlider.value = _progress;
            loadingText.text = _prosentProgress.ToString("F0") + "%";
            movingLoadingThing.localPosition = new Vector3(Mathf.Lerp(startPoint.localPosition.x, endPoint.localPosition.x, _progress), 0f, 0f);

            progress = _prosentProgress;

            yield return null;
        }
    }

    public float Progress() => progress;
}
