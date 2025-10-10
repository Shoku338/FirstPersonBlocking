using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Inventory : MonoBehaviour
{

    public static Inventory Instance { get; private set; }

    private int gearCount;
    public int maxGearCount = 4;

    private int mushroomCount;
    public int maxMushroomCount = 99;

    [SerializeField] TMP_Text gearTextBox;
    [SerializeField] TMP_Text mushroomTextBox;

    void Awake()
    {
        // Make sure only one inventory exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        gearCount = 0;
        gearTextBox.text = gearCount + "/" + maxGearCount;
        mushroomCount = 0;
        mushroomTextBox.text = mushroomCount + "/" + maxMushroomCount;
    }

    private void Update()
    {
        if(gearCount >= maxGearCount){
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }

    public void AddMushroom(){
        mushroomCount++;
        mushroomTextBox.text = mushroomCount + "/" + maxMushroomCount;
    }

    public void AddGearCount(){
        gearCount++;
        gearTextBox.text = gearCount + "/" + maxGearCount;
    }

    public bool HasMushrooms(int amount)
    {
        return mushroomCount >= amount;
    }

    public void RemoveMushrooms(int amount)
    {
        mushroomCount = Mathf.Max(0, mushroomCount - amount);
        mushroomTextBox.text = mushroomCount + "/" + maxMushroomCount;
    }

}
