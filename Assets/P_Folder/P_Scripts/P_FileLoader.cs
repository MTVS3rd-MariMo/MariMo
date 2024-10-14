using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class P_FileLoader : MonoBehaviour
{
    public Transform contentPanel;
    public GameObject listItemPrefab;
    public TMP_InputField searchInput;
    public Button searchButton;

    private List<string> allFiles = new List<string>();

    void Start()
    {
        searchButton.onClick.AddListener(PerformSearch);
        LoadAllFiles(Application.dataPath);
        DisplayFileList(allFiles);
    }

    void LoadAllFiles(string rootPath)
    {
        allFiles.Clear();
        SearchFiles(rootPath);
    }

    void SearchFiles(string path)
    {
        try
        {
            string[] files = Directory.GetFiles(path);
            allFiles.AddRange(files);

            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                SearchFiles(directory);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Error accessing {path}: {e.Message}");
        }
    }

    void DisplayFileList(List<string> files)
    {
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (string file in files)
        {
            GameObject newItem = Instantiate(listItemPrefab, contentPanel);
            Text itemText = newItem.GetComponentInChildren<Text>();
            if (itemText != null)
            {
                itemText.text = Path.GetFileName(file);
            }
        }
    }

    void PerformSearch()
    {
        string searchTerm = searchInput.text.ToLower();
        List<string> searchResults = allFiles.FindAll(file =>
            Path.GetFileName(file).ToLower().Contains(searchTerm));
        DisplayFileList(searchResults);
    }
}
