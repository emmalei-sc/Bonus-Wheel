using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SpinValidationTests
{
    [Test]
    public void SpinToPrize_LandsOnCorrectSlice()
    {
        // Set up a test wheel
        Wheel wheel = new GameObject("TestWheel").AddComponent<Wheel>();
        var testSlices = new List<Wheel.WheelSlice>
        {
            new("Life 30 min", 0.2f),
            new("Brush 3X", 0.1f),
            new("Gems 35", 0.1f),
            new("Hammer 3X", 0.1f),
            new("Coins 750", 0.05f),
            new("Brush 1X", 0.2f),
            new("Gems 75", 0.05f),
            new("Hammer 1X", 0.2f),
        };
        wheel.SetTestSlices(testSlices);

        // Make sure our wheel is valid
        wheel.ValidateWheel();
        Assert.IsTrue(wheel.IsWheelValid());

        // Complete setup
        wheel.InitializeWheel();

        float numSlices = testSlices.Count;
        float sliceAngle = 360f / numSlices;

        // Test each prize
        for (int i=0; i<testSlices.Count; i++)
        {
            float minSliceAngle = sliceAngle * i + wheel.GetWheelArrowOffset();
            float maxSliceAngle = sliceAngle * i + sliceAngle;

            float result = wheel.SpinToPrizeAndGetFinalAngle(i);
            // Make sure the wheel is rotated to the correct sector
            Assert.That(result, Is.InRange(minSliceAngle, maxSliceAngle));
            // Confirm this is the correct prize
            Assert.AreEqual(testSlices[i].winString, wheel.GetReward(i));
        }
    }

}
