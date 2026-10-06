using UnityEngine;
using UnityEngine.SceneManagement;

public class PeligroReinicio : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D otroColisionador)
    {
        Jugador jugador = otroColisionador.GetComponent<Jugador>();
        if (jugador != null)
        {
            if (CompareTag("puerquito"))
            {
                jugador.audioSource.PlayOneShot(jugador.audioPuerquito);
            }

            ReiniciarNivel();
        }
    }

    // Recarga la escena activa cuando el jugador toca este peligro.
    private void ReiniciarNivel()
    {
        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.name);
    }
}
