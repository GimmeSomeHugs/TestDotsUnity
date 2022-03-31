using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public partial class MovementsSystem : SystemBase
{
	BuildPhysicsWorld buildPhysicsWorld;
	protected override void OnCreate()
	{
		buildPhysicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
	}

	protected override void OnUpdate()
	{
		// Assign values to local variables captured in your job here, so that it has
		// everything it needs to do its work when it runs later.
		// For example,
		float deltaTime = Time.DeltaTime;
		// This declares a new kind of job, which is a unit of work to do.
		// The job is declared as an Entities.ForEach with the target components as parameters,
		// meaning it will process all entities in the world that have both
		// Translation and Rotation components. Change it to process the component
		// types you want.
		Entity camera = GetSingletonEntity<CameraMovementsData>();
		var cameraMovementsData = GetComponent<CameraMovementsData>(camera);
		var cameraRotation = GetComponent<Rotation>(camera);
		var instantiatorData = GetSingleton<InstantiatorData>();

		var physicsWorld = buildPhysicsWorld.PhysicsWorld;


		Entities.WithReadOnly(physicsWorld).ForEach((ref MovementsData movementsData, in GroundCheckData groundCheckData, in Translation translation) =>
		{
			float3 raycastStart = translation.Value + new float3(0f, -1.02f, 0f);
			var input = new RaycastInput
			{
				Start = raycastStart,
				End = raycastStart + new float3(0f, -1f, 0f) * groundCheckData.rayLength,
				Filter = new CollisionFilter
				{
					BelongsTo = ~0u,
					CollidesWith = ~0u,
					GroupIndex = 0
				}
			};
			movementsData.isGrounded = physicsWorld.CastRay(input);

		}).ScheduleParallel();

		Entities.WithStructuralChanges().ForEach((ref Rotation rotation, ref Translation translation, ref PhysicsVelocity vel, ref MovementsData movementsData, in InputsData inputsData) =>
		{
			// Implement the work to perform for each entity here.
			// You should only access data that is local or that is a
			// field on this job. Note that the 'rotation' parameter is
			// marked as 'in', which means it cannot be modified,
			// but allows this job to run in parallel with other jobs
			// that want to read Rotation component data.
			// For example,

			movementsData.jumpTimer += deltaTime;
			float rotationValue = inputsData.mouseAxisInputs.x * cameraMovementsData.cameraSensitivity * (cameraMovementsData.invertAxis.x ? -1f : 1f);
			rotation.Value = math.mul(rotation.Value, quaternion.RotateY(deltaTime * rotationValue));
			rotation.Value = quaternion.EulerXYZ(new float3(0f, AddRotationEulerXYZ.toEuler(rotation.Value).y, 0f));
			translation.Value += math.mul(rotation.Value, math.normalizesafe(new float3(inputsData.directionalInputs.x, 0f, inputsData.directionalInputs.y))) * deltaTime * movementsData.moveSpeed;
			if (inputsData.spaceBarDown && movementsData.isGrounded)
			{
				vel.Linear += new float3(0f, 1f, 0f) * movementsData.jumpHeight;
				movementsData.jumpTimer = 0f;
			}

			if (inputsData.leftClickDown)
			{
				Entity en = EntityManager.Instantiate(instantiatorData.prefab);
				BulletData bulletData = GetComponent<BulletData>(en);
				Translation trans = GetComponent<Translation>(en);
				PhysicsVelocity velocity = GetComponent<PhysicsVelocity>(en);
				bulletData.dir = math.normalize(math.mul(math.mul(rotation.Value, cameraRotation.Value), new float3(0f, 0f, 1f)));
				trans.Value = translation.Value + bulletData.dir * 3f + new float3(0f, 0.66f, 0f);
				velocity.Linear = bulletData.dir * bulletData.speed;
				SetComponent(en, bulletData);
				SetComponent(en, trans);
				SetComponent(en, velocity);

			}

		}).Run();
	}
}
