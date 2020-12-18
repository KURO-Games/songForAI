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

    private float _timeCount;
    private bool  _isCalled;
    private bool  _isTaped;
    private bool  _faded;
    private bool  _notesGenerateStarted;

    private NotesGeneratorBase _notesGenerator;
    private CanvasGroup        _startImgCanvasGrp;

    void Start()
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
            NotesGen.GetComponent<NotesGeneratorBase>().NotesGenerate();
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
                    _faded                        = true;
                    _notesGenerator.jacketIsFaded = true;

                    judge.SetActive(true);
                }
            }
        }
    }
    private void Update()
    {
        if (SoundManager.BGMStatus() == CriAtomSource.Status.PlayEnd && !_isCalled)
        {
            _isCalled = true;
            SceneLoadManager.LoadScene("Result");
        }
    }
}
