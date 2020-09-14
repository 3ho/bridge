using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerPrefab : MonoBehaviour
{
    private int last = -1;
    public GameObject r;
    public GameObject b;
    public GameObject g;

    public GameObject grid;
    public GameObject scaleGo;

    private void OnEnable()
    {
        StopCoroutine(playAnim());
        StartCoroutine(playAnim());
    }

    public void updateUI(int color)
    {
        if (last == color)
            return;

        r.SetActive(false);
        b.SetActive(false);
        g.SetActive(false);
        switch (color)
        {
            case ColorUtils.R:
                r.SetActive(true);
                break;
            case ColorUtils.B:
                b.SetActive(true);
                break;
            case ColorUtils.G:
                g.SetActive(true);
                break;
        }
    }

    private System.Collections.IEnumerator playAnim()
    {
        while (true)
        {
            scaleGo.transform.localScale = Vector3.one;
            yield return new WaitForSeconds(1f);
            for (float sc = 1; sc > -1.2; sc -= 0.1f)
            {
                //yield return null;
                yield return new WaitForSeconds(0.05f);
                Vector3 v3 = scaleGo.transform.localScale;
                v3.x = sc;
                scaleGo.transform.localScale = v3;
            }

            // y
            for (float sc = 1; sc > -1.2; sc -= 0.1f)
            {
                //yield return null;
                yield return new WaitForSeconds(0.05f);
                Vector3 v3 = scaleGo.transform.localScale;
                v3.y = sc;
                scaleGo.transform.localScale = v3;
            }
        }
    }
}

