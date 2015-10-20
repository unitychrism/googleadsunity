using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;
using System.Linq;

public class PostBuildProcessor : MonoBehaviour
{
	#if UNITY_CLOUD_BUILD
	// This method is added in the Advanced Features Settings on UCB
	// PostBuildProcessor.OnPostprocessBuildiOS
	public static void OnPostprocessBuildiOS (string exportPath)
	{
		Debug.Log("[UCB Demos] OnPostprocessBuildiOS");
		ProcessPostBuild(BuildTarget.iPhone,exportPath);
	}
	#endif
	
	
	[PostProcessBuild]
	public static void OnPostprocessBuild (BuildTarget buildTarget, string path)
	{
		#if !UNITY_CLOUD_BUILD
		Debug.Log ("[UNITY_CLOUD_BUILD] OnPostprocessBuild");
		ProcessPostBuild (buildTarget, path);
		#endif
	}
	
	internal static void CopyAndReplaceDirectory (string srcPath, string dstPath)
	{		
		if (!Directory.Exists (dstPath))
			Directory.CreateDirectory (dstPath);
		foreach (var file in Directory.GetFiles(srcPath))
			File.Copy (file, Path.Combine (dstPath, Path.GetFileName (file)));
		
		foreach (var dir in Directory.GetDirectories(srcPath))
			CopyAndReplaceDirectory (dir, Path.Combine (dstPath, Path.GetFileName (dir)));
	}
	
	private static void ProcessPostBuild (BuildTarget buildTarget, string path)
	{
		#if UNITY_IOS
		// We only need this IsSceneActive because we do not want to include the library data in other examples then the LibraryDemo
		if (buildTarget == BuildTarget.iOS && IsSceneActive("Assets/Scene/GoogleAdsUnity.unity")) {

			/*
			Debug.Log ("[UNITY_IOS] ProcessPostBuild - Xcode Manipulation API");
			
			string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
				
			PBXProject proj = new PBXProject ();
			proj.ReadFromString (File.ReadAllText (projPath));
			
			string target = proj.TargetGuidByName ("Unity-iPhone");

			Debug.Log("Enabling modules: CLANG_ENABLE_MODULES = YES");
			proj.AddBuildProperty(target, "CLANG_ENABLE_MODULES", "YES");

			File.WriteAllText (projPath, proj.WriteToString ());
			*/

		}
		#endif
	}
	
	static bool IsSceneActive (string sceneName)
	{
		string[] levels = FillLevels ();
		for (int i = 0; i < levels.Length; ++i) {
			if (levels [i] == sceneName) {
				return true;
			}
		}
		return false;
	}
	
	private static string[] FillLevels ()
	{
		return (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray ();
	}
}

