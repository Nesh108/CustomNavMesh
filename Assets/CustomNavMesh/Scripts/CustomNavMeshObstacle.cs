﻿using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// An obstacle for CustomNavMeshAgents to avoid.
/// </summary>
[DisallowMultipleComponent]
public class CustomNavMeshObstacle : CustomMonoBehaviour
{
    /// <summary>
    /// A delegate which can be used to register callback methods to be invoked after the obstacle is changed.
    /// </summary>
    public delegate void OnChange();
    /// <summary>
    /// Subscribe a function to be called after the obstacle is changed.
    /// </summary>
    public event OnChange onChange;

    [SerializeField] NavMeshObstacleShape m_Shape = NavMeshObstacleShape.Box;
    /// <summary>
    /// The shape of the obstacle.
    /// </summary>
    public NavMeshObstacleShape Shape
    {
        get { return m_Shape; }
        set { m_Shape = value; NavMeshObstacle.shape = value; onChange?.Invoke(); }
    }

    [SerializeField] Vector3 m_Center = Vector3.zero;
    /// <summary>
    /// The center of the obstacle, measured in the object's local space.
    /// </summary>
    public Vector3 Center
    {
        get { return m_Center; }
        set { m_Center = value; NavMeshObstacle.center = value; onChange?.Invoke(); }
    }

    [SerializeField] Vector3 m_Size = Vector3.one;
    /// <summary>
    /// The size of the obstacle, measured in the object's local space.
    /// </summary>
    public Vector3 Size
    {
        get { return m_Size; }
        set { m_Size = value; NavMeshObstacle.size = value; onChange?.Invoke(); }
    }

    [SerializeField] bool m_Carve = false;
    /// <summary>
    /// Should this obstacle make a cut-out in the navmesh.
    /// </summary>
    public bool Carving
    {
        get { return m_Carve; }
        set { m_Carve = value; NavMeshObstacle.carving = value; onChange?.Invoke(); }
    }

    [SerializeField] float m_MoveThreshold = 0.1f;
    /// <summary>
    /// Threshold distance for updating a moving carved hole (when carving is enabled).
    /// </summary>
    public float CarvingMoveThreshold
    {
        get { return m_MoveThreshold; }
        set { m_MoveThreshold = value; NavMeshObstacle.carvingMoveThreshold = value; onChange?.Invoke(); }
    }

    [SerializeField] float m_TimeToStationary = 0.5f;
    /// <summary>
    /// Time to wait until obstacle is treated as stationary (when carving and carveOnlyStationary are enabled).
    /// </summary>
    public float CarvingTimeToStationary
    {
        get { return m_TimeToStationary; }
        set { m_TimeToStationary = value; NavMeshObstacle.carvingTimeToStationary = value; onChange?.Invoke(); }
    }

    [SerializeField] bool m_CarveOnlyStationary = true;
    /// <summary>
    /// Should this obstacle be carved when it is constantly moving?
    /// </summary>
    public bool CarveOnlyStationary
    {
        get { return m_CarveOnlyStationary; }
        set { m_CarveOnlyStationary = value; NavMeshObstacle.carveOnlyStationary = value; onChange?.Invoke(); }
    }

    /// <summary>
    /// Radius of the obstacle's capsule shape.
    /// </summary>
    public float Radius
    {
        get { return m_Size.x / 2.0f; }
        set { Size = new Vector3(value * 2.0f, m_Size.y, value * 2.0f); }
    }

    /// <summary>
    /// Height of the obstacle's cylinder shape.
    /// </summary>
    public float Height
    {
        get { return m_Size.y / 2.0f; }
        set { Size = new Vector3(m_Size.x, value * 2.0f, m_Size.z); }
    }

    /// <summary>
    /// Velocity at which the obstacle moves around the NavMesh.
    /// </summary>
    public Vector3 Velocity
    {
        get { return NavMeshObstacle.velocity; }
        set { NavMeshObstacle.velocity = value; }
    }

    NavMeshObstacle navMeshObstacle;
    NavMeshObstacle NavMeshObstacle
    {
        get
        {
            if (navMeshObstacle == null)
            {
                navMeshObstacle = GetComponent<NavMeshObstacle>();
                if (navMeshObstacle == null)
                {
                    navMeshObstacle = gameObject.AddComponent<NavMeshObstacle>();
                }
                else
                {
                    // update existing nav mesh obstacle
                    CopyValues(this, navMeshObstacle);
                }
            }
            return navMeshObstacle;
        }
    }

    protected override void OnCustomEnable()
    {
        // calling the NavMeshObstacle property will add an obstacle if gameObject doesn't have it
        // the custom inspector will only hide after a split second so update the flags now; 
        NavMeshObstacle.hideFlags = HideFlags.HideInInspector;
        NavMeshObstacle.enabled = true;
    }

    protected override void OnCustomDisable()
    {
        if(NavMeshObstacle != null)
        {
            NavMeshObstacle.enabled = false;
        }
    }

    void CopyValues(CustomNavMeshObstacle sourceObs, NavMeshObstacle destObs)
    {
        // only assigning the properties that are different would yield no performance benefits
        destObs.carveOnlyStationary = sourceObs.m_CarveOnlyStationary;
        destObs.carving = sourceObs.m_Carve;
        destObs.carvingMoveThreshold = sourceObs.m_MoveThreshold;
        destObs.carvingTimeToStationary = sourceObs.m_TimeToStationary;
        destObs.center = sourceObs.m_Center;
        destObs.shape = sourceObs.m_Shape;
        destObs.size = sourceObs.m_Size;
    }
}
