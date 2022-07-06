using System;
using Main;
using Mechanics;
using Network;
using UI;
using UnityEngine;
using Mirror;
using Data;

namespace Characters
{
    public class ShipController : NetworkMovableObject
    {
        public event Action<ShipController, Collider> OnCollidedWithSomething;

        [SerializeField] private Transform _cameraAttach;
        private CameraOrbit _cameraOrbit;
        [SerializeField] private PlayerLabel playerLabel;
        private float _shipSpeed;
        private Rigidbody _rigidbody;

        [SyncVar(hook = nameof(OnNameUpdateFromServer))] private string _serverPlayerName;

        private Vector3 currentPositionSmoothVelocity;
        private SpaceShipSettings _spaceShipSettings;

        protected override float speed => _shipSpeed;

        public string PlayerName
        {
            get => gameObject.name;
            set => gameObject.name = value;
        }

        private void OnDestroy()
        {
            OnCollidedWithSomething = null;
        }

        private void OnGUI()
        {
            if (_cameraOrbit == null)            
                return;
            
            _cameraOrbit.ShowPlayerLabels(playerLabel);
        }

        [Server]
        public void Init(SpaceShipSettings spaceShipSettings)
        {
            //if (isServer)
            //{
                _spaceShipSettings = spaceShipSettings;
                RpcInit(spaceShipSettings);
            //}
        }

        [ClientRpc]
        private void RpcInit(SpaceShipSettings spaceShipSettings)
        {
            _spaceShipSettings = spaceShipSettings;
        }

        public override void OnStartAuthority()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)            
                return;

            var networkManager = (SolarSystemNetworkManager)SolarSystemNetworkManager.singleton;
            gameObject.name = networkManager.playerName;
            //gameObject.name = PlayerName;
            _cameraOrbit = FindObjectOfType<CameraOrbit>();
            _cameraOrbit.Initiate(_cameraAttach == null ? transform : _cameraAttach);
            playerLabel = GetComponentInChildren<PlayerLabel>();
            base.OnStartAuthority();
        }

        protected override void HasAuthorityMovement()
        {
            //var spaceShipSettings = SettingsContainer.Instance?.SpaceShipSettings;
            if (_spaceShipSettings == null)            
                return;            

            var isFaster = Input.GetKey(KeyCode.LeftShift);
            var speed = _spaceShipSettings.shipSpeed;
            var faster = isFaster ? _spaceShipSettings.faster : 1.0f;

            _shipSpeed = Mathf.Lerp(_shipSpeed, speed * faster, _spaceShipSettings.acceleration);

            var currentFov = isFaster ? _spaceShipSettings.fasterFov : _spaceShipSettings.normalFov;
            _cameraOrbit.SetFov(currentFov, _spaceShipSettings.changeFovSpeed);

            var velocity = _cameraOrbit.transform.TransformDirection(Vector3.forward) * _shipSpeed;
            _rigidbody.velocity = velocity * (_updatePhase == UpdatePhase.FixedUpdate ? Time.fixedDeltaTime : Time.deltaTime);

            if (!Input.GetKey(KeyCode.C))
            {
                var targetRotation = Quaternion.LookRotation(Quaternion.AngleAxis(_cameraOrbit.LookAngle, -transform.right) * velocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            }

            if (isServer)
            {
                SendToClients();
            }
            else
            {
                CmdSendTransform(transform.position, transform.rotation.eulerAngles);
            }
        }

        protected override void FromOwnerUpdate()
        {
            transform.position = Vector3.SmoothDamp(transform.position, serverPosition, ref currentPositionSmoothVelocity, speed);
            transform.rotation = Quaternion.Euler(serverEulers);                       
        }

        protected override void SendToClients()
        {
            serverPosition = transform.position;
            serverEulers = transform.eulerAngles;
        }

        [Command]
        private void CmdSendTransform(Vector3 position, Vector3 eulers)
        {
            serverPosition = position;
            serverEulers = eulers;
        }

        [ClientCallback]
        private void LateUpdate()
        {
            _cameraOrbit?.CameraMovement();
        }

        [ServerCallback]
        public void UpdateNameFromOwner()
        {
            TargetUpdateNameFromOwner();
        }

        [TargetRpc] 
        private void TargetUpdateNameFromOwner()
        {
            CmdUpdateName(PlayerName);
        }

        [Command]
        private void CmdUpdateName(string name)
        {
            _serverPlayerName = name;
        }

        private void OnNameUpdateFromServer(string oldName, string newName)
        {
            PlayerName = newName;
        }

        [ServerCallback]

        private void OnTriggerEnter(Collider other)
        {
            var damageDealer = other.gameObject.GetComponent<IDamageDealer>();
            if(damageDealer != null)
            {
                Debug.LogError($"hasAuthority false variant");
                NetworkServer.connections[connectionToClient.connectionId].Disconnect();
            }
            else
            {
                //NetworkServer.connections[connectionToClient.connectionId].Disconnect();
                Debug.LogError("hasAuthority true variant");
            }
        }

        public void MoveOwnerToPosition(Vector3 position)
        {
            TargetMoveOwnerToPosition(position);
        }

        [TargetRpc]
        private void TargetMoveOwnerToPosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}
