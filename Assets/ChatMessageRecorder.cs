﻿using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Regulus.Project.ItIsNotAGame1.Data;

public class ChatMessageRecorder : MonoBehaviour
{
	public GameObject TextSource;

	public RectTransform Content;
	private Client _Client;

	private readonly Queue<GameObject> _Texts;

	public int LineCount;

	public ChatMessageRecorder()
	{
		_Texts = new Queue<GameObject>();
	}

	void OnDestroy()
	{
		if (_Client != null)
		{
			_Client.User.VisibleProvider.Supply -= _AddChater;
			_Client.User.VisibleProvider.Unsupply -= _RemoveChater;
		}
	}
	// Use this for initialization
	void Start ()
	{
		_Client = Client.Instance;
		if (_Client != null)
		{
			_Client.User.VisibleProvider.Supply += _AddChater;
			_Client.User.VisibleProvider.Unsupply += _RemoveChater;
		}
	}

	private void _RemoveChater(IVisible obj)
	{
		obj.TalkMessageEvent -= _PushMessage;
	}

	private void _AddChater(IVisible obj)
	{
		obj.TalkMessageEvent += _PushMessage;
	}

	// Update is called once per frame
	void Update () {
	
	}

	private void _PushMessage(string message)
	{
		var textObject = GameObject.Instantiate(TextSource);

		var text = textObject.GetComponent<UnityEngine.UI.Text>();
		text.text = message;
		_Texts.Enqueue(textObject);

		var rectTransform = textObject.GetComponent<RectTransform>();
		rectTransform.SetParent(Content);	    

        if (_Texts.Count > LineCount)
		{
			var removeObject= _Texts.Dequeue();
			GameObject.Destroy(removeObject);
		}


		_ResetHeight();
	}

	void _ResetHeight()
	{
		var height = (from textObject in _Texts
					  let text = TextSource.GetComponent<RectTransform>()
					  select text.rect.height).Sum();
		var rect = Content.rect;

        Debug.Log("Chat Message Height : " + height);
        Content.sizeDelta = new Vector2(0, height);

    }

    
}
