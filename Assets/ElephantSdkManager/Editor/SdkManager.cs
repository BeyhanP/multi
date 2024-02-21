using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ElephantSdkManager.Model;
using ElephantSdkManager.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace ElephantSdkManager
{
    public class SdkManager : EditorWindow
    {
        private const string AssetsPathPrefix = "Assets/";
        private const string DownloadDirectory = AssetsPathPrefix + "ElephantSdkManager";
        private const string GameId_Key = "gamekit_gameId";

        private List<Sdk> _sdkList;

        private EditorCoroutines.EditorCoroutine _editorCoroutine;
        private EditorCoroutines.EditorCoroutine _editorCoroutineSelfUpdate;
        private UnityWebRequest _downloader;
        private UnityWebRequest _gameKitDownloader;
        private string _activity;

        private string _selfUpdateStatus;
        private bool _canUpdateSelf = false;
        private Sdk selfUpdateSdk;
        public string requiredSdks = "";

        private GUIStyle _labelStyle;
        private GUIStyle _headerStyle;
        private readonly GUILayoutOption _fieldWidth = GUILayout.Width(60);

        private Vector2 _scrollPos;
        private GameKitManifest _gameKitManifest;
        private bool _isGameKitRequestFailed;
        
        private string gameid;
        private string tempGameID;

        private string SDKDescription;
        private Vector2 _changeScrollPos;


        private GameData _gameData;

        [MenuItem("Elephant/Manage Core SDK")]
        public static void ShowSdkManager()
        {
            var win = GetWindow<SdkManager>("Manage SDKs");
            win.titleContent = new GUIContent("Elephant Packages");
            win.Focus();
            win.CancelOperation();
            win.OnEnable();
        }

        public void Awake()
        {
            _labelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            _headerStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                fixedHeight = 18
            };

            CancelOperation();
        }

        public void OnEnable()
        {
            PrepareGameData();

            if (!string.IsNullOrEmpty(GetGameId()))
            {
                _editorCoroutine = this.StartCoroutine(FetchManifest());
            }
            
            _editorCoroutineSelfUpdate = this.StartCoroutine(CheckSelfUpdate());
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
        }

        private void PrepareGameData()
        {
            _gameData = Resources.Load<GameData>("GameData");

            if (_gameData == null)
            {
                _gameData = ScriptableObject.CreateInstance<GameData>();
                var path = "Assets/Resources";

                if (!AssetDatabase.IsValidFolder(path))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }

                var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/GameData.asset");

                AssetDatabase.CreateAsset(_gameData, assetPathAndName);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        void OnDisable()
        {
            CancelOperation();
        }

        
        private void ShowLogin()
        {
            using (new EditorGUILayout.VerticalScope("box"))
            using (var s = new EditorGUILayout.ScrollViewScope(_scrollPos, false, false))
            {
                _scrollPos = s.scrollPosition;
                
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    EditorGUILayout.LabelField("Please enter your game ID from Elephant Dashboard",
                        EditorStyles.wordWrappedLabel);
                    tempGameID = EditorGUILayout.TextField("GameID: ", tempGameID);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Space(10);
                        if (GUILayout.Button("Done", _fieldWidth))
                        {
                            gameid = tempGameID;
                            SetGameId(gameid);
                            _editorCoroutine = this.StartCoroutine(FetchManifest());
                        }
                    }
                }


            }
        }

        public void OnGUI()
        {
            if (string.IsNullOrEmpty(GetGameId()))
            {
                ShowLogin();
                return;
            }
            
            var stillWorking = _editorCoroutine != null || _downloader != null;

            if (_sdkList != null && _sdkList.Count > 0)
            {

                using (new EditorGUILayout.VerticalScope("box"))
                using (var s = new EditorGUILayout.ScrollViewScope(_scrollPos, false, false))
                {
                    _scrollPos = s.scrollPosition;

                    var groupedSdkList = _sdkList
                        .GroupBy(item => item.type)
                        .Select(group => group.ToList())
                        .ToList();

                    foreach (var groupSdk in groupedSdkList)
                    {
                        switch (groupSdk[0].type)
                        {
                            case "internal":
                                PopulateGroupSdks(groupSdk, "Elephant SDKs");
                                break;
                            case "is":
                                PopulateGroupSdks(groupSdk, "IronSource");
                                break;
                            case "max":
                                PopulateGroupSdks(groupSdk, "Applovin MAX");
                                break;
                            case "network":
                                PopulateGroupSdks(groupSdk, "Networks");
                                break;
                            default:
                                PopulateGroupSdks(groupSdk, "Elephant SDKs");
                                break;
                        }
                    }
                    PopulateSdkDescription();
                }
            }

            // Indicate async operation in progress.
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
            {
                EditorGUILayout.LabelField(stillWorking ? _activity : " ");
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(10);
                if (GUILayout.Button("Reset Game Id", GUILayout.Width(120)))
                    ResetGameId();

                GUILayout.Space(10);
            }

            GUILayout.Space(10);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(10);
                if (!stillWorking)
                {
                    if (GUILayout.Button("Done", _fieldWidth))
                        Close();
                }
                else
                {
                    if (GUILayout.Button("Cancel", _fieldWidth))
                        CancelOperation();
                }
            }

            GUILayout.Space(10);


            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(_selfUpdateStatus);

                if (selfUpdateSdk != null)
                {
                    EditorGUILayout.LabelField(selfUpdateSdk.version ?? "--");
                    GUI.enabled = _canUpdateSelf;
                    if (GUILayout.Button(new GUIContent
                        {
                            text = "Update",
                        }, _fieldWidth))
                        this.StartCoroutine(DownloadSdkManager(selfUpdateSdk));
                    GUI.enabled = true;
                }
            }

            GUILayout.Space(10);
        }

        private void PopulateGroupSdks(List<Sdk> groupedSdkList, string groupTitle)
        {
            GUILayout.Space(5);
            EditorGUILayout.LabelField(groupTitle, _labelStyle, GUILayout.Height(20));
            
            if (groupTitle.Equals("Elephant SDKs"))
            {
                groupedSdkList = groupedSdkList.OrderBy(sdk => sdk.sdkName).ToList();
            }

            using (new EditorGUILayout.VerticalScope("box"))
            {
                SdkHeaders();
                foreach (var sdk in groupedSdkList)
                {
                    SdkRow(sdk);
                }
            }
        }

        private void PopulateSdkDescription()
        {
            GUILayout.Space(30);
            if (!string.IsNullOrEmpty(SDKDescription))
            {
                using (new EditorGUILayout.VerticalScope("box", GUILayout.ExpandHeight(true)))
                {
                    GUILayout.Space(5);
                    using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
                    {
                        using (var s = new EditorGUILayout.ScrollViewScope(_changeScrollPos, false, false))
                        {
                            _changeScrollPos = s.scrollPosition;
                            GUILayout.Label(SDKDescription, EditorStyles.textField, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                        }
                    }
                    GUILayout.Space(5);
                }
                using (new EditorGUILayout.VerticalScope(GUILayout.ExpandHeight(false)))
                {
                    GUILayout.Space(15);
                }
            }
        }

        private void SdkHeaders()
        {
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Package", _headerStyle);
                GUILayout.Button("Current Version", _headerStyle);
                GUILayout.Button("Latest Version", _headerStyle);
                GUILayout.Space(3);
                GUILayout.Button("Action", _headerStyle, _fieldWidth);
                GUILayout.Button(" ", _headerStyle, GUILayout.Width(1));
                GUILayout.Space(5);
            }

            GUILayout.Space(4);
        }

        private void SdkRow(Sdk sdkInfo, Func<bool, bool> customButton = null)
        {
            var sdkName = sdkInfo.sdkName;
            var latestVersion = sdkInfo.version.Replace("v", string.Empty);
            var cur = sdkInfo.currentVersion;
            cur = !string.IsNullOrEmpty(cur) ? sdkInfo.currentVersion.Replace("v", string.Empty) : "";
            var isInst = !string.IsNullOrEmpty(cur);
            var canInst = !string.IsNullOrEmpty(latestVersion) &&
                          (!isInst || !string.Equals(cur, latestVersion));

            var stillWorking = _editorCoroutine != null || _downloader != null;

            GUILayout.Space(4);
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField(new GUIContent { text = sdkName });
                GUILayout.Space(100);
                GUILayout.Button(new GUIContent
                {
                    text = string.IsNullOrEmpty(cur) ? "--" : cur,
                }, canInst ? EditorStyles.boldLabel : EditorStyles.label);
                GUILayout.Space(100);
                GUILayout.Button(new GUIContent
                {
                    text = latestVersion ?? "--",
                }, canInst ? EditorStyles.boldLabel : EditorStyles.label);
                GUILayout.Space(3);
                if (customButton == null || !customButton(canInst))
                {
                    GUI.enabled = !stillWorking && (canInst) && !VersionUtils.IsEqualVersion(cur, latestVersion);
                    if (GUILayout.Button(new GUIContent
                        {
                            text = isInst ? "Upgrade" : "Install",
                        }, _fieldWidth))
                    {
                        if (!IsInstallAvailable(sdkInfo))
                        {
                            EditorUtility.DisplayDialog("Warning!",
                                "Please install or upgrade required SDKs first:\n" + requiredSdks, "OK");
                            return;
                        }

                        this.StartCoroutine(DownloadSDK(sdkInfo));
                    }

                    

                    GUI.enabled = true;
                }

                GUILayout.Space(5);
                if (GUILayout.Button("Re-Install", GUILayout.Width(80)))
                {
                    GUI.enabled = true;
                    this.StartCoroutine(DownloadSDK(sdkInfo));
                }
            }

            GUILayout.Space(4);
        }
        
        private IEnumerator CheckSelfUpdate()
        {
            yield return null;
            _selfUpdateStatus = "Checking for new versions of SDK Manager";

            var unityWebRequest = new UnityWebRequest(ManifestSource.SelfUpdateURL)
            {
                downloadHandler = new DownloadHandlerBuffer(),
                timeout = 240,
            };

            if (!string.IsNullOrEmpty(unityWebRequest.error))
            {
                Debug.LogError(unityWebRequest.error);
            }

            yield return unityWebRequest.SendWebRequest();

            var responseJson = unityWebRequest.downloadHandler.text;

            if (string.IsNullOrEmpty(responseJson))
            {
                Debug.LogError("Unable to retrieve SDK Manager manifest");

                yield break;
            }

            unityWebRequest.Dispose();

            selfUpdateSdk = JsonUtility.FromJson<Sdk>(responseJson);
            
            if (selfUpdateSdk == null) yield break;
            
            var currentVersion = ElephantSdkManagerVersion.SDK_VERSION.Replace("v", string.Empty);
            var latestVersion = selfUpdateSdk.version.Replace("v", string.Empty);

            if (VersionUtils.CompareVersions(currentVersion, latestVersion) < 0
                && !VersionUtils.IsEqualVersion(currentVersion, latestVersion))
            {
                _selfUpdateStatus = "New Version Available";
                _canUpdateSelf = true;
            }
            else
            {
                _selfUpdateStatus = "Up To Date";
                _canUpdateSelf = false;
            }

        }
        
        private IEnumerator FetchManifest()
        {
            if (string.IsNullOrEmpty(GetGameId()))
            {
                EditorUtility.DisplayDialog("ERROR", "GameID is empty! Please enter a valid GameID", "OK");
                yield break;
            }
            
            yield return null;
            _activity = "Downloading SDK version manifest...";

            var unityWebRequest = new UnityWebRequest(ManifestSource.ManifestURL + GetGameId() + "&version=" +
                                                      ElephantSdkManagerVersion.SDK_VERSION)
            {
                downloadHandler = new DownloadHandlerBuffer(),
                timeout = 240,
            };

            if (!string.IsNullOrEmpty(unityWebRequest.error))
            {
                Debug.LogError(unityWebRequest.error);
            }

            yield return unityWebRequest.SendWebRequest();

            var responseJson = unityWebRequest.downloadHandler.text;

            if (string.IsNullOrEmpty(responseJson))
            {
                Debug.LogError("Unable to retrieve SDK version manifest.  Showing installed SDKs only.");

                yield break;
            }

            unityWebRequest.Dispose();

            var manifest = JsonUtility.FromJson<Manifest>(responseJson);

            if (manifest.sdkList.Count == 0)
            {
                EditorUtility.DisplayDialog("ERROR", "Wrong GameID! Please enter a valid GameID", "OK");
                ResetGameId();
                yield break;
            }

            _sdkList = manifest.sdkList;

            string descriptionURL;

            if (_sdkList[0].sdkName.Equals("GameKit-IS"))
            {
                descriptionURL = ManifestSource.ISChangeLogURL;
            }
            else
            {
                descriptionURL = ManifestSource.MAXChangeLogURL;
            }

            UnityWebRequest unityWebRequestDescriptionJson = UnityWebRequest.Get(descriptionURL);
            var webRequestSDKDescription = unityWebRequestDescriptionJson.SendWebRequest();

            while (!webRequestSDKDescription.isDone)
            {
                yield return null;
            }

            var SDKDescriptionJson = unityWebRequestDescriptionJson.downloadHandler.text;
            ChangeLog changeLogInstance = JsonUtility.FromJson<ChangeLog>(SDKDescriptionJson);
            SDKDescription = changeLogInstance.description;

            CheckVersions();
        }

        private void CheckVersions()
        {
            if (!Directory.Exists(AssetsPathPrefix + "Elephant"))
            {
                Sdk gamekitSdk = _sdkList.Find(sdk => sdk.sdkName.Equals("GameKit-MAX"));
                gamekitSdk.currentVersion = "";
                
                Assembly assemblyForAds = Assembly.GetExecutingAssembly();
                foreach (var type in assemblyForAds.GetTypes())
                {
                    if (type.FullName == null) return;

                    if (type.FullName.Equals("RollicGames.Advertisements.GameKitVersion"))
                    {
                        var fieldInfo = type.GetField("GAMEKIT_VERSION",
                            BindingFlags.NonPublic | BindingFlags.Static);
                        var gameKit = _sdkList.Find(sdk => sdk.sdkName.ToLower().Contains("gamekit"));
                        if (!(fieldInfo is null)) gameKit.currentVersion = fieldInfo.GetValue(null).ToString();
                    }
                }
                _editorCoroutine = null;
                Repaint();
                return;
            }
            
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            foreach (var type in myAssembly.GetTypes())
            {
                if (type.FullName == null) return;

                if (type.FullName.Equals("RollicGames.Advertisements.GameKitVersion"))
                {
                    var fieldInfo = type.GetField("GAMEKIT_VERSION",
                        BindingFlags.NonPublic | BindingFlags.Static);
                    var gameKit = _sdkList.Find(sdk => sdk.sdkName.ToLower().Contains("gamekit"));
                    if (!(fieldInfo is null)) gameKit.currentVersion = fieldInfo.GetValue(null).ToString();
                }
            }
            
            
            AssetDatabase.Refresh();
            _editorCoroutine = null;
            Repaint();
        }
        

        private IEnumerator DownloadSdkManager(Sdk sdkInfo)
        {
            var path = Path.Combine(AssetsPathPrefix, sdkInfo.sdkName + "new");
            _activity = $"Downloading {sdkInfo.sdkName}...";

            // Start the async download job.
            _downloader = new UnityWebRequest(sdkInfo.downloadUrl)
            {
                downloadHandler = new DownloadHandlerFile(path),
                timeout = 240, // seconds
            };
            _downloader.SendWebRequest();
            
            while (!_downloader.isDone)
            {
                yield return null;
                var progress = Mathf.FloorToInt(_downloader.downloadProgress * 100);
                if (EditorUtility.DisplayCancelableProgressBar("Elephant SDK Manager", _activity, progress))
                    _downloader.Abort();
            }
            
            EditorUtility.ClearProgressBar();
            
            if (string.IsNullOrEmpty(_downloader.error))
            {
                if (Directory.Exists(AssetsPathPrefix + sdkInfo.sdkName))
                {
                    FileUtil.DeleteFileOrDirectory(AssetsPathPrefix + sdkInfo.sdkName);
                }

                AssetDatabase.ImportPackage(path, true);
                FileUtil.DeleteFileOrDirectory(path);
            }
            
            _downloader.Dispose();
            _downloader = null;
            _editorCoroutine = null;

            yield return null;
        }

        private string GetGameId()
        {
            return _gameData.gameId;
        }

        private void SetGameId(string gameId)
        {
            _gameData.gameId = gameId;
            EditorUtility.SetDirty(_gameData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void ResetGameId()
        {
            _gameData.gameId = "";
            EditorUtility.SetDirty(_gameData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private IEnumerator GetGameKitIDs()
        {
            _isGameKitRequestFailed = false;

            _gameKitDownloader = new UnityWebRequest(ManifestSource.GameKitURL + GetGameId() + "/config")
            {
                downloadHandler = new DownloadHandlerBuffer(),
                timeout = 240,
            };

            if (!string.IsNullOrEmpty(_gameKitDownloader.error))
            {
                _isGameKitRequestFailed = true;
                Debug.LogError("Unable to retrieve GameKit IDs.");
            }

            yield return _gameKitDownloader.SendWebRequest();

            while (!_gameKitDownloader.isDone)
            {
                yield return null;
            }

            var responseJson = _gameKitDownloader.downloadHandler.text;

            if (string.IsNullOrEmpty(responseJson))
            {
                Debug.LogError("Unable to retrieve GameKit IDs.");
                _isGameKitRequestFailed = true;
                yield break;
            }

            _gameKitDownloader.Dispose();
            _gameKitDownloader = null;
            _gameKitManifest = JsonUtility.FromJson<GameKitManifest>(responseJson);

            if (_gameKitManifest == null || _gameKitManifest.data == null || _gameKitManifest.data.appKey == null)
            {
                _isGameKitRequestFailed = true;
                Debug.LogError(
                    "Unable to retrieve GameKit IDs. Please set your IDs in RollicGames/RollicApplovinIDs.cs file!");
            }
        }

        private IEnumerator DownloadSDK(Sdk sdkInfo)
        {
            if (sdkInfo.sdkName.ToLower().Contains("gamekit"))
            {
                this.StartCoroutine(GetGameKitIDs());
            }
            
            var path = Path.Combine(DownloadDirectory, sdkInfo.sdkName + ".unitypackage");

            if (sdkInfo.downloadUrl.Contains("xml"))
            {
                path = "Assets/IronSource/Editor/" + sdkInfo.sdkName + "Dependencies.xml";
            }
            
            _activity = $"Downloading {sdkInfo.sdkName}...";

            // Start the async download job.
            _downloader = new UnityWebRequest(sdkInfo.downloadUrl)
            {
                downloadHandler = new DownloadHandlerFile(path),
                timeout = 240, // seconds
            };
            _downloader.SendWebRequest();

            // Pause until download done/cancelled/fails, keeping progress bar up to date.
            while (!_downloader.isDone)
            {
                yield return null;
                var progress = Mathf.FloorToInt(_downloader.downloadProgress * 100);
                if (EditorUtility.DisplayCancelableProgressBar("Elephant SDK Manager", _activity, progress))
                    _downloader.Abort();
            }

            EditorUtility.ClearProgressBar();

            if (string.IsNullOrEmpty(_downloader.error))
            {
                if (Directory.Exists(AssetsPathPrefix + sdkInfo.sdkName))
                {
                    FileUtil.DeleteFileOrDirectory(AssetsPathPrefix + sdkInfo.sdkName);
                }

                if (sdkInfo.sdkName.ToLower().Contains("gamekit"))
                {
                    if (Directory.Exists(AssetsPathPrefix + "Elephant"))
                    {
                        FileUtil.DeleteFileOrDirectory(AssetsPathPrefix + "Elephant");
                    }

                    if (Directory.Exists(AssetsPathPrefix + "RollicGames"))
                    {
                        FileUtil.DeleteFileOrDirectory(AssetsPathPrefix + "RollicGames");
                    }
                    
                    if (Directory.Exists(AssetsPathPrefix + "MaxSdk"))
                    {
                        FileUtil.DeleteFileOrDirectory(AssetsPathPrefix + "MaxSdk");
                    }
                    
                    if (Directory.Exists(AssetsPathPrefix + "IronSource"))
                    {
                        FileUtil.DeleteFileOrDirectory(AssetsPathPrefix + "IronSource");
                    }
                    
                    if (Directory.Exists(AssetsPathPrefix + "ExternalDependencyManager"))
                    {
                        FileUtil.DeleteFileOrDirectory(AssetsPathPrefix + "ExternalDependencyManager");
                    }
                    
                    if (Directory.Exists(AssetsPathPrefix + "PlayServicesResolver"))
                    {
                        FileUtil.DeleteFileOrDirectory(AssetsPathPrefix + "PlayServicesResolver");
                    }
                }

                if (!path.Contains("xml"))
                {
                    AssetDatabase.ImportPackage(path, true);
                    FileUtil.DeleteFileOrDirectory(path);
                }
                else
                {
                    CheckVersions();
                }
            }
            
            _downloader.Dispose();
            _downloader = null;
            _editorCoroutine = null;

            yield return null;
        }
        
        private void OnImportPackageCompleted(string packageName)
        {
            CheckVersions();
            CheckPackage(packageName);
        }

        private void CheckPackage(string packageName)
        {
            if (!packageName.ToLower().Contains("gamekit")) return;
            if (_isGameKitRequestFailed)
            {
                EditorUtility.DisplayDialog("ERROR", "Unable to retrieve GameKit IDs. " +
                                                     "Please set your IDs in RollicGames/RollicApplovinIDs.cs file!",
                    "OK");
                return;
            }

            VersionUtils.SetupElephantThirdPartyIDs(_gameKitManifest, packageName);
            VersionUtils.SetupGameKitIDs(_gameKitManifest, packageName);
        }

        private bool IsInstallAvailable(Sdk dependentSdk)
        {
            requiredSdks = "";
            if (_sdkList == null || _sdkList.Count == 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(dependentSdk.depends_on)) return true;

            var requiredSDK = _sdkList.Find(sdk => sdk.sdkName.Equals(dependentSdk.depends_on));

            if (string.IsNullOrEmpty(requiredSDK?.currentVersion))
            {
                requiredSdks = requiredSDK?.sdkName;
                return false;
            }

            if (VersionUtils.CompareVersions(requiredSDK.currentVersion.Replace("v", string.Empty),
                    dependentSdk.depending_sdk_version) >= 0)
            {
                return true;
            }

            requiredSdks = requiredSDK.sdkName;
            return false;
        }

        private void CancelOperation()
        {
            // Stop any async action taking place.
            if (_downloader != null)
            {
                _downloader.Abort(); // The coroutine should resume and clean up.
                return;
            }

            if (_editorCoroutine != null)
                this.StopCoroutine(_editorCoroutine.routine);
            
            if (_editorCoroutineSelfUpdate != null)
                this.StopCoroutine(_editorCoroutineSelfUpdate.routine);

            _editorCoroutineSelfUpdate = null;
            _editorCoroutine = null;
            _downloader = null;
        }
    }
}