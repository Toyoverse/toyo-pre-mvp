using System.Collections;
using UnityEngine;

public class UnboxingVfx : MonoBehaviour
{
    public ParticleSystem chargingParticle;
    public ParticleSystem growingParticle;
    public ParticleSystem chargedParticle;
    public ParticleSystem explosionParticle;
    public float chargingDuration;
    public float lightningDuration;

    private Color _rarityColor = Color.white;
    public RarityColorConfigSO rarityColorConfigSo;

    public void OnEnable()
    {
        SetParticleColor(chargedParticle, _rarityColor);
        SetParticleColor(explosionParticle, _rarityColor);
        StartCoroutine(StartParticlesEffects());
    }

    private IEnumerator StartParticlesEffects()
    {
        chargingParticle.gameObject.SetActive(true);
        growingParticle.gameObject.SetActive(true);
        yield return new WaitForSeconds(chargingDuration);
        chargingParticle.gameObject.SetActive(false);
        growingParticle.gameObject.SetActive(false);
        chargedParticle.gameObject.SetActive(true);
        yield return new WaitForSeconds(lightningDuration);
        chargedParticle.gameObject.SetActive(false);
        explosionParticle.gameObject.SetActive(true);
        while (explosionParticle.isPlaying)
        {
            yield return null;
        }
        explosionParticle.gameObject.SetActive(false);
    }

    /*private IEnumerator StartParticlesEffects()
    {
        SetParticleColor(chargedParticle, _rarityColor);
        SetParticleColor(explosionParticle, _rarityColor);
        chargingParticle.Play();
        growingParticle.Play();
        Debug.Log("CHARGING...");
        yield return new WaitForSeconds(chargingDuration);
        chargingParticle.Stop();
        growingParticle.Stop();
        var _chargedMain = chargedParticle.main;
        _chargedMain.loop = true;
        chargedParticle.Play();
        Debug.Log("LIGHTING...");
        yield return new WaitForSeconds(chargedDuration);
        chargedParticle.Stop();
        explosionParticle.Play();
        Debug.Log("EXPLOSION!");
    }*/

    private void SetParticleColor(ParticleSystem particle, Color color)
    {
        var _particleMain = particle.main;
        _particleMain.startColor = color;
    }

    public void SetRarityColor(TOYO_RARITY toyoRarity)
    {
        foreach (var _rarity in rarityColorConfigSo.rarityColors)
        {
            if(_rarity.rarity != toyoRarity)
                continue;
            _rarityColor = _rarity.color;
            break;
        }
    }
}