using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(MovementsSystem))]
public partial class PlayerInputsSystem : SystemBase
{

	protected override void OnUpdate()
    {
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     float deltaTime = Time.DeltaTime;

        // This declares a new kind of job, which is a unit of work to do.
        // The job is declared as an Entities.ForEach with the target components as parameters,
        // meaning it will process all entities in the world that have both
        // Translation and Rotation components. Change it to process the component
        // types you want.

        float2 directionalInputs = new float2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical"));
        float2 mouseAxisInputs = new float2(UnityEngine.Input.GetAxisRaw("Mouse X"), UnityEngine.Input.GetAxisRaw("Mouse Y"));

        bool spaceBarDown = UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space);
        bool leftClickDown = UnityEngine.Input.GetMouseButtonDown(0);
        bool rightClickDown = UnityEngine.Input.GetMouseButtonDown(1);
        bool leftShift = UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftShift);

        Entities.ForEach((ref InputsData inputsData) =>
        {
            inputsData.directionalInputs = directionalInputs;
            inputsData.mouseAxisInputs = mouseAxisInputs;

            inputsData.spaceBarDown = spaceBarDown;
            inputsData.leftClickDown = leftClickDown;
            inputsData.rightClickDown = rightClickDown;
            inputsData.leftShift = leftShift;

        }).ScheduleParallel();

	}
}
