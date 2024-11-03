using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using System.Linq;

public class P_FileLoader : MonoBehaviour
{
    public GameObject panel_Title;
    public GameObject panel_FileViewer;

    public Transform contentPanel;
    public GameObject listItemPrefab;
    public TMP_InputField searchInput;
    public Button searchButton;

    private List<FileSystemInfo> currentItems = new List<FileSystemInfo>();
    private Stack<string> navigationHistory = new Stack<string>();


    void Start()
    {
        searchButton.onClick.AddListener(PerformSearch);
        LoadMyComputer();
    }

    void LoadMyComputer()
    {
        currentItems.Clear();
        navigationHistory.Clear();

        // 특별 폴더들을 추가
        currentItems.Add(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)));
        currentItems.Add(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));
        currentItems.Add(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)));
        currentItems.Add(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)));
        currentItems.Add(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)));

        // 드라이브들을 추가
        currentItems.AddRange(DriveInfo.GetDrives().Select(drive => drive.RootDirectory));

        DisplayFileList(currentItems);
    }

    void LoadItems(string path)
    {
        currentItems.Clear();
        try
        {
            // 상위 폴더로 가는 항목 추가
            if (Directory.GetParent(path) != null)
            {
                currentItems.Add(new DirectoryInfo(".."));
            }

            // 폴더 추가
            currentItems.AddRange(new DirectoryInfo(path).GetDirectories());

            // PDF 파일 추가
            currentItems.AddRange(new DirectoryInfo(path).GetFiles("*.pdf"));
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Error accessing {path}: {e.Message}");
        }

        DisplayFileList(currentItems);
    }

    void DisplayFileList(List<FileSystemInfo> items)
    {
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        GameObject newback = Instantiate(listItemPrefab, contentPanel);
        P_FileInfo dummyComponent = newback.GetComponent<P_FileInfo>();

        dummyComponent.SetFileName("...");
        dummyComponent.SetFileSize(" ");
        dummyComponent.SetCreationTime(" ");
        dummyComponent.SetFileImage(0);
        dummyComponent.btn_OpenFile.onClick.AddListener(NavigateBack);

        foreach (FileSystemInfo item in items)
        {
            GameObject newItem = Instantiate(listItemPrefab, contentPanel);
            P_FileInfo fileInfoComponent = newItem.GetComponent<P_FileInfo>();

            if (fileInfoComponent != null)
            {
                fileInfoComponent.SetFileName(item.Name == ".." ? "상위 폴더로" : item.Name);
                fileInfoComponent.SetCreationTime(item.CreationTime.ToString("yyyy-MM-dd"));

                if (item is FileInfo fileInfo)
                {
                    fileInfoComponent.SetFileSize(FormatFileSize(fileInfo.Length));
                    fileInfoComponent.btn_OpenFile.onClick.AddListener(() => OpenFile(fileInfo.FullName));
                    fileInfoComponent.SetFileImage(1);
                }
                else if (item is DirectoryInfo dirInfo)
                {
                    fileInfoComponent.SetFileSize("폴더");
                    fileInfoComponent.btn_OpenFile.onClick.AddListener(() => NavigateToFolder(dirInfo.FullName));
                    fileInfoComponent.SetFileImage(0);
                }
            }
        }
    }

    void NavigateToFolder(string folderPath)
    {
        if (folderPath == "..")
        {
            NavigateBack();
            return;
        }

        navigationHistory.Push(folderPath);
        LoadItems(folderPath);
    }

    void NavigateBack()
    {
        if (navigationHistory.Count > 0)
        {
            navigationHistory.Pop(); // 현재 폴더를 제거
            if (navigationHistory.Count > 0)
            {
                LoadItems(navigationHistory.Peek());
            }
            else
            {
                LoadMyComputer();
            }
        }
    }

    void OpenFile(string filePath)
    {
        Debug.Log($"Opening file: {filePath}");

        P_CreatorToolConnectMgr.Instance.pdfPath = filePath;

        panel_FileViewer.SetActive(false);
    }

    string FormatFileSize(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        int counter = 0;
        decimal number = (decimal)bytes;
        while (Math.Round(number / 1024) >= 1)
        {
            number = number / 1024;
            counter++;
        }
        return string.Format("{0:n1} {1}", number, suffixes[counter]);
    }

    void PerformSearch()
    {
        string searchTerm = searchInput.text.ToLower();
        List<FileSystemInfo> searchResults = currentItems.FindAll(item =>
            item.Name.ToLower().Contains(searchTerm));
        DisplayFileList(searchResults);
    }
}