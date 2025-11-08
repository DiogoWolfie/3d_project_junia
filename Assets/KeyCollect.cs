using UnityEngine;

public class KeyCollect : MonoBehaviour
{
    public string playerTag = "Player";
    public AudioClip collectSound;
    [Range(0f, 1f)]
    public float collectVolume = 1f;
    public Light keyLight;
    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag(playerTag))
        {
            collected = true;
            Debug.Log("Key collected!");

            // marca no player que ele tem a chave
            other.GetComponent<PlayerController>().OnKeyCollected();

            // toca som
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position, collectVolume);

            // apaga luz
            if (keyLight != null)
                Destroy(keyLight.gameObject);

            // destrói a chave
            Destroy(gameObject);
        }
    }
}
