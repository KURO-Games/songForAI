using UnityEditor.PackageManager;
using UnityEngine;

public class Rhithm : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float Times;

    [SerializeField]
    private GameObject StartImage;

    [SerializeField]
    private GameObject NotesGen;

    [SerializeField]
    private GameObject judge;

    [SerializeField]
    private GameObject playEffect;

    private float _timeCount;
    private bool  _isCalled;
    private bool  _isTaped;
    private bool  _faded;
    private bool  _notesGenerateStarted;

    private NotesGeneratorBase _notesGenerator;
    private CanvasGroup        _startImgCanvasGrp;

    private void Start()
    {
        _notesGenerator    = NotesGen.GetComponent<NotesGeneratorBase>();
        _startImgCanvasGrp = StartImage.GetComponent<CanvasGroup>();

        SoundManager.AllBGMSoundStop();
        //PlayerPrefs.SetInt("Life", PlayerPrefs.GetInt("Life") - 1);
        Application.targetFrameRate = 60;
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
        if (!_notesGenerateStarted)
        {
            _notesGenerateStarted = true;
            _notesGenerator.NotesGenerate();
        }

        Times += Time.fixedDeltaTime;

        if (Times > 3 && !_faded)
        {
            _startImgCanvasGrp.alpha -= .05f;

            if (_startImgCanvasGrp.alpha <= 0)
            {
                _startImgCanvasGrp.alpha =  0;
                _timeCount                += Time.fixedDeltaTime;

                if(_timeCount > 2)
                {
                    _faded                           = true;
                    NotesGeneratorBase.jacketIsFaded = true;

                    judge.SetActive(true);
                    playEffect.SetActive(true);
                }
            }
        }
    }
    private void Update()
    {
#if SFAI_SOUND
        if (SoundManager.BGMStatus() == CriAtomExPlayer.Status.PlayEnd && !_isCalled)
#else
        if (SoundManager.BGMStatus() == CriAtomSource.Status.PlayEnd && !_isCalled)
#endif
        {
            _isCalled = true;
            SceneLoadManager.LoadScene("Result");
        }
    }
}
