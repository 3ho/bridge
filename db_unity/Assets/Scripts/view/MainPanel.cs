//-L-<btn_back,m_login,btn_start,txt_count,m_battle,m_gird,m_mapRoot,m_gird_x,m_grid_x_count,m_player_prefabs,m_target_home,m_target_home_image>
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public partial class MainPanel : View
{
	private GameObject btn_back;
	private GameObject m_login;
	private GameObject btn_start;
	private GameObject txt_count;
	private Text txt_countText;
	private GameObject m_battle;
	private UIGrid m_gird;
	private GameObject m_mapRoot;
	private GameObject m_gird_x;
	private GameObject m_grid_x_count;
	private GameObject m_player_prefabs;
	private GameObject m_target_home;
	private GameObject m_target_home_image;

	protected override void Awake()
	{
		base.Awake();

		GameObjectData data = gameObject.GetComponent<GameObjectData>();
		btn_back =  data.GameObjects[0].gameObject;
		m_login =  data.GameObjects[1].gameObject;
		btn_start =  data.GameObjects[2].gameObject;
		txt_count =  data.GameObjects[3].gameObject;
		txt_countText = txt_count.GetComponent<Text>();
		m_battle =  data.GameObjects[4].gameObject;
		m_gird = View.AddComponentIfNotExist<UIGrid>(data.GameObjects[5].gameObject);
		m_mapRoot =  data.GameObjects[6].gameObject;
		m_gird_x =  data.GameObjects[7].gameObject;
		m_grid_x_count =  data.GameObjects[8].gameObject;
		m_player_prefabs =  data.GameObjects[9].gameObject;
		m_target_home =  data.GameObjects[10].gameObject;
		m_target_home_image =  data.GameObjects[11].gameObject;
		ViewMgr.Ins.addView(this);
	}

}
