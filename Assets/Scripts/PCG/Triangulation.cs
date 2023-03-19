using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct Edge
{
    public Vector3 vertexA;
    public Vector3 vertexB;
    
    public Edge(Vector3 _vertexA, Vector3 _vertexB)
    {
        vertexA = _vertexA;
        vertexB = _vertexB;
    }

    public Vector3 GetMidpoint()
    {
        return new Vector3((vertexA.x + vertexB.x) / 2, (vertexA.z + vertexB.z) / 2);
    }

    public static bool operator ==(Edge a, Edge b)
    {
        return a.vertexA == b.vertexA && a.vertexB == b.vertexB ||
                a.vertexA == b.vertexB && a.vertexB == b.vertexA;
    }
    
    public static bool operator !=(Edge a, Edge b)
    {
        return a.vertexA != b.vertexA && a.vertexB != b.vertexB &&
                a.vertexA != b.vertexB && a.vertexB != b.vertexA;
    }

    public bool Equals(Edge other)
    {
        return this == other;
    }
    
    public override bool Equals(object obj)
    {
        return obj is Edge edge &&
               (vertexA.Equals(edge.vertexA) && vertexB.Equals(edge.vertexB) ||
                vertexA.Equals(edge.vertexB) && vertexB.Equals(edge.vertexA));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(vertexA, vertexB);
    }
}

public class Triangle
{
    public Vector3 vertexA;
    public Vector3 vertexB;
    public Vector3 vertexC;

    public Vector3 circumCenter;
    public float circumRadius;
    
    public Triangle(Vector3 _vertexA, Vector3 _vertexB, Vector3 _vertexC)
    {
        vertexA = _vertexA;
        vertexB = _vertexB;
        vertexC = _vertexC;

        circumCenter = CalculateCircumCenter(vertexA, vertexB, vertexC);
        circumRadius = GetCircumRadius(vertexA, circumCenter);
    }

    public bool InCircumCircle(Vector3 _vertex)
    {
        float distance = Vector3.Distance(circumCenter, _vertex);

        if (distance < circumRadius)
        {
            return true;
        }

        return false;
    }
    
    private Vector3 CalculateCircumCenter(Vector3 _vertexA, Vector3 _vertexB, Vector3 _vertexC)
    {
        float a = 0;
        float b = 0;
        float c = 0;
        GetLine(_vertexA, _vertexB, ref a, ref b, ref c);
        
        float e = 0;
        float f = 0;
        float g = 0;
        GetLine(_vertexB, _vertexC, ref e, ref f, ref g);

        GetPerpendicularBisector(_vertexA, _vertexB, ref a, ref b, ref c);
        GetPerpendicularBisector(_vertexB, _vertexC, ref e, ref f, ref g);

        Vector3 circumCenter = GetIntersection(a, b, c, e, f, g);
        
        
        return circumCenter;
    }

    private float GetCircumRadius(Vector3 _vertexA, Vector3 _center)
    {
        return Vector3.Distance(_vertexA, _center);
    }
    
    private void GetLine(Vector3 _vertexA, Vector3 _vertexB, ref float a, ref float b, ref float c)
    {
        a = _vertexB.z - _vertexA.z;
        b = _vertexA.x - _vertexB.x;
        c = a * _vertexA.x + b * _vertexB.z;
    }

    private void GetPerpendicularBisector(Vector3 _vertexA, Vector3 _vertexB, ref float a, ref float b, ref float c)
    {
        Vector3 midpoint = new Vector3();
        midpoint.x = (_vertexA.x + _vertexB.x) / 2;
        midpoint.z = (_vertexA.z + _vertexB.z) / 2;
        Debug.Log("Midpoint: " + midpoint);
        
        c = -b * midpoint.x + a * midpoint.z;

        float temp = a;
        a = -b;
        b = temp;
    }
    
