using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackedImageManager : MonoBehaviour
{
    [SerializeField] public TMP_Text ImageTransformText;
    private ARTrackedImageManager aRTrackedImageManager;

    [SerializeField] private GameObject axisPrefab;

    public GameObject imageMarker;

    private void Awake()
    {
        aRTrackedImageManager = GetComponent<ARTrackedImageManager>();

        try
        {
            imageMarker = Instantiate(axisPrefab, Vector3.zero, Quaternion.identity);

        }
        catch (System.Exception e)
        {
            Debug.LogError("Instantiation failed: " + e.Message);
        }
    }

    private void OnEnable()
    {
        aRTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnDisable()
    {
        aRTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    private void OnImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        { //New Image Found
            UpdateImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        { //Existing image changed
            UpdateImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        { //Image tracking lost
            imageMarker.SetActive(false);
            ImageTransformText.text = "No Image";
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        //Get the transform of the detected image
        Vector3 position = trackedImage.transform.position;
        Quaternion rotation = trackedImage.transform.rotation;

        imageMarker.transform.position = position;
        imageMarker.transform.rotation = rotation;
        imageMarker.SetActive(true);
        setTransformText(position, rotation);
    }

    private void setTransformText(Vector3 position, Quaternion rotation)
    {
        string text = position.ToString() + "\n" + rotation.ToString();

        ImageTransformText.text = text;
    }
}
