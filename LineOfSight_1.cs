using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class LineOfSight_1 : MonoBehaviour
{
    //////////////////////////
    /// PROPERTIES
    //////////////////////////
    
    [Header("Common Settings")]
    [SerializeField, RangeAttribute(1, 10), Tooltip("Quality of raycasting, better quality reduce performance")]
    private int _quality = 4;
    [SerializeField, RangeAttribute(1, 361), Tooltip("Maximum angle of viewing area")] 
    private int _maxAngle = 160;
    [SerializeField, Tooltip("Maximum viewing distance")]
    private float _maxDistance = 15;
    [SerializeField, Tooltip("Layers of objects which are not transparent for viewing")]
    private LayerMask _cullingMask = -1;

    [Header("Materials")]
    [SerializeField, Tooltip("Material of Idle status")]
    private Material _idle;
    [SerializeField, Tooltip("Material of Suspicious status")]
    private Material _suspicious;
    [SerializeField, Tooltip("Material of Alerted status")]
    private Material _alerted;

    [Header("Other")]
    [SerializeField, Tooltip("Default status of viewer")]
    private Status _currentStatus;
    [SerializeField, Tooltip("Turn on to display rays in editor window")]
    private bool _displayRaysInEditor = false;
    public bool castRayFlag = true;
    public bool meshRenderer = false;
    //////////////////////////
    /// PUBLIC METHODS
    //////////////////////////

    /// <summary>
    /// Set current status of viewer.
    /// </summary>
    /// <param name="status"></param>
    public void SetStatus(Status status)
    {
        _currentStatus = status;
    }

    /// <summary>
    /// Get current status of viewer.
    /// </summary>
    /// <returns></returns>
    public Status GetStatus()
    {
        return _currentStatus;
    }

    /// <summary>
    /// Automatic check based on tag.
    /// </summary>
    /// <param name="tagName">Name of tag to check</param>
    /// <returns>True if viewer sees anything with given tag, otherwise false</returns>
    public bool SeeByTag(string tagName)
    {
        return _hits.Any(hit => hit.transform && hit.transform.tag == tagName);
    }
    public RaycastHit getTransformByTag(string tagName)
    {
        //return _hits.Any(hit => hit.transform && hit.transform.tag == tagName);
        //_hits.Amy()
        return _hits.Find(hit => hit.transform && hit.transform.tag == tagName);
    }
    
    public List<RaycastHit> getAllTransformByTag(string tagName)
    {
        //return _hits.Any(hit => hit.transform && hit.transform.tag == tagName);
        //_hits.Amy()
        return _hits.FindAll(hit => hit.transform && hit.transform.tag == tagName);
    }
    /// <summary>
    /// Manual check of what the viewer sees.
    /// </summary>
    /// <param name="function">Method which returns true if a ray collides with an appropriate object</param>
    /// <returns>True if something appropriate was seen, otherwise false</returns>
    public bool SeeByFunction(Func<RaycastHit, bool> function)
    {
        return _hits.Any(function);
    }

    /// <summary>
    /// Status of viewer.
    /// </summary>
    public enum Status
    {
        Idle,
        Suspicious,
        Alerted
    }

    //////////////////////////
    /// INTERNALS
    //////////////////////////

    private List<RaycastHit> _hits;
    private MeshRenderer _meshRenderer;
    private Vector3[] _vertices;
    private int[] _triangles;
    private Mesh _mesh;
    private Vector3 rotation;


    private void Start()
    {
        _hits = new List<RaycastHit>();
        _mesh = GetComponent<MeshFilter>().mesh;
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.material = GetMaterialForStatus(_currentStatus);
        rotation = transform.rotation.eulerAngles;
        //_meshRenderer.enabled = false;

    }

    private void Update()
    {
        CastRays();
        if(meshRenderer)
            _meshRenderer.enabled = true;
        else
            _meshRenderer.enabled = false;
    }

    private void LateUpdate()
    {
        _mesh.Clear();
        
        UpdateMesh();
        UpdateMeshMaterial();
    }

    private void OnDrawGizmosSelected()
    {
        if (!_displayRaysInEditor || !_hits.Any()) return;

        Gizmos.color = Color.cyan;
        foreach (RaycastHit hit in _hits)
        {
            Gizmos.DrawSphere(hit.point, 0.04f);
            Gizmos.DrawLine(transform.position, hit.point);
        }
    }

    private void CastRays()
    {
        _hits.Clear();
        if (castRayFlag)
        {
            int numberOfRays = _maxAngle * _quality;
            float currentAngle = _maxAngle / -2.0f;
           // transform.rotation = Quaternion.Euler(rotation);
           
            // for (int j = 0; j <= 2; j++)
            //{
            for (int i = 0; i < numberOfRays; i++)
            {
                Vector3 direction = Quaternion.AngleAxis(currentAngle, transform.up) * transform.forward;
                RaycastHit hit;

                if (Physics.Raycast(transform.position, direction, out hit, _maxDistance, _cullingMask) == false)
                {
                    hit.point = transform.position + (direction * _maxDistance);
                }

                _hits.Add(hit);
                currentAngle += 1f / _quality;
            }
            // transform.Rotate(j, 0f, 0f, Space.Self); // pitch
            //}
        }
       
    }

    private void UpdateMesh()
    {
        if (_hits == null || _hits.Count == 0)
            return;

        if (_mesh.vertices.Length != _hits.Count + 1)
        {
            _mesh.Clear();

            _vertices = new Vector3[_hits.Count + 1];
            _triangles = new int[(_hits.Count - 1)*3];

            int sideIndex = 1;
            for (int i = 0; i < _triangles.Length; i += 3)
            {
                _triangles[i] = 0;
                _triangles[i + 1] = sideIndex;
                _triangles[i + 2] = sideIndex + 1;
                sideIndex++;
            }
        }

        _vertices[0] = Vector3.zero;
        for (int i = 1; i <= _hits.Count; i++)
        {
            _vertices[i] = transform.InverseTransformPoint(_hits[i - 1].point);
        }

        var uv = new Vector2[_vertices.Length];
        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = new Vector2(_vertices[i].x, _vertices[i].z);
        }

        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;
        _mesh.uv = uv;

        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
    }

    private Material GetMaterialForStatus(Status status)
    {
        switch (status)
        {
            case Status.Idle:
                return _idle;
            case Status.Suspicious:
                return _suspicious;
            case Status.Alerted:
                return _alerted;
        }
        return null;
    }

    private void UpdateMeshMaterial()
    {
        _meshRenderer.material = GetMaterialForStatus(_currentStatus);
    }
}