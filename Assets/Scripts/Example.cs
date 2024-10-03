using UnityEngine;
using RedSaw.MissionSystem;

public class Example : MonoBehaviour
{
    [SerializeField] private MissionChain chain;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Start Chain");
            GameAPI.MissionChainManager.StartChain(chain);
        }


        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("MSG: PlayerBehaviourExample");
            GameAPI.Broadcast(new GameMessage(GameEventType.PlayerBehaviourExample));
        }


        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("MSG: CompleteDialogue");
            GameAPI.Broadcast(new GameMessage(GameEventType.CompleteDialogue));
        }
            
    }
}