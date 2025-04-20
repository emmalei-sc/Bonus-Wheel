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
        public Sprite icon;
        public string UIText;
        [Range(0f, 1f)]
        public float dropRate;
        public string winString;
    }

    [Tooltip("degrees")]
    [SerializeField] private float wheelArrowOffset;
    [SerializeField] private int minSpins;
    [SerializeField] private int maxSpins;
    [Tooltip("degrees/sec")]
    [SerializeField] private float spinSpeed;
    [SerializeField] private List<WheelSlice> wheelSlices = new List<WheelSlice>();
    [SerializeField] private Transform wheelParent;
    [SerializeField] private GameObject slicePrefab;

    private int _numSlices;
    private float _sliceAngle;

    private void Start()
    {
        GenerateSlices();
    }

    private void GenerateSlices()
    {
        _numSlices = wheelSlices.Count;

        // Calculate the angle of each slice, based on the number of slices
        _sliceAngle = 360f / _numSlices;
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
            // Invert the rotation so it follows our spin direction
            newSlice.transform.Rotate(new Vector3(0f, 0f, -rotateAngle));
        }
    }

    public void Spin()
    {
        // Pick a weighted random prize
        int randomPrize = GetRandomPrize();

        SpinToSlice(randomPrize);
    }

    private void SpinToSlice(int i)
    {
        // Find slice i and rotate the wheel to a random point in it
        float targetAngle = GetRandomSpinAngleToSlice(i);
        Debug.Log("Target Angle: " + targetAngle);

        // Rotate to target spin angle
        wheelParent.DORotate(Vector3.forward * targetAngle, spinSpeed, RotateMode.FastBeyond360)
            .SetSpeedBased()
            .SetEase(Ease.OutCirc)
            .OnComplete(() => LogReward(i));
    }

    private void LogReward(int i)
    {
        Debug.Log("You won " + wheelSlices[i].winString);
    }
    
    private int GetRandomPrize()
    {
        // TODO - implement weighted random alg
        return Random.Range(1, _numSlices);
    }

    private float GetRandomSpinAngleToSlice(int i)
    {
        // Make a random number of full spins
        int numSpins = Random.Range(minSpins, maxSpins);
        // Spin to the correct slice
        float angleToSlice = _sliceAngle * i;
        // Spin to a random point within the slice
        float randomAngleInSlice = Random.Range(wheelArrowOffset, _sliceAngle);

        return numSpins * 360 + angleToSlice + randomAngleInSlice;
    }
}
