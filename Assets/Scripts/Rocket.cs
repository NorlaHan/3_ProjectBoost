//using System;
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField][Tooltip("Can only change in the editor. Not possible for other script")]
    float rcsThrust = 100f, mainThrust = 100f,thrustVol = 0.2f, decayRate = 0.25f, timeBrforeRestart = 2f;
    [SerializeField]
    AudioClip mainEngineClip, explosionClip, successClip;
    [SerializeField]
    ParticleSystem mainEnginePtl, explosionPtl, successPtl;
    [SerializeField]
    bool invulnerable = false, isTransitioning = false;

    //enum State {Alive, Dead, Transcending }
    //[Tooltip("The state of the player")]
    //State state = State.Alive;



    Rigidbody rigidBody;
    AudioSource audioSource;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update ()
    {        
        RespondToRotateInput();
        RespondToThrustInput();
        // TODO Only if debug 
        if (Debug.isDebugBuild)
        {
            RespondToDebugInput();
        }        
    }

    private void RespondToDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            invulnerable = !invulnerable;
        }
        if ((Input.GetKeyDown(KeyCode.L)))
        {
            OnFinishSequence();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //if (state != State.Alive){ return; }
        if (isTransitioning) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Fuel":
                print("Get fuel");  // TODO Add fule mechanism.
                break;
            case "Finish":
                OnFinishSequence();
                break;
            default:
                OnDeathSequence();
                break;
        }

        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white,2);
        }
        if (collision.relativeVelocity.magnitude > 2) {
            audioSource.volume = thrustVol;
            audioSource.Play();
        }
            
    }

    private void OnFinishSequence()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(successClip);
        successPtl.Play();
        rigidBody.isKinematic = true;
       // state = State.Transcending;
        isTransitioning = true;
        Invoke("LoadNextScene", timeBrforeRestart);
    }

    private void OnDeathSequence()
    {
        if (invulnerable) {return;}
        print("Dead");
        audioSource.Stop();
        audioSource.PlayOneShot(explosionClip);
        explosionPtl.Play();
        //state = State.Dead;
        isTransitioning = true;
        Invoke("LoadCurrentScene", timeBrforeRestart);
    }

    void LoadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //state = State.Alive;
        isTransitioning = false;
    }

    void LoadNextScene()
    {
        int currentScrene = SceneManager.GetActiveScene().buildIndex;
        if (currentScrene < (SceneManager.sceneCountInBuildSettings - 1))
        {
            SceneManager.LoadScene(currentScrene + 1);
        }
        else
        {
            Debug.Log("Congragulation! You Win!!!");
            SceneManager.LoadScene(0);
        }
        //state = State.Alive;
        isTransitioning = false;
    }

    private void RespondToRotateInput()
    {
        //if (state != State.Alive) { return; }   // Lose control if isn't alive
        if (isTransitioning) {return;}
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        // rigidBody.freezeRotation = true;        // Stop rotate acceleration 
        rigidBody.angularVelocity = Vector3.zero;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward* rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward* rotationThisFrame);
        }
        // Recover rotation
        rigidBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
    }

    private void RespondToThrustInput()
    {

       // if (Input.GetKey(KeyCode.Space) && /*state == State.Alive*/ !isTransitioning)    // Pushing thrust.
        if (Input.GetKey(KeyCode.Space) && !isTransitioning) 
        {
            ApplyThrusting();
        }
        else    // not pushing thrust but audio is playing. Function even not alive.
        {
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust()
    {
        if (audioSource.isPlaying && audioSource.volume > 0)
        {
            audioSource.volume -= (Time.deltaTime * decayRate);
        }
        else    // Stop particle when volume is 0
        {
            mainEnginePtl.Stop();
        }
    }

    private void ApplyThrusting()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
        if (!audioSource.isPlaying)     // Start playing audio.
        {
            //audioSource.Play();
            audioSource.PlayOneShot(mainEngineClip);
            audioSource.volume = thrustVol;
        }
        else if (audioSource.volume < thrustVol)    // Audio is plaing but volume isn't large enough.
        {
            audioSource.volume += (Time.deltaTime);
        }
        mainEnginePtl.Play();
    }
}
