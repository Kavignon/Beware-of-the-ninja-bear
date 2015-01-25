#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System;


public class AkWwiseIDConverterMenu : MonoBehaviour {

	private static AkWwiseIDConverter m_converter = new AkWwiseIDConverter();


	[MenuItem("Wwise/Convert Wwise SoundBank IDs")]
	public static void ConvertWwiseSoundBankIDs () {
		m_converter.Convert();
	}
}

class AkWwiseIDConverter
{
	private string m_bankDir = AkBankPath.GetFullBasePath();
	private string m_converterScript = Path.Combine(Path.Combine(Path.Combine(Application.dataPath, "Wwise"), "Tools"), "WwiseIDConverter.py");
	private string m_progTitle = "Wwise: Converting SoundBank IDs";

	public void Convert()
	{
		string bankIdHeaderPath = EditorUtility.OpenFilePanel("Choose Wwise SoundBank ID C++ header", m_bankDir, "h");

		bool isUserCancelled = bankIdHeaderPath == "";
		if (isUserCancelled)
		{
			UnityEngine.Debug.Log("Wwise: User cancelled the action.");
			return;
		}

		ProcessStartInfo start = new ProcessStartInfo();
		start.FileName = "python";
		
		start.Arguments = string.Format("{0} {1}", m_converterScript, bankIdHeaderPath);
		
		start.UseShellExecute = false;
		start.RedirectStandardOutput = true;

		string progMsg = "Wwise: Converting C++ SoundBank IDs into C# ...";
		EditorUtility.DisplayProgressBar(m_progTitle, progMsg, 0.5f);

		using(Process process = Process.Start(start))
		{
		 	process.WaitForExit();
			try
			{
				//ExitCode throws InvalidOperationException if the process is hanging
				int returnCode = process.ExitCode;

				bool isBuildSucceeded = ( returnCode == 0 );
				if ( isBuildSucceeded )
				{
					EditorUtility.DisplayProgressBar(m_progTitle, progMsg, 1.0f);
				UnityEngine.Debug.Log(string.Format("Wwise: SoundBank ID conversion succeeded. Find generated Unity script under {0}.", m_bankDir));
				}
				else
				{
					UnityEngine.Debug.LogError("Wwise: Conversion failed.");
				}
				
				AssetDatabase.Refresh();

				EditorUtility.ClearProgressBar();
			}
			catch (InvalidOperationException ioex)
			{
				AssetDatabase.Refresh();

				EditorUtility.ClearProgressBar();
				UnityEngine.Debug.LogError(string.Format ("Wwise: SoundBank ID conversion process is hanging with exception: {}. Check detailed logs under the Logs folder.", ioex));
			}
		}
	}
}

#endif // #if UNITY_EDITOR	