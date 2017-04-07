using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class DataLogger : MonoBehaviour {

	protected int buttonClicks = 0;
	protected int sliderUses = 0;
	protected float sideChanges = 0f;
	protected float slideTime = 0f;
	private bool isUsingSlider = false;
	public Text testSubjectName;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(isUsingSlider){
			// uppdatera tiden
			slideTime += Time.deltaTime;
		}
	}

	public void IncrementButtonClicks(){
		buttonClicks++;
	}

	public void IncrementSliderUses(){
		sliderUses++;
	}

	public void IncrementParticipantChangedSide(){
		sideChanges += 1f/3;
	}

	public void StartSliderUse(){
		isUsingSlider = true;
	}

	public void StopSliderUse(){
		isUsingSlider = false;
	}

	void OnApplicationQuit() {
		string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/vrvis-logs/all-logs.csv";
		StreamWriter sw;
		if(!File.Exists(path)){
			sw = new StreamWriter(path);
			sw.WriteLine("Name,Button clicks,Slider usages,Slider time,Side changes");
		}
		else{
			sw = new StreamWriter(path, true);
		}
		
		sw.Write(testSubjectName.text + ",");
		sw.Write(buttonClicks.ToString() + ",");
		sw.Write(sliderUses.ToString() + ",");
		sw.Write(slideTime.ToString() + ",");
		sw.Write(((int) sideChanges).ToString());
		sw.WriteLine();
		sw.Close();
	}
}
