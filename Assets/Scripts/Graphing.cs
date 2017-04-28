using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.IO;
public class Graphing : MonoBehaviour {
	//[DllImport("System.Windows.Forms.dll")]
	//private static extern void OpenFileDialog ();
	public Camera mainCam;
	private AudioSource aud;
	public GameObject circle;
	public GameObject[] circles;
	public GameObject circle2;
	public GameObject[] circles2;
	public GameObject light;
	public GameObject[] lights;
	public GameObject openButton;
	public float freq;
	public float amp = 1f;
	private float pausedTime;
	public int samples;
	public int numberofCircles;
	public int numberofCircles2;
	public int numberofLights;
	public int buttonAmt;
	public int div;
	public bool op;
	public bool spec;
	public bool lightarray;
	public bool grid;
	public string url;
	public string prevUrl;
	public string songName;
	public string[] dir;
	public string[] fileExt;
	public string currentDir;
	public string currentDirParent;
	public string currentDirSub;
	public List<string> songList;
	public List<string> fileExtList;
	public List<string> dirList;
	public Text txt;
	public Text playbackTimeText;
	public Text captionText;
	public RenderTexture renderTexture;
	public Sprite play;
	public Sprite pause;
	public Slider s1;
	public Slider s2; 
	public Slider s3;
	public Slider playbackTime;
	void Start () {
		renderTexture.width = Screen.width;
		renderTexture.height = Screen.height;
		mainCam = Camera.main;
		aud = GetComponent<AudioSource> ();
		if(!Application.isEditor)
			Application.runInBackground = true;
		spec = true;
		Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,0));
		float xWidth = worldPos.x;
		float yHeight = worldPos.y;
		for (float i = 0; i < worldPos.x * 2; i+=0.01f) 
		{
			GameObject spawnedCircle = Instantiate (circle, circle.transform.position, circle.transform.rotation) as GameObject;
			GameObject spawnedCircle2 = Instantiate (circle2, circle2.transform.position, circle2.transform.rotation) as GameObject;
			spawnedCircle.transform.position = new Vector3 (i - xWidth, 0);
			spawnedCircle2.transform.position = new Vector3 (i - xWidth, 0);
			circles = GameObject.FindGameObjectsWithTag ("sphere");
			circles2 = GameObject.FindGameObjectsWithTag ("sphere2");
			numberofCircles = circles.Length;
			numberofCircles2 = circles2.Length;
		}
		for (float x = 0; x < xWidth * 2; x+=0.3f) 
		{
			for (float y = 0; y < yHeight * 2; y+=0.3f) 
			{
				GameObject spawnedLight = Instantiate (light, light.transform.position, light.transform.rotation) as GameObject;
				spawnedLight.transform.position = new Vector3 (x - xWidth, y - yHeight, 10);
				lights = GameObject.FindGameObjectsWithTag ("star");
				numberofLights = lights.Length;
			}
		}
		foreach (GameObject light in lights)
			light.SetActive (false);
		GameObject.Find ("Grid").GetComponent<MeshRenderer> ().enabled = false;
		buttonAmt = 0;
		fileExt = new string[6]{"*.wav", "*.ogg", "*.s3m", "*.mod", "*.it", "*.xm"};
		for (int i = 0; i < fileExt.Length; i++) {
			fileExtList.Add (fileExt [i]);
		}
		GameObject.Find ("FileExt Dropdown").GetComponent<Dropdown> ().AddOptions(fileExtList);
		string chosenFileExt = GameObject.Find("FileExt Dropdown").GetComponent<Dropdown>().captionText.text;
		currentDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\";
		currentDirSub = currentDir.Substring(0,currentDir.Length-1);
		string[] currentSubDirs = Directory.GetDirectories (currentDir, "*", SearchOption.TopDirectoryOnly);
		dir = Directory.GetFiles (currentDir, chosenFileExt, SearchOption.TopDirectoryOnly);
		dirList = new List<string> ();
		currentDirParent = Directory.GetParent (currentDirSub).ToString();
		dirList.Add (currentDir);
		dirList.Add (currentDirParent);
		for (int i = 0; i < currentSubDirs.Length; i++) {
			dirList.Add (currentSubDirs[i]);
		}
		GameObject.Find("Directory Dropdown").GetComponent<Dropdown>().AddOptions(dirList);
		dir = Directory.GetFiles (currentDir, chosenFileExt, SearchOption.TopDirectoryOnly);
		songList = new List<string>();
		for (int i = 0; i < dir.Length; i++) {
			songList.Add (dir[i].Substring(dir[i].LastIndexOf(@"\")+1));
		}
		GameObject.Find("Song Dropdown").GetComponent<Dropdown>().AddOptions(songList);
		GameObject.Find("Song Dropdown").GetComponent<Dropdown>().value = 0;
	}
	void Update () {
		Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,0));
		float[] output = aud.GetOutputData (8192, 1);
		float[] spectrum = aud.GetSpectrumData (samples, 0, FFTWindow.BlackmanHarris);
		if (aud.isPlaying) {
			if (spec || op) {
				foreach (GameObject spawnedCircle in circles)
					spawnedCircle.SetActive (true);
				foreach (GameObject spawnedLight in lights)
					spawnedLight.SetActive (false);
				GameObject.Find ("Grid").GetComponent<MeshRenderer> ().enabled = false;
				for (int j = 0; j < circles.Length; j++) {
					if (spec) {
						circles [j].transform.position = new Vector3 (circles [j].transform.position.x, spectrum[j] * amp);
						circles2 [j].transform.position = new Vector3 (circles2 [j].transform.position.x, -spectrum[j] * amp);
					}
					if (op) {
						circles [j].transform.position = new Vector3 (circles2 [j].transform.position.x, output[j]);
					}
					Color color = circles [j].GetComponent<SpriteRenderer> ().color;
					color.r = s1.normalizedValue + spectrum [j] * 200;
					color.g = s2.normalizedValue;
					color.b = s3.normalizedValue;
					circles [j].GetComponent<SpriteRenderer> ().color = color;
					Color color2 = circles2 [j].GetComponent<SpriteRenderer> ().color;
					color2.r = s1.normalizedValue + spectrum [j] * 200;
					color2.g = s2.normalizedValue;
					color2.b = s3.normalizedValue;
					circles2 [j].GetComponent<SpriteRenderer> ().color = color2;
				}
			}
			if (spec) {
				foreach (GameObject spawnedCircle2 in circles2)
					spawnedCircle2.SetActive (true);
			}
			if (op) {
				foreach (GameObject spawnedCircle2 in circles2)
					spawnedCircle2.SetActive (false);
			}
			if (lightarray) {
				foreach (GameObject spawnedLight in lights)
					spawnedLight.SetActive (true);
				foreach (GameObject spawnedCircle in circles)
					spawnedCircle.SetActive (false);
				foreach (GameObject spawnedCircle2 in circles2)
					spawnedCircle2.SetActive (false);
				if (aud.isPlaying) {
					for (int i = 0; i < lights.Length; i++) {
						Color color = lights [i].GetComponent<Light> ().color;
						color.r = s1.normalizedValue;
						color.g = s2.normalizedValue;
						color.b = s3.normalizedValue;
						lights [i].GetComponent<Light> ().color = color;
						lights [i].GetComponent<Light> ().range = spectrum [i] * amp;
					}
				}
			}
			if (grid) {
				foreach (GameObject spawnedLight in lights)
					spawnedLight.SetActive (false);
				foreach (GameObject spawnedCircle in circles)
					spawnedCircle.SetActive (false);
				foreach (GameObject spawnedCircle2 in circles2)
					spawnedCircle2.SetActive (false);
				GameObject gridObj = GameObject.Find ("Grid");
				gridObj.GetComponent<MeshRenderer> ().enabled = true;
				Mesh mesh = gridObj.GetComponent<MeshFilter> ().mesh;
				Vector3[] verts = mesh.vertices;
				Color color = gridObj.GetComponent<MeshRenderer>().material.color;
				color.r = s1.normalizedValue;
				color.g = s2.normalizedValue;
				color.b = s3.normalizedValue;
				for (int i = 0; i < verts.Length; i++) {
					verts[i].y = spectrum[i];
				}
				gridObj.GetComponent<MeshRenderer> ().material.color = color;
				mesh.vertices = verts;
				mesh.RecalculateNormals ();
				mesh.RecalculateBounds ();
			}
			playbackTime.value = aud.time;
			playbackTime.maxValue = aud.clip.length;
			var playingAudioTime = TimeSpan.FromMinutes (aud.time);
			TimeSpan playingAudioTime2 = new TimeSpan (playingAudioTime.Ticks - (playingAudioTime.Ticks % TimeSpan.TicksPerSecond));
			var totalAudioTime = TimeSpan.FromMinutes (aud.clip.length);
			TimeSpan totalAudioTime2 = new TimeSpan (totalAudioTime.Ticks - (totalAudioTime.Ticks % TimeSpan.TicksPerSecond));
			playbackTimeText.text = "" + playingAudioTime2 + "  /  " + totalAudioTime2;
			if (buttonAmt == 0) {
				grid = false;
				spec = true;
			}
			if (buttonAmt == 1) {
				lightarray = false;
				spec = false;
				op = true;
			}
			if (buttonAmt == 2) {
				op = false;
				lightarray = true;
			}
			if (buttonAmt == 3) {
				lightarray = false;
				grid = true;
			}
		}
		if (songList.Count <= 0) {
			openButton.SetActive(false);
		} else if (songList.Count > 0) {
			openButton.SetActive (true);
		}
	}
	public void ExtensionChange(){
		string chosenFileExt = GameObject.Find("FileExt Dropdown").GetComponent<Dropdown>().captionText.text;
		dir = Directory.GetFiles (currentDir, chosenFileExt, SearchOption.TopDirectoryOnly);
		songList = new List<string>();
		for (int i = 0; i < dir.Length; i++) {
			songList.Add (dir[i].Substring(dir[i].LastIndexOf(@"\")+1));
		}
		GameObject.Find("Song Dropdown").GetComponent<Dropdown>().ClearOptions();
		GameObject.Find("Song Dropdown").GetComponent<Dropdown>().AddOptions(songList);
		GameObject.Find("Song Dropdown").GetComponent<Dropdown>().value = 0;
	}
	public void SongChange(){
		captionText.text = captionText.text.Substring (captionText.text.LastIndexOf (@"\")+1);
	}
	public void DirChange(){
		currentDir = GameObject.Find("Directory Dropdown").GetComponent<Dropdown>().captionText.text + @"\";
		currentDirSub = currentDir.Substring(0,currentDir.Length-1);
		currentDirParent = Directory.GetParent(currentDirSub).ToString();
		string[] currentSubDirs = Directory.GetDirectories (currentDirSub, "*", SearchOption.TopDirectoryOnly);
		List<string> dirList = new List<string> ();
		dirList.Add (currentDirSub);
		dirList.Add (currentDirParent);
		for (int i = 0; i < currentSubDirs.Length; i++) {
			dirList.Add (currentSubDirs[i]);
		}
		GameObject.Find ("Directory Dropdown").GetComponent<Dropdown> ().ClearOptions ();
		GameObject.Find("Directory Dropdown").GetComponent<Dropdown>().AddOptions(dirList);
		GameObject.Find("Directory Dropdown").GetComponent<Dropdown>().value = 0;
		string chosenFileExt = GameObject.Find("FileExt Dropdown").GetComponent<Dropdown>().captionText.text;
		dir = Directory.GetFiles (currentDir, chosenFileExt, SearchOption.TopDirectoryOnly);
		songList.Clear ();
		songList = new List<string>();
		for (int i = 0; i < dir.Length; i++) {
			songList.Add (dir[i].Substring(dir[i].LastIndexOf(@"\")+1));
		}
		GameObject.Find("Song Dropdown").GetComponent<Dropdown>().ClearOptions();
		GameObject.Find("Song Dropdown").GetComponent<Dropdown>().AddOptions(songList);
		GameObject.Find("Song Dropdown").GetComponent<Dropdown>().value = 0;
	}
	public void ButtonPress(){
		buttonAmt++;
		if (buttonAmt == 4) {
			buttonAmt = 0;
		}
	}
	public void PlayAndPause(){
		if (aud.isPlaying) {
			aud.Pause ();
			GameObject.Find ("PlayPause").GetComponent<Button> ().image.sprite = play;
		} else if (!aud.isPlaying) {
			aud.Play ();
			GameObject.Find ("PlayPause").GetComponent<Button> ().image.sprite = pause;
		}
	}
	public void OpenFile(){
		pausedTime = aud.time;
		prevUrl = url;
		url = currentDir + captionText.text;
		StartCoroutine ("OpenMusic");
	}
	IEnumerator OpenMusic(){
		WWW www = new WWW("file://" + url);
		yield return www;
		aud.clip = www.GetAudioClip(false, false);
		if (pausedTime > 0.0f && url == prevUrl) {
			aud.time = pausedTime;
		}
		else if (pausedTime <= 0.0f || url != prevUrl) {
			aud.time = 0.0f;
		}
		aud.Play ();
		GameObject.Find ("PlayPause").GetComponent<Button> ().image.sprite = pause;
		StopCoroutine ("OpenMusic");
	}
	public void Dragging()
	{
		aud.volume = 0.5f;
		aud.time = playbackTime.value;
	}
	public void StopDragging(){
		aud.volume = 1.0f;
	}
	public void Quit(){
		Application.Quit ();
	}
}
