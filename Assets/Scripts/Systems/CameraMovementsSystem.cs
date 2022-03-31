using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics.Systems;

[BurstCompile, UpdateAfter(typeof(MovementsSystem))]
public partial struct CameraMovementsSystem : ISystem
{ 
	//private EntityQuery m_group;
	public void OnCreate(ref SystemState state)
    {
		//m_group = state.GetEntityQuery(typeof(RotationEulerXYZ), ComponentType.ReadOnly<CameraMovementsData>());
    }

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        float deltaTime = state.Time.DeltaTime;

		// This declares a new kind of job, which is a unit of work to do.
		// The job is declared as an Entities.ForEach with the target components as parameters,
		// meaning it will process all entities in the world that have both
		// Translation and Rotation components. Change it to process the component
		// types you want.

		//var translationDatas = GetComponentDataFromEntity<Translation>();
		//var rotationDatas = GetComponentDataFromEntity<Rotation>();

		//Entity playerEntity = GetSingletonEntity<MovementsData>();
		//Translation newTrans = translationDatas[playerEntity];
		//Rotation newRot = rotationDatas[playerEntity];

		state.Entities
			.ForEach((ref Rotation rotation, in CameraMovementsData cameraMovementsData, in InputsData inputsData) => {
				//translation.Value = newTrans.Value + new float3(0f, 0.66f, 0f);
				//rotation.Value = newRot.Value;
				// Implement the work to perform for each entity here.
				// You should only access data that is local or that is a
				// field on this job. Note that the 'rotation' parameter is
				// marked as 'in', which means it cannot be modified,
				// but allows this job to run in parallel with other jobs
				// that want to read Rotation component data.
				// For example,
				//     translation.Value += math.mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;
				//if (rotationEuler.Value.x < 90f && cameraMovementsData.move.y < 0f || rotationEuler.Value.x > 270f && rotationEuler.Value.x < 360f && cameraMovementsData.move.y > 0f)
				float rotateValue = inputsData.mouseAxisInputs.y * cameraMovementsData.cameraSensitivity * (cameraMovementsData.invertAxis.y ? 1f : -1f);
				float rotationTest = math.degrees(AddRotationEulerXYZ.toEuler(rotation.Value).x) + rotateValue * deltaTime;
				if ((rotationTest < 85f && rotateValue > 0f) || (rotationTest > -85f && rotateValue < 0f))
					rotation.Value = math.mul(rotation.Value, quaternion.RotateX(deltaTime * rotateValue));
        }).ScheduleParallel();
    }

    

	public void OnDestroy(ref SystemState state)
	{

	}
}
