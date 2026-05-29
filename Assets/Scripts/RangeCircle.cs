using UnityEngine;

public class RangeCircle : MonoBehaviour
{

    public void SetupRange(float radius)
    {
        //Actual multiplier determined by testing: radius of 5 = scale of 0.53 (0.53 / 5 = 0.106) (Manually tested)
        float conversionFactor = 0.106f;

        //Calculates the exact scale based on the tower's current radius
        float finalScale = radius * conversionFactor;

        transform.localScale = new Vector3(finalScale, finalScale, 1f);
    }
}
