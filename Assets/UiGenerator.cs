using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using SFB;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class UiGenerator : MonoBehaviour
{
    public Toggle toggleElement;
    public ContentContainer ContentContainerPrefab;
    public ContentContainer originContentContainer;
    public RectTransform contentContainer;

    // public int horizontalRes = 1920;
    public int horizontalRes = Screen.width;
    // Specify the folder path where your PNG files are located
    private string folderPath = @"D:\Pictures\Test";

    void Start()
    {
        // contentContainer.sizeDelta = new Vector2(horizontalRes, CheckForSubFolders(folderPath, ref originContentContainer));
        // Launch file browser to select a new directory
        folderPath = OpenFileBrowser();
        
        // Check if the folderPath is valid
        if (!string.IsNullOrEmpty(folderPath))
        {
            contentContainer.sizeDelta = new Vector2(horizontalRes, CheckForSubFolders(folderPath, ref originContentContainer));
        }
        else
        {
            Debug.LogError("No folder selected.");
        }
    }

    string OpenFileBrowser()
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

    int CheckForSubFolders(string parentFolderPath, ref ContentContainer parentContainer)
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
                    containerHeight += CheckForSubFolders(subDirectory, ref currentContainer);
                }

                parentContainer.GroupView.sizeDelta = new Vector2(horizontalRes, containerHeight);
            }

            containerHeight += LoadSprites(ref parentContainer, parentFolderPath);
        }
        else
        {
            Debug.Log("The parent folder does not exist.");
        }

        parentContainer.RectTransform.sizeDelta = new Vector2(horizontalRes, containerHeight);
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
                int ceilToInt = imageFiles.Count % 6 == 0 ? Mathf.CeilToInt(imageFiles.Count / 6) : Mathf.CeilToInt(imageFiles.Count / 6) + 1;
                int gridHeight =  40 + (310 * ceilToInt);
                currentContainer.GridView.sizeDelta = new Vector2(horizontalRes, gridHeight);

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

            currentContainer.GridView.sizeDelta = new Vector2(horizontalRes, 0);
        }

        Debug.LogError("Folder does not exist: " + folderPath);
        return 0;
    }

    private Sprite LoadSpriteFromFilePath(string filePath)
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