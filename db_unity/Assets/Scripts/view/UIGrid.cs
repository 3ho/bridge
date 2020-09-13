using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public partial class UIGrid : MonoBehaviour
{
	public GameObject m_color;
	public System.Object context;

	private void Awake()
	{
		m_color =  transform.Find(@"m_color").gameObject;
	}
}
