using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour  
{
    GameObject player;
    Coroutine shiftCoroutine; //At launch, aggressive AI doesn't slow down before reach max speed. And passes from inside other vehicle if it is close enough. This variable used for stop launching coroutine.
    //pass speed, front check range for start overtake, wait time in gear 1 for stuck aggressive AI, speed up rating, slow down rating, max low gear speed, max mid gear speed
    [SerializeField] float passSpeed, checkRange, waitTime, accelarationSpeed, decelerationSpeed, lowGearSpeed, midGearSpeed;
    [SerializeField] int minPassInterval, maxPassInterval; //min pass frequency, max pass frequency (Random.Range)
    [SerializeField] float destroyDistance; //min distance of player to destroy    
    public string type; //car type (calm, aggressive etc.)
    float distance; //Distance of player
    float speed, gear;
    bool isPassDone = true; //to prevent pass before old one is finished
    bool isOvertaking;
    bool passLeftSuccessful , passRightSuccessful; //to try overtake again

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (type == "calm")
        {
            gear = 1;
            Debug.Log("AI: Nice day for drive calmly ain't it? I drive in gear " + gear + ".");
            StartCoroutine(Overtake());
        }
        else if (type == "aggressive")
        {            
            gear = 2;
            Debug.Log("AI: I'm so aggressive arrgh! I drive in gear " + gear + "!");
            StartCoroutine(ChangeLanes()); //its aggressive driver. it will change lanes for fun. Based on interval time
            StartCoroutine(Overtake());
        }
        speed = lowGearSpeed; //All AI starts with at least low gear speed to prevent spawn crashes
        shiftCoroutine = StartCoroutine(Shift()); //Speed ups on start if its agressive AI        
    }

    void Update()
    {
        distance = player.transform.position.z - transform.position.z;

        if (distance >= destroyDistance)
        {
            Destroy(gameObject); //Destroy AI if we are further ahead. for optimization purposes
        }

        transform.position += Vector3.forward * Time.deltaTime * speed;

        Debug.DrawRay(transform.position + Vector3.up, Vector3.forward * checkRange, Color.red);
        Debug.DrawRay(transform.position + Vector3.up, (Vector3.right + Vector3.forward * 1.415f) * 5f, Color.yellow);
        Debug.DrawRay(transform.position + Vector3.up, Vector3.right * 3.1f, Color.yellow);
        Debug.DrawRay(transform.position + Vector3.up, (Vector3.right - Vector3.forward * 1.415f) * 5f, Color.yellow);
        Debug.DrawRay(transform.position + Vector3.up, (Vector3.left + Vector3.forward * 1.415f) * 5f, Color.yellow);
        Debug.DrawRay(transform.position + Vector3.up, Vector3.left * 3.1f, Color.yellow);
        Debug.DrawRay(transform.position + Vector3.up, (Vector3.left - Vector3.forward * 1.415f) * 5f, Color.yellow);
    }

    private IEnumerator Shift()
    {
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
            yield return new WaitForSeconds(1f); //Aggressive AI waits for some time before speeding up. Beacuse raycast sometimes doesn't detect hit so it will give it a second chance to detect if there is a car
            while (speed < midGearSpeed)
            {
                speed += accelarationSpeed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    //NOTE:
    //AI says whatever it does. Carefull when reading. Passing and Overtaking is not same.
    //It will pass for fun if its agressive. But it will overtake to prevent accidents no matter agressive or calm AI.

    private IEnumerator ChangeLanes()  //Randomly lane changer for aggressive AI
    {
        for (; ; ) //loop forever
        {
            int passInterval = Random.Range(minPassInterval, maxPassInterval); //Random number for pass interval to prevent synchronous pass crashes
            yield return new WaitForSeconds(passInterval);

            if (isPassDone && !isOvertaking)
            {
                if (transform.position.x < 4.76f && transform.position.x > -5.06f)
                {
                    int randomNumber = Random.Range(1, 3);  //Means 1 or 2
                    Debug.Log("AI: I'm thinking of a number and it is " + randomNumber + ".");

                    if (randomNumber == 1)
                    {
                        if (passLeftSuccessful)
                        {
                            Debug.Log("AI: And I have decided pass to left.");
                            if (transform.position.x > 1.64f && transform.position.x < 1.66f)                                                     
                                StartCoroutine(PassLeftLong());
                            else
                                StartCoroutine(PassLeftShort());
                        }
                        else if (passRightSuccessful)
                        {
                            Debug.Log("AI: Passing to left is not possible. I'm passing to right.");
                            if (transform.position.x < -1.94f && transform.position.x > -1.96f)
                                StartCoroutine(PassRightLong());
                            else
                                StartCoroutine(PassRightShort());
                        }
                        else
                        {
                            Debug.Log("AI: Passing is impossible.");
                            passLeftSuccessful = true;
                            passRightSuccessful = true;
                        }
                    }
                    else if (randomNumber == 2)
                    {
                        if (passRightSuccessful)
                        {
                            Debug.Log("AI: And I have decided pass to left.");
                            if (transform.position.x < -1.94f && transform.position.x > -1.96f)
                                StartCoroutine(PassRightLong());
                            else
                                StartCoroutine(PassRightShort());
                        }
                        else if (passLeftSuccessful)
                        {
                            Debug.Log("AI: Passing to right is not possible. I'm passing to left.");
                            if (transform.position.x > 1.64f && transform.position.x < 1.66f)
                                StartCoroutine(PassLeftLong());
                            else
                                StartCoroutine(PassLeftShort());
                        }
                        else
                        {
                            Debug.Log("AI: Passing is impossible.");
                            passLeftSuccessful = true;
                            passRightSuccessful = true;
                        }
                    }
                }
                else if (transform.position.x < -8.14f)
                {
                    Debug.Log("AI: I'm passing to right.");
                    StartCoroutine(PassRightShort());
                }
                else if (transform.position.x > 7.84f)
                {
                    Debug.Log("AI: I'm passing to left.");
                    StartCoroutine(PassLeftShort());
                }
            }     
        }
    }

    private IEnumerator Overtake()  //overtake to prevent AI accidents
    {
        for (; ; ) //loop forever
        {
            yield return null;
            //return new WaitForSeconds(checkInterval);

            if (Physics.Raycast(transform.position + Vector3.up, Vector3.forward,out RaycastHit hit, checkRange))
            {
                Debug.Log("AI: There is a car in front of me. I'm overtaking.");
                Debug.DrawRay(transform.position + Vector3.up, Vector3.forward * checkRange, Color.red);

                if (isPassDone && (hit.collider.tag == "AI" || hit.collider.tag == "Player"))
                {
                    isOvertaking = true;

                    if (transform.position.x < 4.76f && transform.position.x > -5.06f)
                    {
                        int randomNumber = Random.Range(1, 3);  //Means 1 or 2
                        Debug.Log("AI: I'm thinking of a number and it is " + randomNumber + ".");

                        if (randomNumber == 1)
                        {                            
                            if (passLeftSuccessful)
                            {
                                Debug.Log("AI: And I have decided to overtake from left.");
                                if (transform.position.x > 1.64f && transform.position.x < 1.66f)
                                StartCoroutine(PassLeftLong());
                                else
                                StartCoroutine(PassLeftShort());
                            }
                            else if (passRightSuccessful)
                            {
                                Debug.Log("AI: Overtaking from left is not possible. I'm overtaking from right.");
                                if (transform.position.x < -1.94f && transform.position.x > -1.96f)
                                StartCoroutine(PassRightLong());
                                else
                                StartCoroutine(PassRightShort());
                            }
                            else if (type == "aggressive")
                            {
                                if (shiftCoroutine != null) //if we get coroutine is null error, codes below won't work
                                    StopCoroutine(shiftCoroutine);

                                gear = 1;
                                shiftCoroutine = StartCoroutine(Shift());
                                Debug.Log("AI: Overtaking is imposible. I'm slowing down.");
                                yield return new WaitForSeconds(waitTime);

                                if (shiftCoroutine != null) //if we get coroutine is null error, codes below won't work
                                    StopCoroutine(shiftCoroutine);

                                gear = 2;
                                shiftCoroutine = StartCoroutine(Shift());
                                Debug.Log("AI: I have waited enough. I'm speeding up.");
                                passLeftSuccessful = true;
                                passRightSuccessful = true;
                            }
                            else//This section for calm drivers.
                            {
                                Debug.Log("AI: Overtaking is impossible.");
                                passLeftSuccessful = true;
                                passRightSuccessful = true;
                            }
                        }
                        else if (randomNumber == 2)
                        {                            
                            if (passRightSuccessful)
                            {
                                Debug.Log("AI: And I have decided to overtake from right.");
                                if (transform.position.x < -1.94f && transform.position.x > -1.96f)
                                StartCoroutine(PassRightLong());
                                else
                                StartCoroutine(PassRightShort());
                            }
                            else if (passLeftSuccessful)
                            {
                                Debug.Log("AI: Overtaking from right is not possible. I'm overtaking from left.");
                                if (transform.position.x > 1.64f && transform.position.x < 1.66f)
                                StartCoroutine(PassLeftLong());
                                else
                                StartCoroutine(PassLeftShort());
                            }
                            else if (type == "aggressive")
                            {
                                if (shiftCoroutine != null) //if we get coroutine is null error, codes below won't work
                                    StopCoroutine(shiftCoroutine);

                                gear = 1;
                                shiftCoroutine = StartCoroutine(Shift());
                                Debug.Log("AI: Overtaking is imposible. I'm slowing down.");
                                yield return new WaitForSeconds(waitTime);

                                if (shiftCoroutine != null) //if we get coroutine is null error, codes below won't work
                                    StopCoroutine(shiftCoroutine);

                                gear = 2;
                                shiftCoroutine = StartCoroutine(Shift());
                                Debug.Log("AI: I have waited enough. I'm speeding up.");
                                passLeftSuccessful = true;
                                passRightSuccessful = true;
                            }
                            else//This section for calm drivers.
                            {
                                Debug.Log("AI: Overtaking is impossible.");
                                passLeftSuccessful = true;
                                passRightSuccessful = true;
                            }
                        }
                    }
                    else if (transform.position.x < -8.14f)
                    {
                        if (passRightSuccessful)
                        {
                            Debug.Log("AI: I'm overtaking from right.");
                            StartCoroutine(PassRightShort());
                        }
                        else if (type == "aggressive")
                        {
                            if (shiftCoroutine != null) //if we get coroutine is null error, codes below won't work
                                StopCoroutine(shiftCoroutine);

                            gear = 1;
                            shiftCoroutine = StartCoroutine(Shift());
                            Debug.Log("AI: Overtaking is imposible. I'm slowing down.");
                            yield return new WaitForSeconds(waitTime);

                            if (shiftCoroutine != null) //if we get coroutine is null error, codes below won't work
                                StopCoroutine(shiftCoroutine);

                            gear = 2;
                            shiftCoroutine = StartCoroutine(Shift());
                            Debug.Log("AI: I have waited enough. I'm speeding up.");
                            passRightSuccessful = true;
                        }
                        else//This section for calm drivers.
                        {
                            Debug.Log("AI: Overtaking is impossible.");
                            passRightSuccessful = true;
                        }
                    }
                    else if (transform.position.x > 7.84f)
                    {
                        if (passLeftSuccessful)
                        {
                            Debug.Log("AI: I'm overtaking from left.");
                            StartCoroutine(PassLeftShort());
                        }
                        else if (type == "aggressive")
                        {
                            if (shiftCoroutine != null) //if we get coroutine is null error, codes below won't work
                                StopCoroutine(shiftCoroutine);

                            gear = 1;
                            shiftCoroutine = StartCoroutine(Shift());
                            Debug.Log("AI: Overtaking is imposible. I'm slowing down.");
                            yield return new WaitForSeconds(waitTime);

                            if (shiftCoroutine != null) //if we get coroutine is null error, codes below won't work
                                StopCoroutine(shiftCoroutine);

                            gear = 2;
                            shiftCoroutine = StartCoroutine(Shift());
                            Debug.Log("AI: I have waited enough. I'm speeding up.");
                            passLeftSuccessful = true;
                        }
                        else //This section for calm drivers.
                        {
                            Debug.Log("AI: Overtaking is impossible.");
                            passLeftSuccessful = true;
                        }
                    }
                }
            }
        }        
    }

    private IEnumerator PassRightShort()
    {
        bool empty = false;

        Physics.Raycast(transform.position + Vector3.up, Vector3.right + Vector3.forward * 1.415f, out RaycastHit hit, 5f); //shooting rays nested beacuse if at least one of them is null, gives exception error

        if (hit.collider == null || (hit.collider.tag != "AI" && hit.collider.tag != "Player"))
        {
            Physics.Raycast(transform.position + Vector3.up, Vector3.right, out hit, 3.1f);
            
            if (hit.collider == null || (hit.collider.tag != "AI" && hit.collider.tag != "Player"))
            {
                Physics.Raycast(transform.position + Vector3.up, Vector3.right - Vector3.forward * 1.415f, out hit, 5f);

                if (hit.collider == null || (hit.collider.tag != "AI" && hit.collider.tag != "Player"))
                    empty = true;
            }
        }
            
        if (empty) 
        {            
            float targetPosX = transform.position.x + 3.1f;
            isPassDone = false;

            while (transform.position.x != targetPosX)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosX, transform.position.y, transform.position.z), passSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            isPassDone = true;
            passRightSuccessful = true;
            isOvertaking = false;
        }
        else
        {
            passRightSuccessful = false;
            Debug.Log("AI: There is a car on my right. Changed my mind.");
            /*Debug.DrawRay(transform.position + Vector3.up, (Vector3.right + Vector3.forward * 1.415f) * 5f, Color.yellow);
            Debug.DrawRay(transform.position + Vector3.up, Vector3.right * 3.1f, Color.yellow);
            Debug.DrawRay(transform.position + Vector3.up, (Vector3.right - Vector3.forward * 1.415f) * 5f, Color.yellow);*/
        }
    }

    private IEnumerator PassRightLong()
    {
        bool empty = false;

        Physics.Raycast(transform.position + Vector3.up, Vector3.right + Vector3.forward * 1.415f, out RaycastHit hit, 5f);

        if (hit.collider == null || (hit.collider.tag != "AI" && hit.collider.tag != "Player"))
        {
            Physics.Raycast(transform.position + Vector3.up, Vector3.right, out hit, 3.1f);

            if (hit.collider == null || (hit.collider.tag != "AI" && hit.collider.tag != "Player"))
            {
                Physics.Raycast(transform.position + Vector3.up, Vector3.right - Vector3.forward * 1.415f, out hit, 5f);

                if (hit.collider == null || (hit.collider.tag != "AI" && hit.collider.tag != "Player"))
                    empty = true;
            }
        }

        if (empty)
        {
            float targetPosX = transform.position.x + 3.6f;
            isPassDone = false;

            while (transform.position.x != targetPosX)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosX, transform.position.y, transform.position.z), passSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            isPassDone = true;
            passRightSuccessful = true;
            isOvertaking = false;
        }
        else
        {
            passRightSuccessful = false;
            Debug.Log("AI: There is a car on my right. Changed my mind.");
            /*Debug.DrawRay(transform.position + Vector3.up, (Vector3.right + Vector3.forward * 1.415f) * 5f, Color.yellow);
            Debug.DrawRay(transform.position + Vector3.up, Vector3.right * 3.1f, Color.yellow);
            Debug.DrawRay(transform.position + Vector3.up, (Vector3.right - Vector3.forward * 1.415f) * 5f, Color.yellow);*/
        }
    }

    private IEnumerator PassLeftShort()
    {
        bool empty = false;

        Physics.Raycast(transform.position + Vector3.up, Vector3.left + Vector3.forward * 1.415f, out RaycastHit hit, 5f);

        if (hit.collider == null || (hit.collider.tag != "AI" && hit.collider.tag != "Player"))
        {
            Physics.Raycast(transform.position + Vector3.up, Vector3.left, out hit, 3.1f);

            if (hit.collider == null || (hit.collider.tag != "AI" && hit.collider.tag != "Player"))
            {
                Physics.Raycast(transform.position + Vector3.up, Vector3.left - Vector3.forward * 1.415f, out hit, 5f);

                if (hit.collider == null || (hit.collider.tag != "AI" && hit.collider.tag != "Player"))
                    empty = true;
            }
        }

        if (empty)
        {
            float targetPosX = transform.position.x - 3.1f;
            isPassDone = false;            

            while (transform.position.x != targetPosX)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosX, transform.position.y, transform.position.z), passSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            isPassDone = true;
            passLeftSuccessful = true;
            isOvertaking = false;
        }
        else
        {
            passLeftSuccessful = false;
            Debug.Log("AI: There is a car on my left. Changed my mind.");
            /*Debug.DrawRay(transform.position + Vector3.up, (Vector3.left + Vector3.forward * 1.415f) * 5f, Color.yellow);
            Debug.DrawRay(transform.position + Vector3.up, Vector3.left * 3.1f, Color.yellow);
            Debug.DrawRay(transform.position + Vector3.up, (Vector3.left - Vector3.forward * 1.415f) * 5f, Color.yellow);*/
        }        
    }

    private IEnumerator PassLeftLong()
    {
        bool empty = false;

        Physics.Raycast(transform.position + Vector3.up, Vector3.left + Vector3.forward * 1.415f, out RaycastHit hit, 5f);

        if (hit.collider == null || (hit.collider.tag != "AI" && hit.collider.tag != "Player"))
        {
            Physics.Raycast(transform.position + Vector3.up, Vector3.left, out hit, 3.1f);

            if (hit.collider == null || (hit.collider.tag != "AI" && hit.collider.tag != "Player"))
            {
                Physics.Raycast(transform.position + Vector3.up, Vector3.left - Vector3.forward * 1.415f, out hit, 5f);

                if (hit.collider == null || (hit.collider.tag != "AI" && hit.collider.tag != "Player"))
                    empty = true;
            }
        }

        if (empty)
        {
            float targetPosX = transform.position.x - 3.6f;
            isPassDone = false;
            while (transform.position.x != targetPosX)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosX, transform.position.y, transform.position.z), passSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            isPassDone = true;
            passLeftSuccessful = true;
            isOvertaking = false;
        }
        else
        {
            passLeftSuccessful = false;
            Debug.Log("AI: There is a car on my left. Changed my mind.");
            /*Debug.DrawRay(transform.position + Vector3.up, (Vector3.left + Vector3.forward * 1.415f) * 5f, Color.yellow);
            Debug.DrawRay(transform.position + Vector3.up, Vector3.left * 3.1f, Color.yellow);
            Debug.DrawRay(transform.position + Vector3.up, (Vector3.left - Vector3.forward * 1.415f) * 5f, Color.yellow);*/
        }
    }
}
