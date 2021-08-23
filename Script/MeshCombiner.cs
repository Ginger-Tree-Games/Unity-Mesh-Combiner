using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    public void CombineMeshesComplex()
    {
        Quaternion startRotation = transform.rotation;
        Vector3 startPosition = transform.position;

        //Set position and rotation to zero so that resulting mesh is not displaced
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;

        Debug.Log(name + " is building combined mesh!");

        //Create list of all meshes that need to be merged
        //MeshFilter[] filters = GetComponentsInChildren<MeshFilter>(false);

        //Create list for all different materials in children
        //List<Material> materials = new List<Material>();

        //Get all mesh renderers in children
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(false);

        Dictionary<string, List<CombineInstance>> materialToCombiners = new Dictionary<string, List<CombineInstance>>();

        Debug.Log(name + " is building new mesh with " + renderers.Length + " children!");

        int totalv = 0;

        //Search children for each diferent kind of material
        foreach(MeshRenderer renderer in renderers)
        {
            if(renderer.transform != transform)
            {
                Material[] rendererMaterials = renderer.sharedMaterials;
                MeshFilter filter = renderer.GetComponent<MeshFilter>();

                for(int i = 0; i < rendererMaterials.Length; i++)
                {
                    if(!materialToCombiners.ContainsKey(rendererMaterials[i].name))
                    {
                        Debug.Log("Adding new material: " + rendererMaterials[i].name);

                        materialToCombiners.Add(rendererMaterials[i].name, new List<CombineInstance>());
                    }

                    CombineInstance combiner = new CombineInstance();
                    combiner.mesh = filter.sharedMesh;
                    combiner.subMeshIndex = i;
                    combiner.transform = filter.transform.localToWorldMatrix;

                    materialToCombiners[rendererMaterials[i].name].Add(combiner);
                }

                totalv += filter.sharedMesh.vertices.Length;
            }
        }

        Debug.Log("Total vertices: " + totalv);
        Debug.Log("Total materials: " + materialToCombiners.Keys.Count);

        //Create list for combined submeshes for each material
        List<Mesh> subMeshes = new List<Mesh>();        
        
        foreach(KeyValuePair<string, List<CombineInstance>> kvp in materialToCombiners)
        {
            Debug.Log("Making new submesh from " + kvp.Value.Count + " combine instances");

            Mesh resultMesh = new Mesh();

            resultMesh.CombineMeshes(kvp.Value.ToArray(), true);

            subMeshes.Add(resultMesh);
        }

        List<CombineInstance> finalCombiners = new List<CombineInstance>();

        Debug.Log("Combining " + subMeshes.Count + " submeshes");

        //combine submeshes
        foreach(Mesh mesh in subMeshes)
        {
            CombineInstance combiner = new CombineInstance();

            combiner.mesh = mesh;
            combiner.subMeshIndex = 0;
            combiner.transform = Matrix4x4.identity;

            finalCombiners.Add(combiner);
        }

        //Create final mesh
        Mesh finalMesh = new Mesh();

        finalMesh.CombineMeshes(finalCombiners.ToArray(), false);


        //Apply mesh to filter
        MeshFilter currentFilter = GetComponent<MeshFilter>();

        if(currentFilter != null)
        {
            currentFilter.sharedMesh = finalMesh;
            Debug.Log("Final combined mesh has " + subMeshes.Count + " materials!");
        }
        else
        {
            Debug.LogError(name + " has no mesh filter!");
        }


        //Apply mesh to collider
        MeshCollider currentCollider = GetComponent<MeshCollider>();

        if (currentCollider != null)
        {
            currentCollider.sharedMesh = finalMesh;
            Debug.Log("Final combined mesh has " + subMeshes.Count + " materials!");
        }
        else
        {
            Debug.LogError(name + " has no mesh collider!");
        }

        transform.rotation = startRotation;
        transform.position = startPosition;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SetChildrenActive(bool is_active)
    {
        //Turns on children for easy recreation of mesh
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(is_active);
        }
    }

    public void CombineMeshesSimple()
    {
        Quaternion startRotation = transform.rotation;
        Vector3 startPosition = transform.position;

        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;

        Debug.Log(name + " is building combined mesh!");

        MeshFilter[] meshes = GetComponentsInChildren<MeshFilter>(false);

        Debug.Log(name + " is building new mesh with " + meshes.Length + " meshes!");

        Mesh resultMesh = new Mesh();

        CombineInstance[] combines = new CombineInstance[meshes.Length];

        for (int i = 0; i < meshes.Length; i++)
        {
            if (meshes[i].transform != transform)
            {
                combines[i].subMeshIndex = 0;
                combines[i].mesh = meshes[i].sharedMesh;
                combines[i].transform = meshes[i].transform.localToWorldMatrix;
            }
        }

        resultMesh.CombineMeshes(combines);

        transform.rotation = startRotation;
        transform.position = startPosition;

        GetComponent<MeshFilter>().sharedMesh = resultMesh;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
