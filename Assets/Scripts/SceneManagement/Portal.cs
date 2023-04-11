using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{

	[SerializeField] int sceneToLoad = -1;
	[SerializeField] DestinationIdentifier destinationPortal;
	[SerializeField] Transform spawnPoint;

	PlayerController player;

	//wanneer je op een portal staat -> trigger
	public void OnPlayerTriggered(PlayerController player)
	{
        player.Character.Animator.IsMoving = false;
        this.player = player;
		StartCoroutine(SwitchScene());
	}

	//reference naar de Fader script
	Fader fader;

	private void Start()
	{
		fader = FindObjectOfType<Fader>();
	}

	IEnumerator SwitchScene()
	{
		//zorgen dat portal niet destroyed na het gebruik
		DontDestroyOnLoad(gameObject);

		GameController.Instance.PauseGame(true);
		yield return fader.FadeIn(0.5f);

		yield return SceneManager.LoadSceneAsync(sceneToLoad);

		//will return the first portal it can find, maar niet degene die we net gebruikte.
		var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
		player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

		yield return fader.FadeOut(0.5f);
        GameController.Instance.PauseGame(false);

        //portal mag weg als we eenmaal klaar zijn
        Destroy(gameObject);
	}

	public Transform SpawnPoint => spawnPoint;
}

public enum DestinationIdentifier { A, B, C, D, E, F, G }
