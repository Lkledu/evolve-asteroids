using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    [Header("UI (User Interface)")]
    [SerializeField]
    private Text m_ScoreText;
    [SerializeField]
    private Text m_HighscoreText;
    [SerializeField]
    private Text m_WaveText;
    [SerializeField]
    private Text m_TimeText;
    [SerializeField]
    private Text m_GenerationText;
    [SerializeField]
    private Text m_ChromosomeText;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject m_Asteroid;
    [SerializeField]
    private GameObject m_Ship;

    [Header("Gameplay")]
    [SerializeField]
    private int m_IncreaseEachWave = 8;
    [SerializeField]
    private float m_RespawnDistance = 2.5f;
    [SerializeField]
    private float m_RespawnTime = 2.0f;
    [SerializeField]
    private bool m_UseMaxTime = true;
    [SerializeField]
    private float m_MaxTime = 15.0f;
    [SerializeField]
    private bool m_ResetHighscore;

    [Header("Genetic Properties")]
    [SerializeField]
    private int m_PopulationSize = 100;
    [Range(0, 10)]
    [SerializeField]
    private int m_TournamentSelectionSize = 3;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float m_CrossoverRate = 0.5f;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float m_MutationRate = 0.02f;
    [SerializeField]
    private int m_MaxGeneration = 10;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float m_ElitismRate = 0.05f;
    [SerializeField]
    private bool m_EvaluateEliteAgain = true;
    [SerializeField]
    private string m_FileName;

    [Header("Random")]
    [SerializeField]
    private bool m_UseRandomSeed;
    [SerializeField]
    private int m_EvolutionaryRandomSeed = 11;
    [SerializeField]
    private int m_GameRandomSeed = 13;

    private List<Chromosome> m_Population = new List<Chromosome>();
    private int m_CurrentChromosome;
    private int m_CurrentGeneration;
    private float m_Time;
    private int m_Score;
    private int m_Highscore;
    private int m_Wave;
    private int m_AsteroidsRemaining;
    private bool m_Run;

    private readonly int k_ChromosomeLength = 256;
    private readonly string k_HighscoreKey = "highscore";

    private IEnumerator m_ResetCoroutine;

    [Header("LeastSquare")]
    private LeastSquare m_lSquare;

    public void Start()
    {
        if (m_ResetHighscore)
        {
            PlayerPrefs.DeleteAll();
        }

        Helper.ResetRandomNumber(m_EvolutionaryRandomSeed);
        m_Population = PopulationRandomInitialize();
        m_ResetCoroutine = ResetGame(false);
        StartCoroutine(m_ResetCoroutine);

        m_lSquare = new LeastSquare();
    }

    public List<Chromosome> PopulationRandomInitialize()
    {
        List<Chromosome> population = new List<Chromosome>();
        while (population.Count < m_PopulationSize)
        {
            Chromosome chromosome = new Chromosome(k_ChromosomeLength);
            if (!population.Contains(chromosome))
            {
                population.Add(chromosome);
            }
        }
        return new List<Chromosome>(population);
    }

    public Chromosome TournamentSelection()
    {
        List<Chromosome> chromosomes = new List<Chromosome>();
        for (int i = 0; i < m_TournamentSelectionSize; i++)
        {
            int index = Helper.NextInt(m_PopulationSize);
            Chromosome chromosome = m_Population[index].Clone() as Chromosome;
            chromosomes.Add(chromosome);
        }

        chromosomes.Sort();
        return chromosomes[0];
    }

    public IEnumerator ResetGame(bool useRespawnTime)
    {
        if (useRespawnTime)
        {
            yield return new WaitForSeconds(m_RespawnTime);
        }

        Random.InitState(m_GameRandomSeed);

        m_Highscore = PlayerPrefs.GetInt(k_HighscoreKey, 0);
        m_Score = 0;
        m_Wave = 1;
        m_Time = 0;

        UpdateHud();

        SpawnShip();
        SpawnShip();
        SpawnShip();
        SpawnShip();
        SpawnShip();

        SpawnAsteroids();

        m_Run = true;

        yield return null;
    }

    public void UpdateHud()
    {
        m_ScoreText.text = $"SCORE: {m_Score}";
        m_HighscoreText.text = "HIGHSCORE: " + m_Highscore;
        m_WaveText.text = string.Format("WAVE: {0}", m_Wave);
        if (m_UseMaxTime)
        {
            m_TimeText.text = $"TIME {(m_MaxTime - m_Time).ToString("0")}";
        }
        else
        {
            m_TimeText.text = $"TIME {m_Time.ToString("0")}";
        }
        m_GenerationText.text = $"GENERATION {m_CurrentGeneration + 1} / {m_MaxGeneration}";
        m_ChromosomeText.text = $"CHROMOSOME {m_CurrentChromosome + 1} / {m_PopulationSize}";
    }

    public List<Chromosome> Elitism()
    {
        m_Population.Sort();
        int length = (int)(m_PopulationSize * m_ElitismRate);

        List<Chromosome> chromosomes = new List<Chromosome>();
        for (int i = 0; i < length; i++)
        {
            Chromosome chromosome = m_Population[i].Clone() as Chromosome;
            chromosome.Survived++;
            chromosomes.Add(chromosome);
        }

        return chromosomes;
    }

    public void Save(bool append)
    {
        if (string.IsNullOrEmpty(m_FileName))
        {
            return;
        }

        using (StreamWriter file = new StreamWriter(m_FileName + ".xls", append))
        {
            double bestFitness = 0.0;
            double averageFitness = 0.0;
            for (int i = 0; i < m_PopulationSize; i++)
            {
                averageFitness += m_Population[i].Fitness;
                if (bestFitness < m_Population[i].Fitness)
                {
                    bestFitness = m_Population[i].Fitness;
                }
            }

            averageFitness /= (double)m_PopulationSize;
            file.WriteLine("{0}\t{1}", averageFitness, bestFitness);
        }

        using (StreamWriter file = new StreamWriter("chromosomes_" + m_FileName + ".xls", append))
        {
            for (int i = 0; i < m_PopulationSize; i++)
            {
                file.Write("{0}\t{1}\t{2}\t", m_Population[i].ToString(), m_Population[i].Fitness, m_Population[i].Survived);
            }

            file.WriteLine();
        }
    }

    public IEnumerator QuitGame()
    {
        yield return new WaitForSeconds(m_RespawnTime);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public bool IsContinue => m_CurrentGeneration < m_MaxGeneration;

    public void NextGeneration()
    {
        m_Population.Sort();

        Save(m_CurrentGeneration > 0);
        m_CurrentGeneration++;
        if (m_CurrentGeneration == m_MaxGeneration)
        {
            StartCoroutine(QuitGame());
        }
        else
        {
            List<Chromosome> newPopulation = new List<Chromosome>(Elitism());
            while (newPopulation.Count < m_PopulationSize)
            {
                Chromosome parent1 = TournamentSelection();
                Chromosome parent2 = TournamentSelection();

                Chromosome offspring = parent1.Crossover(parent2, m_CrossoverRate);
                offspring.Mutate(m_MutationRate);

                if (!newPopulation.Contains(offspring))
                {
                    newPopulation.Add(offspring);
                }
            }
            m_Population = new List<Chromosome>(newPopulation);

            if (m_EvaluateEliteAgain)
            {
                m_CurrentChromosome = 0;
            }
            else
            {
                m_CurrentChromosome = (int)(m_PopulationSize * m_ElitismRate);
            }
        }
    }

    public void Update()
    {
        if (m_Run)
        {
            if (m_UseMaxTime)
            {
                m_Time = Mathf.Clamp(m_Time + Time.deltaTime, 0, m_MaxTime);
            }
            else
            {
                m_Time += Time.deltaTime;
            }
            
            UpdateHud();

            if (m_UseMaxTime && m_Time >= m_MaxTime)
            {
                m_Run = false;
                GameObject ship = GameObject.FindGameObjectWithTag("Ship");
                ShipController shipController = ship.GetComponent<ShipController>();
                shipController.Kill();
            }
        }
    }

    private void SpawnShip()
    {
        Bounds bounds = Camera.main.OrthographicBounds();
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        
        GameObject ship = Instantiate(m_Ship, new Vector3(x,y,0), Quaternion.identity);
        Brain brain = ship.GetComponent<Brain>();
        brain.Chromosome = m_Population[m_CurrentChromosome];
    }

    private void SpawnAsteroids()
    {
        DestroyExistingObjects();

        m_AsteroidsRemaining = m_Wave * m_IncreaseEachWave;
        Bounds bounds = Camera.main.OrthographicBounds();

        GameObject ship = GameObject.FindGameObjectWithTag("Ship");

        int count = 0;
        while (count < m_AsteroidsRemaining)
        {
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);

            if (Vector3.Distance(new Vector3(x, y, 0), ship != null ? ship.transform.position : Vector3.zero) > m_RespawnDistance)
            {
                count++;
                float angle = Random.Range(0.0f, 360.0f);
                Instantiate(m_Asteroid, new Vector3(x, y, 0), Quaternion.Euler(0, 0, angle));
            }
        }

        UpdateHud();
    }

    private void DestroyExistingObjects()
    {
        DestroyExistingObjects("Big Asteroid");
        DestroyExistingObjects("Small Asteroid");
        DestroyExistingObjects("Bullet");
    }

    private void DestroyExistingObjects(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (var obj in objects)
        {
            Destroy(obj);
        }
    }

    public void SplitAsteroid(int size)
    {
        m_AsteroidsRemaining += size;
    }

    public void DecrementAsteroids()
    {
        m_AsteroidsRemaining--;
    }

    public double EvaluationFitness()
    {
        return m_Score;
    }

    public void DecrementLives()
    {
        m_lSquare.x_table.Add((int)m_Time);
        m_lSquare.y_table.Add(m_Highscore);

        m_Run = false;
        m_Population[m_CurrentChromosome].Fitness = EvaluationFitness();

        m_CurrentChromosome++;
        if (m_CurrentChromosome == m_PopulationSize)
        {
            NextGeneration();
            if (m_CurrentGeneration < m_MaxGeneration)
            {
                StopCoroutine(m_ResetCoroutine);
                m_ResetCoroutine = ResetGame(true);
                StartCoroutine(m_ResetCoroutine);
            }
        }
        else
        {
            StopCoroutine(m_ResetCoroutine);
            m_ResetCoroutine = ResetGame(true);
            StartCoroutine(m_ResetCoroutine);
        }
    }

    public void IncrementScore(int score)
    {
        m_Score += score;
        if (m_Score > m_Highscore)
        {
            PlayerPrefs.SetInt(k_HighscoreKey, m_Score);
            PlayerPrefs.Save();
        }

        UpdateHud();

        if (m_AsteroidsRemaining == 0)
        {
            m_Wave++;
            SpawnAsteroids();
        }
    }
}
