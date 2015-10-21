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
	/**
	 * Runs when Post-Export method has been set to 
	 * 'PostBuildProcessor.OnPostprocessBuildiOS' in your Unity Cloud Build
	 * target settings.
	 */
	#if UNITY_CLOUD_BUILD
	// This method is added in the Advanced Features Settings on UCB
	// PostBuildProcessor.OnPostprocessBuildiOS
	public static void OnPostprocessBuildiOS (string exportPath)
	{
		Debug.Log("[UCB Demos] OnPostprocessBuildiOS");
		ProcessPostBuild(BuildTarget.iOS,exportPath);
	}

	public static void OnPreprocessBuildiOS()
	{
		Debug.Log("[UCB Demos] Here is my pre-export method running!");
	}
	#endif
	
	/**
	 * Runs after successful build of an iOS-targetted Unity project
	 * via the editor Build dialog.
	 */
	[PostProcessBuild]
	public static void OnPostprocessBuild (BuildTarget buildTarget, string path)
	{
		#if !UNITY_CLOUD_BUILD
		Debug.Log ("[UNITY_CLOUD_BUILD] OnPostprocessBuild");
		ProcessPostBuild (buildTarget, path);
		#endif
	}

	/**
	 * This ProcessPostBuild method will run via Unity Cloud Build, as well as 
	 * locally when build target is iOS. Using the Xcode Manipulation API, it is
	 * possible to modify build settings values and also perform other actions
	 * such as adding custom frameworks. Link below is the reference documentation
	 * for the Xcode Manipulation API:
	 * 
	 * http://docs.unity3d.com/ScriptReference/iOS.Xcode.PBXProject.html
	 */
	private static void ProcessPostBuild (BuildTarget buildTarget, string path)
	{
		// This code will set modules to enabled in Xcode build settings
		#if UNITY_IOS
		
		Debug.Log ("[UNITY_IOS] ProcessPostBuild - Xcode Manipulation API");
		
		// Go get pbxproj file
		string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
		
		// PBXProject class represents a project build settings file,
		// here is how to read that in.
		PBXProject proj = new PBXProject ();
		proj.ReadFromFile (projPath);
		
		// This is the Xcode target in the generated project
		string target = proj.TargetGuidByName ("Unity-iPhone");
		
		// Here we go: Set 'Enable Modules' to YES to prevent errors using @import syntax
		Debug.Log("Enabling modules: CLANG_ENABLE_MODULES = YES");
		proj.AddBuildProperty(target, "CLANG_ENABLE_MODULES", "YES");
		
		// Write PBXProject object back to the file
		proj.WriteToFile (projPath);
		
		#endif
	}
}

