using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    public GunSystem gun;
    public TextMeshProUGUI ammoText;

    void Update()
    {
        if (gun != null)
        {
            ammoText.text = gun.IsReloading() ? "Reloading" : $"{gun.GetCurrentAmmo()} / {gun.GetMaxAmmo()}";
        }
    }
}
