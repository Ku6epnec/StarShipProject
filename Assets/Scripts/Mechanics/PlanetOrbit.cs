using UnityEngine;
using Mirror;
using UI;

public class PlanetOrbit: NetworkBehaviour
{
    private float speed => smoothTime;

    [SerializeField] private Vector3 aroundPoint;
    [SerializeField] private float smoothTime = .3f;
    [SerializeField] private float circleInSecond = 1f;

    [SerializeField] private float offsetSin = 1;
    [SerializeField] private float offsetCos = 1;
    [SerializeField] private float rotationSpeed;

    [SerializeField] float radius;

    [SerializeField] GameObject planetRing;

    private float radiusRing = 0.0f;

    [SerializeField] private ObjectLabel planetLabel;

    [SyncVar] protected Vector3 serverPosition;
    [SyncVar] protected Vector3 serverEulers;

    private float currentAng;
    private Vector3 currentPositionSmoothVelocity;
    private float currentRotationAngle;

    private const float circleRadians = Mathf.PI * 2;

    public void Init(PlanetData planetData)
    {
        this.radius = planetData.radius;
        this.radiusRing = planetData.radiusRing;
        if (this.radiusRing != 0)
        {
            var ring = Instantiate(planetRing, transform);
            ring.transform.localScale = new Vector3(radiusRing, radiusRing, radiusRing);
        }
        var meshRenderer = GetComponent<MeshRenderer>();
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        materialPropertyBlock.SetColor("_Color", planetData.groundColor);
        meshRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    private void OnGUI()
    {
        planetLabel?.DrawLabel();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        if (isServer)
        {
            ServerMovement();
        }
        else
        {
            ClientMovement();
        }
    }

    private void ServerMovement()
    {
        if (!isServer)
            return;

        Vector3 p = aroundPoint;
        p.x += Mathf.Sin(currentAng) * radius * offsetSin;
        p.z += Mathf.Cos(currentAng) * radius * offsetCos;
        transform.position = p;
        currentRotationAngle += Time.deltaTime * rotationSpeed;
        currentRotationAngle = Mathf.Clamp(currentRotationAngle, 0, 361);
        if (currentRotationAngle >= 360)
            currentRotationAngle = 0;

        transform.rotation = Quaternion.AngleAxis(currentRotationAngle, transform.up);
        currentAng += circleRadians * circleInSecond * Time.deltaTime;

        SendToClients();
    }

    private void SendToClients()
    {
        serverPosition = transform.position;
        serverEulers = transform.eulerAngles;
    }

    private void ClientMovement()
    {
        if (!isClient)
            return;

        transform.position = Vector3.SmoothDamp(transform.position, serverPosition, ref currentPositionSmoothVelocity, speed);
        transform.rotation = Quaternion.Euler(serverEulers);
    }
}

