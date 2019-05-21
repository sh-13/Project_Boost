using UnityEngine.SceneManagement;
using UnityEngine;

public class Rocket : MonoBehaviour {

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] AudioClip DeathClip;
    [SerializeField] AudioClip ThrustClip;
    [SerializeField] AudioClip LevelCompleteClip;
    [SerializeField] ParticleSystem DeathPartical;
    [SerializeField] ParticleSystem ThrustPartical;
    [SerializeField] ParticleSystem LevelCompletePartical;
    [SerializeField] float levelLoadDelay = 2f;
    Rigidbody rigidBody;
    AudioSource audioSource;
    enum State { Live, Dead, Trancending };
    State state = State.Live;
    //const int totalScene = 5;
    bool thrustbool = false;bool rotateleft = false;bool rotateright = false;
    

    // Use this for initialization
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (state == State.Live)
        {
            Thrust();
            Rotation();
        }
    }

    private void Thrust()
    {
        if (thrustbool == true)
        {
            ApplyThrust();
        }
        /*
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            ApplyThrust();
        }
        */
        else
        {
            ThrustPartical.Stop();
            audioSource.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(ThrustClip);
        }
        ThrustPartical.Play();
    }

    private void Rotation()
    {
        rigidBody.freezeRotation = true; // freeze rotation of physics

        if(rotateleft == true)
        {
            RotationLeft();
        }
        else if(rotateright == true)
        {
            RotationRight();
        }
        /*
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            RotationLeft();
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            RotationRight();
        }
        */
        rigidBody.freezeRotation = false; // release rotation of physics
    }

    public void MakeThrust(bool x)
    {
        thrustbool = x;
    }

    public void MakeRotationLeft(bool x)
    {
        rotateleft = x;
    }

    public void MakeRotationRight(bool x)
    {
        rotateright = x;
    }

    private void RotationRight()
    {
        float rotationFrame = rcsThrust * Time.deltaTime;
        transform.Rotate(-Vector3.forward * rotationFrame);
    }

    private void RotationLeft()
    {
        float rotationFrame = rcsThrust * Time.deltaTime;
        transform.Rotate(Vector3.forward * rotationFrame);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Live) return;
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                startfinishsequence();
                break;
            default:
                startdeathsequence();
                break;
        }
    }

    private void startdeathsequence()
    {
        state = State.Dead;
        audioSource.PlayOneShot(DeathClip);
        DeathPartical.Play();
        Invoke("RestartGame", levelLoadDelay);
    }

    private void startfinishsequence()
    {
        state = State.Trancending;
        audioSource.PlayOneShot(LevelCompleteClip);
        LevelCompletePartical.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void RestartGame()
    {
        state = State.Live;
        int x = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(x);
    }

    private void LoadNextScene()
    {
        state = State.Live;
        int x = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(x + 1);
    }
}
