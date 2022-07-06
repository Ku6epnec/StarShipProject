using System.Collections.Generic;
using Characters;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
using Data;

namespace Main
{
    public class SolarSystemNetworkManager : NetworkManager
    {
        [SerializeField] private TMP_InputField _inputName;
        public Dictionary<int, ShipController> _players = new Dictionary<int, ShipController>();
        private SpaceShipSettings _spaceShipSettings;
        private Transform startPos;

        public string playerName
        {
            get => _inputName == null ? string.Empty : _inputName.text;
        }

        public void Init(SpaceShipSettings spaceShipSettings)
        {
            _spaceShipSettings = spaceShipSettings;
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            startPos = GetStartPosition();
            GameObject player = startPos != null
                ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab);

            //player.name = _inputName.text;// $"{playerPrefab.name} [connID={conn.connectionId}]";
            //var spawnTransform = GetStartPosition();
            
            ShipController shipController = player.GetComponent<ShipController>();
            shipController.Init(_spaceShipSettings);
            //var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            _players.Add(conn.connectionId, shipController);
            //shipController.OnCollidedWithSomething += OnShipCollidedWithSomething;

            NetworkServer.AddPlayerForConnection(conn, player);

            shipController.PlayerName = _inputName.text;
            shipController?.UpdateNameFromOwner();

        }

        public void OnShipCollidedWithSomething(ShipController shipController, Collider collider)
        {
            var damageDealer = collider.gameObject.GetComponent<IDamageDealer>();
            if (damageDealer != null)
            {
                Debug.LogError($"hasAuthority - {shipController.hasAuthority}");
                
                if (!shipController.hasAuthority)
                {
                    Debug.LogError("hasAuthority false variant");
                    NetworkServer.connections[shipController.connectionToClient.connectionId].Disconnect();
                }
                else
                {
                    Debug.LogError("hasAuthority true variant");
                    RespawnPlayer(shipController);
                }
            }
        }

        /*public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
        }*/

        public override void OnStopServer()
        {
            base.OnStopServer();

            if (_inputName != null)
                _inputName.gameObject.SetActive(false);
        }
        
        public override void OnStartClient()
        {
            base.OnStartClient();

            if (_inputName != null)
                _inputName.gameObject.SetActive(false);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (_inputName != null)
                _inputName.gameObject.SetActive(false);
        }

        public void RespawnPlayer(ShipController shipController)
        {
            if (shipController!=null)
            {
                startPos = GetStartPosition();
            }
        }    
    }
}
