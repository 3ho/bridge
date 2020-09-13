//-L-<m_alert,txt_alertBox>
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public partial class AlertPanel : View
{
	private GameObject m_alert;
	private GameObject txt_alertBox;
	private Text txt_alertBoxText;

	protected override void Awake()
	{
		base.Awake();

		GameObjectData data = gameObject.GetComponent<GameObjectData>();
		m_alert =  data.GameObjects[0].gameObject;
		txt_alertBox =  data.GameObjects[1].gameObject;
		txt_alertBoxText = txt_alertBox.GetComponent<Text>();
		ViewMgr.Ins.addView(this);
	}

}
