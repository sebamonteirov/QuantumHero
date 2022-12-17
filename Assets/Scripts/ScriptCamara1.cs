using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptCamara1 : MonoBehaviour
{
    [SerializeField]
    Transform target;
    public Vector2 velocity;
    public float smoothTime = 3f;
    public Player player;

    float posX, posY;
    
    public void Awake()
    {
        target = GameObject.FindWithTag("Player").transform;
        //player = GameObject.FindWithTag("Player").Player;
    }

    public void Update()
    {
    	//esto es para redondear los flotantes 
        posX = Mathf.Round(
            Mathf.SmoothDamp(transform.position.x, target.position.x, ref velocity.x, smoothTime)*100) / 100;
        posY = Mathf.Round(
            Mathf.SmoothDamp(transform.position.y, target.position.y, ref velocity.y, smoothTime) * 100) / 100;

        UpdatePosition();
    }

    public void UpdatePosition()
    {
        //aqui se determina la posicion de x, y y z
        transform.position = new Vector3(
        	Mathf.Clamp(posX, player.minX, player.maxX),
        	Mathf.Clamp(posY, player.minY, player.maxY),
        	transform.position.z
        	);
    }
    //es llamada desde Player y cambia con cada warp
    public void SetBound()
    {
        FastMove();
    }

    public void FastMove(){
        transform.position = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
            );
    }
}
