using IAEngine;
using UnityEngine;
using Color = UnityEngine.Color;

[ExecuteAlways]
[RequireComponent(typeof(PolygonCollider2D))]
internal class PolygonCheck : MonoBehaviour
{
    public PolygonCollider2D polygonCollider;
    public Transform checkTrans;

    private void Awake()
    {
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    private void OnDrawGizmosSelected()
    {

    }

    private void OnDrawGizmos()
    {
        if (checkTrans != null)
        {
            if (CheckIsIn())
            {
                Gizmos.DrawCube(checkTrans.position, new Vector3(0.1f, 0.1f, 0.1f));
            }
            else
            {

            }
        }

        if (polygonCollider != null)
        {

            IAToolkit.GizmosHelper.DrawBounds(polygonCollider.bounds, Color.blue);
        }
    }

    private bool CheckIsIn()
    {
        Vector2[] points = polygonCollider.GetPoints();
        Polygon polygon = new Polygon(points.Length);

        for (int i = 0; i < points.Length; i++)
        {
            int index = i;
            polygon.points[index] = new Point(points[index].x, points[index].y);
        }

        Point checkPoint = new Point(checkTrans.position.x, checkTrans.position.y);
        return polygon.InPolygon(checkPoint);
    }
}
