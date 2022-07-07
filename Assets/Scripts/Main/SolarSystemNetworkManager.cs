using System.Collections.Generic;
using Characters;
using UnityEngine;
using Mirror;
using TMPro;
using Data;
using System;

namespace Main
{
    public class SolarSystemNetworkManager : NetworkManager
    {
        public event Action OnServerStart;
        public event Action OnClientStart;

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
            
            ShipController shipController = player.GetComponent<ShipController>();
            shipController.Init(_spaceShipSettings);
            _players.Add(conn.connectionId, shipController);

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

        public override void OnStartServer()
        {
            base.OnStartServer();

            OnServerStart?.Invoke();

            if (_inputName != null)
                _inputName.gameObject.SetActive(false);
        }
        public override void OnStopServer()
        {
            base.OnStopServer();

            if (_inputName != null)
                _inputName.gameObject.SetActive(false);
        }
        
        public override void OnStartClient()
        {
            base.OnStartClient();

            OnClientStart?.Invoke();

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
