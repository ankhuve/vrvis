using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class DataLogger : MonoBehaviour {

	protected int task1buttonClicks = 0;
	protected int task1sliderUses = 0;
	protected int task1sideChanges = 0;
	protected float task1slideTime = 0f;
	protected float task1Time = 0f;
	protected int buttonClicks = 0;
	protected int sliderUses = 0;
	protected float sideChanges = 0f;
	protected float slideTime = 0f;
	protected float taskTime = 0f;
	private bool isUsingSlider = false;
	private bool isDoingTasks = false;
	public Text testSubjectName;
	public Button pausePlayTimerButton;
	public Text tasksTimerText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(isUsingSlider){
			// uppdatera tiden
			slideTime += Time.deltaTime;
		}

		if(isDoingTasks){
			taskTime += Time.deltaTime;
			tasksTimerText.text = taskTime.ToString("F1") + "s";
		}
	}

	public void IncrementButtonClicks(){
		if (isDoingTasks) {
			buttonClicks++;
		}
	}

	public void IncrementSliderUses(){
		if (isDoingTasks) {
			sliderUses++;
		}
	}

	public void IncrementParticipantChangedSide(){
		if (isDoingTasks) {
			sideChanges += 1f/3;
		}
	}

	public void StartSliderUse(){
		if (isDoingTasks) {
			isUsingSlider = true;
		}
	}

	public void StopSliderUse(){
		isUsingSlider = false;
	}

	public void StartTasksTimer() {
		isDoingTasks = true;
		pausePlayTimerButton.transform.GetChild(0).GetComponent<Text>().text = "Pause";
	}

	public void StopTasksTimer() {
		isDoingTasks = false;
		pausePlayTimerButton.transform.GetChild(0).GetComponent<Text>().text = "Start";
	}

	public void ToggleTasksTimer() {
		if(isDoingTasks)
		{
			isDoingTasks = false;
			pausePlayTimerButton.transform.GetChild(0).GetComponent<Text>().text = "Start";
		}
		else
		{
			isDoingTasks = true;
			pausePlayTimerButton.transform.GetChild(0).GetComponent<Text>().text = "Pause";
		}
	}

	public void task1Finished() {
		// Stop timer for first tasks
		StopTasksTimer();

		print("Starting open-ended task");
		//Save previous data
		task1buttonClicks = buttonClicks;
		task1sliderUses = sliderUses;
		task1slideTime = slideTime;
		task1Time = taskTime;
		task1sideChanges = (int) sideChanges;

		//reset data
		buttonClicks = 0;
		sliderUses = 0;
		slideTime = 0f;
		sideChanges = 0f;
		taskTime = 0f;

		print("Button clicks: " + task1buttonClicks.ToString());
		print("sliderUses clicks: " + task1sliderUses);
		print("Slide time: " + task1slideTime);
		print("Side changes: " + task1sideChanges);
		print("Tasks time: " + task1Time);

		StartTasksTimer();
	}

	void OnApplicationQuit() {
		string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/vrvis-logs/all-logs.csv";
		StreamWriter sw;
		if(!File.Exists(path)){
			sw = new StreamWriter(path);
			sw.WriteLine("Name,Task 1 Button clicks,Task 1 Slider usages,Task 1 Slider time,Task 1 Side changes,Task 1 Time,Task 2 Button clicks,Task 2 Slider usages,Task 2 Slider time,Task 2 Side changes,Task 2 Time");
		}
		else{
			sw = new StreamWriter(path, true);
		}
		
		sw.Write(testSubjectName.text + ",");
		sw.Write(task1buttonClicks.ToString() + ",");
		sw.Write(task1sliderUses.ToString() + ",");
		sw.Write(task1slideTime.ToString() + ",");
		sw.Write(((int) task1sideChanges).ToString() + ",");
		sw.Write(task1Time.ToString() + ",");
		sw.Write(buttonClicks.ToString() + ",");
		sw.Write(sliderUses.ToString() + ",");
		sw.Write(slideTime.ToString() + ",");
		sw.Write(((int) sideChanges).ToString() + ",");
		sw.Write(taskTime.ToString());		
		sw.WriteLine();
		sw.Close();
	}
}
