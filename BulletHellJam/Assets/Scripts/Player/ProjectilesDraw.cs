using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilesDraw : MonoBehaviour
{
    LineRenderer lineRenderer;
    List<Vector2> mousePositions;
	[SerializeField] GameObject linePrefab;
    [SerializeField] float deltaDiscretize = .25f;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject screenCenter;
    private bool isDrawing = false;
	private GameObject currentLine;

    [Header("Tint")]
    [SerializeField] private float maxProjectileTint = 100f;
    private float currentProjectileTint;
    [SerializeField] private float rateOfUseTint = .1f;
    [SerializeField] private float rateOfReplenishTint = .1f;

    void Start()
    {
        mousePositions = new List<Vector2>();
        currentProjectileTint = maxProjectileTint;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("TINT: " + currentProjectileTint);

        if(!isDrawing)
        {
            // Reestablecer tinta
            currentProjectileTint += rateOfReplenishTint;
            if(currentProjectileTint >= maxProjectileTint)
                currentProjectileTint = maxProjectileTint;
        }

        // Se deja de dibujar
        if(Input.GetMouseButtonUp(0) && isDrawing == true){
            isDrawing = false;
            CreateBullets();
        }

        // Se comienza a dibujar
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float dist = Vector2.Distance(mousePos, Base.instance.transform.position);
        if(DrawLimit.instance!=null)
        {
            if (dist<DrawLimit.instance.GetCurrentDistanceToBase() && Input.GetMouseButtonDown(0) && currentProjectileTint>0f)
            {
                isDrawing = true;
			    CreateLine();
		    }

            // Se está dibujando
            if (isDrawing && Input.GetMouseButton(0) && currentProjectileTint>0f){
                Vector2 tempMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                float distToBase = Vector2.Distance(tempMousePosition, Base.instance.transform.position);
                if (distToBase<DrawLimit.instance.GetCurrentDistanceToBase() && Vector2.Distance(tempMousePosition, mousePositions[mousePositions.Count - 1]) > deltaDiscretize){
                    UpdateLine(tempMousePosition);

                    // Gastar tinta
                    currentProjectileTint -= rateOfUseTint;

                    AudioManager.instance.PlaySFX(AudioManager.instance.playerProjectiles);
                }
            }
        }

        UIController.instance.UpdateProjectileTint(currentProjectileTint, maxProjectileTint);
    }

    void CreateLine(){
        // Llamamos a CreateLine una sola vez cuando empezamos a dibujar para crear currentLine. currentLine se visualizará porque tendrán un lineRenderer 
        //y tendrá colisión porque tendrá un edgeCollider
		currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
		lineRenderer = currentLine.GetComponent<LineRenderer>();
		mousePositions.Clear();
		// Como lineRenderer es una línea, necesitamos añadir dos puntos a fingerPositions para poder dibujarla sin errores
		mousePositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		mousePositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		// Dibujamos una línea compuesta de dos puntos

        lineRenderer.SetPosition(0, mousePositions[0]);
		lineRenderer.SetPosition(1, mousePositions[1]);
    }

    void UpdateLine(Vector2 newMousePos){
        mousePositions.Add(newMousePos);
		lineRenderer.positionCount++;
		// Convertimos el List de posiciones por las que ha ido pasando el dedo en la línea que vamos a ver
		lineRenderer.SetPosition(lineRenderer.positionCount - 1, newMousePos);
    }

    void CreateBullets(){
        Destroy(lineRenderer, 0f);
        //lineRenderer.material.color = new Color(0f,0f,0f,0f);
        if(lineRenderer.positionCount <= 2){
            return;
        }
        for (int i = 0; i < lineRenderer.positionCount; i++){
            GameObject bullet = Instantiate(bulletPrefab, lineRenderer.GetPosition(i), Quaternion.identity);
            Vector2 dirBullet = new Vector2(lineRenderer.GetPosition(i).x-screenCenter.transform.position.x, lineRenderer.GetPosition(i).y-screenCenter.transform.position.y);
            bullet.GetComponent<bulletBasicController>().setDirection(dirBullet.normalized);
        }   
        
    }
}
