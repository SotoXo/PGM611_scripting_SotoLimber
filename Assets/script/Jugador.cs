using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class Jugador : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidadMovimiento = 5f;
    [SerializeField] private float fuerzaSalto = 7f;

    private readonly HashSet<Collider2D> superficiesDeSuelo = new();
    private Rigidbody2D cuerpoRigido;
    private SpriteRenderer renderizadorSprite;
    private float direccionHorizontal;
    private bool estaEnSuelo;
    private bool saltoSolicitado;

    private void Awake()
    {
        cuerpoRigido = GetComponent<Rigidbody2D>();
        renderizadorSprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        LeerMovimientoHorizontal();
        ActualizarOrientacion();

        if (Input.GetKeyDown(KeyCode.Space) && estaEnSuelo)
        {
            saltoSolicitado = true;
        }
    }

    private void FixedUpdate()
    {
        cuerpoRigido.linearVelocity = new Vector2(
            direccionHorizontal * velocidadMovimiento,
            cuerpoRigido.linearVelocity.y
        );

        if (!saltoSolicitado)
        {
            return;
        }

        cuerpoRigido.linearVelocity = new Vector2(
            cuerpoRigido.linearVelocity.x,
            fuerzaSalto
        );
        saltoSolicitado = false;
    }

    private void LeerMovimientoHorizontal()
    {
        bool moverIzquierda = Input.GetKey(KeyCode.A);
        bool moverDerecha = Input.GetKey(KeyCode.D);

        if (moverIzquierda == moverDerecha)
        {
            direccionHorizontal = 0f;
            return;
        }

        direccionHorizontal = moverIzquierda ? -1f : 1f;
    }

    private void ActualizarOrientacion()
    {
        if (renderizadorSprite == null || direccionHorizontal == 0f)
        {
            return;
        }

        renderizadorSprite.flipX = direccionHorizontal < 0f;
    }

    private void OnCollisionEnter2D(Collision2D colision)
    {
        RegistrarContactoConSuelo(colision);
    }

    private void OnCollisionStay2D(Collision2D colision)
    {
        RegistrarContactoConSuelo(colision);
    }

    private void OnCollisionExit2D(Collision2D colision)
    {
        superficiesDeSuelo.Remove(colision.collider);
        ActualizarEstadoDeSuelo();
    }

    private void RegistrarContactoConSuelo(Collision2D colision)
    {
        bool contactoInferior = false;

        foreach (ContactPoint2D contacto in colision.contacts)
        {
            if (contacto.normal.y > 0.5f)
            {
                contactoInferior = true;
                break;
            }
        }

        if (contactoInferior)
        {
            superficiesDeSuelo.Add(colision.collider);
        }
        else
        {
            superficiesDeSuelo.Remove(colision.collider);
        }

        ActualizarEstadoDeSuelo();
    }

    private void ActualizarEstadoDeSuelo()
    {
        estaEnSuelo = superficiesDeSuelo.Count > 0;

        if (!estaEnSuelo)
        {
            saltoSolicitado = false;
        }
    }
}
