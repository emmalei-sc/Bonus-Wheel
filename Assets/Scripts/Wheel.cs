using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class Wheel : MonoBehaviour
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

    [Header("Spin Settings")]
    [SerializeField] private int minSpins;
    [SerializeField] private int maxSpins;
    [Tooltip("degrees/sec")]
    [SerializeField] private float spinSpeed;

    [Header("Wheel Content")]
    [Tooltip("how many degrees the arrow is offset from 0")]
    [SerializeField] private float wheelArrowOffset;
    [SerializeField] private List<WheelSlice> wheelSlices = new List<WheelSlice>();

    [Header("References")]
    [SerializeField] private Transform wheelParent;
    [SerializeField] private GameObject slicePrefab;

    private int _numSlices = 0;
    private float _sliceAngle = 0f;
    private bool _isSpinning = false;
    private bool _isWheelValid = false;

    private void Start()
    {
        // Ensure wheel is set up properly
        ValidateWheel();

        if (IsWheelValid())
        {
            InitializeWheel(); // Initialize variables
            GenerateSlices(); // Draw our slices in the scene
        }
    }

    public void SetTestSlices(List<WheelSlice> testSlices)
    {
        wheelSlices = testSlices;
    }

    public void ValidateWheel()
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

    public void InitializeWheel()
    {
        _numSlices = wheelSlices.Count;
        _sliceAngle = 360f / _numSlices;
    }

    private void GenerateSlices()
    {
        if (slicePrefab == null)
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

    public void Spin()
    {
        if (!_isWheelValid)
        {
            Debug.Log("Wheel is not valid");
            return;
        }
        if (_isSpinning)
        {
            Debug.Log("Spin is already in progress");
            return;
        }

        // Pick a weighted random prize
        int randomPrize = GetRandomPrize();
        if (randomPrize >= 0 && randomPrize < _numSlices) // Sanity check in case we get an out-of-range value
            SpinToSlice(randomPrize);
    }
#if UNITY_EDITOR
    public float SpinToPrizeAndGetFinalAngle(int prizeIndex) // Wrapper for unit tests
    {
        return GetRandomSpinAngleToSlice(prizeIndex);
    }
#endif

    private int GetRandomPrize()
    {
        // Retrieve a weighted random prize
        float rand = Random.Range(0f, 1f); // Get a random percentage
        for (int i=0; i< _numSlices; i++)
        {
            // If our random percentage falls within this prize's range, return it
            if (rand < wheelSlices[i].dropRate)
            {
                return i;
            }
            rand -= wheelSlices[i].dropRate;
        }

        return -1; // This should never be possible
    }

    private void SpinToSlice(int prizeIndex)
    {
        _isSpinning = true;

        // Find the slice associatd with this prize and rotate the wheel to a random point in it
        float targetAngle = GetRandomSpinAngleToSlice(prizeIndex);

        // Rotate to target spin angle
        wheelParent.DORotate(Vector3.forward * targetAngle, spinSpeed, RotateMode.FastBeyond360)
            .SetSpeedBased()
            .SetEase(Ease.OutCirc)
            .OnComplete(() => EndSpin(prizeIndex)); // Take care of any logic cleanup
    }

    private float GetRandomSpinAngleToSlice(int prizeIndex)
    {
        // Make a random number of full spins
        int numSpins = Random.Range(minSpins, maxSpins);
        // Spin to the correct slice
        float angleToSlice = _sliceAngle * prizeIndex;
        // Spin to a random point within the slice
        float randomAngleInSlice = Random.Range(wheelArrowOffset, _sliceAngle);

        return numSpins * 360 + angleToSlice + randomAngleInSlice;
    }

    private void EndSpin(int prizeIndex)
    {
        _isSpinning = false;

        Debug.Log("You won " + GetReward(prizeIndex));
    }

    public string GetReward(int prizeIndex)
    {
        return wheelSlices[prizeIndex].winString;
    }

    public bool IsWheelValid()
    {
        return _isWheelValid;
    }

    public float GetWheelArrowOffset()
    {
        return wheelArrowOffset;
    }
}

// UnitTest outline

// Keep a dictionary of prize history
// For each new prize:
//      If rewardString already exists as a key, value++
//      If it doesn't exist, create and set value to 1