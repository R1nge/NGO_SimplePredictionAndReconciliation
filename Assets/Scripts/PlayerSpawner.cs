using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject prefab;

    public override void OnNetworkSpawn() => SpawnServerRpc();

    [ServerRpc(RequireOwnership = false)]
    private void SpawnServerRpc(ServerRpcParams rpcParams = default)
    {
        var instance = Instantiate(prefab);
        instance.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
    }
}