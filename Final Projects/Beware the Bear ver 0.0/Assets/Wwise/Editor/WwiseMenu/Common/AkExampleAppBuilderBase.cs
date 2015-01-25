#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2013 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

public abstract class AkExampleAppBuilderBase
{
	protected string m_AppBaseName = "AkUnityGame";	
	protected BuildTarget m_buildTarget = BuildTarget.StandaloneWindows;
	protected string m_platform = "Undefined";
	protected string m_appExtension = "Undefined";
	protected string m_appDir = "Undefined";
	protected string m_pluginDir = Path.Combine(Application.dataPath, "Plugins");
	protected string m_srcPlugin = "Undefined";
	protected string m_destPlugin = "Undefined";
	protected string m_srcPath = "Undefined";
	
	public virtual bool Prebuild()
	{
		string srcPath = m_srcPath;
		FileInfo src = new FileInfo(srcPath);

		// If there is a backup for current architecture, we use it.
		if (src.Exists)
		{
			string destPath = Path.Combine(m_pluginDir, m_destPlugin);
			AkUnityAssetsInstaller.OverwriteFile(srcPath, destPath);
		}
		else
		{
			AkBankPath.ConvertToPosixPath(ref srcPath);
			UnityEngine.Debug.LogError(string.Format("Wwise: Plugin for {0} not found at: {1}. Install via menu first.", m_platform, srcPath));
			return false;
		}

		return true;
	}

	public bool Build()
	{
		// Get filename.
        m_appDir = EditorUtility.SaveFolderPanel("Select Root Folder for Application Build", ".", "");

        bool isUserCancelledBuild = m_appDir == "";
        if (isUserCancelledBuild)
        {
        	UnityEngine.Debug.Log("Wwise: User cancelled the build.");
        	return false;
        }
		
		// Get filename.
		const string SceneExt = "unity";
        string scenePath = EditorUtility.OpenFilePanel("Select Single Scene to Build", Application.dataPath, SceneExt);

        isUserCancelledBuild = scenePath == "";
        if (isUserCancelledBuild)
        {
        	UnityEngine.Debug.Log("Wwise: User cancelled the build.");
        	return false;
        }
        string[] scenesToBuild = new string[] {scenePath};

        if ( ! Prebuild() )
        	return false;

        // Build player.
		string AppFullPath = GetPlatformAppLocationPath(m_appDir);
		UnityEngine.Debug.Log("Wwise: BuildTarget Application location: "+AppFullPath);
		
        BuildPipeline.BuildPlayer(scenesToBuild, AppFullPath, m_buildTarget, BuildOptions.None);

        return true;

	}
	
	protected virtual string GetPlatformAppLocationPath(string AppDir)
	{
		string path = Path.Combine(AppDir, m_AppBaseName+m_appExtension);
		AkBankPath.ConvertToPosixPath(ref path);
		return path;
	}
	
}


#endif // #if UNITY_EDITOR