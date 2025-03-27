using Fusion;
using System.Threading.Tasks;
using UnityEngine;

public class FusionInit : MonoBehaviour
{

    private NetworkRunner runner = null;

    private void Awake() {
        Open();
    }

    private void OnDestroy() {
        Close();
    }

    private async void Open() {
        if(runner != null)
            await Close();

        GameObject runnerObject = new GameObject("Session");
        DontDestroyOnLoad(runnerObject);

        runner = runnerObject.AddComponent<NetworkRunner>();
        Debug.Log($"Create GameObject {runnerObject.name} - Starting Game");

        StartGameResult result = await runner.StartGame(new StartGameArgs {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "TestRoom",
            PlayerCount = 6
        });

        Debug.Log($"Connnection to Room - Result : {result.Ok} ");

        if(!result.Ok)
            await Close();
    }

    private async Task Close() {
        if (runner == null) return;

        await runner.Shutdown();
        Destroy(runner.gameObject);
        runner = null;
    }
}