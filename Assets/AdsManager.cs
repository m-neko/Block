using System.Threading;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string androidGameId;
    [SerializeField] string iOSGameId;
    [SerializeField] string androidAdUnitId = "Interstitial_Android";
    [SerializeField] string iOsAdUnitId = "Interstitial_iOS";
    [SerializeField] bool testMode = false;

    string gameId;
    string adUnitId;
    const int SHOW_WAIT_TIME = 5000;

	void Awake()
	{
        if(Application.platform == RuntimePlatform.IPhonePlayer){
            gameId = iOSGameId;
            adUnitId = iOsAdUnitId;
        }else{
            gameId = androidGameId;
            adUnitId = androidAdUnitId;
        }

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId, testMode, this);
            Advertisement.Load(adUnitId, this);
        }
	}

	void Start()
	{
        Thread.Sleep(SHOW_WAIT_TIME);
        Advertisement.Show(adUnitId, this);  	
	}
	
	void Update()
	{
	}

    public void OnInitializationComplete()
    {
        Debug.Log("OnInitializationComplete()");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message) {}
	
	public void OnUnityAdsAdLoaded(string adUnitId) {}
	public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message) {}
	
	public void OnUnityAdsShowStart(string adUnitId)
    {
        Debug.Log("OnUnityAdsShowStart()");
    }

	public void OnUnityAdsShowClick(string adUnitId) {}
	
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("OnUnityAdsShowComplete()");
    }

	public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message) {}
	
}