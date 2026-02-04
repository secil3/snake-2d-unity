using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Snake : MonoBehaviour
{
    [Header("Movement")]
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    private bool _isDead;
    private int _score;
    [SerializeField] private Food food;
    [SerializeField] private float stepTime = 0.15f;   // yılan hızı
    [SerializeField] private Transform bodyPrefab;     // Body prefab'ı buraya
    [SerializeField] private int initialSize = 4;
    [SerializeField] private TMP_Text gameOverText;

    private Vector2 _direction = Vector2.right;
    private Vector2 _nextDirection = Vector2.right;
    private float _nextStepTime;

    private readonly List<Transform> _segments = new List<Transform>();

    private void Awake()
{
    ResetState();
}


    private void Update()
    {
        // Input: bir sonraki yönü ayarla (tersine dönmeyi engelle)
        if (Input.GetKeyDown(KeyCode.W) && _direction != Vector2.down) _nextDirection = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S) && _direction != Vector2.up) _nextDirection = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A) && _direction != Vector2.right) _nextDirection = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D) && _direction != Vector2.left) _nextDirection = Vector2.right;

if (_isDead && Input.GetKeyDown(KeyCode.R))
{
    Time.timeScale = 1f;
    _isDead = false;
    ResetState();
}

        if (_isDead && Input.GetKeyDown(KeyCode.R))
{
    Time.timeScale = 1f;
    _isDead = false;
    ResetState();
}

    }

    private void FixedUpdate()
    {
        if (_isDead) return;

        if (Time.time < _nextStepTime) return;
        _nextStepTime = Time.time + stepTime;

        _direction = _nextDirection;

       for (int i = _segments.Count - 1; i > 0; i--)
       {
         Vector3 p = _segments[i - 1].position;
        _segments[i].position = new Vector3(Mathf.Round(p.x), Mathf.Round(p.y), 0f);
       }

        float x = Mathf.Round(transform.position.x) + _direction.x;
        float y = Mathf.Round(transform.position.y) + _direction.y;
        transform.position = new Vector3(x, y, 0f);

        // DUVARA ÇARPIŞ KONTROLÜ
        Collider2D hit = Physics2D.OverlapPoint(transform.position);
        if (hit != null && hit.CompareTag("Wall"))
        {
           GameOver();
           return;
        }
        // KENDİNE ÇARPMA KONTROLÜ
        for (int i = 1; i < _segments.Count; i++)
        {   
            if (Vector3.Distance(transform.position, _segments[i].position) < 0.01f)
              {
                GameOver();
                return;
              }
        }


    }
    private void ResetState()
{
    // Kuyruğu temizle (kafa hariç)
    for (int i = 1; i < _segments.Count; i++)
    {
        Destroy(_segments[i].gameObject);
    }

    _segments.Clear();
    _segments.Add(transform);

    // Kafayı grid'e oturt
    transform.position = Vector3.zero;

    // Başlangıç kuyruğunu yeniden oluştur
    for (int i = 1; i < initialSize; i++)
    {
        Transform segment = Instantiate(bodyPrefab);
        segment.position = new Vector3(transform.position.x - i, transform.position.y, 0f);
        _segments.Add(segment);
    }
gameOverText.gameObject.SetActive(false);

    _direction = Vector2.right;
    _nextDirection = Vector2.right;
    _nextStepTime = 0f;
    _score = 0;
    UpdateScoreUI();
}

private void GameOver()
{
    _isDead = true;
    Debug.Log("GAME OVER!");
    Time.timeScale = 0f;
    gameOverText.gameObject.SetActive(true);

}

    public void Grow()
    {
        if (bodyPrefab == null)
        {
            Debug.LogError("Body Prefab atanmadi!");
            return;
        }

        Transform segment = Instantiate(bodyPrefab);
        segment.position = _segments[_segments.Count - 1].position; 
        _segments.Add(segment);
        _score++;
        Debug.Log("Score: " + _score);
        UpdateScoreUI();

    }
    private void UpdateScoreUI()
{
    if (scoreText != null)
        scoreText.text = "Score: " + _score;
}

}
