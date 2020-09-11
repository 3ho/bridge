
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LoginPanel
{
    private int count = 0;
    protected override void onInit()
    {
        UIEventListener.Get(btn_start).onClick = OnStartGame;
    }

    protected override void onShow(System.Object param = null, string childView = null)
    {
    }

    public void Update()
    {
  
    }

    private void OnStartGame(GameObject go)
    {
        count++;
        SetLabelText(txt_count, count);
        Debug.Log("LoginPanel.OnStartGame" + Time.time);

        Battle battle = new Battle();
        SetLabelText(txt_count, battle.mapToString());

    }

    protected override void onDestroy()
    {
        Debug.Log("LoginPanel.onDestroy");
    }
}
