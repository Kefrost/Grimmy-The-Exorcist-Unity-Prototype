using System.Collections;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    public void SetHp(float hp)
    {
        this.transform.localScale = new Vector3(hp, 1f);
    }

    public IEnumerator SetHpSmoothly(float hp)
    {
        float currentHp = this.transform.localScale.x;
        float diff = currentHp - hp;

        while (currentHp - hp > Mathf.Epsilon)
        {
            currentHp -= diff * Time.deltaTime;
            this.transform.localScale = new Vector3(currentHp, 1f);
            yield return null;
        }

        this.transform.localScale = new Vector3(hp, 1f);
    }
}
