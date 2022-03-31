using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[RequireComponent(typeof(ConvertToEntity))]
public class EntityPrefabsGroup : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
	public GameObject prefab;

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{

		dstManager.AddComponentData(entity, new InstantiatorData { prefab = conversionSystem.GetPrimaryEntity(prefab) });

	}

	public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
	{
		referencedPrefabs.Add(prefab);
	}
}