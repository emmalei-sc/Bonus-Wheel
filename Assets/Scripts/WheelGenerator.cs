using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WheelGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct WheelSlice
    {
#if UNITY_EDITOR
        // Constructor for Unit Testing
        public WheelSlice(string winString, float dropRate, string UIText = "", Sprite icon = null)
        {
            this.icon = icon;
            this.UIText = UIText;
            this.dropRate = dropRate;
            this.winString = winString;
        }
#endif

        public Sprite icon;
        public string UIText;
        [Range(0f, 1f)]
        public float dropRate;
        public string winString;
    }

    [Header("Wheel Content")]
    [SerializeField] private List<WheelSlice> wheelSlices = new List<WheelSlice>();

    [Header("References")]
    [SerializeField] private Transform wheelParent;
    [SerializeField] private GameObject slicePrefab;

    private int _numSlices = 0;
    private float _sliceAngle = 0f;
    private bool _isWheelValid = false;

    private void Awake()
    {
        InitializeWheel();
    }

    private void Start()
    {
        GenerateSlices();
    }

#if UNITY_EDITOR
    public void SetTestSlices(List<WheelSlice> testSlices)
    {
        wheelSlices = testSlices;
    }
#endif

    public void InitializeWheel()
    {
        // Ensure wheel is set up properly
        ValidateWheel();
        if (!IsWheelValid())
            return;

        // Initialize variables
        _numSlices = wheelSlices.Count;
        _sliceAngle = 360f / _numSlices;
    }

    private void ValidateWheel()
    {
        bool isValid = true;
        // Ensure wheel is not empty
        if (wheelSlices.Count == 0)
        {
            Debug.LogWarning("Warning: Wheel is empty");
            isValid = false;
        }

        // Ensure drop rates add to 100%
        float totalSum = 0f;
        foreach (WheelSlice slice in wheelSlices)
        {
            totalSum += slice.dropRate;
        }
        if (totalSum != 1f)
        {
            Debug.LogWarning("Warning: Prize drop rates do not add up to 100%");
            isValid = false;
        }

        _isWheelValid = isValid;
    }

    private void GenerateSlices()
    {
        if (slicePrefab == null)
            return;
        if (!_isWheelValid)
            return;

        // Calculate the angle of each slice, based on the number of slices
        float halfAngle = _sliceAngle / 2f;

        // Create our slices
        for (int i = 0; i < _numSlices; i++)
        {
            WheelSlice sliceInfo = wheelSlices[i];

            // Instantiate the slice object
            GameObject newSlice = Instantiate(slicePrefab, wheelParent);

            // Set up prize icon and UI text
            SliceReferences sliceReferences = newSlice.GetComponent<SliceReferences>();
            sliceReferences.SetSprite(sliceInfo.icon);
            sliceReferences.SetText(sliceInfo.UIText);

            // Rotate this slice according to its index
            float rotateAngle = i * _sliceAngle + halfAngle; // Center in the middle of the slice
            // Flip the rotation so the order follows our spin direction
            newSlice.transform.Rotate(new Vector3(0f, 0f, -rotateAngle));
        }
    }

    public float GetDropRate(int prizeIndex)
    {
        return wheelSlices[prizeIndex].dropRate;
    }

    public string GetReward(int prizeIndex)
    {
        return wheelSlices[prizeIndex].winString;
    }

    public bool IsWheelValid()
    {
        return _isWheelValid;
    }

    public float GetNumberOfSlices()
    {
        return _numSlices;
    }

    public float GetSliceAngle()
    {
        return _sliceAngle;
    }
}
