using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ChatInput : MonoBehaviour {


	InputField input;
	InputField.SubmitEvent se;
	public Text output;


	void Start()
	{
		input = gameObject.GetComponent<InputField>();
		se = new InputField.SubmitEvent();
		se.AddListener(SubmitMessage);
		input.onEndEdit = se;
	}

	private void SubmitMessage(string _input)
	{
		string currentMessage = output.text;
		string newText = currentMessage + "/n" + _input;
		output.text = newText;
		input.text = "";
		input.ActivateInputField();
	}
}