    private Vector3 GetIntersection(float a1, float b1, float c1,
        float a2, float b2, float c2)
    {
        Vector3 intersection;
        float determinant = a1 * b2 - a2 * b1;
        if (determinant == 0)
        {
            intersection = new Vector3(int.MaxValue, int.MaxValue);
        }
        else
        {
            float x = (b2 * c1 - b1 * c2) / determinant;
            float z = (a1 * c2 - a2 * c1) / determinant;

            intersection = new Vector3(x, 0, z);
        }

        return intersection;
    }

}

public class Triangulation
{
    
    public List<Triangle> Triangulate(List<Vector3> _vertices)
    {
        //Get Super Triangle
        Triangle superTriangle = GetSuperTriangle(_vertices);
        
        //Initialize Triangle list
        List<Triangle> triangles = new List<Triangle>();
        triangles.Add(superTriangle);
        
        //Triangulate each vertex;
        foreach (var vertex in _vertices)
        {
            triangles = AddVertex(vertex, triangles);
        }
        
        //Remove triangles that share an edge
        foreach (var triangle in triangles)
        {
            if (triangle.vertexA == superTriangle.vertexA || triangle.vertexA == superTriangle.vertexB || triangle.vertexA == superTriangle.vertexC
                || triangle.vertexB == superTriangle.vertexA || triangle.vertexB == superTriangle.vertexB || triangle.vertexB == superTriangle.vertexC
                || triangle.vertexC == superTriangle.vertexA || triangle.vertexC == superTriangle.vertexB || triangle.vertexC == superTriangle.vertexC)
            {
                triangles.Remove(triangle);
            }
        }

        return triangles;
    }

    private List<Triangle> AddVertex(Vector3 _vertex, List<Triangle> _triangles)
    {
        List<Edge> edges = new List<Edge>();

        foreach (var triangle in _triangles)
        {
            if (triangle.InCircumCircle(_vertex))
            {
                edges.Add(new Edge(triangle.vertexA, triangle.vertexB));
                edges.Add(new Edge(triangle.vertexB, triangle.vertexC));
                edges.Add(new Edge(triangle.vertexC, triangle.vertexA));

                _triangles.Remove(triangle);
            }
        }

        edges = GetUniqueEdges(edges);

        foreach (var edge in edges)
        {
            _triangles.Add(new Triangle(edge.vertexA, edge.vertexB, _vertex));
        }

        return _triangles;
    }

    private List<Edge> GetUniqueEdges(List<Edge> _edges)
    {
        List<Edge> uniqueEdges = new List<Edge>();
        for (int i = 0; i < _edges.Count; i++)
        {
            bool isUnique = true;
            for (int j = 0; j < _edges.Count; j++)
            {
                if (i != j && _edges[i].Equals(_edges[j]))
                {
                    isUnique = false;
                    break;
                }
            }

            if (isUnique)
            {
                uniqueEdges.Add(_edges[i]);
            }
        }
        
        return uniqueEdges;
    }
    
    private Triangle GetSuperTriangle(List<Vector3> _vertices)
    {
        float minX = Mathf.Infinity;
        float maxX = Mathf.Infinity;
        float minY = Mathf.Infinity;
        float maxY = Mathf.Infinity;
        
        foreach (var vertex in _vertices)
        {
            minX = Mathf.Min(minX, vertex.x);
            minY = Mathf.Min(minY, vertex.y);

            maxX = Mathf.Max(maxX, vertex.x);
            maxY = Mathf.Max(maxY, vertex.y);
        }

        float x = (maxX - minX) * 10;
        float y = (maxY - minY) * 10;

        Vector3 vertexA = new Vector3(minX - x,0,  minY - y * 3);
        Vector3 vertexB = new Vector3(minX - x, 0, maxY + y * 3);
        Vector3 vertexC = new Vector3(maxX + x * 3, 0, maxY + y);

        return new Triangle(vertexA, vertexB, vertexC);
    }
    
    
}
