﻿#if UNITY_EDITOR && !UNITY_WEBPLAYER && UNITY_IOS
using UnityEngine;

using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using AASUnityEditor.XCodeEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class UPTracePostBuild
{
	[PostProcessBuild]
	public static void OnPostProcessBuild(BuildTarget target2, string path)
	{
#if UNITY_5 || UNITY_5_3_OR_NEWER
		if (target2 == BuildTarget.iOS)
#else
if (target2 == BuildTarget.iPhone)
#endif
		{
			AASUnityEditor.XCodeEditor.XCProject proj = new AASUnityEditor.XCodeEditor.XCProject(path);

// 			string[] projmods = System.IO.Directory.GetFiles(
// System.IO.Path.Combine(System.IO.Path.Combine(Application.dataPath, "PSSDK"), "Plugins"), "SDK.projmods", System.IO.SearchOption.AllDirectories);
			string[] projmods = System.IO.Directory.GetFiles(System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.Combine(Application.dataPath, "CSSDK"), "Plugins"), "IOS"), "Mods"), "SDK.projmods",System.IO.SearchOption.AllDirectories);
			if(projmods.Length == 0)
			{
				Debug.LogWarning("[PolyPostBuild]PolyADSDK.projmods not found!");
			}
			foreach (string p in projmods)
			{
				proj.ApplyMod(p);
			}

			proj.AddOtherLinkerFlags ("-ObjC");
			//proj.AddOtherLinkerFlags ("-fobjc-arc");
			proj.Save();

			string filePath = Path.GetFullPath (path);
			Debug.Log ("==> filePath " + filePath);
			string infofilePath = Path.Combine(filePath, "info.plist" );
			Debug.Log ("==> infofilePath " + infofilePath);

			if( !System.IO.File.Exists( infofilePath ) ) {
				Debug.LogError( infofilePath +", 路径下文件不存在" );
				return;
			}

			Dictionary<string, object> dict = (Dictionary<string, object>)AASPlistCS.Plist.readPlist(infofilePath);
			string dkey = "NSAppTransportSecurity";
			if (!dict.ContainsKey (dkey)) {
				Dictionary<string, bool> dv = new Dictionary<string, bool>(); 
				dv.Add ("NSAllowsArbitraryLoads", true);
				dict.Add (dkey, dv);
				Debug.Log ("==> add " + dkey+ " :" + dict [dkey]);
			} else {
				Debug.Log ("==> exist " + dkey+ " :" + dict[dkey]);
			}


			

		}
	}
}
#endif