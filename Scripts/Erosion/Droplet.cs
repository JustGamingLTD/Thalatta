using UnityEngine;

public class Droplet
{
    Vector2Int position;
    Vector2 velocity;

    float carryAmount;

    public Droplet(Vector2Int position)
    {
        this.position = position;
        velocity = Vector2.zero;
        carryAmount = 0;
    }

    public void ProcessLocation(float[,] heights, Vector3[,] gradients)
    {
        velocity = gradients[position.x, position.y];
        //position += velocity;
    }
}
