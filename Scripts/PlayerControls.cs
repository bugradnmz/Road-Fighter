using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    Touch touch;
    Vector2 startPosition, direction; //first touch position, direction of finger movement
    GameManager gameManager; //script. using for start the game after countdown
    UIManager uiManager;

    Animator carAnimator;
    Animator camAnimator;
    Coroutine shiftCoroutine; //Backbone of the shift system. We will assign our corotuines to variable for able to make them stop else you can't stop.
    Coroutine oilCoroutine; //if you get out of fuel while this coroutine running game does not over. we will stop it when game needs to be over

    [HideInInspector] public float speed = 0; //velocity on transform. Also GameManager using it for UI
    [HideInInspector] public float gear = 1;    
    [SerializeField] float passSpeed, accelarationSpeed, decelerationSpeed, lowGearSpeed, midGearSpeed, highGearSpeed;
    [SerializeField] float fuelDrainRate; //lower is faster for drain rate

    [HideInInspector] public int fuel = 100;
    [HideInInspector] public int tiresCollected;
    [SerializeField] int fillAmount; //fill amount of fuel canisters    

    [HideInInspector] public bool crashed; //used by game manager to end game
    [SerializeField] bool unlimitedFuel; //testing
    bool isPassDone = true;  //We dont want to pass again before last pass finished else we lost our waypoints and pass system won't work  
    bool isControlable;

    void Start()
    {
        carAnimator = transform.GetChild(1).GetComponent<Animator>();
        camAnimator = transform.GetChild(0).GetComponent<Animator>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
    }

    void Update()
    {
        if (gameManager.gameStarted)
        {
            gameManager.gameStarted = false; //to make shoot for once
            isControlable = true;
            shiftCoroutine = StartCoroutine(Shift());

            if (!unlimitedFuel)
                StartCoroutine(DrainFuel());
        }
        transform.position += Vector3.forward * Time.deltaTime * speed;

        #region KEYBOARD CONTROLS FOR TESTING WITHOUT SIMULATOR (DELETE COMMENT MARKERS)     
        /*if (Input.GetKeyDown(KeyCode.W))
        {
            direction = new Vector2(0, 1);
        }        
        else if (Input.GetKeyDown(KeyCode.A))
        {
            direction = new Vector2(-1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            direction = new Vector2(0, -1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            direction = new Vector2(1,0);
        }
        else
        {
            direction = new Vector2(0, 0);
        }
        
        if (direction != new Vector2(0, 0))           //Shift or Pass
        {
            if (direction.y >= 0)                     //Shift Up or Pass
            {
                if (direction.x >= 0)                 //Shift Up or Pass Right
                {
                    if (direction.y >= direction.x)   //Shift Up
                    {
                        if (gear < 3)
                        {
                            gear += 1;

                            if(shiftCoroutine != null) //if we get coroutine is null error, codes below won't work
                                StopCoroutine(shiftCoroutine); //we are stopping old coroutine before starting new one for stable and better speed system

                            shiftCoroutine = StartCoroutine(Shift()); //assign coroutine and start same time
                            carAnimator.Play("car_shift_up", 0, 0);
                            camAnimator.Play("cam_shift_up", 0, 0);
                        }
                    }
                    else                              //Pass Right
                    {
                        if (transform.position.x < 7.85f && isPassDone)
                        {
                            isPassDone = false;

                            if (transform.position.x < -1.94f && transform.position.x > -1.96f)
                            {
                                //Debug.Log("PassRightLong");
                                StartCoroutine(PassRightLong());
                            }
                            else
                            {
                                //Debug.Log("PassRightShort");
                                StartCoroutine(PassRightShort());
                            }
                            carAnimator.Play("car_pass_right", 0, 0);
                            camAnimator.Play("cam_pass_right", 0, 0);
                        }
                    }
                }
                else if (direction.x < 0)             //Shift Up or Pass Left
                {
                    if (direction.y >= -direction.x)  //Shift Up
                    {
                        if (gear < 3)
                        {
                            gear += 1;

                            if (shiftCoroutine != null)
                                StopCoroutine(shiftCoroutine);

                            shiftCoroutine = StartCoroutine(Shift());
                            carAnimator.Play("car_shift_up", 0, 0);
                            camAnimator.Play("cam_shift_up", 0, 0);
                        }
                    }
                    else                              //Pass Left
                    {
                        if (transform.position.x > -8.15f && isPassDone)
                        {
                            isPassDone = false;

                            if (transform.position.x > 1.64f && transform.position.x < 1.66f)
                            {
                                //Debug.Log("PassLeftLong");
                                StartCoroutine(PassLeftLong());
                            }
                            else
                            {
                                //Debug.Log("PassLeftShort");
                                StartCoroutine(PassLeftShort());
                            }
                            carAnimator.Play("car_pass_left", 0, 0);
                            camAnimator.Play("cam_pass_left", 0, 0);
                        }
                    }
                }
            }
            else if (direction.y < 0)                 //Shift Down or Pass
            {
                if (direction.x >= 0)                 //Shift Down or Pass Right
                {
                    if (-direction.y >= direction.x)  //Shift Down
                    {
                        if (gear > 1)
                        {
                            gear -= 1;

                            if (shiftCoroutine != null)
                                StopCoroutine(shiftCoroutine);

                            shiftCoroutine = StartCoroutine(Shift());
                            carAnimator.Play("car_shift_down", 0, 0);
                            camAnimator.Play("cam_shift_down", 0, 0);
                        }
                    }
                    else                              //Pass Right
                    {
                        if (transform.position.x < 7.85f && isPassDone)
                        {
                            isPassDone = false;

                            if (transform.position.x < -1.94f && transform.position.x > -1.96f)
                            {
                                //Debug.Log("PassRightLong");
                                StartCoroutine(PassRightLong());
                            }
                            else
                            {
                                //Debug.Log("PassRightShort");
                                StartCoroutine(PassRightShort());
                            }
                            carAnimator.Play("car_pass_right", 0, 0);
                            camAnimator.Play("cam_pass_right", 0, 0);
                        }
                    }
                }
                else if (direction.x < 0)             //Shift Down or Pass Left
                {
                    if (-direction.y >= -direction.x) //Shift Down
                    {
                        if (gear > 1)
                        {
                            gear -= 1;

                            if (shiftCoroutine != null)
                                StopCoroutine(shiftCoroutine);

                            shiftCoroutine = StartCoroutine(Shift());
                            carAnimator.Play("car_shift_down", 0, 0);
                            camAnimator.Play("cam_shift_down", 0, 0);
                        }
                    }
                    else                              //Pass Left
                    {
                        if (transform.position.x > -8.15f && isPassDone)
                        {
                            isPassDone = false;

                            if (transform.position.x > 1.64f && transform.position.x < 1.66f)
                            {
                                //Debug.Log("PassLeftLong");
                                StartCoroutine(PassLeftLong());
                            }
                            else
                            {
                                //Debug.Log("PassLeftShort");
                                StartCoroutine(PassLeftShort());
                            }
                            carAnimator.Play("car_pass_left", 0, 0);
                            camAnimator.Play("cam_pass_left", 0, 0);
                        }
                    }
                }
            }
        }*/
        #endregion

        #region TOUCH CONTROLS
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPosition = touch.position;
                    break;

                case TouchPhase.Ended:
                    direction = touch.position - startPosition;
                    direction = Vector3.Normalize(direction);     //Turning pixel difference to normalized vector value
                    
                    if (direction != new Vector2(0, 0) && 
                        isControlable)                            //Shift or Pass
                    {
                        if (direction.y >= 0)                     //Shift Up or Pass
                        {
                            if (direction.x >= 0)                 //Shift Up or Pass Right
                            {
                                if (direction.y >= direction.x)   //Shift Up
                                {
                                    if (gear < 3)
                                    {
                                        gear += 1;

                                        if(shiftCoroutine != null) //if we get coroutine is null error, codes below won't work
                                            StopCoroutine(shiftCoroutine); //we are stopping old coroutine before starting new one for stable and better speed system

                                        shiftCoroutine = StartCoroutine(Shift()); //assign coroutine and start same time
                                        carAnimator.Play("car_shift_up", 0, 0);
                                        camAnimator.Play("cam_shift_up", 0, 0);
                                    }
                                }
                                else                              //Pass Right
                                {
                                    if (transform.position.x < 7.85f && isPassDone)
                                    {
                                        isPassDone = false;

                                        if (transform.position.x < -1.94f && transform.position.x > -1.96f)
                                        {
                                            //Debug.Log("PassRightLong");
                                            StartCoroutine(PassRightLong());
                                        }
                                        else
                                        {
                                            //Debug.Log("PassRightShort");
                                            StartCoroutine(PassRightShort());
                                        }
                                        carAnimator.Play("car_pass_right", 0, 0);
                                        camAnimator.Play("cam_pass_right", 0, 0);
                                    }
                                }
                            }
                            else if (direction.x < 0)             //Shift Up or Pass Left
                            {
                                if (direction.y >= -direction.x)  //Shift Up
                                {
                                    if (gear < 3)
                                    {
                                        gear += 1;

                                        if (shiftCoroutine != null)
                                            StopCoroutine(shiftCoroutine);

                                        shiftCoroutine = StartCoroutine(Shift());
                                        carAnimator.Play("car_shift_up", 0, 0);
                                        camAnimator.Play("cam_shift_up", 0, 0);
                                    }
                                }
                                else                              //Pass Left
                                {
                                    if (transform.position.x > -8.15f && isPassDone)
                                    {
                                        isPassDone = false;

                                        if (transform.position.x > 1.64f && transform.position.x < 1.66f)
                                        {
                                            //Debug.Log("PassLeftLong");
                                            StartCoroutine(PassLeftLong());
                                        }
                                        else
                                        {
                                            //Debug.Log("PassLeftShort");
                                            StartCoroutine(PassLeftShort());
                                        }
                                        carAnimator.Play("car_pass_left", 0, 0);
                                        camAnimator.Play("cam_pass_left", 0, 0);
                                    }
                                }
                            }
                        }
                        else if (direction.y < 0)                 //Shift Down or Pass
                        {
                            if (direction.x >= 0)                 //Shift Down or Pass Right
                            {
                                if (-direction.y >= direction.x)  //Shift Down
                                {
                                    if (gear > 1)
                                    {
                                        gear -= 1;

                                        if (shiftCoroutine != null)
                                            StopCoroutine(shiftCoroutine);

                                        shiftCoroutine = StartCoroutine(Shift());
                                        carAnimator.Play("car_shift_down", 0, 0);
                                        camAnimator.Play("cam_shift_down", 0, 0);
                                    }
                                }
                                else                              //Pass Right
                                {
                                    if (transform.position.x < 7.85f && isPassDone)
                                    {
                                        isPassDone = false;

                                        if (transform.position.x < -1.94f && transform.position.x > -1.96f)
                                        {
                                            //Debug.Log("PassRightLong");
                                            StartCoroutine(PassRightLong());
                                        }
                                        else
                                        {
                                            //Debug.Log("PassRightShort");
                                            StartCoroutine(PassRightShort());
                                        }
                                        carAnimator.Play("car_pass_right", 0, 0);
                                        camAnimator.Play("cam_pass_right", 0, 0);
                                    }
                                }
                            }
                            else if (direction.x < 0)             //Shift Down or Pass Left
                            {
                                if (-direction.y >= -direction.x) //Shift Down
                                {
                                    if (gear > 1)
                                    {
                                        gear -= 1;

                                        if (shiftCoroutine != null)
                                            StopCoroutine(shiftCoroutine);

                                        shiftCoroutine = StartCoroutine(Shift());
                                        carAnimator.Play("car_shift_down", 0, 0);
                                        camAnimator.Play("cam_shift_down", 0, 0);
                                    }
                                }
                                else                              //Pass Left
                                {
                                    if (transform.position.x > -8.15f && isPassDone)
                                    {
                                        isPassDone = false;

                                        if (transform.position.x > 1.64f && transform.position.x < 1.66f)
                                        {
                                            //Debug.Log("PassLeftLong");
                                            StartCoroutine(PassLeftLong());
                                        }
                                        else
                                        {
                                            //Debug.Log("PassLeftShort");
                                            StartCoroutine(PassLeftShort());
                                        }
                                        carAnimator.Play("car_pass_left", 0, 0);
                                        camAnimator.Play("cam_pass_left", 0, 0);
                                    }
                                }
                            }
                        }
                    }                    
                    break;
            }            
        }
        #endregion             
    }

    private IEnumerator Shift()
    {
        //Debug.Log("Gear: " + gear);

        if (gear == 1)
        {
            while (speed < lowGearSpeed)
            {
                speed += accelarationSpeed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            while (speed > lowGearSpeed)
            {
                speed -= decelerationSpeed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        else if (gear == 2)
        {
            while (speed < midGearSpeed)
            {
                speed += accelarationSpeed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            while (speed > midGearSpeed)
            {
                speed -= decelerationSpeed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        else if (gear == 3)
        {
            while (speed < highGearSpeed)
            {
                speed += accelarationSpeed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }        
    }

    #region OLD SHIFTING SYSTEM (CAUSES A SPEED BUG IF YOU CHANGE GEAR +2-1 OR -2+1 TOO FAST) - A STABLE SYSTEM DEVELOPED ABOVE
    /*private IEnumerator ShiftUp()
    {
        Debug.Log("Gear: " + gear);

        if (gear == 2)
        {            
            while (speed < midGearSpeed)
            {
                speed += accelarationSpeed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }            
        }
        else if(gear == 3)
        {
            while (speed < highGearSpeed)
            {
                speed += accelarationSpeed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }            
        }
    }

    private IEnumerator ShiftDown()
    {
        Debug.Log("Gear: " + gear);

        if (gear == 2)
        {
            while (speed > midGearSpeed)
            {
                speed -= decelerationSpeed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        else if (gear == 1)
        {
            while (speed > lowGearSpeed)
            {
                speed -= decelerationSpeed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }*/
    #endregion

    private IEnumerator PassRightShort()
    {
        float targetPosX = transform.position.x + 3.1f;
        //Debug.Log("transformX: " + transform.position.x + " targetX: " + targetPosX);
        while (transform.position.x != targetPosX)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosX, transform.position.y, transform.position.z), passSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        isPassDone = true;
    }

    private IEnumerator PassRightLong()
    {
        float targetPosX = transform.position.x + 3.6f;
        //Debug.Log("transformX: " + transform.position.x + " targetX: " + targetPosX);
        while (transform.position.x != targetPosX)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosX, transform.position.y, transform.position.z), passSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        isPassDone = true;
    }

    private IEnumerator PassLeftShort()
    {
        float targetPosX = transform.position.x - 3.1f;
        //Debug.Log("transformX: " + transform.position.x + " targetX: " + targetPosX);
        while (transform.position.x != targetPosX)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosX, transform.position.y, transform.position.z), passSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        isPassDone = true;
    }

    private IEnumerator PassLeftLong()
    {
        float targetPosX = transform.position.x - 3.6f;
        //Debug.Log("transformX: " + transform.position.x + " targetX: " + targetPosX);
        while (transform.position.x != targetPosX)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosX, transform.position.y, transform.position.z), passSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        isPassDone = true;
    }      

    private IEnumerator TiresOiled()
    {
        isControlable = false; //Take controls
        
        if (shiftCoroutine != null) //stop coroutine to make sure already started coroutines won't intervene while player getting rekt
            StopCoroutine(shiftCoroutine);

        float stopMultiplier = speed / 2; //by that way we will stop in same seconds no matter our speed is. Useful for animation

        carAnimator.Play("car_slide", 0, 0);
        camAnimator.Play("cam_shake", 0, 0);

        gear = 1; //Early gear change in code sequance for HUD

        while (speed > 1) //almost stop the car
        {
            speed -= stopMultiplier * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        StartCoroutine(Shift()); //Speed up again from first gear
        isControlable = true; //Give controls back
    }

    public IEnumerator Stop()
    {
        isControlable = false;

        if (shiftCoroutine != null)
            StopCoroutine(shiftCoroutine);

        if (oilCoroutine != null)
            StopCoroutine(oilCoroutine);

        float stopMultiplier = speed / 2;

        carAnimator.Play("car_slide", 0, 0);
        camAnimator.Play("cam_shake", 0, 0);

        gear = 1;//Early gear change in code sequance for HUD

        while (speed > 0) //almost stop the car
        {
            speed -= stopMultiplier * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        speed = 0; //stop
    }

    private IEnumerator DrainFuel()
    {
        while (fuel > 0)
        {
            fuel -= 1;
            yield return new WaitForSeconds(fuelDrainRate);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "AI" && !gameManager.isGameOver)
        {
            Debug.Log("You have crashed!");
            crashed = true;
        }
        if (other.gameObject.tag == "Obstacle" && !gameManager.isGameOver)
        {
            Debug.Log("Your tires are oiled! You lost control, sliding and stopping. Shifting down to first gear.");
            StartCoroutine(uiManager.OilWarning());
            oilCoroutine = StartCoroutine(TiresOiled());
        }
        if (other.gameObject.tag == "Fuel" && !gameManager.isGameOver)
        {
            Debug.Log("You have refueled.");

            if (fillAmount + fuel > 100)
                fuel = 100;
            else
                fuel += fillAmount;

            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Tire" && !gameManager.isGameOver)
        {
            Debug.Log("You have collected a tire.");
            tiresCollected += 1;
            Destroy(other.gameObject);
        }
    }
}
