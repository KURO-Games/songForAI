using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rhithm : MonoBehaviour
{
    bool _isTaped = false;
    bool _faded = false;
    // Start is called before the first frame update
    [SerializeField]
    float Times;
    [SerializeField]
    GameObject StartImage;
    [SerializeField]
    GameObject NotesGen;
    float _StartImageColor;
    private bool isCalled = false;
    void Start()
    {
        _isTaped = false;
        _StartImageColor = StartImage.GetComponent<CanvasGroup>().alpha;
        SoundManager.BGMSoundStop();
    }
    public void ReturnHome()
    {
        if (!_isTaped)
        {
            _isTaped = true;
            SceneLoadManager.LoadScene("Home");
        }
    }
    private void FixedUpdate()
    {
        Times += Time.deltaTime;
        if (Times > 5 && !_faded)
        {
            _StartImageColor -= 0.05f;
            StartImage.GetComponent<CanvasGroup>().alpha = _StartImageColor;
            if (StartImage.GetComponent<CanvasGroup>().alpha <= 0)
            {
                StartImage.GetComponent<CanvasGroup>().alpha = 0;
                _faded = true;
                NotesGen.GetComponent<NotesGenerater>().ButtonPush();
            }
        }
    }
    private void Update()
    {
        if(SoundManager.BGMStatus() == CriAtomSource.Status.PlayEnd&&!isCalled)
        {
            isCalled = true;
            SceneLoadManager.LoadScene("Result");
        }
    }
}
