using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartsDisplay : MonoBehaviour {

	List<GameObject> hearts = new List<GameObject>();
	List<GameObject> heartAnims = new List<GameObject>();
	private PlayerController player;
	// Use this for initialization

	private int lastH = -1;

	void Start () {
		hearts.Add(transform.Find("Canvas/HeartsEmpty").gameObject);
		for (int i = 1; i <= 5; i++) {
			string name = string.Format("Canvas/Hearts {0}-6", i);
			hearts.Add(transform.Find(name).gameObject);
		}
		hearts.Add(transform.Find("Canvas/HeartsFull").gameObject);

		for (int i = 1; i <= 3; i++) {
			string name = string.Format("Canvas/HeartsDamagedAnims/HeartsLeft {0}", i);
			heartAnims.Add(transform.Find(name).gameObject);
			name = string.Format("Canvas/HeartsDamagedAnims/HeartsRight {0}", i);
			heartAnims.Add(transform.Find(name).gameObject);
		}

		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

		lastH = (int)Mathf.Floor((player.HealthPercentage) * (hearts.Count - 1));
	}
	
	// Update is called once per frame
	void Update () {
		int h = (int)Mathf.Floor((player.HealthPercentage) * (hearts.Count-1));
		//Debug.Log(string.Format("{0}, {1}, {2}, {3}", player.currentHealth, player.maxHealth,h, player.HealthPercentage));
		int i = 0;
		foreach (var heart in hearts) {
			heart.SetActive(h == i || i == 0);
			i++;
		}

		if (h != lastH) {
			i = lastH;
			while (i > h && i-1>0) {
				heartAnims[i-1].SetActive(true);
				i--;
			}
		}
		lastH = h;
	}
}
