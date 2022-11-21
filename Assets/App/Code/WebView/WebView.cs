using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebView : MonoBehaviour
{
    [SerializeField] private Texture _backButtonTexture;
    
    //Required to check for a SIM card
    //private const string PluginName = "com.evgenindev.simdetector.Detector";
    
    //WebView Component
    private WebViewObject _webViewObject;
    private GUIStyle _buttonGuiStyle;
    
    //private static AndroidJavaClass _pluginClass;
    private static AndroidJavaObject _pluginInstance;
    private static AndroidJavaClass _unityPlayer;
    private static AndroidJavaObject _unityActivity;

    public string Url { get; private set; }

    // Need uncomment when use of SIM check
    /*public static AndroidJavaClass PluginClass
    {
        get
        {
            if(_pluginClass == null)
            {
                _pluginClass = new AndroidJavaClass(PluginName);
            }
            return _pluginClass;
        }
    }*/
    
    /*public static AndroidJavaObject PluginInstance
    {
        get
        {
            if( _pluginInstance == null)
            {
                _pluginInstance = PluginClass.CallStatic<AndroidJavaObject>("getInstance");
            }
            return _pluginInstance;
        }
    }*/
    
    public static AndroidJavaClass UnityPlayer
    {
        get
        {
            if(_unityPlayer == null)
            {
                _unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            }
            return _unityPlayer;
        }
    }
    
    public static AndroidJavaObject UnityActivity
    {
        get
        {
            if(_unityActivity == null)
            {
                _unityActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
            return _unityActivity;
        }
    }
    
    public void StartWebPageAsync( string url )
    {
        Url = url.Trim();

        InitStyles();
        StartCoroutine( StartWebPage() );
    }

    public bool IsBrandDevice()
    {
        return SystemInfo.deviceModel.ToLower().Contains( "google" );
    }

    public bool IsEmu()
    {
        return false;
        
        /*if ( BuildConfig.DEBUG ) return false // when developer use this build on
        emulator
        val phoneModel = Build.MODEL val buildProduct = Build.PRODUCT
        val buildHardware = Build.HARDWARE
        var result = (Build.FINGERPRINT.startsWith( "generic" )
                   || phoneModel.contains( "google_sdk" )
                   || phoneModel.lowercase( Locale.getDefault() ).contains( "droid4x" )
                   || phoneModel.contains( "Emulator" )
                   || phoneModel.contains( "Android SDK built for x86" )
                   || Build.MANUFACTURER.contains( "Genymotion" )
                   || buildHardware == "goldfish"
                   || buildHardware == "vbox86"
                   || buildProduct == "sdk"
                   || buildProduct == "google_sdk"
                   || buildProduct == "sdk_x86"
                   || buildProduct == "vbox86p"
                   || Build.BOARD.lowercase( Locale.getDefault() ).contains( "nox" )
                   || Build.BOOTLOADER.lowercase( Locale.getDefault() ).contains( "nox" )
                   || buildHardware.lowercase( Locale.getDefault() ).contains( "nox" )
                   || buildProduct.lowercase( Locale.getDefault() ).contains( "nox" ))
        if ( result ) return true
        result = result or( Build.BRAND.startsWith( "generic" ) &&
                            Build.DEVICE.startsWith( "generic" ) ) if ( result ) return true
        result = result or( "google_sdk" == buildProduct ) return result*/
    }

    public bool GetSimStatus()
    {
        return true;
        
        /*int sim = PluginInstance.Call<int>("getSimStatus", UnityActivity);
        return sim == 1;*/
    }

    private IEnumerator StartWebPage()
    {
        _webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        _webViewObject.Init(
            cb: (msg) =>
            {
                Debug.Log(string.Format("CallFromJS[{0}]", msg));
            },
            err: (msg) =>
            {
                Debug.Log(string.Format("CallOnError[{0}]", msg));
            },
            httpErr: (msg) =>
            {
                Debug.Log(string.Format("CallOnHttpError[{0}]", msg));
            },
            started: (msg) =>
            {
                Debug.Log(string.Format("CallOnStarted[{0}]", msg));
            },
            hooked: (msg) =>
            {
                Debug.Log(string.Format("CallOnHooked[{0}]", msg));
            },
            ld: (msg) =>
            {
                Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
#if UNITY_EDITOR_OSX || (!UNITY_ANDROID && !UNITY_WEBPLAYER && !UNITY_WEBGL)
                // NOTE: depending on the situation, you might prefer
                // the 'iframe' approach.
                // cf. https://github.com/gree/unity-webview/issues/189
#if true
                webViewObject.EvaluateJS(@"
                  if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                    window.Unity = {
                      call: function(msg) {
                        window.webkit.messageHandlers.unityControl.postMessage(msg);
                      }
                    }
                  } else {
                    window.Unity = {
                      call: function(msg) {
                        window.location = 'unity:' + msg;
                      }
                    }
                  }
                ");
#else
                webViewObject.EvaluateJS(@"
                  if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                    window.Unity = {
                      call: function(msg) {
                        window.webkit.messageHandlers.unityControl.postMessage(msg);
                      }
                    }
                  } else {
                    window.Unity = {
                      call: function(msg) {
                        var iframe = document.createElement('IFRAME');
                        iframe.setAttribute('src', 'unity:' + msg);
                        document.documentElement.appendChild(iframe);
                        iframe.parentNode.removeChild(iframe);
                        iframe = null;
                      }
                    }
                  }
                ");
#endif
#elif UNITY_WEBPLAYER || UNITY_WEBGL
                webViewObject.EvaluateJS(
                    "window.Unity = {" +
                    "   call:function(msg) {" +
                    "       parent.unityWebView.sendMessage('WebViewObject', msg)" +
                    "   }" +
                    "};");
#endif
                _webViewObject.EvaluateJS(@"Unity.call('ua=' + navigator.userAgent)");
            }
            //transparent: false,
            //zoom: true,
            //ua: "custom user agent string",
            //// android
            //androidForceDarkMode: 0,  // 0: follow system setting, 1: force dark off, 2: force dark on
            //// ios
            //enableWKWebView: true,
            //wkContentMode: 0,  // 0: recommended, 1: mobile, 2: desktop
            //wkAllowsLinkPreview: true,
            //// editor
            //separated: false
            );
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        webViewObject.bitmapRefreshCycle = 1;
#endif
        // cf. https://github.com/gree/unity-webview/pull/512
        // Added alertDialogEnabled flag to enable/disable alert/confirm/prompt dialogs. by KojiNakamaru � Pull Request #512 � gree/unity-webview
        //webViewObject.SetAlertDialogEnabled(false);

        // cf. https://github.com/gree/unity-webview/pull/728
        //webViewObject.SetCameraAccess(true);
        //webViewObject.SetMicrophoneAccess(true);

        // cf. https://github.com/gree/unity-webview/pull/550
        // introduced SetURLPattern(..., hookPattern). by KojiNakamaru � Pull Request #550 � gree/unity-webview
        //webViewObject.SetURLPattern("", "^https://.*youtube.com", "^https://.*google.com");

        // cf. https://github.com/gree/unity-webview/pull/570
        // Add BASIC authentication feature (Android and iOS with WKWebView only) by takeh1k0 � Pull Request #570 � gree/unity-webview
        //webViewObject.SetBasicAuthInfo("id", "password");

        //webViewObject.SetScrollbarsVisibility(true);

        int marginTop = Mathf.FloorToInt(Screen.height * 170f / 1920f);
        
        _webViewObject.SetMargins(0, marginTop, 0, 0);
        _webViewObject.SetTextZoom(100);  // android only. cf. https://stackoverflow.com/questions/21647641/android-webview-set-font-size-system-default/47017410#47017410
        _webViewObject.SetVisibility(true);

#if !UNITY_WEBPLAYER && !UNITY_WEBGL
        if (Url.StartsWith("http"))
        {
            _webViewObject.LoadURL(Url.Replace(" ", "%20"));
        }
        else
        {
            var exts = new string[]{
                ".jpg",
                ".js",
                ".html"  // should be last
            };
            foreach (var ext in exts)
            {
                var url = Url.Replace(".html", ext);
                var src = System.IO.Path.Combine(Application.streamingAssetsPath, url);
                var dst = System.IO.Path.Combine(Application.persistentDataPath, url);
                byte[] result = null;
                
                Debug.Log( $"src : [{src}]" );
                
                if (src.Contains("://"))
                {  // for Android
#if UNITY_2018_4_OR_NEWER
                    // NOTE: a more complete code that utilizes UnityWebRequest can be found in https://github.com/gree/unity-webview/commit/2a07e82f760a8495aa3a77a23453f384869caba7#diff-4379160fa4c2a287f414c07eb10ee36d
                    var unityWebRequest = UnityWebRequest.Get(src);
                    yield return unityWebRequest.SendWebRequest();
                    result = unityWebRequest.downloadHandler.data;
#else
                    var www = new WWW(src);
                    yield return www;
                    result = www.bytes;
#endif
                }
                else
                {
                    result = System.IO.File.ReadAllBytes(src);
                }
                System.IO.File.WriteAllBytes(dst, result);
                if (ext == ".html")
                {
                    _webViewObject.LoadURL("file://" + dst.Replace(" ", "%20"));
                    break;
                }
            }
        }
#else
        if (Url.StartsWith("http")) {
            webViewObject.LoadURL(Url.Replace(" ", "%20"));
        } else {
            webViewObject.LoadURL("StreamingAssets/" + Url.Replace(" ", "%20"));
        }
#endif
        yield break;
    }

    private void InitStyles()
    {
        _buttonGuiStyle = new GUIStyle();
    }

    void OnGUI()
    {
        var size = Screen.height * 150f / 1920f;
        var offset = Screen.height * 20f / 1920f;
        var x = offset;

        GUI.enabled = _webViewObject.CanGoBack();
        if ( GUI.Button( new Rect( x, offset, size, size ), _backButtonTexture, _buttonGuiStyle ) )
        {
            _webViewObject.GoBack();
        }

        /*GUI.enabled = true;
        x += 90;

        GUI.enabled = _webViewObject.CanGoForward();
        if ( GUI.Button( new Rect( x, 10, 80, 80 ), ">" ) )
        {
            _webViewObject.GoForward();
        }

        GUI.enabled = true;
        x += 90;

        if ( GUI.Button( new Rect( x, 10, 80, 80 ), "r" ) )
        {
            _webViewObject.Reload();
        }

        x += 90;

        GUI.TextField( new Rect( x, 10, 180, 80 ), "" + _webViewObject.Progress() );
        x += 190;

        if ( GUI.Button( new Rect( x, 10, 80, 80 ), "*" ) )
        {
            var g = GameObject.Find( "WebViewObject" );
            if ( g != null )
            {
                Destroy( g );
            }
            else
            {
                StartCoroutine( StartWebPage() );
            }
        }

        x += 90;

        if ( GUI.Button( new Rect( x, 10, 80, 80 ), "c" ) )
        {
            Debug.Log( _webViewObject.GetCookies( Url ) );
        }

        x += 90;

        if ( GUI.Button( new Rect( x, 10, 80, 80 ), "x" ) )
        {
            _webViewObject.ClearCookies();
        }

        x += 90;

        if ( GUI.Button( new Rect( x, 10, 80, 80 ), "D" ) )
        {
            _webViewObject.SetInteractionEnabled( false );
        }

        x += 90;

        if ( GUI.Button( new Rect( x, 10, 80, 80 ), "E" ) )
        {
            _webViewObject.SetInteractionEnabled( true );
        }

        x += 90;*/
    }
}
