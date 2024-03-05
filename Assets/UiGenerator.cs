using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using SFB;
using TMPro;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class UiGenerator : MonoBehaviour
{
    public Toggle toggleElement;
    public ContentContainer ContentContainerPrefab;
    public ContentContainer originContentContainer;
    public RectTransform contentContainer;
    public GameObject menuContainer;

    // public int horizontalRes = 1920;
    public const int HorizontalRes = 0;

    private int _imageSize = 300;

    // Specify the folder path where your PNG files are located
    private string folderPath = @"D:\Pictures\Test";

    [SerializeField] private Button refreshCollection;
    [SerializeField] private TMP_InputField imageSizeInput;

    private void Start()
    {
        // contentContainer.sizeDelta = new Vector2(horizontalRes, CheckForSubFolders(folderPath, ref originContentContainer));
        // Launch file browser to select a new directory
        folderPath = OpenFileBrowser();

        // Check if the folderPath is valid
        if (!string.IsNullOrEmpty(folderPath))
        {
            contentContainer.sizeDelta =
                new Vector2(HorizontalRes, CheckForSubFolders(folderPath, ref originContentContainer));
        }
        else
        {
            Debug.LogError("No folder selected.");
        }

        refreshCollection.onClick.AddListener(UpdateLayout);
        menuContainer.SetActive(false);
        imageSizeInput.text = _imageSize.ToString();
    }

    private void UpdateLayout()
    {
        if (_imageSize == Convert.ToInt32(imageSizeInput.text))
            return;

        _imageSize = Convert.ToInt32(imageSizeInput.text);
        originContentContainer.UpdateRectAndImageSizes(_imageSize, Screen.width);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            menuContainer.SetActive(!menuContainer.activeSelf);
    }

    private string OpenFileBrowser()
    {
        // Open a file browser to select a directory
        var paths = StandaloneFileBrowser.OpenFolderPanel("Select Folder", "", false);

        // Check if a folder was selected
        if (paths.Length > 0)
        {
            // Return the selected folder path
            return paths[0];
        }

        // Return an empty string if no folder was selected
        return string.Empty;
    }

    private int CheckForSubFolders(string parentFolderPath, ref ContentContainer parentContainer)
    {
        parentContainer.gameObject.name = Path.GetFileName(parentFolderPath);
        parentContainer.TitleText.text = Path.GetFileName(parentFolderPath);

        int containerHeight = 300;

        // Check if the parent folder exists
        if (Directory.Exists(parentFolderPath))
        {
            // Get an array of subdirectories within the parent folder
            string[] subDirectories = Directory.GetDirectories(parentFolderPath);

            if (subDirectories.Length > 0)
            {
                Debug.Log("Subdirectories in the parent folder:");

                foreach (string subDirectory in subDirectories)
                {
                    var currentContainer = Instantiate(ContentContainerPrefab, parentContainer.GroupView.transform);
                    parentContainer.ChildContainers.Add(currentContainer);
                    containerHeight += CheckForSubFolders(subDirectory, ref currentContainer);
                }

                parentContainer.GroupView.sizeDelta = new Vector2(HorizontalRes, containerHeight);
            }

            containerHeight += LoadSprites(ref parentContainer, parentFolderPath);
        }
        else
        {
            Debug.Log("The parent folder does not exist.");
        }

        parentContainer.RectTransform.sizeDelta = new Vector2(HorizontalRes, containerHeight);
        return containerHeight;
    }

    private int LoadSprites(ref ContentContainer currentContainer, string folderPath)
    {
        // Check if the folder exists
        if (Directory.Exists(folderPath))
        {
            // Get all PNG files in the folder
            List<string> imageFiles = new List<string>();
            imageFiles.AddRange(Directory.GetFiles(folderPath, "*.png").ToList());
            imageFiles.AddRange(Directory.GetFiles(folderPath, "*.jpeg").ToList());
            imageFiles.AddRange(Directory.GetFiles(folderPath, "*.jpg").ToList());

            if (imageFiles.Count > 0)
            {
                // Caches images within the container.
                currentContainer.ImageCount = imageFiles.Count;

                int gridHeight = currentContainer.UpdateRectSize(_imageSize, Screen.width);

                // Load and set each PNG file as the background of the Toggle UI element
                foreach (string filePath in imageFiles)
                {
                    // Load the PNG file into a Sprite
                    Sprite sprite = LoadSpriteFromFilePath(filePath);

                    // Set the Sprite as the background of the Toggle UI element
                    if (sprite != null)
                    {
                        // Instantiate a new Toggle UI element (you can also use an existing one)
                        Toggle toggleInstance = Instantiate(toggleElement, currentContainer.GridView.transform);
                        toggleInstance.name = Path.GetFileName(filePath);
                        // Set the background image of the Toggle
                        toggleInstance.targetGraphic.GetComponent<Image>().sprite = sprite;
                    }
                }

                return gridHeight;
            }

            currentContainer.GridView.sizeDelta = new Vector2(HorizontalRes, 0);
        }

        Debug.LogError("Folder does not exist: " + folderPath);
        return 0;
    }

    private static Sprite LoadSpriteFromFilePath(string filePath)
    {
        // Load the PNG file into a byte array
        byte[] fileData = File.ReadAllBytes(filePath);

        // Create a new texture and load the image data into it
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);

        // Create a Sprite from the texture
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        return sprite;
    }
}