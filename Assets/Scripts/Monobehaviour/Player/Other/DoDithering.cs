using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class DoDithering : MonoBehaviour
{
    [SerializeField] GameObject camera;
    [SerializeField] GameObject objectContainer;
    [SerializeField] LayerMask checkMask;

    float distance;
    private CapsuleCollider collider;



    private void Start()
    {
        collider = GetComponent<CapsuleCollider>();
    }
    private void Update()
    {
        objectContainer.transform.LookAt(camera.transform);
        distance = Vector3.Distance(objectContainer.transform.position, camera.transform.position);
        collider.height = distance;
        collider.center = new Vector3(collider.center.x, (collider.height / 2), collider.center.z);
        
        /*RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, Mathf.Infinity);
        foreach (RaycastHit hit in hits)
        {
            

        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        List<Material> materials = new List<Material>();
        MeshRenderer[] tempRenderers = other.gameObject.GetComponents<MeshRenderer>();
        if (tempRenderers != null && tempRenderers.Length > 0)
        {
            foreach (MeshRenderer rnd in tempRenderers)
            {
                if (!renderers.Contains(rnd))
                {
                    renderers.Add(rnd);
                }
            }
        }
        tempRenderers = null;
        tempRenderers = other.gameObject.GetComponentsInChildren<MeshRenderer>();
        if (tempRenderers != null && tempRenderers.Length > 0)
        {
            foreach (MeshRenderer rnd in tempRenderers)
            {
                if (!renderers.Contains(rnd))
                {
                    renderers.Add(rnd);
                }
            }
        }
        foreach (MeshRenderer rnd in renderers)
        {
            Material[] tempMaterials = rnd.materials;
            if (tempMaterials != null && tempMaterials.Length > 0)
            {
                foreach (Material mtl in tempMaterials)
                {
                    if (!materials.Contains(mtl))
                    {
                        materials.Add(mtl);
                    }
                }
            }

        }

        foreach (Material mtl in materials)
        {
            mtl.SetInt("_Dithering", 1);
        }
        if (other.gameObject.layer == checkMask)
        {
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        List<Material> materials = new List<Material>();
        MeshRenderer[] tempRenderers = other.gameObject.GetComponents<MeshRenderer>();
        if (tempRenderers != null && tempRenderers.Length > 0)
        {
            foreach (MeshRenderer rnd in tempRenderers)
            {
                if (!renderers.Contains(rnd))
                {
                    renderers.Add(rnd);
                }
            }
        }
        tempRenderers = null;
        tempRenderers = other.gameObject.GetComponentsInChildren<MeshRenderer>();
        if (tempRenderers != null && tempRenderers.Length > 0)
        {
            foreach (MeshRenderer rnd in tempRenderers)
            {
                if (!renderers.Contains(rnd))
                {
                    renderers.Add(rnd);
                }
            }
        }
        foreach (MeshRenderer rnd in renderers)
        {
            Material[] tempMaterials = rnd.materials;
            if (tempMaterials != null && tempMaterials.Length > 0)
            {
                foreach (Material mtl in tempMaterials)
                {
                    if (!materials.Contains(mtl))
                    {
                        materials.Add(mtl);
                    }
                }
            }

        }

        foreach (Material mtl in materials)
        {
            mtl.SetInt("_Dithering", 0);
        }
        if (other.gameObject.layer == checkMask)
        {
            
        }
    }
}
