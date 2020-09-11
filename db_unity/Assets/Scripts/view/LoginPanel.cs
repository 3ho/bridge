//-L-<btn_start,txt_count>
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public partial class LoginPanel : View
{
	private GameObject btn_start;
	private GameObject txt_count;
	private Text txt_countText;

	protected override void Awake()
	{
		base.Awake();

		GameObjectData data = gameObject.GetComponent<GameObjectData>();
		btn_start =  data.GameObjects[0].gameObject;
		txt_count =  data.GameObjects[1].gameObject;
		txt_countText = txt_count.GetComponent<Text>();
		ViewMgr.Ins.addView(this);
	}

}
