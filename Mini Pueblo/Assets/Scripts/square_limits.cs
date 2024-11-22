using UnityEngine;

/// <summary>
/// Clase que almacena los límites maximos y mínimos de un cuadrado.
/// </summary>
public class SquareLimits
{
    public readonly float minX;
    public readonly float maxX;
    public readonly float minY;
    public readonly float maxY;

    public SquareLimits(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
    }

    /// <summary>
    /// Devuelve true si point se encuentra dentro de el cuadrado.
    /// </summary>
    /// <param name="point">El punto que se quiere comprobar si esta dentro del cuadrado.</param>
    /// <returns>True si el punto esta dentro del cuadrado.</returns>
    public bool isInsideSquare(Vector3 point)
    {
        if ((point.x <= this.maxX && point.x >= this.minX) && (point.z >= this.minY && point.z <= this.maxY))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
